using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class PrimaryResultsResultProcessor : BaseQueryResultProcessor
    {
        public PrimaryResultsResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
        }


        protected override void ProcessPrimaryRelevantResults(List<ResultItem> relevantResults)
        {
            Cmdlet.WriteDebug(String.Format("Processing {0} relevant results.", relevantResults == null ? 0 : relevantResults.Count));

            if (relevantResults != null)
            {

                foreach (var resultItem in relevantResults)
                {
                    var item = new PSObject();

                    PrimaryResultPrePopulateItem(item);

                    PrimaryResultPopulateItem(resultItem, item);

                    PrimaryResultPostPopulateItem(item);

                    PrimaryResultWriteItem(item);

                }
            }
        }

        protected virtual void PrimaryResultPopulateItem(ResultItem resultItem, PSObject item)
        {
            if (SelectedProperties != null)
            {
                AddSelectedProperties(resultItem, item);
            }
            else
            {
                foreach (var key in resultItem.Keys)
                {
                    item.Properties.Add(
                        new PSVariableProperty(
                            new PSVariable(key, resultItem[key])));
                }
            }
        }

        protected virtual void AddSelectedProperties(ResultItem resultItem, PSObject item)
        {
            // force the order of SelectProperties. If user specifies -Properties "Author,Title", they should appear in that order
            foreach (var selProp in SelectedProperties)
            {
                var key = resultItem.Keys.FirstOrDefault(k => k.Equals(selProp, StringComparison.InvariantCultureIgnoreCase));

                if (!String.IsNullOrWhiteSpace(key))
                {
                    AddItemProperty(item, key, resultItem[key]);
                }
            }
        }

        protected virtual void AddItemProperty(PSObject item, string key, object value)
        {
            item.Properties.Add(
                new PSVariableProperty(
                    new PSVariable(key, value)));
        }

        protected virtual void PrimaryResultWriteItem(PSObject item)
        {
            Cmdlet.WriteObject(item);
        }

        protected virtual void PrimaryResultPrePopulateItem(PSObject item)
        {
            // override in deriving classes if necessary;
        }

        protected virtual void PrimaryResultPostPopulateItem(PSObject item)
        {
            // override in deriving classes if necessary;
        }

        protected override void ProcessSecondaryQueryResults(List<QueryResult> secondaryQueryResults)
        {
            base.ProcessSecondaryQueryResults(secondaryQueryResults);

            if (secondaryQueryResults != null && secondaryQueryResults.Count > 0)
            {
                WarnAboutSecondaryResults();
            }
        }

        protected virtual void WarnAboutSecondaryResults()
        {
            Cmdlet.WriteWarning("There are secondary results. Use -ResultProcessor All to see them.");
        }

    }
}
