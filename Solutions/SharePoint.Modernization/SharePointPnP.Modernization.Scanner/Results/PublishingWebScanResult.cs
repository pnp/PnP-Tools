using OfficeDevPnP.Core.Entities;
using SharePoint.Scanning.Framework;
using System.Collections.Generic;
using System.Linq;

namespace SharePoint.Modernization.Scanner.Results
{
    /// <summary>
    /// Publishing web results
    /// </summary>
    public class PublishingWebScanResult: Scan
    {
        /// <summary>
        /// Web relative Url
        /// </summary>
        public string WebRelativeUrl { get; set; }

        /// <summary>
        /// Web template (e.g. STS#0)
        /// </summary>
        public string WebTemplate { get; set; }

        /// <summary>
        /// Depth of this site in the site tree
        /// </summary>
        public int Level
        {
            get
            {
                return WebRelativeUrl.Count(f => f == '/');
            }
        }

        /// <summary>
        /// This site has unique permissions (broken permission inheritance)
        /// </summary>
        public bool BrokenPermissionInheritance { get; set; }

        /// <summary>
        /// Number of pages in the publishing page library of the site
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// System master page used if non default
        /// </summary>
        public string SystemMasterPage { get; set; }

        /// <summary>
        /// Site master page used if non default
        /// </summary>
        public string SiteMasterPage { get; set; }

        /// <summary>
        /// Alternate CSS defined for this web
        /// </summary>
        public string AlternateCSS { get; set; }

        /// <summary>
        /// Site language
        /// </summary>
        public uint Language { get; set; }

        /// <summary>
        /// Available page layouts
        /// </summary>
        public string AllowedPageLayouts { get; set; }

        /// <summary>
        /// Page layout configuration
        /// </summary>
        public string PageLayoutsConfiguration { get; set; }

        /// <summary>
        /// Default page layout
        /// </summary>
        public string DefaultPageLayout { get; set; }

        /// <summary>
        /// Site administrators
        /// </summary>
        public List<UserEntity> Admins { get; set; }
        /// <summary>
        /// Site owners
        /// </summary>
        public List<UserEntity> Owners { get; set; }

        /// <summary>
        /// Configuration set for the global (=top) navigation, structural or managed
        /// </summary>
        public string GlobalNavigationType { get; set; }

        /// <summary>
        /// Configuration set for the current (= left nav) navigation, structural or managed
        /// </summary>
        public string CurrentNavigationType { get; set; }

        /// <summary>
        /// Show sub sites in global navigation
        /// </summary>
        public bool? GlobalStructuralNavigationShowSubSites { get; set; }

        /// <summary>
        /// Show pages in global navigation
        /// </summary>
        public bool? GlobalStructuralNavigationShowPages { get; set; }

        /// <summary>
        /// Show siblings in global navigation
        /// </summary>
        public bool? GlobalStructuralNavigationShowSiblings { get; set; }

        /// <summary>
        /// Maximum number of global navigation entries visible in the user interface
        /// </summary>
        public int? GlobalStructuralNavigationMaxCount { get; set; }

        /// <summary>
        /// Show sub sites in current navigation
        /// </summary>
        public bool? CurrentStructuralNavigationShowSubSites { get; set; }
        
        /// <summary>
        /// Show pages in current navigation
        /// </summary>
        public bool? CurrentStructuralNavigationShowPages { get; set; }
        
        /// <summary>
        /// Show siblings in current navigation
        /// </summary>
        public bool? CurrentStructuralNavigationShowSiblings { get; set; }

        /// <summary>
        /// Maximum number of current navigation entries visible in the user interface
        /// </summary>
        public int? CurrentStructuralNavigationMaxCount { get; set; }

        /// <summary>
        /// If global managed navigation is used then contains the navigation termset id
        /// </summary>
        public string GlobalManagedNavigationTermSetId { get; set; }

        /// <summary>
        /// If current managed navigation is used then contains the navigation termset id
        /// </summary>
        public string CurrentManagedNavigationTermSetId { get; set; }

        /// <summary>
        /// If managed navigation is used then defines if new pages are automatically added
        /// </summary>
        public bool? ManagedNavigationAddNewPages { get; set; }

        /// <summary>
        /// If managed navigation is used then defines if friendly urls are automatically created
        /// </summary>
        public bool? ManagedNavigationCreateFriendlyUrls { get; set; }

        /// <summary>
        /// Is page publish scheduling configured for the pages library?
        /// </summary>
        public bool LibraryItemScheduling { get; set; }

        /// <summary>
        /// Is content approval defines for the pages library?
        /// </summary>
        public bool LibraryEnableModeration { get; set; }

        /// <summary>
        /// Is versioning enabled for the pages library?
        /// </summary>
        public bool LibraryEnableVersioning { get; set; }

        /// <summary>
        /// Is minor versioning enabled for the pages library?
        /// </summary>
        public bool LibraryEnableMinorVersions { get; set; }

        /// <summary>
        /// Is there an approval workflow enabled for the pages library?
        /// </summary>
        public bool LibraryApprovalWorkflowDefined { get; set; }

        /// <summary>
        /// Variation labels defined for this site
        /// </summary>
        public string VariationLabels { get; set; }

        /// <summary>
        /// The source variation label for this web
        /// </summary>
        public string VariationSourceLabel { get; set; }

    }
}
