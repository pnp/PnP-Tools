using System;
using System.Collections.Generic;

namespace PSSQT.RankLogParser
{
    public class PidMappings : List<PidMapping>
    {
        private static readonly char[] sepChars = { ' ' };

        public PidMappings(string value)  // e.g. value = "[1:content::7:%default] [2:content::1:%default] [3:content::5:%default] [56:content::2:%default] [100:content::3:link] [10:content::6:link] [264:content::14:link] "
        {
            var items = value.Split(sepChars, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                Add(new PidMapping(item));
            }
        }
    }
}