using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.UIExperience.Scanner.Reports
{
    public class ScanSummary
    {
        public int? SiteCollections { get; set; }
        public int? Webs { get; set; }
        public int? Lists { get; set; }
        public string Duration { get; set; }
        public string Version { get; set; }
    }
}
