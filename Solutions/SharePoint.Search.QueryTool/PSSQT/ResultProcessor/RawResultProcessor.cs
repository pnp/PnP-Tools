using SearchQueryTool.Model;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class RawResultProcessor : AbstractQueryResultProcessor
    {
        public RawResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        public override void Process(SearchQueryResult queryResults)
        {
            var item = new PSObject(queryResults);

            Cmdlet.WriteObject(item);
        }

    }
}
