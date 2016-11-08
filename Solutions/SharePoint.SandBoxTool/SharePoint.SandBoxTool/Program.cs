using System;
using CommandLine;

namespace SharePoint.SandBoxTool
{
    /// <summary>
    /// Sandbox Solution scanning tool
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // parse command line
            var options = new Options();
            var parser = new Parser(settings => { settings.MutuallyExclusive = true;
                                                  settings.HelpWriter = Parser.Default.Settings.HelpWriter;
                                                  settings.CaseSensitive = false; });
            
            if (!parser.ParseArguments(args, options))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
#if !ONPREMISES
            // perform additional validation
            if (!String.IsNullOrEmpty(options.ClientID))
            {
                if (String.IsNullOrEmpty(options.ClientSecret))
                {
                    Console.WriteLine("If you specify a client id you also need to specify a client secret");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            if (!String.IsNullOrEmpty(options.ClientSecret))
            {
                if (String.IsNullOrEmpty(options.ClientID))
                {
                    Console.WriteLine("If you specify a client secret you also need to specify a client id");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
#endif
            if (!String.IsNullOrEmpty(options.User))
            {
                if (String.IsNullOrEmpty(options.Password))
                {
                    Console.WriteLine("If you specify a user you also need to specify a password");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            if (!String.IsNullOrEmpty(options.Password))
            {
                if (String.IsNullOrEmpty(options.User))
                {
                    Console.WriteLine("If you specify a password you also need to specify a user");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }

            // Better support for running/testing the tool when SharePoint site certificate is not trusted on the box running the scan
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
            {
                return true;
            };

            // Instantiate scan job
            SBScanner sbScanner = new SBScanner();
            sbScanner.UseThreading = true;
            sbScanner.Mode = options.Mode;
            sbScanner.MaximumThreads = options.Threads;
#if !ONPREMISES
            sbScanner.TenantAdminSite = options.TenantAdminSite;
#endif
            sbScanner.OutputFolder = DateTime.Now.Ticks.ToString();
            sbScanner.Duplicates = options.Duplicates;
            sbScanner.Verbose = options.Verbose;
            sbScanner.Separator = options.Separator;

            // temp debug
            //sbScanner.UseThreading = false;

            // set scan creds
#if !ONPREMISES
            if (options.UsesAppOnly())
            {
                sbScanner.UseAppOnlyAuthentication(options.ClientID, options.ClientSecret);                
            }
            else if (options.UsesCredentials())
            {
                sbScanner.UseOffice365Authentication(options.User, options.Password);
            }

            sbScanner.ExcludeOD4B = options.ExcludeOD4B;
#else
            sbScanner.UseNetworkCredentialsAuthentication(options.User, options.Password, options.Domain);
#endif

#if !ONPREMISES
            // set scan urls
            if (!String.IsNullOrEmpty(options.Tenant))
            {
                sbScanner.AddSite(string.Format("https://{0}.sharepoint.com/*", options.Tenant));
                sbScanner.AddSite(string.Format("https://{0}-my.sharepoint.com/*", options.Tenant));
            }
            else
            {
#endif
                foreach (var url in options.Urls)
                {
                    sbScanner.AddSite(url);
                }
#if !ONPREMISES
            }
#endif
            DateTime start = DateTime.Now;
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is starting...{0}", start.ToString());
            Console.WriteLine("=====================================================");
            Console.WriteLine("Building a list of site collections...");

            // launch the job
            sbScanner.Run();

            // Dump scan results
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is done...now dump the results to a CSV file");
            Console.WriteLine("=====================================================");
            System.IO.Directory.CreateDirectory(sbScanner.OutputFolder);
            string errorfile = string.Format("{0}\\errors.csv", sbScanner.OutputFolder);
            string outputfile = string.Format("{0}\\sandboxreport.csv", sbScanner.OutputFolder);
            Console.WriteLine("Outputting scan results to {0}", outputfile);

            string[] outputHeaders = null;

            if (options.Mode == Mode.scan || options.Mode == Mode.scananddownload)
            {
                outputHeaders = new string[] { "SiteURL", "SiteOwner", "WSPName", "Author", "CreatedDate", "Activated", "HasAssemblies", "SolutionHash", "SolutionID", "SiteID" };
            }
            else if (options.Mode == Mode.scanandanalyze)
            {
                outputHeaders = new string[] { "SiteURL", "SiteOwner", "WSPName", "Author", "CreatedDate", "Activated", "HasAssemblies", "SolutionHash", "SolutionID", "SiteID", "IsEmptyAssembly", "IsInfoPath", "IsEmptyInfoPathAssembly", "HasWebParts", "HasWebTemplate", "HasFeatureReceivers", "HasEventReceivers", "HasListDefinition", "HasWorkflowAction" };
            }

            System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));

            SBScanResult item;
            while (sbScanner.SBScanResults.TryPop(out item))
            {
                if (options.Mode == Mode.scan || options.Mode == Mode.scananddownload)
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, item.SiteURL, item.SiteOwner, item.WSPName, item.Author, item.CreatedDate, item.Activated, item.HasAssemblies, item.SolutionHash, item.SolutionID, item.SiteId)));
                }
                else if (options.Mode == Mode.scanandanalyze)
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, item.SiteURL, item.SiteOwner, item.WSPName, item.Author, item.CreatedDate, item.Activated, 
                                                                                                                     item.HasAssemblies, item.SolutionHash, item.SolutionID, item.SiteId,
                                                                                                                     item.IsEmptyAssembly.HasValue ? item.IsEmptyAssembly.Value.ToString() : "N/A",
                                                                                                                     item.IsInfoPath.HasValue ? item.IsInfoPath.Value.ToString() : "N/A",
                                                                                                                     item.IsEmptyInfoPathAssembly.HasValue ? item.IsEmptyInfoPathAssembly.Value.ToString() : "N/A",
                                                                                                                     item.HasWebParts.HasValue ? item.HasWebParts.Value.ToString() : "N/A", 
                                                                                                                     item.HasWebTemplate.HasValue ? item.HasWebTemplate.Value.ToString() : "N/A", 
                                                                                                                     item.HasFeatureReceivers.HasValue ? item.HasFeatureReceivers.Value.ToString() : "N/A", 
                                                                                                                     item.HasEventReceivers.HasValue ? item.HasEventReceivers.Value.ToString() : "N/A", 
                                                                                                                     item.HasListDefinition.HasValue ? item.HasListDefinition.Value.ToString() : "N/A", 
                                                                                                                     item.HasWorkflowAction.HasValue ? item.HasWorkflowAction.Value.ToString() : "N/A"
                                                                                                                     )));
                }
            }

            // Dump scan errors
            Console.WriteLine("Outputting errors to {0}", errorfile);
            outputHeaders = new string[] { "SiteURL", "Error"};
            System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));
            SBScanError error;
            while (sbScanner.SBScanErrors.TryPop(out error))
            {
                System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(options.Separator, error.SiteURL, error.Error)));
            }
            Console.WriteLine("=====================================================");
            Console.WriteLine("All done. Took {0} for {1} sites", (DateTime.Now - start).ToString(), sbScanner.ScannedSites);
            Console.WriteLine("=====================================================");

        }
    }
}
