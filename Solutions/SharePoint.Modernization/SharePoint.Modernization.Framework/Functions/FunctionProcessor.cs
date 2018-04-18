using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OfficeDevPnP.Core.Pages;
using SharePoint.Modernization.Framework.Entities;

namespace SharePoint.Modernization.Framework.Functions
{
    /// <summary>
    /// Class that executes functions and selectors defined in the mapping 
    /// </summary>
    public class FunctionProcessor
    {
        /// <summary>
        /// Allowed function parameter types
        /// </summary>
        enum FunctionType
        {
            String = 0,
            Integer = 1,
            Bool = 2,
            Guid = 3,
            DateTime = 4
        }

        /// <summary>
        /// Definition of a function parameter
        /// </summary>
        class FunctionParameter
        {
            /// <summary>
            /// Name of the parameter
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Type if the parameter
            /// </summary>
            public FunctionType Type { get; set; }
            /// <summary>
            /// Value of the parameter
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// Definition of a function or selector
        /// </summary>
        class FunctionDefinition
        {
            /// <summary>
            /// AddOn hosting the function/selector. Empty value means the function is hosted by the internal builtin functions library
            /// </summary>
            public string AddOn { get; set; }
            /// <summary>
            /// Name of the function/selector
            /// </summary>
            public string Name { get;set;}
            /// <summary>
            /// Parameter specifying the function result
            /// </summary>
            public FunctionParameter Output { get; set; }
            /// <summary>
            /// List of input parameter used to call the function
            /// </summary>
            public List<FunctionParameter> Input { get; set; }
        }

        /// <summary>
        /// Defines a loaded AddOn function/selector class instance
        /// </summary>
        class AddOnType
        {
            /// <summary>
            /// Name of the addon. The name is used to link the determine which class instance needs to be used to execute a function
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Instance of the class that holds the functions/selectors
            /// </summary>
            public object Instance { get; set; }
            /// <summary>
            /// Assembly holding the functions/selector class
            /// </summary>
            public Assembly Assembly { get; set; }
            /// <summary>
            /// Type of the functions/selector class
            /// </summary>
            public Type Type { get; set; }
        }

        private ClientSidePage page;
        private PageTransformation pageTransformation;
        private List<AddOnType> addOnTypes;
        private object builtInFunctions;

        #region Construction
        /// <summary>
        /// Instantiates the function processor. Also loads the defined add-ons
        /// </summary>
        /// <param name="page">Client side page for which we're executing the functions/selectors as part of the mapping</param>
        /// <param name="pageTransformation">Webpart mapping information</param>
        public FunctionProcessor(ClientSidePage page, PageTransformation pageTransformation)
        {
            this.page = page;
            this.pageTransformation = pageTransformation;

            // instantiate default built in functions class
            this.addOnTypes = new List<AddOnType>();
            this.builtInFunctions = Activator.CreateInstance(typeof(BuiltIn), this.page.Context);

            // instantiate the custom function classes (if there are)
            foreach (var addOn in this.pageTransformation.AddOns)
            {
                try
                {
                    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, addOn.Assembly);
                    var assembly = Assembly.LoadFile(path);
                    var customType = assembly.GetType(addOn.Type);
                    var instance = Activator.CreateInstance(customType, this.page.Context);

                    this.addOnTypes.Add(new AddOnType()
                    {
                        Name = addOn.Name,
                        Assembly = assembly,
                        Instance = instance,
                        Type = customType,
                    });
                }
                catch(Exception ex)
                {
                    // TODO: Add logging
                    throw;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Executes the defined functions and selectors in the provided web part
        /// </summary>
        /// <param name="webPartData">Web Part mapping data</param>
        /// <param name="webPart">Definition of the web part to be transformed</param>
        /// <returns>The ouput of the mapping selector if there was one executed, null otherwise</returns>
        public string Process(ref WebPart webPartData, WebPartEntity webPart)
        {
            // First process the transform functions
            foreach (var property in webPartData.Properties.ToList())
            {
                // No function defined, so skip
                if (string.IsNullOrEmpty(property.Functions))
                {
                    continue;
                }

                // Multiple functions can be specified using ; as delimiter
                var functionsToProcess = property.Functions.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                // Process each function
                foreach (var function in functionsToProcess)
                {
                    // Parse the function
                    FunctionDefinition functionDefinition = ParseFunctionDefinition(function, property, webPartData, webPart);

                    // Execute function
                    MethodInfo methodInfo = null;
                    object functionClassInstance = null;

                    if (string.IsNullOrEmpty(functionDefinition.AddOn))
                    {
                        // Native builtin function
                        methodInfo = typeof(BuiltIn).GetMethod(functionDefinition.Name);
                        functionClassInstance = this.builtInFunctions;
                    }
                    else
                    {
                        // Function specified via addon
                        var addOn = this.addOnTypes.Where(p => p.Name.Equals(functionDefinition.AddOn, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (addOn != null)
                        {
                            methodInfo = addOn.Type.GetMethod(functionDefinition.Name);
                            functionClassInstance = addOn.Instance;
                        }
                    }

                    if (methodInfo != null)
                    {
                        // Execute the function
                        object result = ExecuteMethod(functionClassInstance, functionDefinition, methodInfo);

                        // Update the existing web part property or add a new one
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
                MethodInfo methodInfo = null;
                object functionClassInstance = null;

                if (string.IsNullOrEmpty(functionDefinition.AddOn))
                {
                    // Native builtin function
                    methodInfo = typeof(BuiltIn).GetMethod(functionDefinition.Name);
                    functionClassInstance = this.builtInFunctions;
                }
                else
                {
                    // Function specified via addon
                    var addOn = this.addOnTypes.Where(p => p.Name.Equals(functionDefinition.AddOn, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (addOn != null)
                    {
                        methodInfo = addOn.Type.GetMethod(functionDefinition.Name);
                        functionClassInstance = addOn.Instance;
                    }
                }

                if (methodInfo != null)
                {
                    // Execute the selector
                    object result = ExecuteMethod(functionClassInstance, functionDefinition, methodInfo);
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
            // - MyLib.EncodeGuid()
            // - EncodeGuid({ListId})
            // - EncodeGuid({ListId}, {Param2})
            // - {ViewId} = EncodeGuid()
            // - {ViewId} = EncodeGuid({ListId})
            // - {ViewId} = MyLib.EncodeGuid({ListId})
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


            string functionName = functionString.Substring(0, functionString.IndexOf("("));
            if (functionName.IndexOf(".") > -1)
            {
                // This is a custom function
                def.AddOn = functionName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                def.Name = functionName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
            else
            {
                // this is an BuiltIn function
                def.AddOn = "";
                def.Name = functionString.Substring(0, functionString.IndexOf("("));
            }
            
            def.Input = new List<FunctionParameter>();

            var functionParameters = functionString.Substring(functionString.IndexOf("(") + 1).Replace(")", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var functionParameter in functionParameters)
            {
                FunctionParameter input = new FunctionParameter()
                {
                    Name = functionParameter.Replace("{", "").Replace("}", "").Trim(),
                };

                // Populate the function parameter with a value coming from the analyzed web part
                var wpProp = webPartData.Properties.Where(p => p.Name.Equals(input.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (wpProp != null)
                {
                    // Map types used in the model to types used in function processor
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
                // Call the method without parameters
                result = methodInfo.Invoke(functionClassInstance, null);
            }
            else
            {
                // Method requires input, so fill the parameters
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

                // Call the method with parameters
                result = methodInfo.Invoke(functionClassInstance, paramInput.ToArray());
            }

            // Return the method invocation result
            return result;
        }
        #endregion

    }
}
