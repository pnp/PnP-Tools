using System;

namespace SharePointPnP.Modernization.Framework.Transform
{
    /// <summary>
    /// This class defines the page transformation configuration for when there's a ModernizationCenter hooked up
    /// </summary>
    public class ModernizationCenterInformation
    {
        /// <summary>
        /// Url to the customer's modernization center site
        /// </summary>
        public Uri ModernizationCenterUri { get; set; }

        /// <summary>
        /// Add a page accept banner solution on the generated pages
        /// </summary>
        public bool AddPageAcceptBanner { get; set; }

    }
}
