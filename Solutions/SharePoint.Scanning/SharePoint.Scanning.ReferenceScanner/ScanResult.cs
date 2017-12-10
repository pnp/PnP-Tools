using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Scanning.ReferenceScanner
{
    /// <summary>
    /// Class holding the scan results
    /// </summary>
    public class ScanResult: Scan
    {
        public string SiteTitle { get; set; }
        public string FileName { get; set; }
    }
}
