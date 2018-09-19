using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Scanner.Results;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Analyzers
{
    /// <summary>
    /// Site collection analyzer
    /// </summary>
    public class SiteAnalyzer : BaseAnalyzer
    {
        // Modern list experience - Site block feature that can be enabled to prevent modern library experience in the complete site collection
        public static readonly Guid FeatureId_Site_ModernList = new Guid("E3540C7D-6BEA-403C-A224-1A12EAFEE4C4");
        // PublishingSite SharePoint Server Publishing Infrastructure - Site. Publishing feature will prevent modern pages
        public static readonly Guid FeatureId_Site_Publishing = new Guid("F6924D36-2FA8-4F0B-B16D-06B7250180FA");
        // Stores the page search results for all pages in the site collection
        public List<Dictionary<string, string>> PageSearchResults = null;

        #region Construction
        /// <summary>
        /// Site analyzer construction
        /// </summary>
        /// <param name="url">Url of the web to be analyzed</param>
        /// <param name="siteColUrl">Url of the site collection hosting this web</param>
        public SiteAnalyzer(string url, string siteColUrl, ModernizationScanJob scanJob) : base(url, siteColUrl, scanJob)
        {
        }
        #endregion

        /// <summary>
        /// Analyze the site collection
        /// </summary>
        /// <param name="cc">ClientContext of the site to be analyzed</param>
        /// <returns>Duration of the analysis</returns>
        public override TimeSpan Analyze(ClientContext cc)
        {
            try
            {
                base.Analyze(cc);
                Site site = cc.Site;
                site.EnsureProperties(p => p.UserCustomActions, p => p.Features, p => p.Url, p => p.GroupId);
                Web web = cc.Web;
                cc.Web.EnsureProperties(p => p.WebTemplate, p => p.Configuration);

                SiteScanResult scanResult = new SiteScanResult()
                {
                    SiteColUrl = this.SiteCollectionUrl,
                    SiteURL = this.SiteUrl,
                };

                // Perform specific analysis work

                // Persist web template of the root site
                scanResult.WebTemplate = $"{web.WebTemplate}#{web.Configuration}";

                // Get security information for this site
                if (!this.ScanJob.SkipUserInformation)
                {
                    scanResult.Admins = web.GetAdmins();
                    scanResult.Owners = web.GetOwners();
                    scanResult.Members = web.GetMembers();
                    scanResult.Visitors = web.GetVisitors();
                    scanResult.Office365GroupId = site.GroupId;
                    scanResult.EveryoneClaimsGranted = web.ClaimsHaveRoleAssignment(this.ScanJob.EveryoneClaim, this.ScanJob.EveryoneExceptExternalUsersClaim);
                }

                scanResult.ModernListSiteBlockingFeatureEnabled = site.Features.Where(f => f.DefinitionId == FeatureId_Site_ModernList).Count() > 0;
                scanResult.SitePublishingFeatureEnabled = site.Features.Where(f => f.DefinitionId == FeatureId_Site_Publishing).Count() > 0;

                // Get site user custom actions
                scanResult.SiteUserCustomActions = site.UserCustomActions.Analyze(this.SiteCollectionUrl, this.SiteUrl);

                if (!this.ScanJob.SkipUsageInformation)
                {
                    // Get site usage information
                    List<string> propertiesToRetrieve = new List<string>
                    {
                        "ViewsRecent",
                        "ViewsRecentUniqueUsers",
                        "ViewsLifeTime",
                        "ViewsLifeTimeUniqueUsers"
                    };

                    var results = this.ScanJob.Search(cc.Web, $"path:{this.SiteCollectionUrl} AND contentclass=STS_Site", propertiesToRetrieve);
                    if (results != null && results.Count == 1)
                    {
                        scanResult.ViewsRecent = results[0]["ViewsRecent"].ToInt32();
                        scanResult.ViewsRecentUniqueUsers = results[0]["ViewsRecentUniqueUsers"].ToInt32();
                        scanResult.ViewsLifeTime = results[0]["ViewsLifeTime"].ToInt32();
                        scanResult.ViewsLifeTimeUniqueUsers = results[0]["ViewsLifeTimeUniqueUsers"].ToInt32();
                    }
                }

                try
                {
                    // Get tenant information
                    var siteInformation = this.ScanJob.SPOTenant.GetSitePropertiesByUrl(this.SiteCollectionUrl, true);
                    this.ScanJob.SPOTenant.Context.Load(siteInformation);
                    this.ScanJob.SPOTenant.Context.ExecuteQueryRetry();

                    if (!siteInformation.ServerObjectIsNull())
                    {
                        scanResult.SharingCapabilities = siteInformation.SharingCapability.ToString();
                    }
                }
                // Eat all exceptions for now
                // TODO move to single loop after scanning has been done - post processing
                catch { }


                if (Options.IncludePage(this.ScanJob.Mode))
                {
                    // Use search to retrieve all view information for the indexed webpart/wiki/clientside pages in this site collection
                    // Need to use search inside this site collection?
                    List<string> propertiesToRetrieveForPage = new List<string>
                    {
                        "OriginalPath",
                        "ViewsRecent",
                        "ViewsRecentUniqueUsers",
                        "ViewsLifeTime",
                        "ViewsLifeTimeUniqueUsers"
                    };

                    this.PageSearchResults = this.ScanJob.Search(cc.Web, $"path:{this.SiteCollectionUrl} AND fileextension=aspx AND (contentclass=STS_ListItem_WebPageLibrary OR contentclass=STS_Site OR contentclass=STS_Web)", propertiesToRetrieveForPage);
                }

                if (!this.ScanJob.SiteScanResults.TryAdd(this.SiteCollectionUrl, scanResult))
                {
                    ScanError error = new ScanError()
                    {
                        Error = $"Could not add site scan result for {this.SiteUrl}",
                        SiteColUrl = this.SiteCollectionUrl,
                        SiteURL = this.SiteUrl,
                        Field1 = "SiteAnalyzer",
                    };
                    this.ScanJob.ScanErrors.Push(error);
                }
            }
            finally
            {
                this.StopTime = DateTime.Now;
            }

            // return the duration of this scan
            return new TimeSpan((this.StopTime.Subtract(this.StartTime).Ticks));
        }
    }
}
