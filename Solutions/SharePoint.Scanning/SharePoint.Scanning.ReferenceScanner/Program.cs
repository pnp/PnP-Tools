using OfficeDevPnP.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Scanning.ReferenceScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validate commandline options
            var options = new Options();
            options.ValidateOptions(args);

            //Instantiate scan job
            ReferenceScanJob job = new ReferenceScanJob(options);

            // I'm debugging
            //job.UseThreading = false;

            job.Execute();


            // Sample on how to add custom log entry
            Log.Info("Reference Scanner", "Sample log message");

        }
    }
}
