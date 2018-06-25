using SharePointPnP.Modernization.DocsGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.Modernization.DocsGenerator
{
    public class SelectorAnalyzer
    {
        private Assembly assembly;
        private List<Function> functions;

        public SelectorAnalyzer(Assembly assemblyToAnalyze)
        {
            assembly = assemblyToAnalyze;
            functions = new List<Function>();
        }

        public List<Function> Analyze()
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    var attributes = method.GetCustomAttributes<SharePointPnP.Modernization.Framework.Functions.SelectorDocumentationAttribute>();
                    if (attributes.FirstOrDefault() != null)
                    {
                        var function = new Function
                        {
                            Name = method.Name,
                            Description = attributes.FirstOrDefault().Description,
                            Example = attributes.FirstOrDefault().Example
                        };

                        var parameterInputs = method.GetCustomAttributes<SharePointPnP.Modernization.Framework.Functions.InputDocumentationAttribute>();
                        if (parameterInputs.Any())
                        {
                            foreach (var param in parameterInputs)
                            {
                                function.Inputs.Add(new FunctionParameter()
                                {
                                    Name = String.Format("{{{0}}}", param.Name.Replace("{", "").Replace("}","")),
                                    Description = param.Description
                                });
                            }
                        }

                        var parameterOutputs = method.GetCustomAttributes<SharePointPnP.Modernization.Framework.Functions.OutputDocumentationAttribute>();
                        if (parameterOutputs.Any())
                        {
                            foreach (var param in parameterOutputs)
                            {
                                function.Outputs.Add(new FunctionParameter()
                                {
                                    Name = param.Name.Replace("{", "").Replace("}", ""),
                                    Description = param.Description
                                });
                            }
                        }

                        functions.Add(function);
                    }
                }

            }

            return functions;
        }

    }
}
