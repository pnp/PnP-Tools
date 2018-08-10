using System;
using SharePoint.Scanning.Framework;
using System.Threading;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.Collections.Concurrent;
using SharePoint.Modernization.Scanner.Results;
using SharePoint.Modernization.Scanner.Analyzers;
using Microsoft.Online.SharePoint.TenantAdministration;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using SharePointPnP.Modernization.Framework;

namespace SharePoint.Modernization.Scanner
{
    public class ModernizationScanJob: ScanJob
    {
        #region Variables
        private Int32 SitesToScan = 0;
        public Mode Mode;
        public bool ExportWebPartProperties;
        public bool SkipUsageInformation;
        public bool SkipUserInformation;
        public string EveryoneExceptExternalUsersClaim = "";
        public readonly string EveryoneClaim = "c:0(.s|true";
        public ConcurrentDictionary<string, WebScanResult> WebScanResults;
        public ConcurrentDictionary<string, SiteScanResult> SiteScanResults;
        public ConcurrentDictionary<string, PageScanResult> PageScanResults;
        public ConcurrentDictionary<string, PublishingScanResult> PublishingScanResults;
        public Tenant SPOTenant;
        public PageTransformation PageTransformation;
        #endregion

        #region Construction
        /// <summary>
        /// Instantiate the scanner
        /// </summary>
        /// <param name="options">Options instance</param>
        public ModernizationScanJob(Options options) : base(options as BaseOptions, "ModernizationScanner", "1.0")
        {
            ExpandSubSites = false;
            Mode = options.Mode;
            ExportWebPartProperties = options.ExportWebPartProperties;
            SkipUsageInformation = options.SkipUsageInformation;
            SkipUserInformation = options.SkipUserInformation;

            this.WebScanResults = new ConcurrentDictionary<string, WebScanResult>(options.Threads, 50000);
            this.SiteScanResults = new ConcurrentDictionary<string, SiteScanResult>(options.Threads, 10000);
            this.PageScanResults = new ConcurrentDictionary<string, PageScanResult>(options.Threads, 1000000);
            this.PublishingScanResults = new ConcurrentDictionary<string, PublishingScanResult>(options.Threads, 1000);

            this.TimerJobRun += ModernizationScanJob_TimerJobRun;
        }
        #endregion

        #region Scanner implementation
        private void ModernizationScanJob_TimerJobRun(object sender, OfficeDevPnP.Core.Framework.TimerJobs.TimerJobRunEventArgs e)
        {
            // Validate ClientContext objects
            if (e.WebClientContext == null || e.SiteClientContext == null)
            {
                ScanError error = new ScanError()
                {
                    Error = "No valid ClientContext objects",
                    SiteURL = e.Url,
                    SiteColUrl = e.Url
                };
                this.ScanErrors.Push(error);
                Console.WriteLine("Error for site {1}: {0}", "No valid ClientContext objects", e.Url);

                // bail out
                return;
            }
            
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
                                ccWeb.Load(spSite, p => p.RootWeb, p => p.Url, p => p.GroupId);
                                ccWeb.Load(spSite.RootWeb, p => p.Id);
                                ccWeb.Load(spSite, p => p.UserCustomActions); // User custom action site level
                                ccWeb.Load(spSite, p => p.Features); // Features site level
                                ccWeb.ExecuteQueryRetry();

                                isFirstSiteInList = false;
                            }

                            // Perf optimization: do one call per web to load all the needed properties
                            ccWeb.Load(ccWeb.Web, p => p.Id, p => p.Title, p => p.Url);
                            ccWeb.Load(ccWeb.Web, p => p.WebTemplate, p => p.Configuration);
                            ccWeb.Load(ccWeb.Web, p => p.MasterUrl, p => p.CustomMasterUrl, // master page check
                                                  p => p.AlternateCssUrl, // Alternate CSS
                                                  p => p.UserCustomActions); // Web user custom actions  
                            ccWeb.Load(ccWeb.Web, p => p.Features); // Features web level
                            ccWeb.Load(ccWeb.Web, p => p.RootFolder); // web home page
                            ccWeb.ExecuteQueryRetry();

                            // Split load in multiple batches to minimize timeout exceptions
                            if (!SkipUserInformation)
                            {
                                ccWeb.Load(ccWeb.Web, p => p.SiteUsers, p => p.AssociatedOwnerGroup, p => p.AssociatedMemberGroup, p => p.AssociatedVisitorGroup); // site user and groups
                                ccWeb.Load(ccWeb.Web, p => p.HasUniqueRoleAssignments, p => p.RoleAssignments, p => p.SiteGroups.Include(s => s.Users)); // permission inheritance at web level
                                ccWeb.ExecuteQueryRetry();

                                ccWeb.Load(ccWeb.Web.AssociatedOwnerGroup, p => p.Users); // users in the Owners group
                                ccWeb.Load(ccWeb.Web.AssociatedMemberGroup, p => p.Users); // users in the Members group
                                ccWeb.Load(ccWeb.Web.AssociatedVisitorGroup, p => p.Users); // users in the Visitors group
                                ccWeb.ExecuteQueryRetry();
                            }

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

                            // Web scan
                            WebAnalyzer webAnalyzer = new WebAnalyzer(site, siteCollectionUrl, this, pageSearchResults);
                            var webScanDuration = webAnalyzer.Analyze(ccWeb);
                        }
                    }
                    catch(Exception ex)
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

            //Perform global initialization tasks, things you only want to do once per run
            if (sites.Count > 0)
            {
                try
                {
                    using (ClientContext cc = this.CreateClientContext(sites[0]))
                    {
                        // The everyone except external users claim is different per tenant, so grab the correct value
                        this.EveryoneExceptExternalUsersClaim = cc.Web.GetEveryoneExceptExternalUsersClaim();
                    }
                }
                catch(Exception)
                {
                    // Catch exceptions here, typical case is if the used site collection was locked. Do one more try with the root site 
                    var uri = new Uri(sites[0]);
                    using (ClientContext cc = this.CreateClientContext($"{uri.Scheme}://{uri.DnsSafeHost}/"))
                    {
                        // The everyone except external users claim is different per tenant, so grab the correct value
                        this.EveryoneExceptExternalUsersClaim = cc.Web.GetEveryoneExceptExternalUsersClaim();
                    }
                }
            }

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
            }

            // Load xml mapping data
            XmlSerializer xmlMapping = new XmlSerializer(typeof(PageTransformation));
            using (var stream = new FileStream("webpartmapping.xml", FileMode.Open))
            {
                this.PageTransformation = (PageTransformation)xmlMapping.Deserialize(stream);
            }

            return sites;
        }

        /// <summary>
        /// Override of the scanner execute method, needed to output our results
        /// </summary>
        /// <returns>Time when scanning was started</returns>
        public override DateTime Execute()
        {
            // Triggers the run of the scanning...will result in ModernizationScanJob_TimerJobRun being called per site collection
            var start = base.Execute();

            // Handle the export of the job specific scanning data
            string outputfile = string.Format("{0}\\ModernizationSiteScanResults.csv", this.OutputFolder);
            string[] outputHeaders = new string[] { "SiteCollectionUrl", "SiteUrl",
                                                    "ReadyForGroupify", "GroupifyBlockers", "GroupifyWarnings", "GroupMode", "PermissionWarnings",
                                                    "ModernHomePage", "ModernUIWarnings",
                                                    "WebTemplate", "Office365GroupId", "MasterPage", "AlternateCSS", "UserCustomActions",
                                                    "SubSites", "SubSitesWithBrokenPermissionInheritance", "ModernPageWebFeatureDisabled", "ModernPageFeatureWasEnabledBySPO",
                                                    "ModernListSiteBlockingFeatureEnabled", "ModernListWebBlockingFeatureEnabled", "SitePublishingFeatureEnabled", "WebPublishingFeatureEnabled",
                                                    "ViewsRecent", "ViewsRecentUniqueUsers", "ViewsLifeTime", "ViewsLifeTimeUniqueUsers",
                                                    "Everyone(ExceptExternalUsers)Claim", "UsesADGroups", "ExternalSharing",
                                                    "Admins", "AdminContainsEveryone(ExceptExternalUsers)Claim", "AdminContainsADGroups",
                                                    "Owners", "OwnersContainsEveryone(ExceptExternalUsers)Claim", "OwnersContainsADGroups",
                                                    "Members", "MembersContainsEveryone(ExceptExternalUsers)Claim", "MembersContainsADGroups",
                                                    "Visitors", "VisitorsContainsEveryone(ExceptExternalUsers)Claim", "VisitorsContainsADGroups"
                                                  };
            Console.WriteLine("Outputting scan results to {0}", outputfile);
            using (StreamWriter outfile = new StreamWriter(outputfile))
            {
                outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
                foreach (var item in this.SiteScanResults)
                {
                    var groupifyBlockers = item.Value.GroupifyBlockers();
                    var groupifyWarnings = item.Value.GroupifyWarnings();
                    var modernWarnings = item.Value.ModernWarnings();
                    var groupSecurity = item.Value.PermissionModel(this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim);

                    outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL),
                                                                                       (groupifyBlockers.Count > 0 ? "FALSE" : "TRUE"), ToCsv(SiteScanResult.FormatList(groupifyBlockers)), ToCsv(SiteScanResult.FormatList(groupifyWarnings)), ToCsv(groupSecurity.Item1), ToCsv(SiteScanResult.FormatList(groupSecurity.Item2)),
                                                                                       item.Value.ModernHomePage, ToCsv(SiteScanResult.FormatList(modernWarnings)),
                                                                                       ToCsv(item.Value.WebTemplate), ToCsv(item.Value.Office365GroupId != Guid.Empty ? item.Value.Office365GroupId.ToString() : ""), item.Value.MasterPage, item.Value.AlternateCSS, ((item.Value.SiteUserCustomActions != null && item.Value.SiteUserCustomActions.Count > 0) || (item.Value.WebUserCustomActions != null && item.Value.WebUserCustomActions.Count > 0)),
                                                                                       item.Value.SubSites, item.Value.SubSitesWithBrokenPermissionInheritance, item.Value.ModernPageWebFeatureDisabled, item.Value.ModernPageFeatureWasEnabledBySPO,
                                                                                       item.Value.ModernListSiteBlockingFeatureEnabled, item.Value.ModernListWebBlockingFeatureEnabled, item.Value.SitePublishingFeatureEnabled, item.Value.WebPublishingFeatureEnabled,
                                                                                       (SkipUsageInformation ? 0: item.Value.ViewsRecent), (SkipUsageInformation ? 0 : item.Value.ViewsRecentUniqueUsers), (SkipUsageInformation ? 0 : item.Value.ViewsLifeTime), (SkipUsageInformation ? 0 : item.Value.ViewsLifeTimeUniqueUsers),
                                                                                       item.Value.EveryoneClaimsGranted, item.Value.ContainsADGroup(), ToCsv(item.Value.SharingCapabilities),
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Admins, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim)), item.Value.HasClaim(item.Value.Admins, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim), item.Value.ContainsADGroup(item.Value.Admins),
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Owners, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim)), item.Value.HasClaim(item.Value.Owners, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim), item.Value.ContainsADGroup(item.Value.Owners),
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Members, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim)), item.Value.HasClaim(item.Value.Members, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim), item.Value.ContainsADGroup(item.Value.Members),
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Visitors, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim)), item.Value.HasClaim(item.Value.Visitors, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim), item.Value.ContainsADGroup(item.Value.Visitors)
                                                )));
                }
            }

            outputfile = string.Format("{0}\\ModernizationWebScanResults.csv", this.OutputFolder);
            outputHeaders = new string[] { "SiteCollectionUrl", "SiteUrl",
                                           "WebTemplate", "BrokenPermissionInheritance", "ModernPageWebFeatureDisabled", "ModernPageFeatureWasEnabledBySPO", "WebPublishingFeatureEnabled",
                                           "MasterPage", "CustomMasterPage", "AlternateCSS", "UserCustomActions",
                                           "Everyone(ExceptExternalUsers)Claim",
                                           "UniqueOwners",
                                           "UniqueMembers",
                                           "UniqueVisitors"
                                         };
            Console.WriteLine("Outputting scan results to {0}", outputfile);
            using (StreamWriter outfile = new StreamWriter(outputfile))
            {
                outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
                foreach (var item in this.WebScanResults)
                {
                    outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL),
                                                                                       ToCsv(item.Value.WebTemplate), item.Value.BrokenPermissionInheritance, item.Value.ModernPageWebFeatureDisabled, item.Value.ModernPageFeatureWasEnabledBySPO, item.Value.WebPublishingFeatureEnabled,
                                                                                       ToCsv(item.Value.MasterPage), ToCsv(item.Value.CustomMasterPage), ToCsv(item.Value.AlternateCSS), (item.Value.WebUserCustomActions.Count > 0),
                                                                                       item.Value.EveryoneClaimsGranted,
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Owners, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim)),
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Members, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim)),
                                                                                       ToCsv(SiteScanResult.FormatUserList(item.Value.Visitors, this.EveryoneClaim, this.EveryoneExceptExternalUsersClaim))
                                                )));
                }
            }

            outputfile = string.Format("{0}\\ModernizationUserCustomActionScanResults.csv", this.OutputFolder);
            outputHeaders = new string[] { "SiteCollectionUrl", "SiteUrl",
                                           "Title", "Name", "Location", "RegistrationType", "RegistrationId", "Reason", "CommandAction", "ScriptBlock", "ScriptSrc"
                                         };
            Console.WriteLine("Outputting scan results to {0}", outputfile);
            using (StreamWriter outfile = new StreamWriter(outputfile))
            {
                outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
                foreach (var item in this.SiteScanResults)
                {
                    if (item.Value.SiteUserCustomActions == null || item.Value.SiteUserCustomActions.Count == 0)
                    {
                        continue;
                    }

                    foreach (var uca in item.Value.SiteUserCustomActions)
                    {
                        outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL),
                                                                                           ToCsv(uca.Title), ToCsv(uca.Name), ToCsv(uca.Location), uca.RegistrationType, ToCsv(uca.RegistrationId), ToCsv(uca.Problem), ToCsv(uca.CommandAction), ToCsv(uca.ScriptBlock), ToCsv(uca.ScriptSrc)
                                                     )));
                    }
                }
                foreach (var item in this.WebScanResults)
                {
                    if (item.Value.WebUserCustomActions == null || item.Value.WebUserCustomActions.Count == 0)
                    {
                        continue;
                    }

                    foreach (var uca in item.Value.WebUserCustomActions)
                    {
                        outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL),
                                                                                           ToCsv(uca.Title), ToCsv(uca.Name), ToCsv(uca.Location), uca.RegistrationType, ToCsv(uca.RegistrationId), ToCsv(uca.Problem), ToCsv(uca.CommandAction), ToCsv(uca.ScriptBlock), ToCsv(uca.ScriptSrc)
                                                     )));
                    }
                }
            }

            if (Options.IncludePage(this.Mode))
            {
                outputfile = string.Format("{0}\\PageScanResults.csv", this.OutputFolder);
                outputHeaders = new string[] { "SiteCollectionUrl", "SiteUrl", "PageUrl", "Library", "HomePage",
                                           "Type", "Layout", "Mapping %", "Unmapped web parts", "ModifiedBy", "ModifiedAt",
                                           "ViewsRecent", "ViewsRecentUniqueUsers", "ViewsLifeTime", "ViewsLifeTimeUniqueUsers"};
                Console.WriteLine("Outputting scan results to {0}", outputfile);

                string header1 = string.Join(this.Separator, outputHeaders);
                string header2 = "";
                for (int i = 1; i <= 30; i++)
                {
                    if (ExportWebPartProperties)
                    {
                        header2 = header2 + $"{this.Separator}WPType{i}{this.Separator}WPTitle{i}{this.Separator}WPData{i}";
                    }
                    else
                    {
                        header2 = header2 + $"{this.Separator}WPType{i}{this.Separator}WPTitle{i}";
                    }
                }

                List<string> UniqueWebParts = new List<string>();
                using (StreamWriter outfile = new StreamWriter(outputfile))
                {
                    outfile.Write(string.Format("{0}\r\n", header1 + header2));
                    foreach (var item in this.PageScanResults)
                    {
                        var part1 = string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL), ToCsv(item.Value.PageUrl), ToCsv(item.Value.Library), item.Value.HomePage,
                                                                ToCsv(item.Value.PageType), ToCsv(item.Value.Layout), "{MappingPercentage}", "{UnmappedWebParts}", ToCsv(item.Value.ModifiedBy), item.Value.ModifiedAt,
                                                                (SkipUsageInformation ? 0 : item.Value.ViewsRecent), (SkipUsageInformation ? 0 : item.Value.ViewsRecentUniqueUsers), (SkipUsageInformation ? 0 : item.Value.ViewsLifeTime), (SkipUsageInformation ? 0 : item.Value.ViewsLifeTimeUniqueUsers));

                        string part2 = "";
                        if (item.Value.WebParts != null)
                        {
                            int webPartsOnPage = item.Value.WebParts.Count();
                            int webPartsOnPageMapped = 0;
                            List<string> nonMappedWebParts = new List<string>();
                            foreach (var webPart in item.Value.WebParts.OrderBy(p => p.Row).ThenBy(p => p.Column).ThenBy(p => p.Order))
                            {
                                var found = this.PageTransformation.WebParts.Where(p => p.Type.Equals(webPart.Type, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (found != null && found.Mappings != null)
                                {
                                    webPartsOnPageMapped++;
                                }
                                else
                                {
                                    var t = webPart.Type.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    if (!nonMappedWebParts.Contains(t))
                                    {
                                        nonMappedWebParts.Add(t);
                                    }
                                }

                                if (ExportWebPartProperties)
                                {
                                    part2 = part2 + $"{this.Separator}{ToCsv(webPart.TypeShort())}{this.Separator}{ToCsv(webPart.Title)}{this.Separator}{ToCsv(webPart.Json())}";
                                }
                                else
                                {
                                    part2 = part2 + $"{this.Separator}{ToCsv(webPart.TypeShort())}{this.Separator}{ToCsv(webPart.Title)}";
                                }

                                if (!UniqueWebParts.Contains(webPart.Type))
                                {
                                    UniqueWebParts.Add(webPart.Type);
                                }
                            }
                            part1 = part1.Replace("{MappingPercentage}", webPartsOnPage == 0 ? "100" : String.Format("{0:0}", (((double)webPartsOnPageMapped / (double)webPartsOnPage) * 100))).Replace("{UnmappedWebParts}", SiteScanResult.FormatList(nonMappedWebParts));
                        }
                        else
                        {
                            part1 = part1.Replace("{MappingPercentage}", "").Replace("{UnmappedWebParts}", "");
                        }

                        outfile.Write(string.Format("{0}\r\n", part1 + (!string.IsNullOrEmpty(part2) ? part2 : "")));
                    }
                }

                outputfile = string.Format("{0}\\UniqueWebParts.csv", this.OutputFolder);
                Console.WriteLine("Outputting scan results to {0}", outputfile);
                using (StreamWriter outfile = new StreamWriter(outputfile))
                {
                    outfile.Write(string.Format("{0}\r\n", $"Type{this.Separator}InMappingFile"));
                    foreach (var type in UniqueWebParts)
                    {
                        var found = this.PageTransformation.WebParts.Where(p => p.Type.Equals(type, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        outfile.Write(string.Format("{0}\r\n", $"{ToCsv(type)}{this.Separator}{found != null}"));
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
