using SearchQueryTool.Model;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    /*
     * I don't want to break backwards compatibility, so I'l leaving Basic as is and adding BasicAll instead
     */

    public class BasicAllResultProcessor : BaseQueryResultProcessor
    {
        public BasicAllResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        public override void Process(SearchQueryResult queryResults)
        {
            var item = new PSObject();

            PopulateItemFromSearchQueryResult(queryResults, item);

            if (queryResults.PrimaryQueryResult != null)
            {
                PopulateItemFromQueryResult(queryResults.PrimaryQueryResult, item, 1);
            }

            PopulateItemFromSecondaryResults(queryResults, item);

            Cmdlet.WriteObject(item);
        }

        protected virtual void PopulateItemFromSecondaryResults(SearchQueryResult queryResults, PSObject item)
        {
            if (queryResults.SecondaryQueryResults != null)
            {
                int resultGroup = 2;   // Primary results are result group 1

                foreach (var queryResult in queryResults.SecondaryQueryResults)
                {
                    PopulateItemFromQueryResult(queryResult, item, resultGroup);

                    resultGroup++;
                }

            }
        }

        protected virtual void PopulateItemFromSearchQueryResult(SearchQueryResult queryResults, PSObject item)
        {
            item.Properties.Add(new PSVariableProperty(new PSVariable("StatusCode", queryResults.StatusCode)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("StatusDescription", queryResults.StatusDescription)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("HTTPProtocalVersion", queryResults.HttpProtocolVersion)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ContentType", queryResults.ContentType)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ElapsedTime", queryResults.ElapsedTime)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("ElapsedMilliseconds", queryResults.ElapsedMilliseconds)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryElapsedTime", queryResults.QueryElapsedTime)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("NumSecondaryResults", queryResults.SecondaryQueryResults != null ? queryResults.SecondaryQueryResults.Count : 0)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("NumTriggeredRules", queryResults.TriggeredRules != null ? queryResults.TriggeredRules.Count : 0)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("TriggeredRules", queryResults.TriggeredRules)));
        }

        protected virtual void PopulateItemFromQueryResult(QueryResult queryResult, PSObject item, int resultGroup)
        {
            //item.Properties.Add(new PSVariableProperty(new PSVariable("ResponseHeaders", String.Join(",",queryResults.ResponseHeaders.AllKeys))));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] TotalRows", queryResult.TotalRows)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] TotalRowsInclDups", queryResult.TotalRowsIncludingDuplicates)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] QueryId", queryResult.QueryId)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] QueryModification", queryResult.QueryModification)));
            item.Properties.Add(new PSVariableProperty(new PSVariable($"[{resultGroup}] QueryRuleId", queryResult.QueryRuleId)));
        }
    }
}
