using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Framework.TimerJobs.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.SharePoint.Client;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using Microsoft.SharePoint.Client.Search.Query;

namespace SharePoint.Scanning.Framework
{
    /// <summary>
    /// Base scanner class, inherit from this class if you're building you're own scanning solutions
    /// </summary>
    public abstract class ScanJob : TimerJob
    {
        #region Variables
        public Int32 ScannedSites = 0;
        public Int32 ScannedWebs = 0;
        public Int32 ScannedLists = 0;
        private static volatile bool firstSiteCollectionDone = false;
        private object scannedSitesLock = new object();
        private object scannedWebsLock = new object();
        private object scannedListsLock = new object();
        public DateTime StartTime;
        public string OutputFolder;
        public string Separator = ",";
        public string CsvFile;
        public string Tenant;
        public string ClientTag;
        public IList<string> Urls;

        // Result stacks
        public ConcurrentStack<ScanError> ScanErrors = new ConcurrentStack<ScanError>();
        #endregion

        #region Construction
        /// <summary>
        /// Constructs scan job
        /// </summary>
        /// <param name="options">Scanner options</param>
        /// <param name="jobName">Name of the job</param>
        /// <param name="jobVersion">Version of the job</param>
        public ScanJob(BaseOptions options, string jobName, string jobVersion) : base(jobName, jobVersion)
        {
            // Basic scan job configuration
            this.UseThreading = true;
            this.MaximumThreads = options.Threads;
            this.TenantAdminSite = options.TenantAdminSite;
            this.OutputFolder = DateTime.Now.Ticks.ToString();
            this.Separator = options.Separator;
            this.ExcludeOD4B = !options.IncludeOD4B;
            this.CsvFile = options.CsvFile;
            this.Tenant = options.Tenant;
            this.Urls = options.Urls;
            this.ClientTag = ConstructClientTag(jobName);

            // Authentication setup
            if (options.AuthenticationTypeProvided() == AuthenticationType.AppOnly)
            {
                this.UseAppOnlyAuthentication(options.ClientID, options.ClientSecret);
            }
            else if (options.AuthenticationTypeProvided() == AuthenticationType.AzureADAppOnly)
            {
                this.UseAzureADAppOnlyAuthentication(options.ClientID, options.AzureTenant, options.CertificatePfx, options.CertificatePfxPassword);
            }
            else if (options.AuthenticationTypeProvided() == AuthenticationType.Office365)
            {
                this.UseOffice365Authentication(options.User, options.Password);
            }

            // Configure sites to scan
            if (!String.IsNullOrEmpty(this.Tenant))
            {
                this.AddSite(string.Format("https://{0}.sharepoint.com/*", this.Tenant));
                this.AddSite(string.Format("https://{0}-my.sharepoint.com/*", this.Tenant));
            }
            else if (this.Urls != null && this.Urls.Count > 0)
            {
                foreach (var url in this.Urls)
                {
                    this.AddSite(url.Trim());
                }
            }
            else if (!String.IsNullOrEmpty(this.CsvFile))
            {
                foreach (var row in LoadSitesFromCsv(this.CsvFile, this.Separator.ToCharArray().First()))
                {
                    this.AddSite(row[0].Trim()); //first column in the row contains url
                }
            }
            else
            {
                Console.WriteLine("No site selection specified, assume the job will use search to retrieve a list of sites");
            }

            this.StartTime = DateTime.Now;
        }
        #endregion

        #region Base execution methods
        /// <summary>
        /// Main method to execute the scanner
        /// </summary>
        /// <returns>DateTime when the scanning was started</returns>
        public virtual DateTime Execute()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is starting...{0}", start.ToString());
            Console.WriteLine("=====================================================");

            // Launch the job
            this.Run();

            // Dump scan results
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is done...now dump the results to a CSV file");
            Console.WriteLine("=====================================================");

            // Export the common CSV's (like errors)
            System.IO.Directory.CreateDirectory(this.OutputFolder);
            string outputfile = null;
            string[] outputHeaders = null;

            #region Errors
            string errorfile = string.Format("{0}\\errors.csv", this.OutputFolder);
            Console.WriteLine("Outputting errors to {0}", errorfile);
            outputHeaders = new string[] { "Site Url", "Site Collection Url", "Error", "Field1", "Field2", "Field3" };
            System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
            ScanError error;
            while (this.ScanErrors.TryPop(out error))
            {
                System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(error.SiteURL), ToCsv(error.SiteColUrl), ToCsv(error.Error), ToCsv(error.Field1), ToCsv(error.Field2), ToCsv(error.Field3))));
            }
            #endregion

            #region Scanner data
            outputfile = string.Format("{0}\\ScannerSummary.csv", this.OutputFolder);
            Console.WriteLine("Outputting information over the done scan to {0}", outputfile);
            outputHeaders = new string[] { "Site collections scanned", "Webs scanned", "List scanned", "Scan duration", "Scanner version" };
            System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));

            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(BaseOptions.UrlToFileName(assembly.EscapedCodeBase));
            string version = fvi.FileVersion;
            TimeSpan ts = DateTime.Now.Subtract(this.StartTime);
            System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(this.Separator, this.ScannedSites, this.ScannedWebs, this.ScannedLists, $"{ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds", version)));
            #endregion

            return start;
        }

        /// <summary>
        /// Load csv file and return data
        /// </summary>
        /// <param name="path">Path to CSV file</param>
        /// <param name="separator">Separator used in the CSV file</param>
        /// <returns>List of site collections</returns>
        private static IEnumerable<string[]> LoadSitesFromCsv(string path, params char[] separator)
        {
            return from line in System.IO.File.ReadLines(path)
                   let parts = (from p in line.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                                select p)
                   select parts.ToArray();
        }

        /// <summary>
        /// Prep a clienttag that will be used in telemetry 
        /// </summary>
        /// <param name="jobName">Scanner job</param>
        /// <returns>PnP Telemetry clienttag string</returns>
        private static string ConstructClientTag(string jobName)
        {
            jobName = jobName.Replace(" ", "");
            jobName = $"SPDev:{jobName}";
            return jobName.Length <= 32 ? jobName : jobName.Substring(0, 32);
        }

        /// <summary>
        /// Thread safe increase of the sites counter
        /// </summary>
        public void IncreaseScannedSites()
        {
            lock (scannedSitesLock)
            {
                ScannedSites++;
            }
        }

        /// <summary>
        /// Thread safe increase of the webs counter
        /// </summary>
        public void IncreaseScannedWebs()
        {
            lock (scannedWebsLock)
            {
                ScannedWebs++;
            }
        }

        /// <summary>
        /// Thread safe increase of the lists counter
        /// </summary>
        public void IncreaseScannedLists()
        {
            lock (scannedListsLock)
            {
                ScannedLists++;
            }
        }

        /// <summary>
        /// Triggers the steps needed once we're processing the first site collection
        /// </summary>
        /// <param name="ccWeb">ClientContext of the rootweb of the first site collection</param>
        public void SetFirstSiteCollectionDone(ClientContext ccWeb)
        {
            SetFirstSiteCollectionDone(ccWeb, "ScanningFramework");
        }

        /// <summary>
        /// Triggers the steps needed once we're processing the first site collection
        /// </summary>
        /// <param name="ccWeb">ClientContext of the rootweb of the first site collection</param>
        /// <param name="scannerName">Name of the scanner to log in telemetry. Leave empty to not log at all</param>
        public void SetFirstSiteCollectionDone(ClientContext ccWeb, string scannerName)
        {
            if (!firstSiteCollectionDone)
            {
                firstSiteCollectionDone = true;

                // Telemetry
                if (!string.IsNullOrEmpty(scannerName))
                {
                    ccWeb.ClientTag = $"SPDev:{scannerName}";
                    ccWeb.Load(ccWeb.Web, p => p.Description, p => p.Id);
                    ccWeb.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// Drop carriage returns, leading and trailing spaces + escape embedded quotes
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>CSV friendly string</returns>
        public static string ToCsv(string value)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                return $"\"{value.Trim().Replace("\r\n", string.Empty).Replace("\"", "\"\"")}\"";
            }
        }

        protected static string GetRealmFromTargetUrl(Uri targetApplicationUri)
        {
            WebRequest request = WebRequest.Create(targetApplicationUri + "/_vti_bin/client.svc");
            request.Headers.Add("Authorization: Bearer ");

            try
            {
                using (request.GetResponse())
                {
                }
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    return null;
                }

                string bearerResponseHeader = e.Response.Headers["WWW-Authenticate"];
                if (string.IsNullOrEmpty(bearerResponseHeader))
                {
                    return null;
                }

                const string bearer = "Bearer realm=\"";
                int bearerIndex = bearerResponseHeader.IndexOf(bearer, StringComparison.Ordinal);
                if (bearerIndex < 0)
                {
                    return null;
                }

                int realmIndex = bearerIndex + bearer.Length;

                if (bearerResponseHeader.Length >= realmIndex + 36)
                {
                    string targetRealm = bearerResponseHeader.Substring(realmIndex, 36);

                    Guid realmGuid;

                    if (Guid.TryParse(targetRealm, out realmGuid))
                    {
                        return targetRealm;
                    }
                }
            }
            return null;
        }

        public List<Dictionary<string, string>> Search(Web web, string keywordQueryValue, List<string> propertiesToRetrieve, bool trimDuplicates = false)
        {
            try
            {
                List<Dictionary<string, string>> sites = new List<Dictionary<string, string>>();

                KeywordQuery keywordQuery = new KeywordQuery(web.Context);
                keywordQuery.TrimDuplicates = trimDuplicates;                

                //property IndexDocId is required, so add it if not yet present
                if (!propertiesToRetrieve.Contains("IndexDocId"))
                {
                    propertiesToRetrieve.Add("IndexDocId");
                }

                int totalRows = 0;

                Console.WriteLine($"Start search query {keywordQueryValue}");
                totalRows = this.ProcessQuery(web, keywordQueryValue, propertiesToRetrieve, sites, keywordQuery);
                Console.WriteLine($"Found {totalRows} rows...");
                if (totalRows > 0)
                {
                    while (totalRows > 0)
                    {
                        string lastIndexDocIdString = "";
                        double lastIndexDocId = 0;

                        if (sites.Last().TryGetValue("IndexDocId", out lastIndexDocIdString))
                        {
                            lastIndexDocId = double.Parse(lastIndexDocIdString);
                            Console.WriteLine($"Retrieving a batch of up to 500 search results");
                            totalRows = this.ProcessQuery(web, keywordQueryValue + " AND IndexDocId >" + lastIndexDocId, propertiesToRetrieve, sites, keywordQuery);// From the second Query get the next set (rowlimit) of search result based on IndexDocId
                        }
                    }
                }

                return sites;
            }
            catch (Exception)
            {
                // rethrow does lose one line of stack trace, but we want to log the error at the component boundary
                throw;
            }
        }

        private int ProcessQuery(Web web, string keywordQueryValue, List<string> propertiesToRetrieve, List<Dictionary<string, string>> sites, KeywordQuery keywordQuery)
        {
            int totalRows = 0;
            keywordQuery.QueryText = keywordQueryValue;
            keywordQuery.RowLimit = 500;

            // Make the query return the requested properties
            foreach (var property in propertiesToRetrieve)
            {
                keywordQuery.SelectProperties.Add(property);
            }

            // Ensure sorting is done on IndexDocId to allow for performant paging
            keywordQuery.SortList.Add("IndexDocId", SortDirection.Ascending);

            SearchExecutor searchExec = new SearchExecutor(web.Context);

            // Important to avoid trimming "similar" site collections
            keywordQuery.TrimDuplicates = false;

            ClientResult<ResultTableCollection> results = searchExec.ExecuteQuery(keywordQuery);
            web.Context.ExecuteQueryRetry();

            if (results != null)
            {
                if (results.Value[0].RowCount > 0)
                {
                    totalRows = results.Value[0].TotalRows;

                    foreach (var row in results.Value[0].ResultRows)
                    {
                        Dictionary<string, string> item = new Dictionary<string, string>();

                        foreach(var property in propertiesToRetrieve)
                        {
                            if (row[property] != null)
                            {
                                item.Add(property, row[property].ToString());
                            }
                            else
                            {
                                item.Add(property, "");
                            }
                        }
                        sites.Add(item);
                    }
                }
            }

            return totalRows;
        }
        #endregion
    }
}
