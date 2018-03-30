using SharePoint.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Transform
{
    public static class TokenParser
    {
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
