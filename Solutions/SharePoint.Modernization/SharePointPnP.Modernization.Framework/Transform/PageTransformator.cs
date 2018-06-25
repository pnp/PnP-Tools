using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Pages;
using SharePointPnP.Modernization.Framework.Entities;
using SharePointPnP.Modernization.Framework.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SharePointPnP.Modernization.Framework.Transform
{

    /// <summary>
    /// Transforms a classic wiki/webpart page into a modern client side page
    /// </summary>
    public class PageTransformator
    {
        private ClientContext clientContext;
        private PageTransformation pageTransformation;

        #region Construction
        /// <summary>
        /// Creates a page transformator instance
        /// </summary>
        /// <param name="clientContext">ClientContext of the site holding the page</param>
        public PageTransformator(ClientContext clientContext): this(clientContext, "webpartmapping.xml")
        {
        }

        /// <summary>
        /// Creates a page transformator instance
        /// </summary>
        /// <param name="clientContext">ClientContext of the site holding the page</param>
        /// <param name="pageTransformationFile">Used page mapping file</param>
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

        /// <summary>
        /// Creates a page transformator instance
        /// </summary>
        /// <param name="clientContext">ClientContext of the site holding the page</param>
        /// <param name="pageTransformationModel">Page transformation model</param>
        public PageTransformator(ClientContext clientContext, PageTransformation pageTransformationModel)
        {
            this.clientContext = clientContext;
            this.pageTransformation = pageTransformationModel;
        }
        #endregion

        /// <summary>
        /// Transform the page
        /// </summary>
        /// <param name="pageTransformationInformation">Information about the page to transform</param>
        public void Transform(PageTransformationInformation pageTransformationInformation)
        {
            #region Input validation
            if (pageTransformationInformation.SourcePage == null)
            {
                throw new ArgumentNullException("SourcePage cannot be null");
            }

            // Validate page and it's eligibility for transformation
            if (!pageTransformationInformation.SourcePage.FieldExistsAndUsed(Constants.FileRefField) || !pageTransformationInformation.SourcePage.FieldExistsAndUsed(Constants.FileLeafRefField))
            {
                throw new ArgumentException("Page is not valid due to missing FileRef or FileLeafRef value");
            }

            string pageType = pageTransformationInformation.SourcePage.PageType();

            if (pageType.Equals("ClientSidePage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page is a client side page...guess you don't want to transform it...");
            }

            if (pageType.Equals("AspxPage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page is an basic aspx page...can't transform that one, sorry!");
            }
            #endregion

            #region Telemetry
            clientContext.ClientTag = $"SPDev:PageTransformator";
            clientContext.Load(clientContext.Web, p => p.Description, p => p.Id);
            clientContext.ExecuteQuery();
            #endregion

            #region Page creation
            // If no targetname specified then we'll come up with one
            if (string.IsNullOrEmpty(pageTransformationInformation.TargetPageName))
            {
                if (string.IsNullOrEmpty(pageTransformationInformation.TargetPagePrefix))
                {
                    pageTransformationInformation.SetDefaultTargetPagePrefix();
                }

                pageTransformationInformation.TargetPageName = $"{pageTransformationInformation.TargetPagePrefix}{pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString()}";
            }

            // Check if page name is free to use
            bool pageExists = false;
            ClientSidePage targetPage = null;
            try
            {
                targetPage = clientContext.Web.LoadClientSidePage(pageTransformationInformation.TargetPageName);
                pageExists = true;
            }
            catch (ArgumentException) { }

            if (pageExists)
            {
                if (!pageTransformationInformation.Overwrite)
                {
                    throw new ArgumentException($"There already exists a page with name {pageTransformationInformation.TargetPageName}.");
                }
                else
                {
                    targetPage.ClearPage();
                }
            }
            else
            {
                // Create a new client side page
                targetPage = clientContext.Web.AddClientSidePage(pageTransformationInformation.TargetPageName);
            }
            #endregion

            // TODO: add logic to implement page level permission settings to be copied as well

            #region Home page handling
            bool replacedByOOBHomePage = false;
            // Check if the transformed page is the web's home page
            clientContext.Web.EnsureProperties(w => w.RootFolder.WelcomePage);
            var homePageUrl = clientContext.Web.RootFolder.WelcomePage;
            var homepageName = Path.GetFileName(clientContext.Web.RootFolder.WelcomePage);
            if (homepageName.Equals(pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                targetPage.LayoutType = ClientSidePageLayoutType.Home;
                if (pageTransformationInformation.ReplaceHomePageWithDefaultHomePage)
                {
                    targetPage.KeepDefaultWebParts = true;
                    replacedByOOBHomePage = true;
                }
            }
            #endregion

            #region Article page handling
            if (!replacedByOOBHomePage)
            {
                #region Configure header from target page
                if (pageTransformationInformation.PageHeader == null || pageTransformationInformation.PageHeader.Type == ClientSidePageHeaderType.None)
                {
                    targetPage.RemovePageHeader();
                }
                else if (pageTransformationInformation.PageHeader.Type == ClientSidePageHeaderType.Default)
                {
                    targetPage.SetDefaultPageHeader();
                }
                else if (pageTransformationInformation.PageHeader.Type == ClientSidePageHeaderType.Custom)
                {
                    targetPage.SetCustomPageHeader(pageTransformationInformation.PageHeader.ImageServerRelativeUrl, pageTransformationInformation.PageHeader.TranslateX, pageTransformationInformation.PageHeader.TranslateY);
                }
                #endregion

                #region Analysis of the source page
                // Analyze the source page
                Tuple<PageLayout, List<WebPartEntity>> pageData = null;

                if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase))
                {
                    pageData = new WikiPage(pageTransformationInformation.SourcePage, pageTransformation).Analyze();

                    // Wiki pages can contain embedded images and videos, which is not supported by the target RTE...split wiki text blocks so the transformator can handle the images and videos as separate web parts
                    if (pageTransformationInformation.HandleWikiImagesAndVideos)
                    {
                        pageData = new Tuple<PageLayout, List<WebPartEntity>>(pageData.Item1, new WikiTransformator().TransformPlusSplit(pageData.Item2));
                    }
                }
                else if (pageType.Equals("WebPartPage", StringComparison.InvariantCultureIgnoreCase))
                {
                    pageData = new WebPartPage(pageTransformationInformation.SourcePage, pageTransformation).Analyze(true);
                }
                #endregion

                #region Page title configuration
                // Set page title
                if (pageType.Equals("WikiPage", StringComparison.InvariantCultureIgnoreCase))
                {
                    SetPageTitle(pageTransformationInformation, targetPage);
                }
                else if (pageType.Equals("WebPartPage"))
                {
                    bool titleFound = false;
                    var titleBarWebPart = pageData.Item2.Where(p => p.Type == WebParts.TitleBar).FirstOrDefault();
                    if (titleBarWebPart != null)
                    {
                        if (titleBarWebPart.Properties.ContainsKey("HeaderTitle") && !string.IsNullOrEmpty(titleBarWebPart.Properties["HeaderTitle"]))
                        {
                            targetPage.PageTitle = titleBarWebPart.Properties["HeaderTitle"];
                            titleFound = true;
                        }
                    }

                    if (!titleFound)
                    {
                        SetPageTitle(pageTransformationInformation, targetPage);
                    }
                }

                if (pageTransformationInformation.PageTitleOverride != null)
                {
                    targetPage.PageTitle = pageTransformationInformation.PageTitleOverride(targetPage.PageTitle);
                }
                #endregion

                #region Page layout configuration
                // Use the default layout transformator
                ILayoutTransformator layoutTransformator = new LayoutTransformator(targetPage);

                // Do we have an override?
                if (pageTransformationInformation.LayoutTransformatorOverride != null)
                {
                    layoutTransformator = pageTransformationInformation.LayoutTransformatorOverride(targetPage);
                }

                // Apply the layout to the page
                layoutTransformator.Transform(pageData.Item1);
                #endregion

                #region Content transformation
                // Use the default content transformator
                IContentTransformator contentTransformator = new ContentTransformator(targetPage, pageTransformation);

                // Do we have an override?
                if (pageTransformationInformation.ContentTransformatorOverride != null)
                {
                    contentTransformator = pageTransformationInformation.ContentTransformatorOverride(targetPage, pageTransformation);
                }

                // Run the content transformator
                contentTransformator.Transform(pageData.Item2);
                #endregion
            }
            #endregion

            #region Page persisting
            // Persist the client side page
            targetPage.Save(pageTransformationInformation.TargetPageName);
            targetPage.Publish();

            // All went well so far...swap pages if that's needed
            if (pageTransformationInformation.TargetPageTakesSourcePageName)
            {
                //Load the source page
                var sourcePageUrl = pageTransformationInformation.SourcePage[Constants.FileRefField].ToString();
                var orginalSourcePageName = pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString();

                string path = sourcePageUrl.Replace(pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString(), "");

                var sourcePage = this.clientContext.Web.GetFileByServerRelativeUrl(sourcePageUrl);
                this.clientContext.Load(sourcePage);
                this.clientContext.ExecuteQueryRetry();

                if (string.IsNullOrEmpty(pageTransformationInformation.SourcePagePrefix))
                {
                    pageTransformationInformation.SetDefaultSourcePagePrefix();
                }
                var newSourcePageUrl = $"{pageTransformationInformation.SourcePagePrefix}{pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString()}";

                // Rename source page using the sourcepageprefix
                sourcePage.MoveTo($"{path}{newSourcePageUrl}", MoveOperations.None);
                this.clientContext.ExecuteQueryRetry();

                //Load the created target page
                var targetPageUrl = $"{path}{pageTransformationInformation.TargetPageName}";
                var targetPageFile = this.clientContext.Web.GetFileByServerRelativeUrl(targetPageUrl);
                this.clientContext.Load(targetPageFile);
                this.clientContext.ExecuteQueryRetry();

                // Rename the target page to the original source page name
                targetPageFile.MoveTo($"{path}{orginalSourcePageName}", MoveOperations.Overwrite);
                this.clientContext.ExecuteQueryRetry();
            }
            #endregion
        }

        /// <summary>
        /// Loads a page transformation model from file
        /// </summary>
        /// <param name="pageTransformationFile">File holding the page transformation model</param>
        /// <returns>Page transformation model</returns>
        public static PageTransformation LoadPageTransformationModel(string pageTransformationFile)
        {
            // Load xml mapping data
            XmlSerializer xmlMapping = new XmlSerializer(typeof(PageTransformation));
            using (var stream = new FileStream(pageTransformationFile, FileMode.Open))
            {
                return (PageTransformation)xmlMapping.Deserialize(stream);
            }
        }

        #region Helper methods
        private static void SetPageTitle(PageTransformationInformation pageTransformationInformation, ClientSidePage targetPage)
        {
            if (pageTransformationInformation.SourcePage.FieldExistsAndUsed(Constants.FileLeafRefField))
            {
                string pageTitle = Path.GetFileNameWithoutExtension((pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString()));
                if (!string.IsNullOrEmpty(pageTitle))
                {
                    pageTitle = pageTitle.First().ToString().ToUpper() + pageTitle.Substring(1);
                    targetPage.PageTitle = pageTitle;
                }
            }
        }
        #endregion

    }
}
