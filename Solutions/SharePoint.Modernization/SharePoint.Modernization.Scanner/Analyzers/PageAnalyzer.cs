using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Scanner.Results;
using SharePoint.Scanning.Framework;

namespace SharePoint.Modernization.Scanner.Analyzers
{
    /// <summary>
    /// Analyses a page
    /// </summary>
    public class PageAnalyzer : BaseAnalyzer
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
        private List<Dictionary<string, string>> pageSearchResults;
        #endregion

        #region Construction
        /// <summary>
        /// page analyzer construction
        /// </summary>
        /// <param name="url">Url of the web to be analyzed</param>
        /// <param name="siteColUrl">Url of the site collection hosting this web</param>
        public PageAnalyzer(string url, string siteColUrl, ModernizationScanJob scanJob, List<Dictionary<string, string>> pageSearchResults) : base(url, siteColUrl, scanJob)
        {
            this.pageSearchResults = pageSearchResults;
        }
        #endregion

        /// <summary>
        /// Analyses a page
        /// </summary>
        /// <param name="cc">ClientContext instance used to retrieve page data</param>
        /// <returns>Duration of the page analysis</returns>
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
                                    PageScanResult pageResult = new PageScanResult()
                                    {
                                        SiteColUrl = this.SiteCollectionUrl,
                                        SiteURL = this.SiteUrl,
                                        PageUrl = pageUrl,
                                        Library = sitePagesLibrary.RootFolder.ServerRelativeUrl,
                                    };

                                    // Is this page the web's home page?
                                    if (pageUrl.EndsWith(homePageUrl, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        pageResult.HomePage = true;
                                    }

                                    // Get the type of the page
                                    pageResult.PageType = page.PageType();

                                    // Get page web parts
                                    var pageAnalysis = page.WebParts(this.ScanJob.PageTransformation);
                                    if (pageAnalysis != null)
                                    {
                                        pageResult.Layout = pageAnalysis.Item1.ToString().Replace("Wiki_", "").Replace("WebPart_", "");
                                        pageResult.WebParts = pageAnalysis.Item2;
                                    }

                                    // Get page change information
                                    pageResult.ModifiedAt = page.LastModifiedDateTime();
                                    pageResult.ModifiedBy = page.LastModifiedBy();

                                    // Grab this page from the search results to connect view information                                
                                    string fullPageUrl = $"https://{new Uri(this.SiteCollectionUrl).DnsSafeHost}{pageUrl}";
                                    if (pageResult.HomePage)
                                    {
                                        fullPageUrl = this.SiteUrl;
                                    }

                                    var searchPage = this.pageSearchResults.Where(x => x.Values.Contains(fullPageUrl)).FirstOrDefault();
                                    if (searchPage != null)
                                    {
                                        // Recent = last 14 days
                                        pageResult.ViewsRecent = searchPage["ViewsRecent"].ToInt32();
                                        pageResult.ViewsRecentUniqueUsers = searchPage["ViewsRecentUniqueUsers"].ToInt32();
                                        pageResult.ViewsLifeTime = searchPage["ViewsLifeTime"].ToInt32();
                                        pageResult.ViewsLifeTimeUniqueUsers = searchPage["ViewsLifeTimeUniqueUsers"].ToInt32();
                                    }

                                    if (!this.ScanJob.PageScanResults.TryAdd(pageResult.PageUrl, pageResult))
                                    {
                                        ScanError error = new ScanError()
                                        {
                                            Error = $"Could not add page scan result for {pageResult.PageUrl}",
                                            SiteColUrl = this.SiteCollectionUrl,
                                            SiteURL = this.SiteUrl,
                                            Field1 = "PageAnalyzer",
                                        };
                                        this.ScanJob.ScanErrors.Push(error);
                                    }
                                    var duration = new TimeSpan((DateTime.Now.Subtract(start).Ticks));
                                    Console.WriteLine($"Scan of page {pageUrl} took {duration.Seconds} seconds");
                                }
                                catch(Exception ex)
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
