using System;

namespace AngleSharp.Dom
{

    /// <summary>
    /// Extension methods for AngleSharp
    /// </summary>
    public static partial class AngleSharpExtensions
    {
        /// <summary>
        /// Performs a partial match on a list of tokens (e.g. classes on an element)
        /// </summary>
        /// <param name="tokenList">List of tokens to search in</param>
        /// <param name="filter">Partial token to match via an StartsWidth</param>
        /// <returns>First matching token if found, null if no match</returns>
        public static string PartialMatch(this ITokenList tokenList, string filter)
        {
            // No tokens then bail out
            if (tokenList.Length == 0)
            {
                return null;
            }

            foreach(var token in tokenList)
            {
                if (token.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
                {
                    return token;
                }
            }

            return null;
        }

    }
}
