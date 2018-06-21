using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SharePointPnP.Modernization.Framework;

namespace SharePointPnP.Modernization.DocsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the information about the builtin functions and selectors
            var assembly = Assembly.GetAssembly(typeof(SharePointPnP.Modernization.Framework.Functions.BuiltIn));
            var functionAnalyzer = new FunctionAnalyzer(assembly);
            var functions = functionAnalyzer.Analyze();
            var selectorAnalyzer = new SelectorAnalyzer(assembly);
            var selectors = selectorAnalyzer.Analyze();

            // Generate MD page for it
            string path = "modernize-userinterface-site-pages-api.md";
            var builder = new StringBuilder();
            builder.Append($"# Page Transformation Functions and Selectors{Environment.NewLine}{Environment.NewLine}");
            builder.Append($"## Summary{Environment.NewLine}{Environment.NewLine}");
            builder.Append($"The default page transformation configuration (webpartmapping.xml) uses built-in functions and selectors to drive the mapping from classic web parts to modern 1st party and 3rd party web parts. This page lists all the available functions.{Environment.NewLine}{Environment.NewLine}");
            builder.Append($"> [!Note]{Environment.NewLine}");
            builder.Append($"> This page is auto-generated, if you see issues please create a PR against the respective Transformation framework code base.{Environment.NewLine}{Environment.NewLine}");

            builder.Append($"## Functions{Environment.NewLine}{Environment.NewLine}");
            foreach(var function in functions)
            {
                builder.Append($"### {function.Name}{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"**Description:** {function.Description}{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"**Example:** `{function.Example}`{Environment.NewLine}{Environment.NewLine}");

                if (function.Inputs.Any())
                {
                    builder.Append($"#### Input parameters{Environment.NewLine}{Environment.NewLine}");
                    builder.Append($"Name|Description{Environment.NewLine}");
                    builder.Append($":-----|:----------{Environment.NewLine}");

                    foreach (var input in function.Inputs)
                    {
                        builder.Append($"{input.Name}|{input.Description}{Environment.NewLine}");
                    }
                }

                builder.Append($"#### Output parameters{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"Name|Description{Environment.NewLine}");
                builder.Append($":-----|:----------{Environment.NewLine}");

                foreach (var output in function.Outputs)
                {
                    builder.Append($"{output.Name}|{output.Description}{Environment.NewLine}");
                }
            }

            builder.Append($"## Selectors{Environment.NewLine}");
            foreach (var selector in selectors)
            {
                builder.Append($"### {selector.Name}{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"**Description:** {selector.Description}{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"**Example:** `{selector.Example}`{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"#### Input parameters{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"Name|Description{Environment.NewLine}");
                builder.Append($":-----|:----------{Environment.NewLine}");

                foreach (var input in selector.Inputs)
                {
                    builder.Append($"{input.Name}|{input.Description}{Environment.NewLine}");
                }

                builder.Append($"#### Output values{Environment.NewLine}{Environment.NewLine}");
                builder.Append($"Name|Description{Environment.NewLine}");
                builder.Append($":-----|:----------{Environment.NewLine}");

                foreach (var output in selector.Outputs)
                {
                    builder.Append($"{output.Name}|{output.Description}{Environment.NewLine}");
                }
            }

            System.IO.File.WriteAllText(path, builder.ToString());
        }
    }
}
