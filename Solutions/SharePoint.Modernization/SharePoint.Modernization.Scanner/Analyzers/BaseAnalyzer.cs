using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Analyzers
{
    /// <summary>
    /// Base class for analyzers
    /// </summary>
    public abstract class BaseAnalyzer: IBaseAnalyzer
    {
        private string url;
        private string siteColUrl;
        public ModernizationScanJob ScanJob;

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="url">Url of the current web</param>
        /// <param name="siteColUrl">url of the current site collection</param>
        public BaseAnalyzer(string url, string siteColUrl, ModernizationScanJob scanJob)
        {
            this.url = url;
            this.siteColUrl = siteColUrl;
            this.ScanJob = scanJob;
        }

        /// <summary>
        /// Scan start time
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// Scan stop time
        /// </summary>
        public DateTime StopTime { get; set; }
        
        /// <summary>
        /// Site collection url being scanned
        /// </summary>
        public string SiteCollectionUrl
        {
            get
            {
                return this.siteColUrl;
            }
        }

        /// <summary>
        /// Site being scanned
        /// </summary>
        public string SiteUrl
        {
            get
            {
                return this.url;
            }
        }

        /// <summary>
        /// Virtual Analyze method
        /// </summary>
        /// <param name="cc">ClientContext of the web to be analyzed</param>
        /// <returns>Duration of the analysis</returns>
        public virtual TimeSpan Analyze(ClientContext cc)
        {
            this.StartTime = DateTime.Now;
            return new TimeSpan();
        }
    }
}
