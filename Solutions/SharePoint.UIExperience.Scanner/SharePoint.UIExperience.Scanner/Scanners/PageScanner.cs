using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.UIExperience.Scanner.Scanners
{
    /// <summary>
    /// Scans for sites where modern pages are not working or supported
    /// </summary>
    public class PageScanner
    {
        // Modern page experience - Site feature that needs to be enabled to support modern page creation
        private static readonly Guid FeatureId_Site_ModernPage = new Guid("B6917CB1-93A0-4B97-A84D-7CF49975D4EC");
        // PublishingWeb SharePoint Server Publishing - Web. Publishing feature will prevent modern pages
        private static readonly Guid FeatureId_Web_Publishing = new Guid("94C94CA6-B32F-4DA9-A9E3-1F3D343D7ECB");
        // PublishingSite SharePoint Server Publishing Infrastructure - Site. Publishing feature will prevent modern pages
        private static readonly Guid FeatureId_Site_Publishing = new Guid("F6924D36-2FA8-4F0B-B16D-06B7250180FA");
        // Site Page content type
        private static readonly string SitePageContentTypeId = "0x0101009D1CB255DA76424F860D91F20E6C4118";

        private string url;
        private string siteColUrl;

        public PageScanner(string url, string siteColUrl)
        {
            this.url = url;
            this.siteColUrl = siteColUrl;
        }

        /// <summary>
        /// Analyze site for page compatibility
        /// </summary>
        /// <param name="cc">ClientContext object of the site to scan</param>
        /// <returns>PageResult Object representing compatibility</returns>
        public PageResult Analyze(ClientContext cc)
        {

            Console.WriteLine("Page compatability... " + this.url);
            Web web = cc.Web;

            PageResult featureResult = new PageResult()
            {
                Url = this.url,
                SiteUrl = this.url,
                SiteColUrl = this.siteColUrl,
            };
            cc.Web.EnsureProperties(p => p.Features, p => p.WebTemplate, p => p.Configuration);

            // Log used web template
            if (web.WebTemplate != null)
            {
                featureResult.WebTemplate = $"{web.WebTemplate}#{web.Configuration}";
            }

            // Page feature check: users can disable this to prevent modern page creation
            featureResult.BlockedViaDisabledModernPageWebFeature = web.Features.Where(f => f.DefinitionId == FeatureId_Site_ModernPage).Count() == 0;

            // If the web template is STS#0 or GROUP#0 then the feature was activated by SPO, other templates never got it
            featureResult.WasEnabledBySPO = false;
            if (featureResult.WebTemplate.Equals("STS#0", StringComparison.InvariantCultureIgnoreCase) || featureResult.WebTemplate.Equals("GROUP#0", StringComparison.InvariantCulture))
            {
                // Since we did not enable the feature for all STS#0 sites (sites with publishing active did not get it, nor sites having a large number of webpart/wiki pages) we
                // check if it should have been turned on by checking for the "site page" content type being added to the site pages library                
                var listsToScan = cc.Web.GetListsToScan();
                var sitePagesLibrary = listsToScan.Where(p => p.BaseTemplate == (int)ListTemplateType.WebPageLibrary).FirstOrDefault();

                if (sitePagesLibrary != null)
                {
                    cc.Load(sitePagesLibrary, p => p.ContentTypes.Include(c => c.Id));
                    cc.ExecuteQueryRetry();
                    if (sitePagesLibrary.ContentTypes.BestMatch(SitePageContentTypeId) != null)
                    {
                        featureResult.WasEnabledBySPO = true;
                    }
                }
            }

            // Only return when there's a situation that blocks modern
            if (featureResult.BlockedViaDisabledModernPageWebFeature)
            {
                return featureResult;
            }
            else
            {
                return null;
            }
        }

    }
}
