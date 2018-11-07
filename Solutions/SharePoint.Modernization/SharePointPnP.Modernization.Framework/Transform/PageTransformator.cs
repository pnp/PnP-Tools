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

            if (pageType.Equals("PublishingPage", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Page transformation for publishing pages is currently not supported.");
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
                    pageData = new Tuple<PageLayout, List<WebPartEntity>>(pageData.Item1, new WikiTransformatorSimple().TransformPlusSplit(pageData.Item2, pageTransformationInformation.HandleWikiImagesAndVideos));
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

                #region Page Banner creation
                if (!pageTransformationInformation.TargetPageTakesSourcePageName)
                {
                    if (pageTransformationInformation.ModernizationCenterInformation != null && pageTransformationInformation.ModernizationCenterInformation.AddPageAcceptBanner)
                    {
                        // Bump the row values for the existing web parts as we've inserted a new section
                        foreach (var section in targetPage.Sections)
                        {
                            section.Order = section.Order + 1;
                        }

                        // Add new section for banner part
                        targetPage.Sections.Insert(0, new CanvasSection(targetPage, CanvasSectionTemplate.OneColumn, 0));

                        // Bump the row values for the existing web parts as we've inserted a new section
                        foreach(var webpart in pageData.Item2)
                        {
                            webpart.Row = webpart.Row + 1;
                        }


                        var sourcePageUrl = pageTransformationInformation.SourcePage[Constants.FileRefField].ToString();
                        var orginalSourcePageName = pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString();
                        clientContext.Web.EnsureProperty(p => p.Url);
                        Uri host = new Uri(clientContext.Web.Url);

                        string path = $"{host.Scheme}://{host.DnsSafeHost}{sourcePageUrl.Replace(pageTransformationInformation.SourcePage[Constants.FileLeafRefField].ToString(), "")}";

                        // Add "fake" banner web part that then will be transformed onto the page
                        Dictionary<string, string> props = new Dictionary<string, string>(2)
                        {
                            { "SourcePage", $"{path}{orginalSourcePageName}" },
                            { "TargetPage", $"{path}{pageTransformationInformation.TargetPageName}" }
                        };

                        WebPartEntity bannerWebPart = new WebPartEntity()
                        {
                            Type = WebParts.PageAcceptanceBanner,
                            Column = 1,
                            Row = 1,
                            Title = "",
                            Order = 0,
                            Properties = props,
                        };
                        pageData.Item2.Insert(0, bannerWebPart);
                    }
                }
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

            #region Page persisting + permissions
            #region Save the page
            // Persist the client side page
            targetPage.Save(pageTransformationInformation.TargetPageName);
            targetPage.Publish();
            #endregion

            #region Permission handling
            if (pageTransformationInformation.KeepPageSpecificPermissions)
            {
                pageTransformationInformation.SourcePage.EnsureProperty(p => p.HasUniqueRoleAssignments);
                if (pageTransformationInformation.SourcePage.HasUniqueRoleAssignments)
                {
                    // Copy the unique permissions from source to target
                    // Get the unique permissions
                    this.clientContext.Load(pageTransformationInformation.SourcePage, a => a.RoleAssignments.Include(roleAsg => roleAsg.Member.LoginName,
                        roleAsg => roleAsg.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.Description)));
                    this.clientContext.ExecuteQueryRetry();

                    // Get target page information
                    this.clientContext.Load(targetPage.PageListItem, p => p.HasUniqueRoleAssignments, a => a.RoleAssignments.Include(roleAsg => roleAsg.Member.LoginName,
                        roleAsg => roleAsg.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.Description)));
                    this.clientContext.ExecuteQueryRetry();

                    // Break permission inheritance on the target page if not done yet
                    if (!targetPage.PageListItem.HasUniqueRoleAssignments)
                    {
                        targetPage.PageListItem.BreakRoleInheritance(false, false);
                        this.clientContext.ExecuteQueryRetry();
                    }

                    // Apply new permissions
                    foreach(var roleAssignment in pageTransformationInformation.SourcePage.RoleAssignments)
                    {
                        var principal = this.clientContext.Web.SiteUsers.GetByLoginName(roleAssignment.Member.LoginName);
                        if (principal != null)
                        {
                            var roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(this.clientContext);
                            foreach(var roleDef in roleAssignment.RoleDefinitionBindings)
                            {
                                roleDefinitionBindingCollection.Add(roleDef);
                            }

                            targetPage.PageListItem.RoleAssignments.Add(principal, roleDefinitionBindingCollection);
                        }
                    }
                    this.clientContext.ExecuteQueryRetry();
                }
            }
            #endregion

            #region Page name switching
            // All went well so far...swap pages if that's needed
            if (pageTransformationInformation.TargetPageTakesSourcePageName)
            {
                //Load the source page
                SwapPages(pageTransformationInformation);
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// Performs the logic needed to swap a genered Migrated_Page.aspx to Page.aspx and then Page.aspx to Old_Page.aspx
        /// </summary>
        /// <param name="pageTransformationInformation">Information about the page to transform</param>
        public void SwapPages(PageTransformationInformation pageTransformationInformation)
        {
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
            // STEP1: First copy the source page to a new name. We on purpose use CopyTo as we want to avoid that "linked" url's get 
            //        patched up during a MoveTo operation as that would also patch the url's in our new modern page
            sourcePage.CopyTo($"{path}{newSourcePageUrl}", true);
            this.clientContext.ExecuteQueryRetry();

            //Load the created target page
            var targetPageUrl = $"{path}{pageTransformationInformation.TargetPageName}";
            var targetPageFile = this.clientContext.Web.GetFileByServerRelativeUrl(targetPageUrl);
            this.clientContext.Load(targetPageFile);
            this.clientContext.ExecuteQueryRetry();

            // STEP2: Fix possible navigation entries to point to the "copied" source page first
            // Rename the target page to the original source page name
            // CopyTo and MoveTo with option to overwrite first internally delete the file to overwrite, which
            // results in all page navigation nodes pointing to this file to be deleted. Hence let's point these
            // navigation entries first to the copied version of the page we just created
            this.clientContext.Web.Context.Load(this.clientContext.Web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
            this.clientContext.Web.Context.ExecuteQueryRetry();

            bool navWasFixed = false;
            IQueryable<NavigationNode> currentNavNodes = null;
            IQueryable<NavigationNode> globalNavNodes = null;
            var currentNavigation = this.clientContext.Web.Navigation.QuickLaunch;
            var globalNavigation = this.clientContext.Web.Navigation.TopNavigationBar;
            // Check for nav nodes
            currentNavNodes = currentNavigation.Where(n => n.Url.Equals(sourcePageUrl, StringComparison.InvariantCultureIgnoreCase));
            globalNavNodes = globalNavigation.Where(n => n.Url.Equals(sourcePageUrl, StringComparison.InvariantCultureIgnoreCase));

            if (currentNavNodes.Count() > 0 || globalNavNodes.Count() > 0)
            {
                navWasFixed = true;
                foreach (var node in currentNavNodes)
                {
                    node.Url = $"{path}{newSourcePageUrl}";
                    node.Update();
                }
                foreach (var node in globalNavNodes)
                {
                    node.Url = $"{path}{newSourcePageUrl}";
                    node.Update();
                }
                this.clientContext.ExecuteQueryRetry();
            }

            // STEP3: Now copy the created modern page over the original source page, at this point the new page has the same name as the original page had before transformation
            targetPageFile.CopyTo($"{path}{orginalSourcePageName}", true);
            this.clientContext.ExecuteQueryRetry();

            // STEP4: Finish with restoring the page navigation: update the navlinks to point back the original page name
            if (navWasFixed)
            {
                // Reload the navigation entries as did update them
                this.clientContext.Web.Context.Load(this.clientContext.Web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
                this.clientContext.Web.Context.ExecuteQueryRetry();

                currentNavigation = this.clientContext.Web.Navigation.QuickLaunch;
                globalNavigation = this.clientContext.Web.Navigation.TopNavigationBar;
                if (!string.IsNullOrEmpty($"{path}{newSourcePageUrl}"))
                {
                    currentNavNodes = currentNavigation.Where(n => n.Url.Equals($"{path}{newSourcePageUrl}", StringComparison.InvariantCultureIgnoreCase));
                    globalNavNodes = globalNavigation.Where(n => n.Url.Equals($"{path}{newSourcePageUrl}", StringComparison.InvariantCultureIgnoreCase));
                }

                foreach (var node in currentNavNodes)
                {
                    node.Url = sourcePageUrl;
                    node.Update();
                }
                foreach (var node in globalNavNodes)
                {
                    node.Url = sourcePageUrl;
                    node.Update();
                }
                this.clientContext.ExecuteQueryRetry();
            }

            //STEP5: Conclude with deleting the originally created modern page as we did copy that already in step 3
            targetPageFile.DeleteObject();
            this.clientContext.ExecuteQueryRetry();
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
