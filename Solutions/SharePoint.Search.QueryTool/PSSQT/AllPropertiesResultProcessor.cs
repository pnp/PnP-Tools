using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SearchQueryTool.Model;
using PSSQT.Helpers;
using SearchQueryTool.Helpers;

namespace PSSQT
{
    class AllPropertiesMaxRowsExhausted : Exception
    {
        public AllPropertiesMaxRowsExhausted() : base()
        {

        }
    }

    class AllPropertiesResultProcessor : PrimaryResultsResultProcessor
    {
        private static readonly string managedPropertiesRefiner = "ManagedProperties(filter=600/0/*)";

        private SearchQueryRequest request;

        protected int MaxRows { get; set; } = 100;
        protected int CurrentRow { get; set; } = 0;

        protected bool RetrieveAllPropertiesFailed = false;

        public AllPropertiesResultProcessor(SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest) : 
            base(cmdlet)
        {
            request = searchQueryRequest;
        }

        protected override void EnsurePropertiesPresent()
        {
            base.EnsurePropertiesPresent();

            Cmdlet.AddSelectProperty("WorkId");
        }

        protected override void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            CurrentRow = 0;

            try
            {
                base.ProcessPrimaryRelevantResults(relevantResults);

            }
            catch (AllPropertiesMaxRowsExhausted)
            {
                Cmdlet.WriteWarning(String.Format("Max number of rows ({0}) the {1} will process, was exhausted ", MaxRows, GetType().Name));
            }
        }

        protected override void PrimaryResultPopulateItem(ResultItem resultItem, PSObject item)
        {
            if (++CurrentRow > MaxRows)
            {
                throw new AllPropertiesMaxRowsExhausted();
            }

            RetrieveAllPropertiesFailed = false;

            // Perform a new search for this resultItem.workid and retrieve the refiner "managedproperties(filter=600/0/*)"
            var clonedRequest = request.Clone();
            var key = resultItem.Id();

            if (key == null)
            {
                throw new RuntimeException("Could not retrieve workid from result item.");    // todo: add more detail
            }

            clonedRequest.QueryText = String.Format("WorkId:\"{0}\"", resultItem[key]);
            clonedRequest.Refiners = managedPropertiesRefiner;

            try
            {
                var requestResponsePair = HttpRequestRunner.RunWebRequest(clonedRequest);

                var queryResults = requestResponsePair.GetResultItem<SearchQueryResult>();

                // Ensure we got primary results and refiner results
                if (queryResults.PrimaryQueryResult == null || queryResults.PrimaryQueryResult.RefinerResults == null)
                {
                    throw new RuntimeException("The ManagedProperties property does not contain values.");  // todo: add more detail
                }

                var refiners = queryResults.PrimaryQueryResult.RefinerResults[0];

                // Extract properties from refiner and query again
                clonedRequest.Refiners = String.Empty;

                clonedRequest.SelectProperties = String.Join(",", refiners.Select(x => x.Name).ToArray());
                clonedRequest.SelectProperties = clonedRequest.SelectProperties.Replace(",ClassificationLastScan", ""); // this mp messes up the call
                clonedRequest.HttpMethodType = HttpMethodType.Post;

                requestResponsePair = HttpRequestRunner.RunWebRequest(clonedRequest);

                queryResults = requestResponsePair.GetResultItem<SearchQueryResult>();

                if (queryResults.PrimaryQueryResult == null || queryResults.PrimaryQueryResult.RelevantResults == null)
                {
                    throw new RuntimeException("Failed to retrieve all properties for result item.");  // todo: add more detail
                }

                ResultItem relevantResult = queryResults.PrimaryQueryResult.RelevantResults[0];

                //Cmdlet.WriteVerbose(String.Format("AllPropertiesResultProcessor: TotalRows: {0}", MaxRows));

                AddAllProperties(item, key, relevantResult);

            }
            catch (Exception e)
            {
                RetrieveAllPropertiesFailed = true;
                Cmdlet.WriteWarning(String.Format("Failed to read all properties for result item with {0} {1}", key, resultItem[key]));  // todo: add detail
                Cmdlet.WriteDebug(e.Message);
            }

        }

        protected virtual void AddAllProperties(PSObject item, string key, ResultItem relevantResult)
        {
            AddItemProperty(item, key, relevantResult[key]);
            AddItemProperty(item, "AllProperties", relevantResult);
        }

        protected override void PrimaryResultWriteItem(PSObject item)
        {
            if (! RetrieveAllPropertiesFailed)
            {
                base.PrimaryResultWriteItem(item);
            }
        }

        protected override void UserNotification()
        {
            if (Cmdlet.RowLimit > MaxRows)
            {
                Cmdlet.WriteWarning(String.Format("The {1} will only process {0} primary results, then quit. Consider using -StartRow if necessary", MaxRows, GetType().Name));
            }
        }

        protected override void WarnAboutSecondaryResults()
        {
            // do nothing
        }
    }
}
