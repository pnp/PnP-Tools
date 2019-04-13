using SearchQueryTool.Model;
using System;
using System.Collections.Generic;

namespace PSSQT.ResultProcessor
{
    public class BaseQueryResultProcessor : AbstractQueryResultProcessor
    {
        public BaseQueryResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }

        public List<string> SelectedProperties
        {
            get
            {
                // this will throw an exception if we implement something other that SearchSPIndex in the future. Serves as a reminder to look into this.
                return ((SearchSPIndexCmdlet)Cmdlet).SelectProperties;
            }
        }

        public override void Process(SearchQueryResult queryResults)
        {
            ProcessSearchQueryResultInfo(queryResults);


            if (queryResults.PrimaryQueryResult.TotalRows == 0)
            {
                Cmdlet.WriteWarning("The query returned zero results.");
            }
            else
            {
                ProcessPrimaryQueryResults(queryResults.PrimaryQueryResult);
            }

            if (queryResults.SecondaryQueryResults != null)
            {
                ProcessSecondaryQueryResults(queryResults.SecondaryQueryResults);
            }

            if (queryResults.TriggeredRules != null)
            {
                ProcessTriggeredRules(queryResults.TriggeredRules);
            }

        }

        protected virtual void ProcessPrimaryQueryResults(QueryResult primaryQueryResult)
        {
            ProcessPrimaryQueryResultsInfo(primaryQueryResult);
            ProcessPrimaryRelevantResults(primaryQueryResult.RelevantResults);
            ProcessPrimaryRefinerResults(primaryQueryResult.RefinerResults);
        }

        protected virtual void ProcessSecondaryQueryResults(List<QueryResult> secondaryQueryResults)
        {
            ProcessSecondaryQueryResultsListInfo(secondaryQueryResults);

            if (secondaryQueryResults != null)
            {
                int resultGroup = 2;   // Primary results are result group 1
                foreach (var queryResult in secondaryQueryResults)
                {
                    ProcessSecondaryQueryResultsInfo(resultGroup, queryResult);
                    ProcessSecondaryRelevantResults(resultGroup, queryResult.RelevantResults);
                    ProcessSecondryRefinerResults(resultGroup, queryResult.RefinerResults);

                    resultGroup++;
                }
            }
        }

        protected virtual void ProcessTriggeredRules(List<string> triggeredRules)
        {
            WriteVerbose(String.Format("Number of triggered rules: {0}", triggeredRules != null ? triggeredRules.Count : 0));
        }



        protected virtual void ProcessSearchQueryResultInfo(SearchQueryResult queryResults)
        {
            WriteVerbose(String.Format("HTTP status code: {0}", queryResults.StatusCode));
            WriteVerbose(String.Format("Status description: {0}", queryResults.StatusDescription));
            WriteVerbose(String.Format("HTTP protocol version: {0}", queryResults.HttpProtocolVersion));
            WriteVerbose(String.Format("Response content type: {0}", queryResults.ContentType));
            WriteVerbose(String.Format("Is Partial Result: {0}", queryResults.IsPartial));

            if (queryResults.IsPartial)
            {
                WarnAboutPartialResults();
            }

            if (!string.IsNullOrWhiteSpace(queryResults.MultiGeoSearchStatus))
            {
                WriteVerbose(String.Format("Multi Geo Search Status: {0}", queryResults.MultiGeoSearchStatus));

                if (!queryResults.MultiGeoSearchStatus.Equals("Full", StringComparison.InvariantCultureIgnoreCase))
                {
                    WarnAboutPartialGeoResults(queryResults.MultiGeoSearchStatus);
                }
            }


            Cmdlet.WriteDebug(String.Format("Response headers: {0}", String.Join(",", queryResults.ResponseHeaders.AllKeys)));
        }

        protected virtual void WarnAboutPartialResults()
        {
            Cmdlet.WriteWarning("Search returned partial results!");
        }

        protected virtual void WarnAboutPartialGeoResults(string multiGeoSearchStatus)
        {
            Cmdlet.WriteWarning($"Multi-Geo results are not complete! Multi-Geo search status: {multiGeoSearchStatus}");
        }

        protected virtual void ProcessPrimaryQueryResultsInfo(QueryResult primaryQueryResult)
        {
            WriteVerbose(String.Format("Total rows: {0}", primaryQueryResult.TotalRows));
            WriteVerbose(String.Format("Total rows incl. duplicates: {0}", primaryQueryResult.TotalRowsIncludingDuplicates));
            WriteVerbose(String.Format("Query id: {0}", primaryQueryResult.QueryId));
            WriteVerbose(String.Format("Query modification: {0}", primaryQueryResult.QueryModification));
            WriteVerbose(String.Format("Query rule id: {0}", primaryQueryResult.QueryRuleId));
        }

        protected virtual void ProcessPrimaryRefinerResults(List<RefinerResult> refinerResults)
        {
            //WriteInfo(String.Format("Number of refiners: {0}", refinerResults != null ? refinerResults.Count : 0));
        }

        protected virtual void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            WriteVerbose(String.Format("Number of relevant primary results: {0}", relevantResults != null ? relevantResults.Count : 0));
        }

        protected virtual void ProcessSecondaryQueryResultsListInfo(List<QueryResult> secondaryQueryResults)
        {
            WriteVerbose(String.Format("Number of secondary result groups: {0}", secondaryQueryResults != null ? secondaryQueryResults.Count : 0));
        }

        protected virtual void ProcessSecondaryQueryResultsInfo(int resultGroup, QueryResult queryResult)
        {
            WriteVerbose(String.Format("Total rows in group {1}: {0}", queryResult.TotalRows, resultGroup));
            WriteVerbose(String.Format("Total rows incl. duplicates in group {1}: {0}", queryResult.TotalRowsIncludingDuplicates, resultGroup));
            WriteVerbose(String.Format("Query id group {1}: {0}", queryResult.QueryId, resultGroup));
            WriteVerbose(String.Format("Query modification in group {1}: {0}", queryResult.QueryModification, resultGroup));
            WriteVerbose(String.Format("Query rule id in group {1}: {0}", queryResult.QueryRuleId, resultGroup));

            if (!String.IsNullOrWhiteSpace(queryResult.ResultTitle))
            {
                WriteVerbose(String.Format("Result title in group {1}: {0}", queryResult.ResultTitle, resultGroup));
            }

            if (!String.IsNullOrWhiteSpace(queryResult.ResultTitleUrl))
            {
                WriteVerbose(String.Format("Result title url in group {1}: {0}", queryResult.ResultTitleUrl, resultGroup));
            }
        }

        protected virtual void ProcessSecondaryRelevantResults(int resultGroup, List<ResultItem> relevantResults)
        {
            WriteVerbose(String.Format("Number of relevant secondary results in group {1}: {0}", relevantResults != null ? relevantResults.Count : 0, resultGroup));
        }

        protected virtual void ProcessSecondryRefinerResults(int resultGroup, List<RefinerResult> refinerResults)
        {
            //throw new NotImplementedException();
        }

        protected virtual void WriteVerbose(string text)
        {
            Cmdlet.WriteVerbose(text);
        }
    }
}
