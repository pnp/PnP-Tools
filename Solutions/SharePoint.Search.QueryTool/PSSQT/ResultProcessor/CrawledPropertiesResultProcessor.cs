using SearchQueryTool.Model;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class CrawledPropertiesResultProcessor : RefinerResultProcessor
    {
        private SearchQueryRequest request;

        public CrawledPropertiesResultProcessor(SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest) :
            base(cmdlet)
        {
            this.request = searchQueryRequest;
        }

        public override void Configure()
        {
            base.Configure();

            request.Refiners = "CrawledProperties(filter=900/0/*)";
        }

        protected override void ZeroResultsWriteWarning()
        {
            Cmdlet.WriteWarning("The query returned zero results for the crawledproperties(filter=900/0/*) refiner.");
        }

    }
}
