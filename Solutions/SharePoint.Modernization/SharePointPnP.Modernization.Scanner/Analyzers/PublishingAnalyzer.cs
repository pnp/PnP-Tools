using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Scanner.Results;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SharePoint.Modernization.Scanner.Analyzers
{
    public class PublishingAnalyzer : BaseAnalyzer
    {
        const string AvailablePageLayouts = "__PageLayouts";
        const string DefaultPageLayout = "__DefaultPageLayout";
        const string AvailableWebTemplates = "__WebTemplates";
        const string InheritWebTemplates = "__InheritWebTemplates";
        const string WebNavigationSettings = "_webnavigationsettings";
        const string FileRefField = "FileRef";
        const string FileLeafRefField = "FileLeafRef";
        const string PublishingPageLayoutField = "PublishingPageLayout";

        // Queries
        const string CAMLQueryByExtension = @"
                <View Scope='RecursiveAll'>
                  <Query>
                    <Where>
                      <Contains>
                        <FieldRef Name='File_x0020_Type'/>
                        <Value Type='text'>aspx</Value>
                      </Contains>
                    </Where>
                  </Query>
                  <ViewFields>
                    <FieldRef Name='ContentTypeId' />
                    <FieldRef Name='FileRef' />
                    <FieldRef Name='FileLeafRef' />
                    <FieldRef Name='File_x0020_Type' />
                    <FieldRef Name='Editor' />
                    <FieldRef Name='Modified' />
                    <FieldRef Name='PublishingPageLayout' />
                    <FieldRef Name='Audience' />
                    <FieldRef Name='PublishingRollupImage' />
                  </ViewFields>  
                </View>";

        WebScanResult webScanResult;
        SiteScanResult siteScanResult;

        #region Construction
        /// <summary>
        /// Publishing analyzer construction
        /// </summary>
        /// <param name="url">Url of the web to be analyzed</param>
        /// <param name="siteColUrl">Url of the site collection hosting this web</param>
        public PublishingAnalyzer(string url, string siteColUrl, ModernizationScanJob scanJob, WebScanResult webScanResult, SiteScanResult siteScanResult) : base(url, siteColUrl, scanJob)
        {
            this.webScanResult = webScanResult;
            this.siteScanResult = siteScanResult;
        }
        #endregion

        public override TimeSpan Analyze(ClientContext cc)
        {
            try
            {
                base.Analyze(cc);

                // Only scan when it's a valid publishing portal
                var pageCount = ContinueScanning(cc);
                if (pageCount > 0 || pageCount == -1)
                {
                    try
                    {
                        PublishingScanResult scanResult = new PublishingScanResult()
                        {
                            SiteColUrl = this.SiteCollectionUrl,
                            SiteURL = this.SiteUrl,
                            WebRelativeUrl = this.SiteUrl.Replace(this.SiteCollectionUrl, ""),
                            WebTemplate = this.webScanResult.WebTemplate,
                            BrokenPermissionInheritance = this.webScanResult.BrokenPermissionInheritance,
                            PageCount = pageCount == -1 ? 0 : pageCount,
                            SiteMasterPage = this.webScanResult.CustomMasterPage,
                            SystemMasterPage = this.webScanResult.MasterPage,
                            AlternateCSS = this.webScanResult.AlternateCSS,
                            Admins = this.siteScanResult.Admins,
                            Owners = this.webScanResult.Owners,
                        };

                        Web web = cc.Web;

                        // Load additional web properties
                        web.EnsureProperties(p => p.Language);
                        scanResult.Language = web.Language;

                        // PageLayouts handling
                        var availablePageLayouts = web.GetPropertyBagValueString(AvailablePageLayouts, "");
                        var defaultPageLayout = web.GetPropertyBagValueString(DefaultPageLayout, "");

                        if (string.IsNullOrEmpty(availablePageLayouts))
                        {
                            scanResult.PageLayoutsConfiguration = "Any";
                        }
                        else if (availablePageLayouts.Equals("__inherit", StringComparison.InvariantCultureIgnoreCase))
                        {
                            scanResult.PageLayoutsConfiguration = "Inherit from parent";
                        }
                        else
                        {
                            scanResult.PageLayoutsConfiguration = "Defined list";

                            // Fill the defined list
                            var element = XElement.Parse(availablePageLayouts);
                            var nodes = element.Descendants("layout");
                            if (nodes != null && nodes.Count() > 0)
                            {
                                string allowedPageLayouts = "";

                                foreach (var node in nodes)
                                {
                                    allowedPageLayouts = allowedPageLayouts + node.Attribute("url").Value.Replace("_catalogs/masterpage/", "") + ",";
                                }

                                allowedPageLayouts = allowedPageLayouts.TrimEnd(new char[] { ',' });

                                scanResult.AllowedPageLayouts = allowedPageLayouts;
                            }
                        }

                        if (!string.IsNullOrEmpty(defaultPageLayout))
                        {
                            var element = XElement.Parse(defaultPageLayout);
                            scanResult.DefaultPageLayout = element.Attribute("url").Value.Replace("_catalogs/masterpage/", "");
                        }

                        // Navigation
                        var navigationSettings = web.GetNavigationSettings();
                        if (navigationSettings != null)
                        {
                            if (navigationSettings.GlobalNavigation.ManagedNavigation)
                            {
                                scanResult.GlobalNavigationType = "Managed";
                            }
                            else
                            {
                                scanResult.GlobalNavigationType = "Structural";
                                scanResult.GlobalStructuralNavigationMaxCount = navigationSettings.GlobalNavigation.MaxDynamicItems;
                                scanResult.GlobalStructuralNavigationShowPages = navigationSettings.GlobalNavigation.ShowPages;
                                scanResult.GlobalStructuralNavigationShowSiblings = navigationSettings.GlobalNavigation.ShowSiblings;
                                scanResult.GlobalStructuralNavigationShowSubSites = navigationSettings.GlobalNavigation.ShowSubsites;
                            }

                            if (navigationSettings.CurrentNavigation.ManagedNavigation)
                            {
                                scanResult.CurrentNavigationType = "Managed";
                            }
                            else
                            {
                                scanResult.CurrentNavigationType = "Structural";
                                scanResult.CurrentStructuralNavigationMaxCount = navigationSettings.CurrentNavigation.MaxDynamicItems;
                                scanResult.CurrentStructuralNavigationShowPages = navigationSettings.CurrentNavigation.ShowPages;
                                scanResult.CurrentStructuralNavigationShowSiblings = navigationSettings.CurrentNavigation.ShowSiblings;
                                scanResult.CurrentStructuralNavigationShowSubSites = navigationSettings.CurrentNavigation.ShowSubsites;
                            }

                            if (navigationSettings.GlobalNavigation.ManagedNavigation || navigationSettings.CurrentNavigation.ManagedNavigation)
                            {
                                scanResult.ManagedNavigationAddNewPages = navigationSettings.AddNewPagesToNavigation;
                                scanResult.ManagedNavigationCreateFriendlyUrls = navigationSettings.CreateFriendlyUrlsForNewPages;

                                // get information about the managed nav term set configuration
                                var managedNavXml = web.GetPropertyBagValueString(WebNavigationSettings, "");

                                if (!string.IsNullOrEmpty(managedNavXml))
                                {
                                    var managedNavSettings = XElement.Parse(managedNavXml);
                                    IEnumerable<XElement> navNodes = managedNavSettings.XPathSelectElements("./SiteMapProviderSettings/TaxonomySiteMapProviderSettings");
                                    foreach (var node in navNodes)
                                    {
                                        if (node.Attribute("Name").Value.Equals("CurrentNavigationTaxonomyProvider", StringComparison.InvariantCulture))
                                        {
                                            if (node.Attribute("TermSetId") != null)
                                            {
                                                scanResult.CurrentManagedNavigationTermSetId = node.Attribute("TermSetId").Value;
                                            }
                                            else if (node.Attribute("UseParentSiteMap") != null)
                                            {
                                                scanResult.CurrentManagedNavigationTermSetId = "Inherit from parent";
                                            }
                                        }
                                        else if (node.Attribute("Name").Value.Equals("GlobalNavigationTaxonomyProvider", StringComparison.InvariantCulture))
                                        {
                                            if (node.Attribute("TermSetId") != null)
                                            {
                                                scanResult.GlobalManagedNavigationTermSetId = node.Attribute("TermSetId").Value;
                                            }
                                            else if (node.Attribute("UseParentSiteMap") != null)
                                            {
                                                scanResult.GlobalManagedNavigationTermSetId = "Inherit from parent";
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Pages library
                        var pagesLibrary = web.GetListsToScan().Where(p => p.BaseTemplate == 850).FirstOrDefault();
                        if (pagesLibrary != null)
                        {
                            pagesLibrary.EnsureProperties(p => p.EnableModeration, p => p.EnableVersioning, p => p.EnableMinorVersions, p => p.EventReceivers, p => p.Fields, p => p.DefaultContentApprovalWorkflowId);
                            scanResult.LibraryEnableModeration = pagesLibrary.EnableModeration;
                            scanResult.LibraryEnableVersioning = pagesLibrary.EnableVersioning;
                            scanResult.LibraryEnableMinorVersions = pagesLibrary.EnableMinorVersions;
                            scanResult.LibraryItemScheduling = pagesLibrary.ItemSchedulingEnabled();
                            scanResult.LibraryApprovalWorkflowDefined = pagesLibrary.DefaultContentApprovalWorkflowId != Guid.Empty;
                        }

                        // Variations
                        if (scanResult.Level == 0)
                        {
                            var variationLabels = cc.GetVariationLabels();

                            string labels = "";
                            string sourceLabel = "";
                            foreach (var label in variationLabels)
                            {
                                labels = labels + $"{label.Title} ({label.Language}),";

                                if (label.IsSource)
                                {
                                    sourceLabel = label.Title;
                                }

                            }

                            scanResult.VariationLabels = labels.TrimEnd(new char[] { ',' }); ;
                            scanResult.VariationSourceLabel = sourceLabel;
                        }

                        // Scan pages inside the pages library
                        if (pagesLibrary != null && Options.IncludePublishingWithPages(this.ScanJob.Mode))
                        {
                            CamlQuery query = new CamlQuery
                            {
                                ViewXml = CAMLQueryByExtension,
                            };

                            var pages = pagesLibrary.GetItems(query);

                            // Load additional page related information
                            IEnumerable<ListItem> enumerable = web.Context.LoadQuery(pages.IncludeWithDefaultProperties((ListItem item) => item.ContentType));
                            web.Context.ExecuteQueryRetry();

                            if (enumerable.FirstOrDefault() != null)
                            {
                                foreach (var page in enumerable)
                                {
                                    string pageUrl = null;
                                    try
                                    {
                                        if (page.FieldValues.ContainsKey(FileRefField) && !String.IsNullOrEmpty(page[FileRefField].ToString()))
                                        {
                                            pageUrl = page[FileRefField].ToString();
                                        }
                                        else
                                        {
                                            //skip page
                                            continue;
                                        }

                                        // Basic information about the page
                                        PublishingPageScanResult pageScanResult = new PublishingPageScanResult()
                                        {
                                            SiteColUrl = this.SiteCollectionUrl,
                                            SiteURL = this.SiteUrl,
                                            WebRelativeUrl = scanResult.WebRelativeUrl,
                                            PageRelativeUrl = scanResult.WebRelativeUrl.Length > 0 ? pageUrl.Replace(scanResult.WebRelativeUrl, "") : pageUrl,
                                        };

                                        // Page name
                                        if (page.FieldValues.ContainsKey(FileLeafRefField) && !String.IsNullOrEmpty(page[FileLeafRefField].ToString()))
                                        {
                                            pageScanResult.PageName = page[FileLeafRefField].ToString();
                                        }

                                        // Get page change information
                                        pageScanResult.ModifiedAt = page.LastModifiedDateTime();
                                        pageScanResult.ModifiedBy = page.LastModifiedBy();

                                        // Page layout
                                        pageScanResult.PageLayout = page.PageLayout();
                                        pageScanResult.PageLayoutFile = page.PageLayoutFile().Replace(pageScanResult.SiteColUrl, "").Replace("/_catalogs/masterpage/", "");

                                        // Page audiences
                                        var audiences = page.Audiences();
                                        if (audiences != null)
                                        {
                                            pageScanResult.GlobalAudiences = audiences.GlobalAudiences;
                                            pageScanResult.SecurityGroupAudiences = audiences.SecurityGroups;
                                            pageScanResult.SharePointGroupAudiences = audiences.SharePointGroups;
                                        }

                                        // Contenttype
                                        pageScanResult.ContentType = page.ContentType.Name;
                                        pageScanResult.ContentTypeId = page.ContentType.Id.StringValue;

                                        // Get page web parts
                                        var pageAnalysis = page.WebParts(this.ScanJob.PageTransformation);
                                        if (pageAnalysis != null)
                                        {
                                            pageScanResult.WebParts = pageAnalysis.Item2;
                                        }

                                        // Persist publishing page scan results
                                        if (!this.ScanJob.PublishingPageScanResults.TryAdd(pageUrl, pageScanResult))
                                        {
                                            ScanError error = new ScanError()
                                            {
                                                Error = $"Could not add publishing page scan result for {pageScanResult.PageRelativeUrl}",
                                                SiteColUrl = this.SiteCollectionUrl,
                                                SiteURL = this.SiteUrl,
                                                Field1 = "PublishingAnalyzer",
                                                Field2 = pageScanResult.PageRelativeUrl,
                                            };
                                            this.ScanJob.ScanErrors.Push(error);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ScanError error = new ScanError()
                                        {
                                            Error = ex.Message,
                                            SiteColUrl = this.SiteCollectionUrl,
                                            SiteURL = this.SiteUrl,
                                            Field1 = "MainPublishingPageAnalyzerLoop",
                                            Field2 = ex.StackTrace,
                                            Field3 = pageUrl
                                        };
                                        this.ScanJob.ScanErrors.Push(error);
                                        Console.WriteLine("Error for page {1}: {0}", ex.Message, pageUrl);
                                    }
                                }
                            }

                        }

                        // Persist publishing scan results
                        if (!this.ScanJob.PublishingScanResults.TryAdd(this.SiteUrl, scanResult))
                        {
                            ScanError error = new ScanError()
                            {
                                Error = $"Could not add publishing scan result for {this.SiteUrl}",
                                SiteColUrl = this.SiteCollectionUrl,
                                SiteURL = this.SiteUrl,
                                Field1 = "PublishingAnalyzer",
                            };
                            this.ScanJob.ScanErrors.Push(error);
                        }
                    }
                    catch(Exception ex)
                    {
                        ScanError error = new ScanError()
                        {
                            Error = ex.Message,
                            SiteColUrl = this.SiteCollectionUrl,
                            SiteURL = this.SiteUrl,
                            Field1 = "MainPublishingAnalyzerLoop",
                            Field2 = ex.StackTrace,
                        };
                        this.ScanJob.ScanErrors.Push(error);
                        Console.WriteLine("Error for web {1}: {0}", ex.Message, this.SiteUrl);
                    }
                }
            }
            finally
            {
                this.StopTime = DateTime.Now;
            }

            // return the duration of this scan
            return new TimeSpan((this.StopTime.Subtract(this.StartTime).Ticks));
        }

        private int ContinueScanning(ClientContext cc)
        {
            // Check site collection
            if (this.siteScanResult != null)
            {                
                Web web = cc.Web;

                // "Classic" publishing portal found
                if ((this.siteScanResult.WebTemplate == "BLANKINTERNET#0" || this.siteScanResult.WebTemplate == "ENTERWIKI#0" || this.siteScanResult.WebTemplate == "SRCHCEN#0") &&
                    (this.siteScanResult.SitePublishingFeatureEnabled && this.siteScanResult.WebPublishingFeatureEnabled))
                {
                    var pagesLibrary = web.GetListsToScan().Where(p => p.BaseTemplate == 850).FirstOrDefault();
                    if (pagesLibrary != null)
                    {
                        if (pagesLibrary.ItemCount > 0)
                        {
                            return pagesLibrary.ItemCount;
                        }
                    }

                    // always return a value in this case, if no pages found as this is a "classic" portal
                    return -1;
                }

                // Publishing enabled on non typical publishing portal site...check if there are pages in the Pages library
                if (this.siteScanResult.SitePublishingFeatureEnabled && this.siteScanResult.WebPublishingFeatureEnabled)
                {
                    
                    var pagesLibrary = web.GetListsToScan().Where(p => p.BaseTemplate == 850).FirstOrDefault();
                    if (pagesLibrary != null)
                    {
                        return pagesLibrary.ItemCount;
                    }
                }
            }
            return 0;
        }

    }
}
