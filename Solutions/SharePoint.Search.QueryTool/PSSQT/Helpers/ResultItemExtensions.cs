using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT.Helpers
{
    public static class ResultItemExtensions
    {

        public static string Id(this ResultItem resultItem)
        {
            return resultItem.Keys.FirstOrDefault(k => k.Equals("WorkId", StringComparison.InvariantCultureIgnoreCase) || k.Equals("DocId", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}