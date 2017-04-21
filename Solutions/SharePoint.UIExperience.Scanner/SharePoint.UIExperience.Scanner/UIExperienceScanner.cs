using Microsoft.SharePoint.Client;
using SharePoint.UIExperience.Scanner.Framework.TimerJobs;
using SharePoint.UIExperience.Scanner.Scanners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace SharePoint.UIExperience.Scanner
{
    /// <summary>
    /// UIExperience scan job
    /// </summary>
    public class UIExperienceScanner : TimerJob
    {
        // Result dictionaries
        public ConcurrentDictionary<string, PageResult> PageResults = new ConcurrentDictionary<string, PageResult>();
        public ConcurrentDictionary<string, CustomizationResult> CustomizationResults = new ConcurrentDictionary<string, CustomizationResult>();
        public ConcurrentDictionary<string, ListResult> ListResults = new ConcurrentDictionary<string, ListResult>();
        // Stacks with additional details
        public ConcurrentStack<CustomActionsResult> CustomActionScanResults = new ConcurrentStack<CustomActionsResult>();
        public ConcurrentStack<AlternateCSSResult> AlternateCSSResults = new ConcurrentStack<AlternateCSSResult>();
        public ConcurrentStack<MasterPageResult> MasterPageResults = new ConcurrentStack<MasterPageResult>();
        public ConcurrentStack<UIExperienceScanError> UIExpScanErrors = new ConcurrentStack<UIExperienceScanError>();
        public Int32 ScannedSites = 0;
        public Int32 ScannedWebs = 0;
        public Int32 ScannedLists = 0;
        public DateTime StartTime;
        public string OutputFolder = "";
        public string Separator = ",";
        public bool ExcludeListsOnlyBlockedByOobReasons = false;
        internal List<Mode> Modes;

        private static volatile bool firstSiteCollectionDone = false;
        private object scannedSitesLock = new object();
        private object scannedWebsLock = new object();
        private object scannedListsLock = new object();
        private Int32 SitesToScan = 0;

        public UIExperienceScanner() : base("UIExperienceInventory")
        {
            TimerJobRun += UIExperienceScanner_TimerJobRun;
            ExpandSubSites = false; // we'll expand subsites at site collection level to optimize the data reading (no splitting a single site collection across multiple threads)
            this.StartTime = DateTime.Now;
        }

        /// <summary>
        /// Grab the number of sites that need to be scanned...will be needed to show progress when we're having a long run
        /// </summary>
        /// <param name="addedSites"></param>
        /// <returns></returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            var sites = base.ResolveAddedSites(addedSites);
            this.SitesToScan = sites.Count;
            return sites;
        }

        /// <summary>
        /// Event handler that's being executed by the threads processing the sites. Everything in here must be coded in a thread-safe manner
        /// </summary>
        private void UIExperienceScanner_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            // Get all the sub sites in the site we're processing
            IEnumerable<string> expandedSites = GetAllSubSites(e.SiteClientContext.Site);

            bool isFirstSiteInList = true;
            string siteCollectionUrl = "";

            lock (scannedSitesLock)
            {
                ScannedSites++;
            }

            // Manually iterate over the content
            foreach (string site in expandedSites)
            {
                lock (scannedWebsLock)
                {
                    ScannedWebs++;
                }

                // Clone the existing ClientContext for the sub web
                using (ClientContext ccWeb = e.SiteClientContext.Clone(site))
                {
                    Console.WriteLine("Processing site {0}...", site);
                    try
                    {
                        if (!firstSiteCollectionDone)
                        {
                            firstSiteCollectionDone = true;

                            // Telemetry
                            ccWeb.ClientTag = "SPDev:UIExperienceScanner";
                            ccWeb.Load(ccWeb.Web, p => p.Description, p => p.Id);
                            ccWeb.ExecuteQuery();
                        }

                        if (isFirstSiteInList)
                        {
                            // Perf optimization: do one call per site to load all the needed properties
                            var spSite = (ccWeb as ClientContext).Site;
                            ccWeb.Load(spSite, p => p.RootWeb, p => p.Url); // Needed in IsSubSite
                            ccWeb.Load(spSite.RootWeb, p => p.Id); // Needed in IsSubSite

                            if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.IgnoredCustomizations))
                            {
                                ccWeb.Load(spSite, p => p.UserCustomActions); // User custom action site level
                            }
                            if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.BlockedLists))
                            {
                                ccWeb.Load(spSite, p => p.Features); // Features site level
                            }

                            isFirstSiteInList = false;
                        }

                        // Perf optimization: do one call per web to load all the needed properties
                        ccWeb.Load(ccWeb.Web, p => p.Id); // Needed in IsSubSite

                        if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.IgnoredCustomizations))
                        {
                            ccWeb.Load(ccWeb.Web, p => p.MasterUrl, p => p.CustomMasterUrl, // master page check
                                                  p => p.AlternateCssUrl, // Alternate CSS
                                                  p => p.UserCustomActions); // Web user custom actions                                                  
                        }
                        if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.BlockedPages) || Modes.Contains(Mode.BlockedLists))
                        {
                            ccWeb.Load(ccWeb.Web, p => p.Features, p => p.WebTemplate, p => p.Configuration); // Features web level, web template                                                 

                        }
                        
                        // Lists is needed in all three scenarios, so we always prefetch them.
                        ccWeb.Load(ccWeb.Web, p => p.Lists.Include(li => li.UserCustomActions, li => li.Title, li => li.Hidden, li => li.DefaultViewUrl, li => li.BaseTemplate, li => li.RootFolder, li => li.ListExperienceOptions)); // List check, includes list user custom actions                                                                                         
                        ccWeb.ExecuteQueryRetry();

                        // Fill site collection url
                        if (string.IsNullOrEmpty(siteCollectionUrl))
                        {
                            siteCollectionUrl = ccWeb.Site.Url;
                        }

                        // Scanning to identify sites that can't show modern pages
                        if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.BlockedPages))
                        {
                            PageScanner featureScanner = new PageScanner(site, siteCollectionUrl);
                            var featureScanResult = featureScanner.Analyze(ccWeb);
                            if (featureScanResult != null)
                            {
                                if (!this.PageResults.TryAdd(Guid.NewGuid().ToString() + featureScanResult.Url, featureScanResult))
                                {
                                    UIExperienceScanError error = new UIExperienceScanError()
                                    {
                                        Error = $"Could not add page scan result for {featureScanResult.Url}",
                                        SiteColUrl = siteCollectionUrl,
                                        SiteURL = site,
                                    };
                                    this.UIExpScanErrors.Push(error);
                                    Console.WriteLine($"Could not add page scan result for {featureScanResult.Url}");
                                }
                            }
                        }

                        // Scanning to identify lists which can't be shown in modern
                        if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.BlockedLists))
                        {
                            ListScanner listConfig = new ListScanner(site, siteCollectionUrl, this.ExcludeListsOnlyBlockedByOobReasons);
                            var scannedLists = listConfig.Analyze(ccWeb, ref this.ListResults, ref this.UIExpScanErrors);
                            lock(scannedListsLock)
                            {
                                this.ScannedLists = this.ScannedLists + scannedLists;
                            }
                        }

                        // Scanning for customizations which will be ignored in modern pages and lists
                        if (Modes.Contains(Mode.Scan) || Modes.Contains(Mode.IgnoredCustomizations))
                        {
                            // Custom CSS scanner
                            AlternateCSSScanner cssScanner = new AlternateCSSScanner(site, siteCollectionUrl);
                            var alternateCSSResult = cssScanner.Analyze(ccWeb);
                            if (alternateCSSResult != null)
                            {
                                this.AlternateCSSResults.Push(alternateCSSResult);
                                if (this.CustomizationResults.ContainsKey(alternateCSSResult.Url))
                                {
                                    var customizationResult = this.CustomizationResults[alternateCSSResult.Url];
                                    customizationResult.IgnoredAlternateCSS = alternateCSSResult.AlternateCSS;
                                    if (!this.CustomizationResults.TryUpdate(alternateCSSResult.Url, customizationResult, customizationResult))
                                    {
                                        UIExperienceScanError error = new UIExperienceScanError()
                                        {
                                            Error = $"Could not update CSS scan result for {customizationResult.Url}",
                                            SiteURL = site,
                                            SiteColUrl = siteCollectionUrl
                                        };
                                        this.UIExpScanErrors.Push(error);
                                        Console.WriteLine($"Could not update CSS scan result for {customizationResult.Url}");
                                    }
                                }
                                else
                                {
                                    var customizationResult = new CustomizationResult()
                                    {
                                        SiteUrl = alternateCSSResult.SiteUrl,
                                        Url = alternateCSSResult.Url,
                                        IgnoredAlternateCSS = alternateCSSResult.AlternateCSS
                                    };

                                    if (!this.CustomizationResults.TryAdd(customizationResult.Url, customizationResult))
                                    {
                                        UIExperienceScanError error = new UIExperienceScanError()
                                        {
                                            Error = $"Could not add CSS scan result for {customizationResult.Url}",
                                            SiteURL = site,
                                            SiteColUrl = siteCollectionUrl
                                        };
                                        this.UIExpScanErrors.Push(error);
                                        Console.WriteLine($"Could not add CSS scan result for {customizationResult.Url}");
                                    }
                                }
                            }

                            // Custom master page scanner
                            MasterPageScanner masterScanner = new MasterPageScanner(site, siteCollectionUrl);
                            var masterPageResult = masterScanner.Analyze(ccWeb);
                            if (masterPageResult != null)
                            {
                                this.MasterPageResults.Push(masterPageResult);
                                if (this.CustomizationResults.ContainsKey(masterPageResult.Url))
                                {
                                    var customizationResult = this.CustomizationResults[masterPageResult.Url];
                                    customizationResult.IgnoredMasterPage = masterPageResult.MasterPage;
                                    customizationResult.IgnoredCustomMasterPage = masterPageResult.CustomMasterPage;
                                    if (!this.CustomizationResults.TryUpdate(masterPageResult.Url, customizationResult, customizationResult))
                                    {
                                        UIExperienceScanError error = new UIExperienceScanError()
                                        {
                                            Error = $"Could not update MasterPage scan result for {customizationResult.Url}",
                                            SiteURL = site,
                                            SiteColUrl = siteCollectionUrl
                                        };
                                        this.UIExpScanErrors.Push(error);
                                        Console.WriteLine($"Could not update MasterPage scan result for {customizationResult.Url}");
                                    }
                                }
                                else
                                {
                                    var customizationResult = new CustomizationResult()
                                    {
                                        SiteUrl = masterPageResult.SiteUrl,
                                        Url = masterPageResult.Url,
                                        IgnoredMasterPage = masterPageResult.MasterPage,
                                        IgnoredCustomMasterPage = masterPageResult.CustomMasterPage
                                    };

                                    if (!this.CustomizationResults.TryAdd(customizationResult.Url, customizationResult))
                                    {
                                        UIExperienceScanError error = new UIExperienceScanError()
                                        {
                                            Error = $"Could not add MasterPage scan result for {customizationResult.Url}",
                                            SiteURL = site,
                                            SiteColUrl = siteCollectionUrl
                                        };
                                        this.UIExpScanErrors.Push(error);
                                        Console.WriteLine($"Could not add MasterPage scan result for {customizationResult.Url}");
                                    }
                                }
                            }

                            // Custom action scanner
                            CustomActionScanner customActions = new CustomActionScanner(site, siteCollectionUrl);
                            customActions.Analyze(ccWeb, ref this.CustomActionScanResults, ref this.CustomizationResults, ref this.UIExpScanErrors);

                        }
                    }
                    catch (Exception ex)
                    {
                        UIExperienceScanError error = new UIExperienceScanError()
                        {
                            Error = ex.Message,
                            SiteURL = site,
                            SiteColUrl = siteCollectionUrl
                        };
                        this.UIExpScanErrors.Push(error);
                        Console.WriteLine("Error for site {1}: {0}", ex.Message, site);
                    }
                }

                try
                {
                    TimeSpan ts = DateTime.Now.Subtract(this.StartTime);
                    Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}. Processed {this.ScannedSites} of {this.SitesToScan} site collections ({Math.Round(((float)this.ScannedSites / (float)this.SitesToScan) * 100)}%). Process running for {ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error showing progress: {ex.ToString()}");
                }
            }
        }
    }
}