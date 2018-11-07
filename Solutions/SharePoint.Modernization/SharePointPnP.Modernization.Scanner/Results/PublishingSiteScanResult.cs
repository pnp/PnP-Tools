using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;

namespace SharePoint.Modernization.Scanner.Results
{

    /// <summary>
    /// Publishing Site results
    /// </summary>
    public class PublishingSiteScanResult : Scan
    {

        public PublishingSiteScanResult()
        {
            this.UsedSystemMasterPages = new List<string>();
            this.UsedSiteMasterPages = new List<string>();
            this.UsedPageLayouts = new List<string>();
            this.UsedLanguages = new List<uint>();
        }

        /// <summary>
        /// Complexity classification of this publishing portal
        /// </summary>
        public SiteComplexity Classification { get; set; }

        /// <summary>
        /// Number of webs in this publishing portal
        /// </summary>
        public int NumberOfWebs { get; set; }

        /// <summary>
        /// Number of publishing pages in this publishing portal
        /// </summary>
        public int NumberOfPages { get; set; }

        /// <summary>
        /// List of the used system master pages across all webs in this publishing portal
        /// </summary>
        public List<string> UsedSystemMasterPages { get; set; }

        /// <summary>
        /// List of the used site master pages across all webs in this publishing portal
        /// </summary>
        public List<string> UsedSiteMasterPages { get; set; }

        /// <summary>
        /// List of the used page layouts across all pages in this publishing portal. Will be empty if page analysis was not selected in the scanner
        /// </summary>
        public List<string> UsedPageLayouts { get; set; }

        /// <summary>
        /// Most recent page update date. Will be empty if page analysis was not selected in the scanner
        /// </summary>
        public DateTime? LastPageUpdateDate { get; set; }

        /// <summary>
        /// The languages used in this publishing portal
        /// </summary>
        public List<uint> UsedLanguages { get; set; }
    }
}
