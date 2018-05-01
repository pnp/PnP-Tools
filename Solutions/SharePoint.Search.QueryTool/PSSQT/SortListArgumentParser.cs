using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{
    class SortListArgumentParser : StringListArgumentParser
    {
        private static readonly string sortDescending = ":descending";
        private static readonly string sortAscending = ":ascending";

        public SortListArgumentParser(List<string> list) :
            base(list)
        {
        }


        protected override List<string> NormalizeList(List<string> list, bool splitIndividualItems)
        {
            var sortListItems = base.NormalizeList(list, splitIndividualItems);

            List<string> results = new List<string>();

            foreach (var item in sortListItems)
            {
                if (item.EndsWith(sortDescending) || item.EndsWith(sortAscending))
                {
                    results.Add(item);
                }
                else
                {
                    var parts = item.Split(':');

                    if (parts.Length > 1)
                    {
                        if (sortAscending.StartsWith(String.Format(":{0}", parts[1])))
                        {
                            results.Add(String.Format("{0}{1}", parts[0], sortAscending));
                        }
                        else if (sortDescending.StartsWith(String.Format(":{0}", parts[1])))
                        {
                            results.Add(String.Format("{0}{1}", parts[0], sortDescending));
                        }
                        else
                        {
                            throw new Exception(String.Format("Unrecognized sort direction specifier: {0}, Use {1} or {2}", parts[1], sortAscending, sortDescending));
                        }
                    }
                    else
                    {
                        results.Add(String.Format("{0}{1}", item, sortAscending));
                    }
                }
            }

            return results;
        }

        protected override void AddItem(List<string> result, string stringitem)
        {
            if (stringitem.Equals("Rank", StringComparison.InvariantCultureIgnoreCase))
            {
                result.Add("Rank");   // todo: check this. For now we still do tolower()
            }
            else
            {
                base.AddItem(result, stringitem);
            }
        }

        protected override void PostProcessList(List<string> result)
        {
            // do nothing
        }
    }
}
