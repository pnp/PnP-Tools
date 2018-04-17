using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    /// <summary>
    /// Information used to configure the page transformation process
    /// </summary>
    public class PageTransformationInformation
    {
        #region Construction
        /// <summary>
        /// Instantiates the page transformation class
        /// </summary>
        /// <param name="sourcePage">Page we want to transform</param>
        public PageTransformationInformation(ListItem sourcePage) : this(sourcePage, null, false)
        {
        }

        /// <summary>
        /// Instantiates the page transformation class
        /// </summary>
        /// <param name="sourcePage">Page we want to transform</param>
        /// <param name="targetPageName">Name of the target page</param>
        public PageTransformationInformation(ListItem sourcePage, string targetPageName) : this(sourcePage, targetPageName, false)
        {
        }

        /// <summary>
        /// Instantiates the page transformation class
        /// </summary>
        /// <param name="sourcePage">Page we want to transform</param>
        /// <param name="targetPageName">Name of the target page</param>
        /// <param name="overwrite">Do we overwrite the target page if it already exists</param>
        public PageTransformationInformation(ListItem sourcePage, string targetPageName, bool overwrite)
        {
            SourcePage = sourcePage;
            TargetPageName = targetPageName;
            Overwrite = overwrite;
        }
        #endregion

        #region Core Properties
        /// <summary>
        /// Source wiki/webpart page we want to transform
        /// </summary>
        public ListItem SourcePage { get; set; }
        /// <summary>
        /// Name for the transformed page
        /// </summary>
        public string TargetPageName { get; set; }
        /// <summary>
        /// Overwrite the target page if it already exists?
        /// </summary>
        public bool Overwrite { get; set; }
        #endregion

        #region Override properties
        /// <summary>
        /// Custom function callout that can be triggered to provide a tailored page title
        /// </summary>
        public Func<string, string> PageTitleOverride { get; set; }
        /// <summary>
        /// Custom layout transformator to be used for this page
        /// </summary>
        public Func<ClientSidePage, ILayoutTransformator> LayoutTransformatorOverride { get; set; }
        /// <summary>
        /// Custom content transformator to be used for this page
        /// </summary>
        public Func<ClientSidePage, PageTransformation, IContentTransformator> ContentTransformatorOverride { get; set; }
        #endregion

    }
}
