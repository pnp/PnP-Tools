using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Visio.Scanner.Analyzers
{
    /// <summary>
    /// Site collection analyzer
    /// </summary>
    public class SiteAnalyzer : BaseAnalyzer
    {
        // Stores the page search results for all pages in the site collection
        public List<Dictionary<string, string>> PageSearchResults = null;

        #region Construction
        /// <summary>
        /// Site analyzer construction
        /// </summary>
        /// <param name="url">Url of the web to be analyzed</param>
        /// <param name="siteColUrl">Url of the site collection hosting this web</param>
        public SiteAnalyzer(string url, string siteColUrl, VisioScanJob scanJob) : base(url, siteColUrl, scanJob)
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

                // Use search to retrieve all view information for the indexed webpart/wiki/clientside pages in this site collection
                // Need to use search inside this site collection?
                List<string> propertiesToRetrieve = new List<string>
                    {
                        "OriginalPath",
                        "ViewsRecent",
                        "ViewsRecentUniqueUsers",
                        "ViewsLifeTime",
                        "ViewsLifeTimeUniqueUsers"
                    };

                this.PageSearchResults = this.ScanJob.Search(cc.Web, $"path:{this.SiteCollectionUrl} AND fileextension=aspx AND (contentclass=STS_ListItem_WebPageLibrary OR contentclass=STS_Site OR contentclass=STS_Web)", propertiesToRetrieve);

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
