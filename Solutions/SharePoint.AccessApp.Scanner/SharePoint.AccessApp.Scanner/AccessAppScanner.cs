using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using SharePoint.AccessApp.Scanner.Framework.TimerJobs;
using SharePoint.AccessApp.Scanner.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharePoint.AccessApp.Scanner
{
    public class AccessAppScanner: TimerJob
    {
        public ConcurrentStack<AccessAppScanData> AccessAppResults = new ConcurrentStack<AccessAppScanData>();
        public ConcurrentStack<AccessAppScanError> AccessAppScanErrors = new ConcurrentStack<AccessAppScanError>();

        public Int32 ScannedSites = 0;
        public Int32 ScannedWebs = 0;
        public DateTime StartTime;
        public string OutputFolder = "";
        public string Separator = ",";
        public bool ExcludeListsOnlyBlockedByOobReasons = false;
        public string Tenant = "";
        public bool UseSearchQuery = false;
        internal List<Mode> Modes;

        private static volatile bool firstSiteCollectionDone = false;
        private object scannedSitesLock = new object();
        private object scannedWebsLock = new object();
        private Int32 SitesToScan = 0;

        public AccessAppScanner() : base("AccessAppScanner")
        {
            TimerJobRun += AccessAppScanner_TimerJobRun;
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
            if (!this.UseSearchQuery)
            {
                var sites = base.ResolveAddedSites(addedSites);
                this.SitesToScan = sites.Count;
                return sites;
            }
            else
            {
                List<string> searchedSites = new List<string>(100);

                string tenantAdmin = "";
                if (!string.IsNullOrEmpty(this.TenantAdminSite))
                {
                    tenantAdmin = this.TenantAdminSite;
                }
                else
                {
                    tenantAdmin = $"https://{this.Tenant}-admin.sharepoint.com";
                }

                this.Realm = TokenHelper.GetRealmFromTargetUrl(new Uri(tenantAdmin));
                using (ClientContext ccAdmin = this.CreateClientContext(tenantAdmin))
                {
                    var results = ccAdmin.Web.SiteSearch("contentclass:STS_Web (WebTemplate:ACCSVC OR WebTemplate:ACCSRV)");
                    foreach (var site in results)
                    {
                        if (!searchedSites.Contains(site.Url))
                        {
                            searchedSites.Add(site.Url);
                        }
                    }
                }
                this.SitesToScan = searchedSites.Count;
                return searchedSites;
            }
        }

        /// <summary>
        /// Event handler that's being executed by the threads processing the sites. Everything in here must be coded in a thread-safe manner
        /// </summary>
        private void AccessAppScanner_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            try
            {
                IEnumerable<string> expandedSites = null;
                if (!this.UseSearchQuery)
                {
                    // Get all the sub sites in the site we're processing
                    expandedSites = GetAllSubSites(e.SiteClientContext.Site);
                }
                else
                {
                    expandedSites = new List<string>();
                    (expandedSites as List<string>).Add(e.Url);
                }

                bool isFirstSiteInList = true;
                string siteCollectionUrl = "";
                int viewsRecent = -1;
                int viewsRecentUnique = -1;
                int viewsLifetime = -1;
                int viewsLifetimeUnique = -1;

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

                    // Create a client context using a AuthenticationManager per domain (without this you'll get access denied on the app domains)
                    using (ClientContext ccWeb = this.CreateClientContext(site))
                    {
                        ClientResult<ResultTableCollection> siteQueryResults = null;

                        Console.WriteLine("Processing site {0}...", site);
                        try
                        {
                            if (!firstSiteCollectionDone)
                            {
                                firstSiteCollectionDone = true;

                                // Telemetry
                                ccWeb.ClientTag = "SPDev:AccessAppScanner";
                                ccWeb.Load(ccWeb.Web, p => p.Description, p => p.Id);
                                ccWeb.ExecuteQuery();
                            }

                            if (isFirstSiteInList)
                            {
                                // Perf optimization: do one call per site to load all the needed properties
                                var spSite = (ccWeb as ClientContext).Site;
                                ccWeb.Load(spSite, p => p.RootWeb, p => p.Url); // Needed in IsSubSite
                                ccWeb.Load(spSite.RootWeb, p => p.Id); // Needed in IsSubSite

                                isFirstSiteInList = false;
                            }

                            // Perf optimization: do one call per web to load all the needed properties
                            ccWeb.Load(ccWeb.Web, p => p.Id, p => p.WebTemplate, p => p.Configuration, p => p.Title, p => p.Created, p => p.AppInstanceId, p => p.ParentWeb, p => p.LastItemUserModifiedDate);
                            ccWeb.ExecuteQueryRetry();

                            // Fill site collection url
                            if (string.IsNullOrEmpty(siteCollectionUrl))
                            {
                                siteCollectionUrl = ccWeb.Site.Url;
                            }

                            // Scanning to identify sites that can't show modern pages
                            if (Modes.Contains(Mode.Scan))
                            {
                                if (ccWeb.Web.WebTemplate.Equals("ACCSVC", StringComparison.InvariantCultureIgnoreCase) ||
                                    ccWeb.Web.WebTemplate.Equals("ACCSRV", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    // Get the date when this Access App was last accessed
                                    var lastAccessedDate = ccWeb.Web.GetPropertyBagValueString("accsvcLastAccessedDate", "");
                                    var lastModifiedDate = ccWeb.Web.LastItemUserModifiedDate;
                                                                       
                                    // Query for usage on the actual site collection, do this only once per site collection
                                    if (viewsRecent == -1)
                                    {
                                        KeywordQuery siteQuery = new KeywordQuery(e.SiteClientContext);
                                        siteQuery.QueryText = string.Format("path:{0} NOT(contentclass:STS_Site) NOT(contentclass:STS_Web)", siteCollectionUrl);
                                        siteQuery.SelectProperties.Clear();
                                        siteQuery.SelectProperties.Add("ViewsRecent");
                                        siteQuery.SelectProperties.Add("ViewsRecentUniqueUsers");
                                        siteQuery.SelectProperties.Add("ViewsLifeTime");
                                        siteQuery.SelectProperties.Add("ViewsLifeTimeUniqueUsers");
                                        siteQuery.SortList.Add("ViewsRecent", SortDirection.Descending);
                                        siteQuery.TrimDuplicates = false;
                                        siteQuery.RowLimit = 500;

                                        SearchExecutor seachExecutor = new SearchExecutor(e.SiteClientContext);
                                        siteQueryResults = seachExecutor.ExecuteQuery(siteQuery);
                                        e.SiteClientContext.ExecuteQueryRetry();

                                        // Fill site usage information
                                        if (siteQueryResults != null)
                                        {
                                            viewsRecent = 0;
                                            viewsRecentUnique = 0;
                                            viewsLifetime = 0;
                                            viewsLifetimeUnique = 0;

                                            if (siteQueryResults.Value != null)
                                            {
                                                foreach (ResultTable t in siteQueryResults.Value)
                                                {
                                                    foreach (IDictionary<string, object> r in t.ResultRows)
                                                    {
                                                        viewsRecent += r["ViewsRecent"] != null ? int.Parse(r["ViewsRecent"].ToString()) : 0;
                                                        viewsRecentUnique += r["ViewsRecentUniqueUsers"] != null ? int.Parse(r["ViewsRecentUniqueUsers"].ToString()) : 0;
                                                        viewsLifetime += r["ViewsLifeTime"] != null ? int.Parse(r["ViewsLifeTime"].ToString()) : 0;
                                                        viewsLifetimeUnique += r["ViewsLifeTimeUniqueUsers"] != null ? int.Parse(r["ViewsLifeTimeUniqueUsers"].ToString()) : 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    var accessAppResult = new AccessAppScanData()
                                    {
                                        ViewsRecent = viewsRecent,
                                        ViewsRecentUnique = viewsRecentUnique,
                                        ViewsLifetime = viewsLifetime,
                                        ViewsLifetimeUnique = viewsLifetimeUnique,
                                        SiteColUrl = siteCollectionUrl,
                                        SiteUrl = site,
                                        WebTitle = ccWeb.Web.Title,
                                        WebCreatedDate = ccWeb.Web.Created,
                                        WebTemplate = $"{ccWeb.Web.WebTemplate}#{ccWeb.Web.Configuration}",
                                        AppInstanceId = ccWeb.Web.AppInstanceId,
                                        WebId = ccWeb.Web.Id,
                                        LastAccessedDate = lastAccessedDate,
                                        LastModifiedByUserDate = (ccWeb.Web.WebTemplate.Equals("ACCSRV", StringComparison.InvariantCultureIgnoreCase) ? lastModifiedDate.ToString() : "")
                                    };

                                    Console.WriteLine($"Access App found in {site}.");

                                    if (accessAppResult.AppInstanceId != Guid.Empty)
                                    {
                                        Uri siteCollectionUri = new Uri(siteCollectionUrl);
                                        string parentWebUrl = $"{siteCollectionUri.Scheme}://{siteCollectionUri.DnsSafeHost}:{siteCollectionUri.Port}{ccWeb.Web.ParentWeb.ServerRelativeUrl}";
                                        using (var ccParent = this.CreateClientContext(parentWebUrl))
                                        {
                                            try
                                            {
                                                var appInstance = ccParent.Web.GetAppInstanceById(accessAppResult.AppInstanceId);
                                                ccParent.Load(appInstance);
                                                ccParent.ExecuteQueryRetry();

                                                accessAppResult.ParentSiteUrl = parentWebUrl;
                                                accessAppResult.AppInstanceStatus = appInstance.Status.ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                                AccessAppScanError error = new AccessAppScanError()
                                                {
                                                    Error = ex.Message,
                                                    SiteUrl = site,
                                                    SiteColUrl = siteCollectionUrl
                                                };
                                                this.AccessAppScanErrors.Push(error);
                                                Console.WriteLine("Error while reading Access App status for {1}: {0}", ex.Message, site);
                                            }
                                        }
                                    }
                                    AccessAppResults.Push(accessAppResult);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            AccessAppScanError error = new AccessAppScanError()
                            {
                                Error = ex.Message,
                                SiteUrl = site,
                                SiteColUrl = siteCollectionUrl
                            };
                            this.AccessAppScanErrors.Push(error);
                            Console.WriteLine("Error for site {1}: {0}", ex.Message, site);
                        }
                    }

                    try
                    {
                        TimeSpan ts = DateTime.Now.Subtract(this.StartTime);
                        Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}. Processed {this.ScannedSites} of {this.SitesToScan} site collections ({Math.Round(((float)this.ScannedSites / (float)this.SitesToScan) * 100)}%). Process running for {ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error showing progress: {ex.ToDetailedString()}");
                    }
                }
            }
            catch(Exception ex)
            {
                AccessAppScanError error = new AccessAppScanError()
                {
                    Error = ex.Message,
                    SiteUrl = e.SiteClientContext.Site.Url,
                    SiteColUrl = e.SiteClientContext.Site.Url
                };
                this.AccessAppScanErrors.Push(error);
                Console.WriteLine("Error for site {1}: {0}", ex.Message, e.SiteClientContext.Site.Url);
            }
        }
    }
}
