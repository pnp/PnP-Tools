using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Visio.Scanner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validate commandline options
            var options = new Options();
            options.ValidateOptions(args);

            //Instantiate scan job
            VisioScanJob job = new VisioScanJob(options)
            {
                // I'm debugging
                //UseThreading = false
            };

            job.Execute();
        }
    }
}
