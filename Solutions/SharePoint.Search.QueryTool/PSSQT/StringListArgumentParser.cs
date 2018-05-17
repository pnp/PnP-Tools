using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{
    class StringListArgumentParser
    {
        protected List<string> list;
        protected string defaultValue;

        public StringListArgumentParser(List<string> list, string defaulValue = default(String))
        {
            this.list = list;
            this.defaultValue = defaulValue;
        }

        public virtual string Parse()
        {
            string listAsStr = null;

            if (list != null)
            {
                var normalizedList = NormalizeList(list, true);

                if (normalizedList != null)
                {
                    listAsStr = String.Join(",", NormalizeList(list, true));
                }

            }

            return StringOrDefault(listAsStr);
        }

        // This method takes a list of strings and removes any duplicates.
        // It also looks at each individual entry in the list and splits the value on comma and add each split entry to the list
        // This is for backward compatibility where a argument used to be a string, possibly comma separated list, and now it has been changed to a list of strings
        protected virtual List<string> NormalizeList(List<string> list, bool splitIndividualItems)
        {
            List<string> result = null;

            if (list != null)
            {
                result = new List<string>();

                foreach (var listitem in list)
                {
                    var item = listitem.Trim();

                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        string[] stringitems;

                        if (splitIndividualItems)
                        {
                            stringitems = item.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            stringitems = new string[] { item };
                        }

                        foreach (var stringitem in stringitems)
                        {
                            var trimmedItem = stringitem.Trim();

                            if (!result.Contains(trimmedItem, StringComparer.InvariantCultureIgnoreCase))
                            {
                                AddItem(result, trimmedItem);
                            }
                        }
                    }
                }

                PostProcessList(result);
            }

            return result;
        }

        protected virtual void PostProcessList(List<string> result)
        {
            result.ForEach(e => e.ToLower());
        }

        protected virtual void AddItem(List<string> result, string stringitem)
        {
            result.Add(stringitem);
        }

        protected virtual string StringOrDefault(string listAsStr)
        {
            return listAsStr ?? defaultValue;
        }

    }
}
