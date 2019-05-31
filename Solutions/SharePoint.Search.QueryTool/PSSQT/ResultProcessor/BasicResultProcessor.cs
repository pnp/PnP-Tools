using SearchQueryTool.Model;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class BasicResultProcessor : BasicAllResultProcessor
    {
        public BasicResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void PopulateItemFromSecondaryResults(SearchQueryResult queryResults, PSObject item)
        {
            // no secondary result information
        }

        protected override void PopulateItemFromQueryResult(QueryResult queryResult, PSObject item, int resultGroup)
        {
            item.Properties.Add(new PSVariableProperty(new PSVariable("TotalRows", queryResult.TotalRows)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("TotalRowsInclDups", queryResult.TotalRowsIncludingDuplicates)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryId", queryResult.QueryId)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryModification", queryResult.QueryModification)));
            item.Properties.Add(new PSVariableProperty(new PSVariable("QueryRuleId", queryResult.QueryRuleId)));
        }

    }
}
