using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Scanner.Analyzers
{
    /// <summary>
    /// Interface that all analyzers need to implement
    /// </summary>
    public interface IBaseAnalyzer
    {

        /// <summary>
        /// Analyzer run
        /// </summary>
        /// <param name="cc">ClientContext of the web to be analyzed</param>
        /// <returns>Duration of the analysis</returns>
        TimeSpan Analyze(ClientContext cc);
    }
}
