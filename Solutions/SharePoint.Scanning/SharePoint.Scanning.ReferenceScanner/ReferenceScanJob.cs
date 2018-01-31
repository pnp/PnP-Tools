using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SharePoint.Scanning.ReferenceScanner
{
    /// <summary>
    /// Sample scanning job
    /// </summary>
    public class ReferenceScanJob : ScanJob
    {
        #region Variables
        internal List<Mode> ScanModes;
        internal bool UseSearchQuery = false;
        private Int32 SitesToScan = 0;
        public ConcurrentDictionary<string, ScanResult> ScanResults;
        #endregion

        #region Construction
        public ReferenceScanJob(Options options) : base(options as BaseOptions, "ReferenceScanJob", "1.0")
        {
            // Configure job specific settings
            ScanModes = options.ScanModes;
            UseSearchQuery = !options.DontUseSearchQuery;
            ExpandSubSites = false; // false is default value, shown her for demo purposes
            ScanResults = new ConcurrentDictionary<string, ScanResult>(options.Threads, 10000);

            // Connect the eventhandler
            TimerJobRun += ReferenceScanJob_TimerJobRun;
        }
        #endregion

        #region Scanner implementation
        /// <summary>
        /// Grab the number of sites that need to be scanned...will be needed to show progress when we're having a long run
        /// </summary>
        /// <param name="addedSites"></param>
        /// <returns></returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            if (!this.UseSearchQuery)
            {
                var sites = base.ResolveAddedSites(addedSites);
                this.SitesToScan = sites.Count;
                return sites;
            }
            else
            {
                // Use search approach to determine which sites to process
                List<string> searchedSites = new List<string>(100);

                string tenantAdmin = "";
                if (!string.IsNullOrEmpty(this.TenantAdminSite))
                {
                    tenantAdmin = this.TenantAdminSite;
                }
                else
                {
                    if (string.IsNullOrEmpty(this.Tenant))
                    {
                        this.Tenant = new Uri(addedSites[0]).DnsSafeHost.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }

                    tenantAdmin = $"https://{this.Tenant}-admin.sharepoint.com";
                }

                this.Realm = GetRealmFromTargetUrl(new Uri(tenantAdmin));


                using (ClientContext ccAdmin = this.CreateClientContext(tenantAdmin))
                {
                    List<string> propertiesToRetrieve = new List<string>
                    {
                        "Title",
                        "SPSiteUrl",
                        "FileExtension",
                        "OriginalPath"
                    };

                    // Get sites with a given web template
                    //var results = Search(ccAdmin.Web, "contentclass:STS_Web (WebTemplate:ACCSVC OR WebTemplate:ACCSRV)", propertiesToRetrieve);
                    // Get sites that contain a certain set of files
                    var results = this.Search(ccAdmin.Web, "((fileextension=htm OR fileextension=html) AND contentclass=STS_ListItem_DocumentLibrary)", propertiesToRetrieve);
                    foreach (var site in results)
                    {
                        if (!string.IsNullOrEmpty(site["SPSiteUrl"]) && !searchedSites.Contains(site["SPSiteUrl"]))
                        {
                            searchedSites.Add(site["SPSiteUrl"]);
                        }
                    }
                }

                this.SitesToScan = searchedSites.Count;
                return searchedSites;
            }
        }

        private void ReferenceScanJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            // thread safe increase of the sites counter
            IncreaseScannedSites();

            try
            {
                Console.WriteLine("Processing site {0}...", e.Url);

                #region Basic sample
                /*
                // Set the first site collection done flag + perform telemetry
                SetFirstSiteCollectionDone(e.WebClientContext);

                // add your custom scan logic here, ensure the catch errors as we don't want to terminate scanning
                e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
                e.WebClientContext.ExecuteQueryRetry();
                ScanResult result = new ScanResult()
                {
                    SiteColUrl = e.Url,
                    SiteURL = e.Url,
                    SiteTitle = e.WebClientContext.Web.Title
                };

                // Store the scan result
                if (!ScanResults.TryAdd(e.Url, result))
                {
                    ScanError error = new ScanError()
                    {
                        SiteURL = e.Url,
                        SiteColUrl = e.Url,
                        Error = "Could not add scan result for this site"
                    };
                    this.ScanErrors.Push(error);
                }
                */
                #endregion

                #region Search based sample
                /**
                // Set the first site collection done flag + perform telemetry
                SetFirstSiteCollectionDone(e.WebClientContext);

                // Need to use search inside this site collection?
                List<string> propertiesToRetrieve = new List<string>
                {
                    "Title",
                    "SPSiteUrl",
                    "FileExtension",
                    "OriginalPath"
                };
                var searchResults = this.Search(e.SiteClientContext.Web, $"((fileextension=htm OR fileextension=html) AND contentclass=STS_ListItem_DocumentLibrary AND Path:{e.Url.TrimEnd('/')}/*)", propertiesToRetrieve);
                foreach (var searchResult in searchResults)
                {

                    ScanResult result = new ScanResult()
                    {
                        SiteColUrl = e.Url,
                        FileName = searchResult["OriginalPath"]
                    };

                    // Get web url
                    var webUrlData = Web.GetWebUrlFromPageUrl(e.SiteClientContext, result.FileName);
                    e.SiteClientContext.ExecuteQueryRetry();
                    result.SiteURL = webUrlData.Value;

                    // Store the scan result, use FileName as unique key in this sample
                    if (!ScanResults.TryAdd(result.FileName, result))
                    {
                        ScanError error = new ScanError()
                        {
                            SiteURL = e.Url,
                            SiteColUrl = e.Url,
                            Error = "Could not add scan result for this web"
                        };
                        this.ScanErrors.Push(error);
                    }
                }
                **/
                #endregion

                #region Sub site iteration sample                
                // Set the first site collection done flag + perform telemetry
                SetFirstSiteCollectionDone(e.WebClientContext);

                // Manually iterate over the content
                IEnumerable<string> expandedSites = GetAllSubSites(e.SiteClientContext.Site);
                bool isFirstSiteInList = true;
                string siteCollectionUrl = "";

                foreach (string site in expandedSites)
                {
                    // thread safe increase of the webs counter
                    IncreaseScannedWebs();

                    // Clone the existing ClientContext for the sub web
                    using (ClientContext ccWeb = e.SiteClientContext.Clone(site))
                    {
                        Console.WriteLine("Processing site {0}...", site);

                        if (isFirstSiteInList)
                        {
                            // Perf optimization: do one call per site to load all the needed properties
                            var spSite = (ccWeb as ClientContext).Site;
                            ccWeb.Load(spSite, p => p.RootWeb, p => p.Url); 
                            ccWeb.Load(spSite.RootWeb, p => p.Id); 

                            isFirstSiteInList = false;
                        }

                        // Perf optimization: do one call per web to load all the needed properties
                        ccWeb.Load(ccWeb.Web, p => p.Id, p => p.Title);
                        ccWeb.Load(ccWeb.Web, p => p.WebTemplate, p => p.Configuration);
                        ccWeb.Load(ccWeb.Web, p => p.Lists.Include(li => li.UserCustomActions, li => li.Title, li => li.Hidden, li => li.DefaultViewUrl, li => li.BaseTemplate, li => li.RootFolder, li => li.ListExperienceOptions));                                                                                         
                        ccWeb.ExecuteQueryRetry();

                        // Fill site collection url
                        if (string.IsNullOrEmpty(siteCollectionUrl))
                        {
                            siteCollectionUrl = ccWeb.Site.Url;
                        }

                        // Need to know if this is a sub site?
                        if (ccWeb.Web.IsSubSite())
                        {
                            // Sub site specific logic
                        }

                        ScanResult result = new ScanResult()
                        {
                            SiteColUrl = e.Url,
                            SiteURL = site,
                            SiteTitle = ccWeb.Web.Title,
                        };

                        // Store the scan result
                        if (!ScanResults.TryAdd(site, result))
                        {
                            ScanError error = new ScanError()
                            {
                                SiteURL = site,
                                SiteColUrl = e.Url,
                                Error = "Could not add scan result for this web"
                            };
                            this.ScanErrors.Push(error);
                        }
                    }
                }                
                #endregion
            }
            catch (Exception ex)
            {
                ScanError error = new ScanError()
                {
                    Error = ex.Message,
                    SiteColUrl = e.Url,
                    SiteURL = e.Url,
                    Field1 = "put additional info here"
                };
                this.ScanErrors.Push(error);
                Console.WriteLine("Error for site {1}: {0}", ex.Message, e.Url);
            }

            // Output the scanning progress
            try
            {
                TimeSpan ts = DateTime.Now.Subtract(this.StartTime);
                Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}. Processed {this.ScannedSites} of {this.SitesToScan} site collections ({Math.Round(((float)this.ScannedSites / (float)this.SitesToScan) * 100)}%). Process running for {ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing progress: {ex.ToString()}");
            }

        }

        public override DateTime Execute()
        {
            // Triggers the run of the scanning...will result in ReferenceScanJob_TimerJobRun being called per site collection or per site
            var start = base.Execute();

            // Handle the export of the job specific scanning data
            string outputfile = string.Format("{0}\\ReferenceScanResults.csv", this.OutputFolder);
            string[] outputHeaders = new string[] {"Site Collection Url", "Site Url", "Title", "File name" };
            Console.WriteLine("Outputting reference scan results to {0}", outputfile);
            System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
            foreach (var item in this.ScanResults)
            {
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL), ToCsv(item.Value.SiteTitle), ToCsv(item.Value.FileName))));
            }

            Console.WriteLine("=====================================================");
            Console.WriteLine("All done. Took {0} for {1} sites", (DateTime.Now - start).ToString(), this.ScannedSites);
            Console.WriteLine("=====================================================");

            return start;
        }
        #endregion
    }
}
