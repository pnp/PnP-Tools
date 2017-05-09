using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.AccessApp.Scanner
{

    /// <summary>
    /// Object holding scan results
    /// </summary>
    public abstract class AccessAppScanResult
    {
        public string SiteUrl { get; set; }
        public string SiteColUrl { get; set; }
    }

    public class AccessAppScanData: AccessAppScanResult
    {
        public string ParentSiteUrl { get; set; }
        public string WebTitle { get; set; }
        public DateTime WebCreatedDate { get; set; }
        public string WebTemplate { get; set; }
        public Guid AppInstanceId { get; set; }
        public Guid WebId { get; set; }
        public string AppInstanceStatus { get; set; }
        public int ViewsRecent { get; set; }
        public int ViewsRecentUnique { get; set; }
        public int ViewsLifetime { get; set; }
        public int ViewsLifetimeUnique { get; set; }
    }

    /// <summary>
    /// Class holding the errors detected during scanning
    /// </summary>
    public class AccessAppScanError: AccessAppScanResult
    {
        public string Error { get; set; }
    }
}
