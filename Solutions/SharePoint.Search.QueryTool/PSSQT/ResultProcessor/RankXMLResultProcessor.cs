using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSSQT.ResultProcessor
{
    public class RankXMLResultProcessor : BaseRankDetailResultProcessor
    {
        public static readonly int maxResults = 100;

        public RankXMLResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void ProcessPrimaryQueryResults(QueryResult primaryQueryResult)
        {
                EnsureMaxResults(primaryQueryResult);

                base.ProcessPrimaryQueryResults(primaryQueryResult);
        }

        protected void EnsureMaxResults(QueryResult primaryQueryResult)
        {
            if (primaryQueryResult.TotalRows > maxResults)
            {
                // plan is to catch and resubmit with filter limiting result set to RowLimit or 100, whichever is smaller
                var ex = new RankDetailTooManyResults(String.Format("Too many results for rank detail: {0}. Max is {1}.", primaryQueryResult.TotalRows, maxResults));

                ex.QueryFilter = GetQueryFilter(primaryQueryResult.RelevantResults);

                throw ex;
            }
        }

        private string GetQueryFilter(List<ResultItem> relevantResults)
        {
            List<string> workids = new List<string>();
            int n = 1;

            foreach (var resultItem in relevantResults)
            {
                var key = resultItem.Keys.FirstOrDefault(k => k.Equals("workid", StringComparison.InvariantCultureIgnoreCase));

                if (!String.IsNullOrWhiteSpace(key))
                {
                    workids.Add(String.Format("WorkId={0}", resultItem[key]));
                }

                if (++n > maxResults) break;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(" (");
            sb.Append(String.Join(" OR ", workids));
            sb.Append(")");

            var filter = sb.ToString();

            Cmdlet.WriteDebug(String.Format("IncludeRankFilter: {0}", filter));

            return filter;
        }

    }
}
