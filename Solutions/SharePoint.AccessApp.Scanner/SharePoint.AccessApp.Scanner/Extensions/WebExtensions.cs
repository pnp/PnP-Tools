using Microsoft.SharePoint.Client.Search.Query;
using SharePoint.AccessApp.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with site (both site collection and web site) creation, status, retrieval and settings
    /// </summary>
    public static partial class WebExtensions
    {
        /// <summary>
        /// Checks if the current web is a sub site or not
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <returns>True is sub site, false otherwise</returns>
        public static bool IsSubSite(this Web web)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));

            var site = (web.Context as ClientContext).Site;
            var rootWeb = site.EnsureProperty(s => s.RootWeb);

            web.EnsureProperty(w => w.Id);
            rootWeb.EnsureProperty(w => w.Id);

            if (rootWeb.Id != web.Id)
            {
                return true;
            }
            return false;
        }

        public static List<SiteEntity> SiteSearch(this Web web, string keywordQueryValue, bool trimDuplicates = false)
        {
            try
            {
                //Log.Debug(Constants.LOGGING_SOURCE, "Site search '{0}'", keywordQueryValue);

                List<SiteEntity> sites = new List<SiteEntity>();

                KeywordQuery keywordQuery = new KeywordQuery(web.Context);
                keywordQuery.TrimDuplicates = trimDuplicates;

                //int startRow = 0;
                int totalRows = 0;

                totalRows = web.ProcessQuery(keywordQueryValue, sites, keywordQuery);


                if (totalRows > 0)
                {
                    while (totalRows > 0)
                    {
                        totalRows = web.ProcessQuery(keywordQueryValue + " AND IndexDocId >" + sites.Last().IndexDocId, sites, keywordQuery);// From the second Query get the next set (rowlimit) of search result based on IndexDocId
                    }
                }

                return sites;
            }
            catch (Exception ex)
            {
                //Log.Error(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_SiteSearchUnhandledException, ex.Message);
                // rethrow does lose one line of stack trace, but we want to log the error at the component boundary
                throw;
            }
        }

        private static int ProcessQuery(this Web web, string keywordQueryValue, List<SiteEntity> sites, KeywordQuery keywordQuery)
        {
            int totalRows = 0;

            keywordQuery.QueryText = keywordQueryValue;
            keywordQuery.RowLimit = 500;
            // keywordQuery.StartRow = startRow;
            keywordQuery.SelectProperties.Add("Title");
            keywordQuery.SelectProperties.Add("Path");
            keywordQuery.SelectProperties.Add("Description");
            keywordQuery.SelectProperties.Add("WebTemplate");
            keywordQuery.SelectProperties.Add("IndexDocId"); // Change : Include IndexDocId property to get the IndexDocId for paging
            keywordQuery.SortList.Add("IndexDocId", SortDirection.Ascending); // Change : Sort by IndexDocId
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
                        sites.Add(new SiteEntity
                        {
                            Title = row["Title"] != null ? row["Title"].ToString() : "",
                            Url = row["Path"] != null ? row["Path"].ToString() : "",
                            Description = row["Description"] != null ? row["Description"].ToString() : "",
                            Template = row["WebTemplate"] != null ? row["WebTemplate"].ToString() : "",
                            IndexDocId = row["DocId"] != null ? double.Parse(row["DocId"].ToString()) : 0, // Change : Include IndexDocId in the sites List
                        });
                    }
                }
            }

            return totalRows;
        }


    }
}
