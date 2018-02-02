using SharePoint.Scanning.Framework;
using SharePoint.Visio.Scanner.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Visio.Scanner.Results
{
    public class VisioWebPartScanResult: Scan
    {
        public string PageUrl { get; set; }
        public string Library { get; set; }

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
