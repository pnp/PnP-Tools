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

            //Instantiate scan job
            ModernizationScanJob job = new ModernizationScanJob(options)
            {

                // I'm debugging
                //UseThreading = false
            };

            job.Execute();
        }
    }
}
