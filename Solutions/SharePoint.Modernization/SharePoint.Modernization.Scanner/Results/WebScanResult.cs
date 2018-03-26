using OfficeDevPnP.Core.Entities;
using SharePoint.Scanning.Framework;
using System.Collections.Generic;

namespace SharePoint.Modernization.Scanner.Results
{
    /// <summary>
    /// Stores the results of the Web scan
    /// </summary>
    public class WebScanResult : Scan
    {
        /// <summary>
        /// Web template (e.g. STS#0)
        /// </summary>
        public string WebTemplate { get; set; }

        /// <summary>
        /// Modern page feature was disabled
        /// </summary>
        public bool ModernPageWebFeatureDisabled { get; set; }

        /// <summary>
        /// Was the modern page feature enabled
        /// </summary>
        public bool ModernPageFeatureWasEnabledBySPO { get; set; }

        /// <summary>
        /// Modern lists was disabled due to the web scoped blocking feature
        /// </summary>
        public bool ModernListWebBlockingFeatureEnabled { get; set; }

        /// <summary>
        /// Is the publishing web feature enabled?
        /// </summary>
        public bool WebPublishingFeatureEnabled { get; set; }

        /// <summary>
        /// Does this web have a modern home page?
        /// </summary>
        public bool ModernHomePage { get; set; }

        /// <summary>
        /// Non OOB master page used
        /// </summary>
        public string MasterPage { get; set; }

        /// <summary>
        /// Non OOB custom master page used
        /// </summary>
        public string CustomMasterPage { get; set; }

        /// <summary>
        /// Alternate CSS defined for this web
        /// </summary>
        public string AlternateCSS { get; set; }

        /// <summary>
        /// User custom actions which are ignored on modern UI
        /// </summary>
        public List<UserCustomActionResult> WebUserCustomActions { get; set;}

        /// <summary>
        /// This site has unique permissions (broken permission inheritance)
        /// </summary>
        public bool BrokenPermissionInheritance { get; set; }

        /// <summary>
        /// Site owners
        /// </summary>
        public List<UserEntity> Owners { get; set; }

        /// <summary>
        /// Site members
        /// </summary>
        public List<UserEntity> Members { get; set; }

        /// <summary>
        /// Site visitors
        /// </summary>
        public List<UserEntity> Visitors { get; set; }

        /// <summary>
        /// Is the everyone or everyone except external users claim used somewhere on this web
        /// </summary>
        public bool EveryoneClaimsGranted { get; set; }
    }
}
