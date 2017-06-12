using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchQueryTool.Helpers
{
    public class DataConverter
    {
        /// <summary>
        ///     Tries the convert to int.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static int? TryConvertToInt(string text)
        {
            var num = 0;
            if (Int32.TryParse(text, out num))
            {
                return num;
            }

            return null;
        }

        /// <summary>
        ///     Tries the convert to long.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static long? TryConvertToLong(string text)
        {
            long num = 0;
            if (Int64.TryParse(text, out num))
            {
                return num;
            }

            return null;
        }
    }
}
