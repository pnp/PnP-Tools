using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;
using SharePoint.Modernization.Framework.Entities;

namespace SharePoint.Modernization.Framework.Functions
{
    public class FunctionProcessor
    {
        enum FunctionType
        {
            String = 0,
            Integer = 1,
            Bool = 2,
            Guid = 3,
            DateTime = 4
        }

        class FunctionParameter
        {
            public string Name { get; set; }
            public FunctionType Type { get; set; }
            public string Value { get; set; }
        }

        class FunctionDefinition
        {
            public string Name { get;set;}
            public FunctionParameter Output { get; set; }
            public List<FunctionParameter> Input { get; set; }
        }


        private ClientSidePage page;

        #region Construction
        public FunctionProcessor(ClientSidePage page)
        {
            this.page = page;
        }
        #endregion

        #region Public methods
        public string Process(ref WebPart webPartData, WebPartEntity webPart)
        {

            var builtInFunctions = Activator.CreateInstance(typeof(BuiltIn), this.page.Context);

            // First process the transform functions
            foreach (var property in webPartData.Properties.ToList())
            {
                if (string.IsNullOrEmpty(property.Transform))
                {
                    continue;
                }

                var functionsToProcess = property.Transform.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var function in functionsToProcess)
                {                   
                    // Parse the function
                    FunctionDefinition functionDefinition = ParseFunctionDefinition(function, property, webPartData, webPart);

                    // Execute function
                    MethodInfo methodInfo = typeof(BuiltIn).GetMethod(functionDefinition.Name);
                    if (methodInfo != null)
                    {
                        object result = ExecuteMethod(builtInFunctions, functionDefinition, methodInfo);

                        if (webPart.Properties.Keys.Contains<string>(functionDefinition.Output.Name))
                        {
                            webPart.Properties[functionDefinition.Output.Name] = result.ToString();
                        }
                        else
                        {
                            webPart.Properties.Add(functionDefinition.Output.Name, result.ToString());
                        }
                    }

                }
            }

            // Process the selector function
            if (!string.IsNullOrEmpty(webPartData.Mappings.Selector))
            {
                FunctionDefinition functionDefinition = ParseFunctionDefinition(webPartData.Mappings.Selector, null, webPartData, webPart);

                // Execute function
                MethodInfo methodInfo = typeof(BuiltIn).GetMethod(functionDefinition.Name);
                if (methodInfo != null)
                {
                    object result = ExecuteMethod(builtInFunctions, functionDefinition, methodInfo);
                    return result.ToString();
                }
            }

            return null;
        }

        #endregion

        #region Helper methods
        private static FunctionDefinition ParseFunctionDefinition(string function, Property property, WebPart webPartData, WebPartEntity webPart)
        {
            // Supported function syntax: 
            // - EncodeGuid()
            // - EncodeGuid({ListId})
            // - EncodeGuid({ListId}, {Param2})
            // - {ViewId} = EncodeGuid()
            // - {ViewId} = EncodeGuid({ListId})
            // - {ViewId} = EncodeGuid({ListId}, {Param2})

            FunctionDefinition def = new FunctionDefinition();

            string functionString = null;
            if (function.IndexOf("=") > 0)
            {
                var split = function.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                FunctionParameter output = new FunctionParameter()
                {
                    Name = split[0].Replace("{", "").Replace("}", "").Trim(),
                    Type = FunctionType.String
                };

                def.Output = output;
                functionString = split[1].Trim();
            }
            else
            {
                FunctionParameter output = new FunctionParameter()
                {
                    Name = property != null ? property.Name : "SelectedMapping",
                    Type = FunctionType.String
                };

                def.Output = output;
                functionString = function.Trim();
            }

            def.Name = functionString.Substring(0, functionString.IndexOf("("));
            def.Input = new List<FunctionParameter>();

            var functionParameters = functionString.Substring(functionString.IndexOf("(") + 1).Replace(")", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var functionParameter in functionParameters)
            {
                FunctionParameter input = new FunctionParameter()
                {
                    Name = functionParameter.Replace("{", "").Replace("}", "").Trim(),
                };

                var wpProp = webPartData.Properties.Where(p => p.Name.Equals(input.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (wpProp != null)
                {
                    input.Type = MapType(wpProp.Type.ToString());

                    var wpInstanceProp = webPart.Properties.Where(p => p.Key.Equals(input.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    input.Value = wpInstanceProp.Value;
                }
                else
                {
                    throw new Exception($"Parameter {input.Name} was used but is not listed as a web part property that can be used.");
                }

                def.Input.Add(input);
            }

            return def;
        }

        private static FunctionType MapType(string inputType)
        {
            inputType = inputType.ToLower().Trim();

            if (inputType == "string")
            {
                return FunctionType.String;
            }
            else if (inputType == "integer")
            {
                return FunctionType.Integer;
            }
            else if (inputType == "bool")
            {
                return FunctionType.Bool;
            }
            else if (inputType == "guid")
            {
                return FunctionType.Guid;
            }
            else if (inputType == "datetime")
            {
                return FunctionType.DateTime;
            }

            return FunctionType.String;
        }

        private static object ExecuteMethod(object functionClassInstance, FunctionDefinition functionDefinition, MethodInfo methodInfo)
        {
            object result = null;
            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                result = methodInfo.Invoke(functionClassInstance, null);
            }
            else
            {
                List<object> paramInput = new List<object>(functionDefinition.Input.Count);
                foreach (var param in functionDefinition.Input)
                {
                    switch (param.Type)
                    {
                        case FunctionType.String:
                            {
                                paramInput.Add(param.Value);
                                break;
                            }
                        case FunctionType.Integer:
                            {
                                if (Int32.TryParse(param.Value, out Int32 i))
                                {
                                    paramInput.Add(i);
                                }
                                else
                                {
                                    paramInput.Add(Int32.MinValue);
                                }
                                break;
                            }
                        case FunctionType.Guid:
                            {
                                if (Guid.TryParse(param.Value, out Guid g))
                                {
                                    paramInput.Add(g);
                                }
                                else
                                {
                                    paramInput.Add(Guid.Empty);
                                }
                                break;
                            }
                        case FunctionType.DateTime:
                            {
                                if (DateTime.TryParse(param.Value, out DateTime d))
                                {
                                    paramInput.Add(d);
                                }
                                else
                                {
                                    paramInput.Add(DateTime.MinValue);
                                }
                                break;
                            }
                        case FunctionType.Bool:
                            {
                                if (bool.TryParse(param.Value, out bool b))
                                {
                                    paramInput.Add(b);
                                }
                                else
                                {
                                    paramInput.Add(false);
                                }
                                break;
                            }
                    }

                }

                result = methodInfo.Invoke(functionClassInstance, paramInput.ToArray());
            }

            return result;
        }
        #endregion

    }
}
