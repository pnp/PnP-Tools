using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.UIExperience.Scanner.Scanners
{
    /// <summary>
    /// Scan a site for custom master page usage
    /// </summary>
    public class MasterPageScanner
    {
        private string url;

        public MasterPageScanner(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Scans passed web for custom masterpage usage
        /// </summary>
        /// <param name="cc">ClientContext object of the site to scan</param>
        /// <returns>MasterPageResult containing information about the custom master page usage</returns>
        public MasterPageResult Analyze(ClientContext cc)
        {
            Console.WriteLine("Master page... " + url);

            List<string> excludeMasterPage = new List<string>();
            excludeMasterPage.Add("v4.master");
            excludeMasterPage.Add("minimal.master");
            excludeMasterPage.Add("seattle.master");
            excludeMasterPage.Add("oslo.master");
            excludeMasterPage.Add("mysite15.master"); // mysite host
            excludeMasterPage.Add("boston.master"); // modern group sites

            Web web = cc.Web;
            web.EnsureProperties(p => p.CustomMasterUrl, p => p.MasterUrl, p => p.ServerRelativeUrl);

            MasterPageResult result = null;
            if (cc.Web.MasterUrl != null && !excludeMasterPage.Contains(cc.Web.MasterUrl.Substring(cc.Web.MasterUrl.LastIndexOf("/") + 1).ToLower()))
            {
                result = new MasterPageResult()
                {
                    Url = url,
                    SiteUrl = url,
                    MasterPage = cc.Web.MasterUrl,
                };
            }
            if (cc.Web.CustomMasterUrl != null && !excludeMasterPage.Contains(cc.Web.CustomMasterUrl.Substring(cc.Web.CustomMasterUrl.LastIndexOf("/") + 1).ToLower()))
            {
                if (result != null)
                {
                    result.CustomMasterPage = cc.Web.CustomMasterUrl;
                }
                else
                {
                    result = new MasterPageResult()
                    {
                        Url = url,
                        SiteUrl = url,
                        CustomMasterPage = cc.Web.CustomMasterUrl,
                    };
                }
            }
            return result;

            //// old implementation, removed for performance reasons
            //List masterPageGallery = web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
            //CamlQuery query = new CamlQuery();
            //// Use query Scope='RecursiveAll' to iterate through sub folders of Master page library because we might have file in folder hierarchy
            //query.ViewXml = "<View Scope='RecursiveAll'><Query><Where><Contains><FieldRef Name='FileRef'/><Value Type='Text'>.master</Value></Contains></Where></Query></View>";
            //ListItemCollection galleryItems = masterPageGallery.GetItems(query);
            //web.Context.Load(masterPageGallery);
            //web.Context.Load(galleryItems);
            //web.Context.ExecuteQueryRetry();

            //// Use Modified_x0020_By, _Level, StreamHash properties to find modified OOB master page
            //StringBuilder sb = new StringBuilder();            

            //foreach (var item in galleryItems)
            //{
            //    string masterPageName = item["FileLeafRef"].ToString();

            //    // Skip OOB master pages, unless they're customized
            //    if (excludeMasterPage.Contains(masterPageName) && string.IsNullOrEmpty(item["StreamHash"].ToString()))
            //    {
            //        continue;
            //    }

            //    // Check if the master page is being used
            //    if ($"{cc.Web.ServerRelativeUrl}/_catalogs/masterpage/{masterPageName}".Equals(cc.Web.MasterUrl, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        if (string.IsNullOrEmpty(sb.ToString()))
            //        {
            //            sb.Append(masterPageName);
            //        }
            //        else
            //        {
            //            sb.Append(",").Append(masterPageName);
            //        }
            //    }

            //    // Check if the custom master page is being used
            //    if ($"{cc.Web.ServerRelativeUrl}/_catalogs/masterpage/{masterPageName}".Equals(cc.Web.CustomMasterUrl, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        if (string.IsNullOrEmpty(sb.ToString()))
            //        {
            //            sb.Append(masterPageName);
            //        }
            //        else
            //        {
            //            sb.Append(",").Append(masterPageName);
            //        }
            //    }
            //}

            //if (!String.IsNullOrEmpty(sb.ToString()))
            //{
            //    MasterPageResult result = new MasterPageResult() { Url = url, SiteUrl = url, MasterPage = sb.ToString() };
            //    return result;
            //}
            //else
            //{
            //    return null;
            //}

        }

    }
}
