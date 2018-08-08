using OfficeDevPnP.Core.Entities;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Results
{
    public class PublishingScanResult: Scan
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
        /// Variation label set (if any)
        /// </summary>
        public string VariationLabel { get; set; }

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

        public string GlobalNavigationType { get; set; }

        public string CurrentNavigationType { get; set; }

        public bool? GlobalStruturalNavigationShowSubSites { get; set; }
        public bool? GlobalStruturalNavigationShowPages { get; set; }
        public bool? GlobalStruturalNavigationShowSiblings { get; set; }
        public int? GlobalStruturalNavigationMaxCount { get; set; }
        public bool? CurrentStruturalNavigationShowSubSites { get; set; }
        public bool? CurrentStruturalNavigationShowPages { get; set; }
        public bool? CurrentStruturalNavigationShowSiblings { get; set; }
        public int? CurrentStruturalNavigationMaxCount { get; set; }

        public string GlobalManagedNavigationTermSetId { get; set; }
        public string CurrentManagedNavigationTermSetId { get; set; }
        public bool? ManagedNavigationAddNewPages { get; set; }
        public bool? ManagedNavigationCreateFriendlyUrls { get; set; }

        public bool LibraryItemScheduling { get; set; }
        public bool LibraryEnableModeration { get; set; }
        public bool LibraryEnableVersioning { get; set; }
        public bool LibraryEnableMinorVersions { get; set; }
        public bool LibraryApprovalWorkflowDefined { get; set; }

        public string VariationLabels { get; set; }
        public string VariationSourceLabel { get; set; }

    }
}
