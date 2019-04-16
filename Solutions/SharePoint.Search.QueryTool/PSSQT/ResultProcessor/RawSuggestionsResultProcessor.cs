using SearchQueryTool.Model;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{

    public class RawSuggestionsResultProcessor : SuggestionsResultProcessor
    {
        public RawSuggestionsResultProcessor(Cmdlet cmdlet) : base(cmdlet)
        {
        }


        public override void Process(SearchSuggestionsResult searchSuggestionsResult)
        {
            var item = new PSObject(searchSuggestionsResult);

            Cmdlet.WriteObject(item);
        }
    }
}
