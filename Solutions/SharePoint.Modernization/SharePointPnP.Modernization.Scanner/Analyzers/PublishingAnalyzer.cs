using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Scanner.Results;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Queries
        const string CAMLQueryByExtension = @"
                <View Scope='Recursive'>
                  <Query>
                    <Where>
                      <Contains>
                        <FieldRef Name='File_x0020_Type'/>
                        <Value Type='text'>aspx</Value>
                      </Contains>
                    </Where>
                  </Query>
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
                            scanResult.GlobalStruturalNavigationMaxCount = navigationSettings.GlobalNavigation.MaxDynamicItems;
                            scanResult.GlobalStruturalNavigationShowPages = navigationSettings.GlobalNavigation.ShowPages;
                            scanResult.GlobalStruturalNavigationShowSiblings = navigationSettings.GlobalNavigation.ShowSiblings;
                            scanResult.GlobalStruturalNavigationShowSubSites = navigationSettings.GlobalNavigation.ShowSubsites;
                        }

                        if (navigationSettings.CurrentNavigation.ManagedNavigation)
                        {
                            scanResult.CurrentNavigationType = "Managed";
                        }
                        else
                        {
                            scanResult.CurrentNavigationType = "Structural";
                            scanResult.CurrentStruturalNavigationMaxCount = navigationSettings.CurrentNavigation.MaxDynamicItems;
                            scanResult.CurrentStruturalNavigationShowPages = navigationSettings.CurrentNavigation.ShowPages;
                            scanResult.CurrentStruturalNavigationShowSiblings = navigationSettings.CurrentNavigation.ShowSiblings;
                            scanResult.CurrentStruturalNavigationShowSubSites = navigationSettings.CurrentNavigation.ShowSubsites;
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
                        foreach(var label in variationLabels)
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
