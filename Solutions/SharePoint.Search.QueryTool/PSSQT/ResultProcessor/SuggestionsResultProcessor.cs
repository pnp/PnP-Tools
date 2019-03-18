using SearchQueryTool.Model;
using System;
using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class SuggestionsResultProcessor : ISuggestionsResultProcessor
    {
        public SuggestionsResultProcessor(Cmdlet cmdlet)
        {
            this.Cmdlet = cmdlet;
        }

        public Cmdlet Cmdlet { get; }

        public void Configure()
        {
            
        }

        public bool HandleException(Exception ex, SearchSuggestionsRequest searchSuggestionsRequest)
        {
            throw new NotImplementedException();
        }

        public virtual void Process(SearchSuggestionsResult searchSuggestionsResult)
        {
            foreach (var resultItem in searchSuggestionsResult.SuggestionResults)
            {
                var item = new PSObject();

                item.Properties.Add(new PSVariableProperty(new PSVariable("Query", resultItem.Query)));
                item.Properties.Add(new PSVariableProperty(new PSVariable("IsPersonal", resultItem.IsPersonal)));

                Cmdlet.WriteObject(item);
            }
        }
    }
}
