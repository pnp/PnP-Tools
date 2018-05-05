using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Functions
{

    /// <summary>
    /// Base attribute to document a function or selector
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class BaseFunctionDocumentationAttribute : Attribute
    {
        public string Description { get; set; }
        public string Example { get; set; }
    }

    public sealed class FunctionDocumentationAttribute: BaseFunctionDocumentationAttribute
    {
    }

    public sealed class SelectorDocumentationAttribute : BaseFunctionDocumentationAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ParameterDocumentationAttribute: Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public sealed class InputDocumentationAttribute: ParameterDocumentationAttribute
    {

    }

    public sealed class OutputDocumentationAttribute : ParameterDocumentationAttribute
    {

    }

}
