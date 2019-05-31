using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class AllResultsResultProcessor : PrimaryResultsResultProcessor
    {
        private static readonly string resultGroupPropertyName = "ResultGroup";
        public AllResultsResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void PrimaryResultPrePopulateItem(PSObject item)
        {
            item.Properties.Add(
                new PSVariableProperty(
                    new PSVariable(resultGroupPropertyName, 1)));
        }

        protected override void ProcessSecondaryRelevantResults(int resultGroup, List<ResultItem> relevantResults)
        {
            Cmdlet.WriteDebug(String.Format("Processing {0} relevant results in secondary result group {1}.", relevantResults != null ? relevantResults.Count : 0, resultGroup));

            if (relevantResults != null)
            {

                foreach (var resultItem in relevantResults)
                {
                    var item = new PSObject();

                    SecondaryResultPrePopulateItem(resultGroup, item);

                    AddSelectedProperties(resultItem, item);

                    SecondaryResultPostPopulateItem(resultGroup, item);

                    SecondaryResultWriteItem(resultGroup, item);

                }
            }
        }

        protected virtual void SecondaryResultPrePopulateItem(int resultGroup, PSObject item)
        {
            item.Properties.Add(
                new PSVariableProperty(
                    new PSVariable(resultGroupPropertyName, resultGroup)));
        }

        protected virtual void SecondaryResultPostPopulateItem(int resultGroup, PSObject item)
        {
        }

        protected virtual void SecondaryResultWriteItem(int resultGroup, PSObject item)
        {
            Cmdlet.WriteObject(item);
        }

        protected override void WarnAboutSecondaryResults()
        {
            // do nothing
        }

    }
}
