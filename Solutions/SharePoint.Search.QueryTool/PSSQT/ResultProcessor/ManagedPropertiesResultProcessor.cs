using SearchQueryTool.Model;

namespace PSSQT.ResultProcessor
{
    public class ManagedPropertiesResultProcessor : RefinerResultProcessor
    {
        private SearchQueryRequest request;

        public ManagedPropertiesResultProcessor(SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest) :
            base(cmdlet)
        {
            this.request = searchQueryRequest;
        }

        public override void Configure()
        {
            base.Configure();

            request.Refiners = "ManagedProperties(filter=600/0/*)";
        }

        protected override void ZeroResultsWriteWarning()
        {
            Cmdlet.WriteWarning("The query returned zero results for the managedproperties(filter=600/0/*) refiner.");
        }

    }
}
