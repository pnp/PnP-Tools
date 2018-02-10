using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharePoint.Scanning.Framework;
using System.Collections.Concurrent;
using Microsoft.SharePoint.Client;
using System.Threading;
using System.IO;

namespace SharePoint.PermissiveFile.Scanner
{
    /// <summary>
    /// Scanner that finds html/htm files and analyzes those. These files do not open anymore when using a strict model instead of permissive
    /// </summary>
    public class PermissiveScanJob : ScanJob
    {
        #region Variables
        private Int32 SitesToScan = 0;
        private IList<string> FileTypes;
        public ConcurrentDictionary<string, ScanResult> ScanResults = new ConcurrentDictionary<string, ScanResult>();
        #endregion

        #region Construction
        /// <summary>
        /// Instantiate the scanner
        /// </summary>
        /// <param name="options">Options instance</param>
        public PermissiveScanJob(Options options) : base(options as BaseOptions, "PermissiveScanner", "1.0")
        {
            ExpandSubSites = false;
            this.FileTypes = options.FileTypes;

            this.TimerJobRun += PermissiveScanJob_TimerJobRun;
        }
        #endregion

        #region Scanner implementation
        /// <summary>
        /// Override the default site resolving as we're using search to scope to the needed sites
        /// </summary>
        /// <param name="addedSites">Collection of sites from the default resolving</param>
        /// <returns>Updated set of site collections, which will be processed by the scanner</returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            if (addedSites != null && addedSites.Count > 0)
            {
                var sites = base.ResolveAddedSites(addedSites);
                this.SitesToScan = sites.Count;
                return sites;
            }
            else
            {
                try
                {
                    // Use search approach to determine which sites to process
                    List<string> searchedSites = new List<string>(100);

                    string tenantAdmin = "";
                    if (!string.IsNullOrEmpty(this.TenantAdminSite))
                    {
                        tenantAdmin = this.TenantAdminSite;
                    }
                    else
                    {
                        tenantAdmin = $"https://{this.Tenant}-admin.sharepoint.com";
                    }

                    this.Realm = GetRealmFromTargetUrl(new Uri(tenantAdmin));


                    using (ClientContext ccAdmin = this.CreateClientContext(tenantAdmin))
                    {
                        List<string> propertiesToRetrieve = new List<string>
                    {
                        "SPSiteUrl",
                        "FileExtension",
                        "OriginalPath"
                    };

                        // Get sites that contain a certain set of files, we'll only process these
                        var results = this.Search(ccAdmin.Web, $"({this.GetBaseSearchQuery()})", propertiesToRetrieve);
                        foreach (var site in results)
                        {
                            if (!string.IsNullOrEmpty(site["SPSiteUrl"]) && !searchedSites.Contains(site["SPSiteUrl"]))
                            {
                                searchedSites.Add(site["SPSiteUrl"]);
                            }
                        }
                    }

                    this.SitesToScan = searchedSites.Count;
                    return searchedSites;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Probem during application initialization. Application will terminate.");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.ToDetailedString());
                    Environment.Exit(1);
                }
            }

            return null;
        }

        private void PermissiveScanJob_TimerJobRun(object sender, OfficeDevPnP.Core.Framework.TimerJobs.TimerJobRunEventArgs e)
        {
            // thread safe increase of the sites counter
            IncreaseScannedSites();

            try
            {
                Console.WriteLine("Processing site {0}...", e.Url);

                // Set the first site collection done flag + perform telemetry
                SetFirstSiteCollectionDone(e.WebClientContext, this.Name);

                // Need to use search inside this site collection?
                List<string> propertiesToRetrieve = new List<string>
                {
                    "SPSiteUrl",
                    "FileExtension",
                    "OriginalPath",
                    "ViewsRecent",
                    "ViewsRecentUniqueUsers",
                    "ViewsLifeTime",
                    "ViewsLifeTimeUniqueUsers",
                    "LastModifiedTime",
                    "ModifiedBy"
                };
                var searchResults = this.Search(e.SiteClientContext.Web, $"({this.GetBaseSearchQuery()} AND Path:{e.Url.TrimEnd('/')}/*)", propertiesToRetrieve);
                foreach (var searchResult in searchResults)
                {

                    ScanResult result = new ScanResult()
                    {
                        SiteColUrl = e.Url,
                        FileName = searchResult["OriginalPath"],
                        FileExtension = searchResult["FileExtension"],
                        ViewsRecent = searchResult["ViewsRecent"].ToInt32(),
                        ViewsRecentUniqueUsers = searchResult["ViewsRecentUniqueUsers"].ToInt32(),
                        ViewsLifeTime = searchResult["ViewsLifeTime"].ToInt32(),
                        ViewsLifeTimeUniqueUsers = searchResult["ViewsLifeTimeUniqueUsers"].ToInt32(),
                        ModifiedAt = searchResult["LastModifiedTime"],
                        ModifiedBy = searchResult["ModifiedBy"]
                    };

                    // Analyse the files
                    var webUrlData = Web.GetWebUrlFromPageUrl(e.SiteClientContext, result.FileName);

                    Uri fileUri;
                    if (Uri.TryCreate(result.FileName, UriKind.Absolute, out fileUri) && 
                       (result.FileExtension.ToLower().Equals("html") || result.FileExtension.ToLower().Equals("htm")))
                    {
                        var fileContents = e.SiteClientContext.Web.GetFileAsString(fileUri.LocalPath);
                        var htmlScan = new HtmlScanner().Scan(fileContents);

                        result.EmbeddedLinkCount = htmlScan.LinkReferences;
                        result.EmbeddedLocalHtmlLinkCount = htmlScan.LocalHtmlLinkReferences;
                        result.EmbeddedScriptTagCount = htmlScan.ScriptReferences;                        
                    }

                    e.SiteClientContext.Load(e.SiteClientContext.Web, p => p.SiteUsers, p => p.AssociatedOwnerGroup);
                    e.SiteClientContext.Load(e.SiteClientContext.Web.AssociatedOwnerGroup, p => p.Users);
                    e.SiteClientContext.ExecuteQueryRetry();

                    if (e.SiteClientContext.Web.SiteUsers != null)
                    {
                        try
                        {
                            var admins = e.SiteClientContext.Web.SiteUsers.Where(p => p.IsSiteAdmin);
                            if (admins != null && admins.Count() > 0)
                            {
                                foreach (var admin in admins)
                                {
                                    if (!string.IsNullOrEmpty(admin.Email))
                                    {
                                        result.SiteAdmins = AddSiteOwner(result.SiteAdmins, admin.Email);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //Eat exceptions...rather log all files in the main result list instead of dropping some due to error getting owners
                        }

                        try
                        {
                            if (e.SiteClientContext.Web.AssociatedOwnerGroup != null && e.SiteClientContext.Web.AssociatedOwnerGroup.Users != null && e.SiteClientContext.Web.AssociatedOwnerGroup.Users.Count > 0)
                            {
                                foreach (var owner in e.SiteClientContext.Web.AssociatedOwnerGroup.Users)
                                {
                                    if (!string.IsNullOrEmpty(owner.Email))
                                    {
                                        result.SiteAdmins = AddSiteOwner(result.SiteAdmins, owner.Email);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //Eat exceptions...rather log all files in the main result list instead of dropping some due to error getting owners
                        }

                    }

                    result.SiteURL = webUrlData.Value;

                    // Store the scan result, use FileName as unique key in this sample
                    if (!ScanResults.TryAdd(result.FileName, result))
                    {
                        ScanError error = new ScanError()
                        {
                            SiteURL = result.SiteURL,
                            SiteColUrl = e.Url,
                            Error = "Could not add scan result for this web"                           
                        };
                        this.ScanErrors.Push(error);
                    }
                }
            }
            catch (Exception ex)
            {
                ScanError error = new ScanError()
                {
                    Error = ex.Message,
                    SiteColUrl = e.Url,
                    SiteURL = e.Url,
                };
                this.ScanErrors.Push(error);
                Console.WriteLine("Error for site {1}: {0}", ex.Message, e.Url);
            }

            // Output the scanning progress
            try
            {
                TimeSpan ts = DateTime.Now.Subtract(this.StartTime);
                Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}. Processed {this.ScannedSites} of {this.SitesToScan} site collections ({Math.Round(((float)this.ScannedSites / (float)this.SitesToScan) * 100)}%). Process running for {ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing progress: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Override of the scanner execute method, needed to output our results
        /// </summary>
        /// <returns>Time when scanning was started</returns>
        public override DateTime Execute()
        {
            // Triggers the run of the scanning...will result in PermissiveScanJob_TimerJobRun being called per site collection
            var start = base.Execute();

            // Handle the export of the job specific scanning data
            string outputfile = string.Format("{0}\\PermissiveScanResults.csv", this.OutputFolder);
            string[] outputHeaders = new string[] { "Site Collection Url", "Site Url", "File extension", "File name", "Link count", "Embedded html link count", "Script tag count",
                                                    "ModifiedBy", "ModifiedAt",
                                                    "ViewsRecent", "ViewsRecentUniqueUsers", "ViewsLifeTime", "ViewsLifeTimeUniqueUsers",
                                                    "Site admins and owners" };

            Console.WriteLine("Outputting scan results to {0}", outputfile);
            using (StreamWriter outfile = new StreamWriter(outputfile))
            {
                outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, outputHeaders)));
                foreach (var item in this.ScanResults)
                {
                    outfile.Write(string.Format("{0}\r\n", string.Join(this.Separator, ToCsv(item.Value.SiteColUrl), ToCsv(item.Value.SiteURL), ToCsv(item.Value.FileExtension), ToCsv(item.Value.FileName),
                                                                                       item.Value.EmbeddedLinkCount, item.Value.EmbeddedLocalHtmlLinkCount, item.Value.EmbeddedScriptTagCount,
                                                                                       ToCsv(item.Value.ModifiedBy), ToCsv(item.Value.ModifiedAt),
                                                                                       item.Value.ViewsRecent, item.Value.ViewsRecentUniqueUsers, item.Value.ViewsLifeTime, item.Value.ViewsLifeTimeUniqueUsers,
                                                                                       ToCsv(item.Value.SiteAdmins))));
                }
            }

            Console.WriteLine("=====================================================");
            Console.WriteLine("All done. Took {0} for {1} sites", (DateTime.Now - start).ToString(), this.ScannedSites);
            Console.WriteLine("=====================================================");

            return start;
        }

        private string AddSiteOwner(string ownerlist, string newOwner)
        {
            if (string.IsNullOrEmpty(ownerlist) || !ownerlist.Contains(newOwner))
            {
                return ownerlist + (!string.IsNullOrEmpty(ownerlist) ? $"|{newOwner}" : $"{newOwner}");
            }
            else
            {
                return ownerlist;
            }
        }

        private string GetBaseSearchQuery()
        {
            string additionalFileTypesToScan = "";
            if (this.FileTypes != null && this.FileTypes.Count > 0)
            {
                foreach (var fileType in this.FileTypes)
                {
                    additionalFileTypesToScan = additionalFileTypesToScan + $" OR fileextension={fileType}";
                }                
            }

            return $"(fileextension=htm OR fileextension=html{additionalFileTypesToScan}) AND contentclass=STS_ListItem_DocumentLibrary";
        }
        #endregion
    }
}
