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

    }
}
