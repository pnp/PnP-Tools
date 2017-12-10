using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.PermissiveFile.Scanner
{
    /// <summary>
    /// Permissive file scanner
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

            //Instantiate scan job
            PermissiveScanJob job = new PermissiveScanJob(options)
            {

                // I'm debugging
                //UseThreading = false
            };

            job.Execute();

        }
    }
}
