using System;

namespace PSSQT.RankLogParser
{
    public class PidMapping
    {
        private static readonly char[] trimChars = { ']' };
        private static readonly char[] sepChars = { ':' };

        public PidMapping(string value)  // e.g. value = [1:content::7:%default]
        {
            if (value.StartsWith("["))
                value = value.Substring(1);
            if (value.EndsWith("]"))
                value = value.TrimEnd(trimChars);

            var items = value.Split(sepChars, StringSplitOptions.RemoveEmptyEntries);

            Pid = int.Parse(items[0]);
            Context = int.Parse(items[2]);
            UpdateGroup = items[3]; 
        }

        public int Context { get; private set; }
        public int Pid { get; private set; }
        public string UpdateGroup { get; private set; }
    }
}