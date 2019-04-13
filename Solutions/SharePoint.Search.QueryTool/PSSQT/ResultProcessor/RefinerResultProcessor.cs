using SearchQueryTool.Model;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class RefinerResultProcessor : BaseQueryResultProcessor
    {
        public RefinerResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {

        }

        protected override void ProcessPrimaryRefinerResults(List<RefinerResult> refinerResults)
        {
            if (refinerResults != null && refinerResults.Count > 0)
            {
                foreach (var refinerResult in refinerResults)
                {

                    foreach (var refinerItem in refinerResult)
                    {
                        var item = new PSObject();

                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("RefinerName", refinerResult.Name)));

                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("RefinerCount", refinerResult.Count)));

                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("Name", refinerItem.Name)));
                        item.Properties.Add(
                                    new PSVariableProperty(
                                        new PSVariable("Count", refinerItem.Count)));
                        // This seems to always be the same as Name?
                        //item.Properties.Add(
                        //            new PSVariableProperty(
                        //                new PSVariable("Value", refinerItem.Value)));

                        Cmdlet.WriteObject(item);
                    }

                }
            }
            else
            {
                ZeroResultsWriteWarning();

            }
        }

        protected virtual void ZeroResultsWriteWarning()
        {
            Cmdlet.WriteWarning("The query returned zero refiner results. Make sure you specify which refiners to retrieve by using -Refiners.");
        }
    }


}
