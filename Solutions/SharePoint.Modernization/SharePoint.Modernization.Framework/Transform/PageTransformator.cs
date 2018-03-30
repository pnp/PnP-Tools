using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Framework.Entities;
using SharePoint.Modernization.Framework.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OfficeDevPnP.Core.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    public class PageTransformator
    {

        private ClientContext clientContext;
        private PageTransformation pageTransformation;

        #region Construction
        public PageTransformator(ClientContext clientContext): this(clientContext, "webpartmapping.xml")
        {
        }

        public PageTransformator(ClientContext clientContext, string pageTransformationFile)
        {
            this.clientContext = clientContext;

            // Load xml mapping data
            XmlSerializer xmlMapping = new XmlSerializer(typeof(PageTransformation));
            using (var stream = new FileStream(pageTransformationFile, FileMode.Open))
            {
                this.pageTransformation = (PageTransformation)xmlMapping.Deserialize(stream);
            }

        }
        #endregion

        public void Transform(ListItem sourcePage, 
                              Func<string,string> pageTitleOverride = null,
                              Func<ClientSidePage, ILayoutTransformator> layoutTransformatorOverride = null)
        {
            Transform(sourcePage, null, false, pageTitleOverride, layoutTransformatorOverride);
        }

        public void Transform(ListItem sourcePage, bool overwrite, 
                              Func<string, string> pageTitleOverride = null,
                              Func<ClientSidePage, ILayoutTransformator> layoutTransformatorOverride = null)
        {
            Transform(sourcePage, null, overwrite, pageTitleOverride, layoutTransformatorOverride);
        }

        public void Transform(ListItem sourcePage, string targetPageName, bool overwrite, 
                              Func<string, string> pageTitleOverride = null,
                              Func<ClientSidePage, ILayoutTransformator> layoutTransformatorOverride = null)
        {
            #region Input validation
            if (sourcePage == null)
            {
                throw new ArgumentNullException("SourcePage cannot be null");
            }

            // Validate page and it's eligibility for transformation
            if (!sourcePage.FieldExistsAndUsed(Constants.FileRefField) || !sourcePage.FieldExistsAndUsed(Constants.FileLeafRefField))
            {
                throw new ArgumentException("Page is not valid due to missing FileRef or FileLeafRef value");
            }

            string pageType = sourcePage.PageType();

            if (pageType.Equals("ClientSidePage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page is a client side page...guess you don't want to transform it...");
            }

            if (pageType.Equals("AspxPage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page is an basic aspx page...can't transform that one, sorry!");
            }
            #endregion

            #region Page creation
            // If no targetname specified then we'll come up with one
            if (string.IsNullOrEmpty(targetPageName))
            {
                targetPageName = $"Migrated_{sourcePage[Constants.FileLeafRefField].ToString()}";
            }

            // Check if page name is free to use
            bool pageExists = false;
            ClientSidePage targetPage = null;
            try
            {
                targetPage = clientContext.Web.LoadClientSidePage(targetPageName);
                pageExists = true;
            }
            catch (ArgumentException) { }

            if (pageExists)
            {
                if (!overwrite)
                {
                    throw new ArgumentException($"There already exists a page with name {targetPageName}.");
                }
                else
                {
                    targetPage.ClearPage();
                }
            }
            else
            {
                // Create a new client side page
                targetPage = clientContext.Web.AddClientSidePage(targetPageName);
            }
            #endregion

            #region Analysis of the source page
            // Analyze the source page
            Tuple<PageLayout, List<WebPartEntity>> pageData = null;

            if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase))
            {
                pageData = new WikiPage(sourcePage, pageTransformation).Analyze();
            }
            else if (pageType.Equals("WebPartPage", StringComparison.InvariantCultureIgnoreCase))
            {
                pageData = new WebPartPage(sourcePage, pageTransformation).Analyze(true);
            }
            #endregion

            #region Page title configuration
            // Set page title
            if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase) && sourcePage.FieldExistsAndUsed(Constants.FileTitleField))
            {
                targetPage.PageTitle = sourcePage[Constants.FileTitleField].ToString();
            }
            else if (pageType.Equals("WebPartPage"))
            {
                var titleBarWebPart = pageData.Item2.Where(p => p.Type == WebParts.TitleBar).FirstOrDefault();
                if (titleBarWebPart != null)
                {
                    if (titleBarWebPart.Properties.ContainsKey("HeaderTitle") && !string.IsNullOrEmpty(titleBarWebPart.Properties["HeaderTitle"]))
                    {
                        targetPage.PageTitle = titleBarWebPart.Properties["HeaderTitle"];
                    }
                }
            }

            if (pageTitleOverride != null)
            {
                targetPage.PageTitle = pageTitleOverride(targetPage.PageTitle);
            }
            #endregion

            #region Page layout configuration
            // Use the default layout transformator
            ILayoutTransformator layoutTransformator = new LayoutTransformator(targetPage);

            // Do we have an override?
            if (layoutTransformatorOverride != null)
            {
                layoutTransformator = layoutTransformatorOverride(targetPage);
            }
            
            // Apply the layout to the page
            layoutTransformator.ApplyLayout(pageData.Item1);
            #endregion

            #region Page contents transformation
            // Transform the page contents

            #endregion

            #region Page persisting
            // Persist the client side page
            targetPage.Save(targetPageName);
            targetPage.Publish();
            #endregion
        }
    }
}
