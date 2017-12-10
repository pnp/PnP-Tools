using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.PermissiveFile.Scanner
{
    /// <summary>
    /// Class holding the scan results
    /// </summary>
    public class ScanResult: Scan
    {
        /// <summary>
        /// Name of the scanned file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Extension of the scanned file
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// Number of script tags in the scanned file
        /// </summary>
        public int EmbeddedScriptTagCount { get; set; }
        /// <summary>
        /// Number of links to local (relative) html/htm files
        /// </summary>
        public int EmbeddedLocalHtmlLinkCount { get; set; }
        /// <summary>
        /// Number of links in this file
        /// </summary>
        public int EmbeddedLinkCount { get; set; }
        /// <summary>
        /// Site administrators/owners
        /// </summary>
        public string SiteAdmins { get; set; }
    }
}
