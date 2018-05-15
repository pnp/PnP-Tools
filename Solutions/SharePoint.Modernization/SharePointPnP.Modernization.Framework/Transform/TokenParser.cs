using SharePointPnP.Modernization.Framework.Entities;
using System.Text.RegularExpressions;

namespace SharePointPnP.Modernization.Framework.Transform
{
    /// <summary>
    /// Resolves tokens by their actual representation
    /// </summary>
    public static class TokenParser
    {

        /// <summary>
        /// Replaces the tokens in the provided input string with their values
        /// </summary>
        /// <param name="input">String with tokens</param>
        /// <param name="webPartData">Web part information holding all possible tokens for this web part</param>
        /// <returns>A string with tokens replaced by actual values</returns>
        public static string ReplaceTokens(string input, WebPartEntity webPartData)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var tokenChars = new[] { '{' };
            if (string.IsNullOrEmpty(input) || input.IndexOfAny(tokenChars) == -1)
            {
                return input;
            }

            string origInput;
            do
            {
                origInput = input;
                foreach (var property in webPartData.Properties)
                {
                    if (property.Value != null)
                    {
                        var regex = new Regex($"{{{property.Key}}}", RegexOptions.IgnoreCase);
                        if (regex.IsMatch(input))
                        {
                            input = regex.Replace(input, property.Value);
                        }
                    }
                }
            } while (origInput != input && input.IndexOfAny(tokenChars) >= 0);

            return input;
        }
    }
}
