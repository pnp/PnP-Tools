using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using SharePoint.Scanning.Framework;
using SharePoint.Visio.Scanner.Analyzers;
using SharePoint.Visio.Scanner.Results;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharePoint.Visio.Scanner
{
    public class VisioScanJob : ScanJob
    {
        #region Variables
        private Int32 SitesToScan = 0;
        public Mode Mode;
        public ConcurrentDictionary<string, VdwScanResult> VdwScanResults;
        public ConcurrentDictionary<string, VisioWebPartScanResult> VisioWebPartScanResults;
        public Tenant SPOTenant;
        #endregion

        #region Construction
        /// <summary>
        /// Instantiate the scanner
        /// </summary>
        /// <param name="options">Options instance</param>
        public VisioScanJob(Options options) : base(options as BaseOptions, "VisioScanner", "1.0")
        {
            ExpandSubSites = false;
            Mode = options.Mode;

            this.VdwScanResults = new ConcurrentDictionary<string, VdwScanResult>(options.Threads, 50000);
            this.VisioWebPartScanResults = new ConcurrentDictionary<string, VisioWebPartScanResult>(options.Threads, 50000);

            // Web part scan if mode == full
            if (this.Mode == Mode.Full)
            {
                this.TimerJobRun += VisioScanJob_TimerJobRun;
            }
        }
        #endregion

        #region Scanner implementation
        private void VisioScanJob_TimerJobRun(object sender, OfficeDevPnP.Core.Framework.TimerJobs.TimerJobRunEventArgs e)
        {
            // thread safe increase of the sites counter
            IncreaseScannedSites();

            try
            {
                // Set the first site collection done flag + perform telemetry
                SetFirstSiteCollectionDone(e.WebClientContext, this.Name);

                // Manually iterate over the content
                IEnumerable<string> expandedSites = e.SiteClientContext.Site.GetAllSubSites();
                bool isFirstSiteInList = true;
                string siteCollectionUrl = "";
                List<Dictionary<string, string>> pageSearchResults = null;

                foreach (string site in expandedSites)
                {
                    try
                    {
                        // thread safe increase of the webs counter
                        IncreaseScannedWebs();

                        // Clone the existing ClientContext for the sub web
                        using (ClientContext ccWeb = e.SiteClientContext.Clone(site))
                        {
                            Console.WriteLine("Processing site {0}...", site);

                            // Allow max server time out, might be needed for sites having a lot of users
                            ccWeb.RequestTimeout = Timeout.Infinite;

                            if (isFirstSiteInList)
                            {
                                // Perf optimization: do one call per site to load all the needed properties
                                var spSite = (ccWeb as ClientContext).Site;
                                ccWeb.Load(spSite, p => p.Url);
                                ccWeb.ExecuteQueryRetry();

                                isFirstSiteInList = false;
                            }

                            ListCollection listCollection = ccWeb.Web.Lists;
                            ccWeb.Load(listCollection, coll => coll.Include(li => li.Title, li => li.Hidden, li => li.DefaultViewUrl, li => li.BaseTemplate, li => li.RootFolder));
                            ccWeb.ExecuteQueryRetry();

                            // Do things only once per site collection
                            if (string.IsNullOrEmpty(siteCollectionUrl))
                            {
                                // Cross check Url property availability
                                ccWeb.Site.EnsureProperty(s => s.Url);
                                siteCollectionUrl = ccWeb.Site.Url;

                                // Site scan
                                SiteAnalyzer siteAnalyzer = new SiteAnalyzer(site, siteCollectionUrl, this);
                                var siteScanDuration = siteAnalyzer.Analyze(ccWeb);
                                pageSearchResults = siteAnalyzer.PageSearchResults;
                            }

                            VisioWebPartAnalyzer visioWebPartAnalyzer = new VisioWebPartAnalyzer(site, siteCollectionUrl, this, pageSearchResults);
                            visioWebPartAnalyzer.Analyze(ccWeb);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScanError error = new ScanError()
                        {
                            Error = ex.Message,
                            SiteColUrl = e.Url,
                            SiteURL = site,
                            Field1 = "MainWebLoop",
                            Field2 = ex.StackTrace,
                        };
                        this.ScanErrors.Push(error);
                        Console.WriteLine("Error for site {1}: {0}", ex.Message, site);
                    }
                }
            }
            catch (Exception ex)
            {
                ScanError error = new ScanError()
                {
                    Error = ex.Message,
                    SiteColUrl = e.Url,
                    SiteURL = e.Url,
                    Field1 = "MainSiteLoop",
                    Field2 = ex.StackTrace,
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

        /// <summary>
        /// Grab the number of sites that need to be scanned...will be needed to show progress when we're having a long run
        /// </summary>
        /// <param name="addedSites">List of sites found to scan</param>
        /// <returns>Updated list of sites to scan</returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            var sites = base.ResolveAddedSites(addedSites);
            this.SitesToScan = sites.Count;

            // Setup tenant context
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
                this.SPOTenant = new Tenant(ccAdmin);

                List<string> propertiesToRetrieve = new List<string>
                    {
                        "SPSiteUrl",
                        "SPWebUrl",
                        "FileExtension",
                        "OriginalPath"
                    };

                // Do one tenant wide search to find all VDW files
                Console.WriteLine("Phase 1: Get all the VDW files...");
                var results = this.Search(ccAdmin.Web, $"fileextension=VDW", propertiesToRetrieve);
                foreach (var vdw in results)
                {
                    if (!string.IsNullOrEmpty(vdw["OriginalPath"]))
                    {
                        this.VdwScanResults.TryAdd(vdw["OriginalPath"], new VdwScanResult()
                        {
                            SiteColUrl = !string.IsNullOrEmpty(vdw["SPSiteUrl"]) ? vdw["SPSiteUrl"] : "",
                            SiteURL = !string.IsNullOrEmpty(vdw["SPWebUrl"]) ? vdw["SPWebUrl"] : "",
                            FileExtension = !string.IsNullOrEmpty(vdw["FileExtension"]) ? vdw["FileExtension"] : "",
                            OriginalPath = !string.IsNullOrEmpty(vdw["OriginalPath"]) ? vdw["OriginalPath"] : "",

                        });
                    }
                }
            }

            return sites;
        }

        /// <summary>
        /// Override of the scanner execute method, needed to output our results
        /// </summary>
        /// <returns>Time when scanning was started</returns>
        public override DateTime Execute()
        {
            // Triggers the run of the scanning...will result in VisioScanJob_TimerJobRun being called per site collection
            var start = base.Execute();

            string outputfile = string.Format("{0}\\VisioVDWResults.csv", this.OutputFolder);
            string[] outputHeaders = new string[] { "SiteCollectionUrl", "SiteUrl", "OriginalPath" };
            Console.WriteLine("Outputting scan results to {0}", outputfile);
            using (StreamWriter outfile = new StreamWriter(outputfile))
            {
                outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
                foreach (var item in this.VdwScanResults)
                {
                    outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL), ToCsv(item.Value.OriginalPath))));
                }
            }

            if (this.Mode == Mode.Full)
            {
                outputfile = string.Format("{0}\\VisioWebPartResults.csv", this.OutputFolder);
                outputHeaders = new string[] { "SiteCollectionUrl", "SiteUrl", "PageUrl", "Library",
                                               "ModifiedBy", "ModifiedAt", "ViewsRecent", "ViewsRecentUniqueUsers", "ViewsLifeTime", "ViewsLifeTimeUniqueUsers"};
                Console.WriteLine("Outputting scan results to {0}", outputfile);

                string header1 = string.Join(this.Separator, outputHeaders);
                string header2 = "";
                for (int i = 1; i <= 15; i++)
                {
                    header2 = header2 + $"{this.Separator}WPType{i}{this.Separator}WPTitle{i}{this.Separator}WPData{i}";
                }

                using (StreamWriter outfile = new StreamWriter(outputfile))
                {
                    outfile.Write(string.Format("{0}\r\n", header1 + header2));

                    foreach (var item in this.VisioWebPartScanResults)
                    {
                        var part1 = string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL), ToCsv(item.Value.PageUrl), ToCsv(item.Value.Library), 
                                                                ToCsv(item.Value.ModifiedBy), item.Value.ModifiedAt, item.Value.ViewsRecent, item.Value.ViewsRecentUniqueUsers, item.Value.ViewsLifeTime, item.Value.ViewsLifeTimeUniqueUsers);

                        string part2 = "";
                        if (item.Value.WebParts != null)
                        {
                            foreach (var webPart in item.Value.WebParts.OrderBy(p => p.Order))
                            {
                                part2 = part2 + $"{this.Separator}{ToCsv(webPart.TypeShort())}{this.Separator}{ToCsv(webPart.Title)}{this.Separator}{ToCsv(webPart.Json())}";
                            }
                        }
                        outfile.Write(string.Format("{0}\r\n", part1 + (!string.IsNullOrEmpty(part2) ? part2 : "")));
                    }
                }
            }

            Console.WriteLine("=====================================================");
            Console.WriteLine("All done. Took {0} for {1} sites", (DateTime.Now - start).ToString(), this.ScannedSites);
            Console.WriteLine("=====================================================");

            return start;
        }
        #endregion


    }
}
