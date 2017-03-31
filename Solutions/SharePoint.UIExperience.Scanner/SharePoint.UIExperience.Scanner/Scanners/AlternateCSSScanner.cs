using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.UIExperience.Scanner.Scanners
{
    /// <summary>
    /// Scans a site for AlternateCSS usage
    /// </summary>
    public class AlternateCSSScanner
    {
        private string url;
        private string siteColUrl;

        public AlternateCSSScanner(string url, string siteColUrl)
        {
            this.url = url;
            this.siteColUrl = siteColUrl;
        }

        /// <summary>
        /// Scans passed web for AlternateCSS usage
        /// </summary>
        /// <param name="cc">ClientContext object of the site to scan</param>
        /// <returns>AlternateCSSResult object containing the AlternateCSS values</returns>
        public AlternateCSSResult Analyze(ClientContext cc)
        {
            Console.WriteLine("AlternateCSS... " + url);
            Web web = cc.Web;
            web.EnsureProperty(p => p.AlternateCssUrl);        

            if (!string.IsNullOrEmpty(web.AlternateCssUrl))
            {
                AlternateCSSResult result = new AlternateCSSResult()
                {
                    Url = this.url,
                    SiteUrl = this.url,
                    SiteColUrl = this.siteColUrl,
                    AlternateCSS = web.AlternateCssUrl
                };

                // Only return when there's a situation that blocks modern
                return result;
            }
            else
            {
                return null;
            }        
        }
    }
}
