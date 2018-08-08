using SharePoint.Modernization.Scanner.Reports;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharePoint.Modernization.Scanner
{
    /// <summary>
    /// SharePoint PnP Modernization scanner
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method to execute the program
        /// </summary>
        /// <param name="args">Command line arguments</param>

        static void Main(string[] args)
        {
            // Validate commandline options
            var options = new Options();
            options.ValidateOptions(args);

            if (options.ExportPaths != null && options.ExportPaths.Count > 0)
            {
                Generator generator = new Generator();
                generator.CreateGroupifyReport(options.ExportPaths);
                generator.CreatePageReport(options.ExportPaths);
            }
            else
            {
                //Instantiate scan job
                ModernizationScanJob job = new ModernizationScanJob(options)
                {

                    // I'm debugging
                    //UseThreading = false
                };

                job.Execute();

                // Create reports
                if (!options.SkipReport)
                {
                    string workingFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    List<string> paths = new List<string>
                    {
                        Path.Combine(workingFolder, job.OutputFolder)
                    };

                    var generator = new Generator();

                    generator.CreateGroupifyReport(paths);

                    if (Options.IncludePage(options.Mode))
                    {
                        generator.CreatePageReport(paths);
                    }
                }
            }            
        }
    }
}
