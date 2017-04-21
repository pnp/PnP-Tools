using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with site (both site collection and web site) creation, status, retrieval and settings
    /// </summary>
    public static partial class WebExtensions
    {
        /// <summary>
        /// Checks if the current web is a sub site or not
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <returns>True is sub site, false otherwise</returns>
        public static bool IsSubSite(this Web web)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));

            var site = (web.Context as ClientContext).Site;
            var rootWeb = site.EnsureProperty(s => s.RootWeb);

            web.EnsureProperty(w => w.Id);
            rootWeb.EnsureProperty(w => w.Id);

            if (rootWeb.Id != web.Id)
            {
                return true;
            }
            return false;
        }           

        /// <summary>
        /// Gets a list of SharePoint lists to scan for modern compatibility
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <returns>List of SharePoint lists to scan</returns>
        public static List<List> GetListsToScan(this Web web)
        {
            List<List> lists = new List<List>(10);

            ListCollection listCollection = web.Lists;
            listCollection.EnsureProperties(coll => coll.Include(li => li.UserCustomActions, li => li.Title, li => li.Hidden, li => li.DefaultViewUrl, li => li.BaseTemplate, li => li.RootFolder, li => li.ListExperienceOptions));
                                                                                           
            // Let's process the visible lists
            foreach (List list in listCollection.Where(p => p.Hidden == false))
            {
                if (list.DefaultViewUrl.Contains("_catalogs"))
                {
                    // skip catalogs
                    continue;
                }

                if (list.BaseTemplate == 544)
                {
                    // skip MicroFeed (544)
                    continue;
                }

                lists.Add(list);
            }

            return lists;
        }

    }
}
