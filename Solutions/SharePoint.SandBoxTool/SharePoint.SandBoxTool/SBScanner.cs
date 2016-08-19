using Microsoft.SharePoint.Client;
using SharePoint.SandBoxTool.Framework.TimerJobs;
using SharePoint.SolutionAnalyzer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharePoint.SandBoxTool
{
    /// <summary>
    /// Sandboxed solution scan job. 
    /// </summary>
    public class SBScanner : TimerJob
    {
        public ConcurrentStack<SBScanResult> SBScanResults = new ConcurrentStack<SBScanResult>();
        public ConcurrentStack<SBScanError> SBScanErrors = new ConcurrentStack<SBScanError>();
        public ConcurrentDictionary<string, string> SBProcessed = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 100);
        public Int32 ScannedSites = 0;
        public Mode Mode;
        public string OutputFolder = "";
        public bool Duplicates = false;
        public bool Verbose = false;

        private static volatile bool firstSiteCollectionDone = false;
        private object scannedSitesLock = new object();

        public SBScanner() : base("SBInventory")
        {
            TimerJobRun += SBScanner_TimerJobRun;
            ExpandSubSites = false;
        }

        /// <summary>
        /// Event handler that's being executed by the threads processing the sites. Everything in here must be coded in a thread-safe manner
        /// </summary>
        private void SBScanner_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            lock (scannedSitesLock)
            {
                ScannedSites++;
            }

            Console.WriteLine("Processing site {0}...", e.Url);
            try
            {
                if (!firstSiteCollectionDone)
                {
                    firstSiteCollectionDone = true;

                    // Telemetry
                    e.WebClientContext.ClientTag = "SPDev:SBScanner";
                    e.WebClientContext.Load(e.WebClientContext.Web, p => p.Description);
                    e.WebClientContext.ExecuteQuery();
                }

                // Query the solution gallery
                CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery();
                ListItemCollection itemCollection = e.WebClientContext.Web.GetCatalog(121).GetItems(camlQuery);
                e.WebClientContext.Load(e.WebClientContext.Site, s => s.Id);
                e.WebClientContext.Load(itemCollection);
                e.WebClientContext.ExecuteQueryRetry();

                string siteOwner = string.Empty;
                int totalSolutions = 0;
                int assemblySolutions = 0;
                int activeSolutions = 0;
                int activeAssemblySolutions = 0;

                foreach (ListItem item in itemCollection)
                {
                    // We've found solutions
                    totalSolutions++;
                    bool status = false;
                    if (item["Status"] != null)
                    {
                        activeSolutions++;
                        status = true;
                    }
                    bool hasAssembly = false;
                    foreach (string s in item["MetaInfo"].ToString().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (s.Contains("SolutionHasAssemblies"))
                        {
                            if (s.Contains("1"))
                            {
                                assemblySolutions++;
                                if (status)
                                {
                                    activeAssemblySolutions++;
                                }
                                hasAssembly = true;
                            }
                            break;
                        }
                    }

                    if (hasAssembly && string.IsNullOrEmpty(siteOwner))
                    {
                        // Let's add site owners for the solutions which need our attention
                        List<string> admins = new List<string>();
                        UserCollection users = e.WebClientContext.Web.SiteUsers;
                        e.WebClientContext.Load(users);
                        e.WebClientContext.ExecuteQueryRetry();
                        foreach (User u in users)
                        {
                            if (u.IsSiteAdmin)
                            {
                                if (!string.IsNullOrEmpty(u.Email) && u.Email.Contains("@"))
                                {
                                    admins.Add(u.Email);
                                }
                            }
                        }

                        siteOwner = string.Join(";", admins.ToArray());
                    }

                    SBScanResult result = new SBScanResult()
                    {
                        SiteURL = e.Url,
                        SiteOwner = hasAssembly ? siteOwner : "",
                        WSPName = item["FileLeafRef"].ToString(),
                        Author = ((FieldUserValue)item["Author"]).LookupValue.Replace(",", ""),
                        CreatedDate = Convert.ToDateTime(item["Created"]),
                        Activated = status,
                        HasAssemblies = hasAssembly,
                        SolutionHash = item["SolutionHash"].ToString(),
                        SiteId = e.WebClientContext.Site.Id.ToString(),
                    };

                    // Doing more than a simple scan...
                    if (Mode == Mode.scananddownload || Mode == Mode.scanandanalyze)
                    {
                        // Only download the solution when there's an assembly. By default we're only downloading and scanning each unique solution just once
                        if (hasAssembly && (!SBProcessed.ContainsKey(result.SolutionHash) || Duplicates==true))
                        {
                            // Add this solution hash to the dictionary
                            SBProcessed.TryAdd(result.SolutionHash, "");

                            // Download the WSP package
                            ClientResult<Stream> data = item.File.OpenBinaryStream();
                            e.WebClientContext.Load(item.File);
                            e.WebClientContext.ExecuteQueryRetry();

                            if (data != null)
                            {
                                int position = 1;
                                int bufferSize = 200000;
                                Byte[] readBuffer = new Byte[bufferSize];
                                string localFilePath = System.IO.Path.Combine(".", this.OutputFolder, e.WebClientContext.Site.Id.ToString());
                                System.IO.Directory.CreateDirectory(localFilePath);

                                string wspPath = System.IO.Path.Combine(localFilePath, item["FileLeafRef"].ToString());
                                using (System.IO.Stream stream = System.IO.File.Create(wspPath))
                                {
                                    while (position > 0)
                                    {
                                        // data.Value holds the Stream
                                        position = data.Value.Read(readBuffer, 0, bufferSize);
                                        stream.Write(readBuffer, 0, position);
                                        readBuffer = new Byte[bufferSize];
                                    }
                                    stream.Flush();
                                }

                                // Analyze the WSP package by cracking it open and looking inside
                                if (Mode == Mode.scanandanalyze)
                                {
                                    Analyzer analyzer = new Analyzer();
                                    analyzer.Init(this.Verbose);
                                    var res = analyzer.ProcessFileInfo(System.IO.Path.GetFullPath(wspPath));

                                    result.IsInfoPath = res.InfoPathSolution;
                                    result.HasWebParts = (res.WebPartsCount > 0) || (res.UserControlsCount > 0) || res.Features.Where(f => f.WebParts.Any()).Count() > 0;
                                    result.HasWebTemplate = res.Features.Where(f => f.WebTemplateDetails.Any()).Count() > 0;
                                    result.HasFeatureReceivers = res.FeatureReceiversCount > 0 || res.Features.Where(f => f.FeatureReceivers.Any()).Count() > 0;
                                    result.HasEventReceivers = res.EventHandlersCount > 0 || res.Features.Where(f => f.EventReceivers.Any()).Count() > 0;
                                    result.HasListDefinition = res.ListTemplatesCount > 0 || res.Features.Where(f => f.ListTemplates.Any()).Count() > 0;
                                    result.HasWorkflowAction = res.Features.Where(f => f.WorkflowActionDetails.Any()).Count() > 0;
                                }
                            }
                        }
                    }

                    this.SBScanResults.Push(result);
                }

                if (totalSolutions > 0)
                {
                    Console.WriteLine("Site {0} processed. Found {1} solutions in total of which {2} have assemblies and are activated", e.Url, totalSolutions, activeAssemblySolutions);
                }

            }
            catch (Exception ex)
            {
                SBScanError error = new SBScanError()
                {
                    Error = ex.Message,
                    SiteURL = e.Url,
                };
                this.SBScanErrors.Push(error);
                Console.WriteLine("Error for site {1}: {0}", ex.Message, e.Url);
            }
        }

    }
}
