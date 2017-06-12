using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchQueryTool.Helpers
{
    public static class SampleStrings
    {
        public static string GetExampleStringFor(string propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                string property = propertyName.Trim().ToLower();
                return HelpPageResources.ResourceManager.GetString(property + "_ex");
            }

            return null;
        }
    }
}
