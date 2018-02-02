using Microsoft.SharePoint.Client;
using SharePoint.Scanning.Framework;
using SharePoint.Visio.Scanner.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Visio.Scanner.Analyzers
{
    public class VisioWebPartAnalyzer: BaseAnalyzer
    {
        #region Variables
        private const string CAMLQueryByExtension = @"
                <View Scope='Recursive'>
                  <Query>
                    <Where>
                      <Contains>
                        <FieldRef Name='File_x0020_Type'/>
                        <Value Type='text'>aspx</Value>
                      </Contains>
                    </Where>
                  </Query>
                </View>";
        private const string FileRefField = "FileRef";
        // Stores the page search results for all pages in the site collection
        private List<Dictionary<string, string>> pageSearchResults;
        #endregion

        #region Construction
        /// <summary>
        /// Visio web part analyzer construction
        /// </summary>
        /// <param name="url">Url of the web to be analyzed</param>
        /// <param name="siteColUrl">Url of the site collection hosting this web</param>
        public VisioWebPartAnalyzer(string url, string siteColUrl, VisioScanJob scanJob, List<Dictionary<string, string>> pageSearchResults) : base(url, siteColUrl, scanJob)
        {
            this.pageSearchResults = pageSearchResults;
        }
        #endregion

        /// <summary>
        /// Analyze the web
        /// </summary>
        /// <param name="cc">ClientContext of the web to be analyzed</param>
        /// <returns>Duration of the analysis</returns>
        public override TimeSpan Analyze(ClientContext cc)
        {
            try
            {
                base.Analyze(cc);

                Web web = cc.Web;
                cc.Web.EnsureProperties(p => p.WebTemplate, p => p.Configuration, p => p.RootFolder);

                var homePageUrl = web.RootFolder.WelcomePage;
                var listsToScan = web.GetListsToScan();
                var sitePagesLibraries = listsToScan.Where(p => p.BaseTemplate == (int)ListTemplateType.WebPageLibrary);

                if (sitePagesLibraries.Count() > 0)
                {
                    foreach (var sitePagesLibrary in sitePagesLibraries)
                    {
                        CamlQuery query = new CamlQuery
                        {
                            ViewXml = CAMLQueryByExtension
                        };
                        var pages = sitePagesLibrary.GetItems(query);
                        web.Context.Load(pages);
                        web.Context.ExecuteQueryRetry();

                        if (pages.FirstOrDefault() != null)
                        {
                            DateTime start;
                            bool forceCheckout = sitePagesLibrary.ForceCheckout;
                            foreach (var page in pages)
                            {
                                string pageUrl = null;
                                try
                                {
                                    if (page.FieldValues.ContainsKey(FileRefField) && !String.IsNullOrEmpty(page[FileRefField].ToString()))
                                    {
                                        pageUrl = page[FileRefField].ToString();
                                    }
                                    else
                                    {
                                        //skip page
                                        continue;
                                    }

                                    start = DateTime.Now;

                                    VisioWebPartScanResult visioWebPartResult = null;

                                    // Get page web parts
                                    var foundWebParts = page.WebParts();
                                    if (foundWebParts != null)
                                    {
                                        // Do we have a visio web part?                                        
                                        var visioParts = foundWebParts.Where(p => p.Type == "Microsoft.Office.Visio.Server.WebControls.VisioWebAccess, Microsoft.Office.Visio.Server, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c").ToList();
                                        if (visioParts.Count > 0)
                                        {
                                            Console.WriteLine($"Visio web part found on page {pageUrl}");

                                            visioWebPartResult = new VisioWebPartScanResult()
                                            {
                                                SiteColUrl = this.SiteCollectionUrl,
                                                SiteURL = this.SiteUrl,
                                                PageUrl = pageUrl,
                                                Library = sitePagesLibrary.RootFolder.ServerRelativeUrl,
                                                WebParts = foundWebParts,
                                            };

                                            // Get page change information
                                            visioWebPartResult.ModifiedAt = page.LastModifiedDateTime();
                                            visioWebPartResult.ModifiedBy = page.LastModifiedBy();

                                            // Grab this page from the search results to connect view information   
                                            // Is this page the web's home page?
                                            bool isHomePage = false;
                                            if (pageUrl.EndsWith(homePageUrl, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                isHomePage = true;
                                            }

                                            string fullPageUrl = $"https://{new Uri(this.SiteCollectionUrl).DnsSafeHost}{pageUrl}";
                                            if (isHomePage)
                                            {
                                                fullPageUrl = this.SiteUrl;
                                            }

                                            var searchPage = this.pageSearchResults.Where(x => x.Values.Contains(fullPageUrl)).FirstOrDefault();
                                            if (searchPage != null)
                                            {
                                                // Recent = last 14 days
                                                visioWebPartResult.ViewsRecent = searchPage["ViewsRecent"].ToInt32();
                                                visioWebPartResult.ViewsRecentUniqueUsers = searchPage["ViewsRecentUniqueUsers"].ToInt32();
                                                visioWebPartResult.ViewsLifeTime = searchPage["ViewsLifeTime"].ToInt32();
                                                visioWebPartResult.ViewsLifeTimeUniqueUsers = searchPage["ViewsLifeTimeUniqueUsers"].ToInt32();
                                            }

                                            if (!this.ScanJob.VisioWebPartScanResults.TryAdd(visioWebPartResult.PageUrl, visioWebPartResult))
                                            {
                                                ScanError error = new ScanError()
                                                {
                                                    Error = $"Could not add page scan result for {visioWebPartResult.PageUrl}",
                                                    SiteColUrl = this.SiteCollectionUrl,
                                                    SiteURL = this.SiteUrl,
                                                    Field1 = "PageAnalyzer",
                                                };
                                                this.ScanJob.ScanErrors.Push(error);
                                            }
                                        }
                                    }
                                                                       
                                    var duration = new TimeSpan((DateTime.Now.Subtract(start).Ticks));
                                    Console.WriteLine($"Scan of page {pageUrl} took {duration.Seconds} seconds");
                                }
                                catch (Exception ex)
                                {
                                    ScanError error = new ScanError()
                                    {
                                        Error = ex.Message,
                                        SiteColUrl = this.SiteCollectionUrl,
                                        SiteURL = this.SiteUrl,
                                        Field1 = "MainPageAnalyzerLoop",
                                        Field2 = ex.StackTrace,
                                        Field3 = pageUrl
                                    };
                                    this.ScanJob.ScanErrors.Push(error);
                                    Console.WriteLine("Error for page {1}: {0}", ex.Message, pageUrl);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                this.StopTime = DateTime.Now;
            }

            // return the duration of this scan
            return new TimeSpan((this.StopTime.Subtract(this.StartTime).Ticks));
        }
    }
}
