using SharePoint.Modernization.Framework;
using SharePoint.Modernization.Framework.Entities;
using SharePoint.Modernization.Framework.Pages;
using SharePoint.Modernization.Framework.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Extension methods for the ListItem object
    /// </summary>
    public static class ListItemExtensions
    {

        #region Analyze page
        /// <summary>
        /// Determines the type of page
        /// </summary>
        /// <param name="item">Page list item</param>
        /// <returns>Type of page</returns>
        public static string PageType(this ListItem item)
        {
            if (FieldExistsAndUsed(item, Constants.HtmlFileTypeField) && !String.IsNullOrEmpty(item[Constants.HtmlFileTypeField].ToString()))
            {
                if (item[Constants.HtmlFileTypeField].ToString().Equals("SharePoint.WebPartPage.Document", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "WebPartPage";
                }
            }

            if (FieldExistsAndUsed(item, Constants.WikiField) && !String.IsNullOrEmpty(item[Constants.WikiField].ToString()))
            {
                return "WikiPage";
            }

            if (FieldExistsAndUsed(item, Constants.ClientSideApplicationIdField) && item[Constants.ClientSideApplicationIdField].ToString().Equals(Constants.FeatureId_Web_ModernPage.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return "ClientSidePage";
            }

            if (FieldExistsAndUsed(item, Constants.WikiField))
            {
                return "WikiPage";
            }

            return "AspxPage";
        }

        /// <summary>
        /// Gets the web part information from the page
        /// </summary>
        /// <param name="item">Page list item</param>
        /// <param name="pageTransformation">PageTransformation model loaded from XML</param>
        /// <returns>Page layout + collection of web parts on the page</returns>
        public static Tuple<PageLayout, List<WebPartEntity>> WebParts(this ListItem item, PageTransformation pageTransformation)
        {
            string pageType = item.PageType();

            if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase))
            {
                return new WikiPage(item, pageTransformation).Analyze();
            }
            else if (pageType.Equals("WebPartPage", StringComparison.InvariantCultureIgnoreCase))
            {
                return new WebPartPage(item, pageTransformation).Analyze();
            }

            return null;
        }
        #endregion

        #region Page usage information
        /// <summary>
        /// Get's the page last modified date time
        /// </summary>
        /// <param name="item">Page list item</param>
        /// <returns>DateTime of the last modification</returns>
        public static DateTime LastModifiedDateTime(this ListItem item)
        {
            if (FieldExistsAndUsed(item, Constants.ModifiedField) && !String.IsNullOrEmpty(item[Constants.ModifiedField].ToString()))
            {
                DateTime dt;
                if (DateTime.TryParse(item[Constants.ModifiedField].ToString(), out dt))
                {
                    return dt;
                }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Get's the page last modified by
        /// </summary>
        /// <param name="item">Page list item</param>
        /// <returns>Last modified by user/account</returns>
        public static string LastModifiedBy(this ListItem item)
        {
            if (FieldExistsAndUsed(item, Constants.ModifiedByField) && !String.IsNullOrEmpty(item[Constants.ModifiedByField].ToString()))
            {
                string lastModifiedBy = ((FieldUserValue)item[Constants.ModifiedByField]).Email;
                if (string.IsNullOrEmpty(lastModifiedBy))
                {
                    lastModifiedBy = ((FieldUserValue)item[Constants.ModifiedByField]).LookupValue;
                }
                return lastModifiedBy;
            }

            return "";
        }
        #endregion

        #region Transform page
        public static void Transform(this ListItem item, ClientContext clientContext, 
                                     Func<string, string> pageTitleOverride = null,
                                     Func<ClientSidePage, ILayoutTransformator> layoutTransformatorOverride = null)
        {
            new PageTransformator(clientContext).Transform(item, pageTitleOverride, layoutTransformatorOverride);
        }
        #endregion

        #region helper methods
        public static bool FieldExistsAndUsed(this ListItem item, string fieldName)
        {
            return (item.FieldValues.ContainsKey(fieldName) && item[fieldName] != null);
        }
        #endregion

    }
}
