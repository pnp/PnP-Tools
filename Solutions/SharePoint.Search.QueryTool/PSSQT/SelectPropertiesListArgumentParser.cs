using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSSQT
{
    class SelectPropertiesListArgumentParser : StringListArgumentParser
    {
        private static readonly string hardcodedDefaultSelectProperties = "Title,Path,WorkId";
        private static readonly string re_env_pattern = @"^:(\w+):$";
        private static Regex re_env = new Regex(re_env_pattern, RegexOptions.IgnoreCase);

        private string searchQueryRequestSelectProperties;

        // PLEASE NOTE: second argument is NOT the default value!
        public SelectPropertiesListArgumentParser(List<string> list, SearchQueryRequest searchQueryRequest) :
            base(list)
        {
            this.searchQueryRequestSelectProperties = searchQueryRequest.SelectProperties;
        }

 

        protected override List<string> NormalizeList(List<string> list, bool splitIndividualItems)
        {
            var result = base.NormalizeList(list, splitIndividualItems);

            if (result.Contains(":default:", StringComparer.InvariantCultureIgnoreCase))
            {
                if (result.Count > 1)
                {
                    throw new ArgumentException("You cannot specify other properties in addition to :default:");
                }

                return null;    // Use whatever is returned from SharePoint
            }

            return result;
        }

        protected override string StringOrDefault(string listAsStr)
        {
            if (listAsStr == null)
            {
                if (list != null)    // user specified :default:
                {
                    return null;
                }
                else
                {
                    return searchQueryRequestSelectProperties ?? DefaultSelectProperties();
                }
            }

            return listAsStr;
        }


        protected virtual string DefaultSelectProperties()
        {
            // check environment variable "PSSQT_DefaultSelectProperties"
            if (GetPropertiesFromEnvironmentVariable("PSSQT_DefaultSelectProperties", out string value))
            {
                return value;
            }

            return hardcodedDefaultSelectProperties;
        }

        protected bool GetPropertiesFromEnvironmentVariable(string name, out string value)
        {
            var envVar = Environment.GetEnvironmentVariable(name);

            if (envVar == null)
            {
                value = null;
                return false;
            }

            // Cmdlet.WriteVerbose(String.Format("Using environment variable {0} which has a value {1}", name, envVar));

            var list = envVar.Split(',').ToList();

            list = NormalizeList(list, true);

            value = list != null ? String.Join(",", list) : null;

            return true;
        }
    }
}
