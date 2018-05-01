using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{
    class AllPropertiesInlineResultProcessor : AllPropertiesResultProcessor
    {
        public AllPropertiesInlineResultProcessor(SearchSPIndexCmdlet cmdlet, SearchQueryRequest searchQueryRequest) : 
            base(cmdlet, searchQueryRequest)
        {
            MaxRows = 1;
        }

        protected override void AddAllProperties(PSObject item, string key, ResultItem relevantResult)
        {
            foreach (var propertyName in relevantResult.Keys.OrderBy(p => p))
            {
                AddItemProperty(item, propertyName, relevantResult[propertyName]);
            }
        }
    }
}
