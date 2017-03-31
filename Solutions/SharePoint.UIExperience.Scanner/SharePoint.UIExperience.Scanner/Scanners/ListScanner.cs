using Microsoft.SharePoint.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SharePoint.UIExperience.Scanner.Scanners
{
    /// <summary>
    /// Scans a site for lists that are not compatible with modern
    /// </summary>
    public class ListScanner
    {
        private string url;
        private string siteColUrl;
        private bool excludeListsOnlyBlockedByOobReasons = false;

        public ListScanner(string url, string siteColUrl, bool excludeListsOnlyBlockedByOobReasons)
        {
            this.url = url;
            this.siteColUrl = siteColUrl;
            this.excludeListsOnlyBlockedByOobReasons = excludeListsOnlyBlockedByOobReasons;
        }

        /// <summary>
        /// Analyzes lists in the passed site for modern compatibility
        /// </summary>
        /// <param name="cc">ClientContext object of the site to scan</param>
        /// <param name="ListResults">Collection of ListResult objects</param>
        public int Analyze(ClientContext cc, ref ConcurrentDictionary<string, ListResult> ListResults, ref ConcurrentStack<UIExperienceScanError> UIExpScanErrors)
        {
            Console.WriteLine("List compatability... " + this.url);
            var baseUri = new Uri(url);
            var webAppUrl = baseUri.Scheme + "://" + baseUri.Host;

            var lists = cc.Web.GetListsToScan();

            foreach (var list in lists)
            {
                ListResult listResult;
                if (list.DefaultViewUrl.ToLower().Contains(".aspx"))
                {
                    File file = cc.Web.GetFileByServerRelativeUrl(list.DefaultViewUrl);
                    listResult = file.ModernCompatability(list, ref UIExpScanErrors);
                }
                else
                {
                    listResult = new ListResult()
                    {
                        BlockedByNotBeingAbleToLoadPage = true
                    };
                }

                if (listResult != null && !listResult.WorksInModern)
                {
                    if (excludeListsOnlyBlockedByOobReasons && listResult.OnlyBlockedByOOBReasons)
                    {
                        continue;
                    }

                    listResult.SiteUrl = this.url;                    
                    listResult.Url = $"{webAppUrl}{list.DefaultViewUrl}";
                    listResult.SiteColUrl = this.siteColUrl;
                    listResult.ListTitle = list.Title;
                    if (!ListResults.TryAdd(listResult.Url, listResult))
                    {
                        UIExperienceScanError error = new UIExperienceScanError()
                        {
                            Error = $"Could not add list scan result for {listResult.Url}",
                            SiteURL = this.url,
                            SiteColUrl = this.siteColUrl
                        };
                        UIExpScanErrors.Push(error);
                        Console.WriteLine($"Could not add list scan result for {listResult.Url}");
                    }
                }
            }
            return lists.Count;
        }       
    }
}
