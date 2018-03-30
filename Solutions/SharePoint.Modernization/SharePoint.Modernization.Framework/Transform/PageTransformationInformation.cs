using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    public class PageTransformationInformation
    {
        #region Construction
        public PageTransformationInformation(ListItem sourcePage) : this(sourcePage, null, false)
        {
        }

        public PageTransformationInformation(ListItem sourcePage, string targetPageName) : this(sourcePage, targetPageName, false)
        {
        }

        public PageTransformationInformation(ListItem sourcePage, string targetPageName, bool overwrite)
        {
            SourcePage = sourcePage;
            TargetPageName = targetPageName;
            Overwrite = overwrite;
        }
        #endregion

        #region Core Properties
        public ListItem SourcePage { get; set; }
        public string TargetPageName { get; set; }
        public bool Overwrite { get; set; }
        #endregion

        #region Override properties
        public Func<string, string> PageTitleOverride { get; set; }
        public Func<ClientSidePage, ILayoutTransformator> LayoutTransformatorOverride { get; set; }
        public Func<ClientSidePage, PageTransformation, IContentTransformator> ContentTransformatorOverride { get; set; }
        #endregion

    }
}
