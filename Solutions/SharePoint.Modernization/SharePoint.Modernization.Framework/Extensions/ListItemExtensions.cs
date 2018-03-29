using SharePoint.Modernization.Framework;
using SharePoint.Modernization.Framework.Entities;
using SharePoint.Modernization.Framework.Pages;
using SharePoint.Modernization.Framework.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Extension methods for the ListItem object
    /// </summary>
    public static class ListItemExtensions
    {
        private const string HtmlFileTypeField = "HTML_x0020_File_x0020_Type";
        private const string WikiField = "WikiField";
        private const string ModifiedField = "Modified";
        private const string ModifiedByField = "Editor";
        private const string ClientSideApplicationId = "ClientSideApplicationId";
        private static readonly Guid FeatureId_Web_ModernPage = new Guid("B6917CB1-93A0-4B97-A84D-7CF49975D4EC");

        #region Analyze page
        /// <summary>
        /// Determines the type of page
        /// </summary>
        /// <param name="item">Page list item</param>
        /// <returns>Type of page</returns>
        public static string PageType(this ListItem item)
        {
            if (FieldExistsAndUsed(item, HtmlFileTypeField) && !String.IsNullOrEmpty(item[HtmlFileTypeField].ToString()))
            {
                if (item[HtmlFileTypeField].ToString().Equals("SharePoint.WebPartPage.Document", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "WebPartPage";
                }
            }

            if (FieldExistsAndUsed(item, WikiField) && !String.IsNullOrEmpty(item[WikiField].ToString()))
            {
                return "WikiPage";
            }

            if (FieldExistsAndUsed(item, ClientSideApplicationId) && item[ClientSideApplicationId].ToString().Equals(FeatureId_Web_ModernPage.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return "ClientSidePage";
            }

            if (FieldExistsAndUsed(item, WikiField))
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
        public static Tuple<string, List<WebPartEntity>> WebParts(this ListItem item, PageTransformation pageTransformation)
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
            if (FieldExistsAndUsed(item, ModifiedField) && !String.IsNullOrEmpty(item[ModifiedField].ToString()))
            {
                DateTime dt;
                if (DateTime.TryParse(item[ModifiedField].ToString(), out dt))
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
            if (FieldExistsAndUsed(item, ModifiedByField) && !String.IsNullOrEmpty(item[ModifiedByField].ToString()))
            {
                string lastModifiedBy = ((FieldUserValue)item[ModifiedByField]).Email;
                if (string.IsNullOrEmpty(lastModifiedBy))
                {
                    lastModifiedBy = ((FieldUserValue)item[ModifiedByField]).LookupValue;
                }
                return lastModifiedBy;
            }

            return "";
        }
        #endregion

        #region Transform page
        public static void Transform(this ListItem item, ClientContext clientContext)
        {
            new PageTransformator(clientContext).Transform(item);
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
