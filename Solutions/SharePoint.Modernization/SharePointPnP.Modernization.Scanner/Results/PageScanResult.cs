using SharePoint.Scanning.Framework;
using SharePointPnP.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;

namespace SharePoint.Modernization.Scanner.Results
{
    /// <summary>
    /// Stores the result of scanned page
    /// </summary>
    public class PageScanResult: Scan
    {
        public string PageUrl { get; set; }
        public string PageType { get; set; }
        public string Library { get; set; }
        public string Layout { get; set; }
        public bool HomePage { get; set; }

        // Page modification information
        public DateTime ModifiedAt { get; set; }
        public String ModifiedBy { get; set; }
        public int ViewsRecent { get; set; }
        public int ViewsRecentUniqueUsers { get; set; }
        public int ViewsLifeTime { get; set; }
        public int ViewsLifeTimeUniqueUsers { get; set; }

        // Page web part information
        public List<WebPartEntity> WebParts { get; set; }

    }
}
