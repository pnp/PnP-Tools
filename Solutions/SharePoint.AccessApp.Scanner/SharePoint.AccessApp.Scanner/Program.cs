using System;
using CommandLine;
using System.Reflection;
using System.Diagnostics;

namespace SharePoint.AccessApp.Scanner
{
    /// <summary>
    /// Access App scanning tool
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {            
            // parse command line
            var options = new Options();

            if (args.Length == 0)
            {
                Console.WriteLine(options.GetUsage());
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            var parser = new Parser(settings =>
            {
                settings.MutuallyExclusive = true;
                settings.HelpWriter = Parser.Default.Settings.HelpWriter;
                settings.CaseSensitive = false;
            });

            if (!parser.ParseArguments(args, options))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            System.Collections.Generic.List<Mode> lstmode = options.GetAllOptions(args);

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
            AccessAppScanner accessAppScanner = new AccessAppScanner()
            {
                UseThreading = true,
                Modes = lstmode,
                MaximumThreads = options.Threads,
                TenantAdminSite = options.TenantAdminSite,
                OutputFolder = DateTime.Now.Ticks.ToString(),
                Separator = options.Separator,
                Tenant = options.Tenant,
                UseSearchQuery = !options.DontUseSearchQuery
            };

            // temp debug
            //accessAppScanner.UseThreading = false;

            // set scan creds
            if (options.UsesAppOnly())
            {
                accessAppScanner.UseAppOnlyAuthentication(options.ClientID, options.ClientSecret);
            }
            else if (options.UsesCredentials())
            {
                accessAppScanner.UseOffice365Authentication(options.User, options.Password);
            }

            accessAppScanner.ExcludeOD4B = options.ExcludeOD4B;

            if (!accessAppScanner.UseSearchQuery)
            {
                // set scan urls
                if (!String.IsNullOrEmpty(options.Tenant))
                {
                    accessAppScanner.AddSite(string.Format("https://{0}.sharepoint.com/*", options.Tenant));
                    if (!accessAppScanner.ExcludeOD4B)
                    {
                        accessAppScanner.AddSite(string.Format("https://{0}-my.sharepoint.com/*", options.Tenant));
                    }
                }
                else
                {
                    foreach (var url in options.Urls)
                    {
                        accessAppScanner.AddSite(url);
                    }
                }
            }

            DateTime start = DateTime.Now;
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is starting...{0}", start.ToString());
            Console.WriteLine("=====================================================");
            Console.WriteLine("Building a list of site collections...");

            // Launch the job
            accessAppScanner.Run();

            // Dump scan results
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is done...now dump the results to a CSV file");
            Console.WriteLine("=====================================================");
            System.IO.Directory.CreateDirectory(accessAppScanner.OutputFolder);
            string outputfile = null;
            string[] outputHeaders = null;

            #region Access App report
            if (lstmode.Contains(Mode.Scan))
            {
                // output summary report
                outputfile = string.Format("{0}\\AccessApps.csv", accessAppScanner.OutputFolder);
                Console.WriteLine("Outputting Access App scan to {0}", outputfile);
                outputHeaders = new string[] { "Site Url", "Parent Site Url", "Site Collection Url", "Web Title",
                                               "Web Template", "App Created On", "Access 2013 App Last Accessed On", "Access 2010 App Last Modified By User On", "App Instance Status", "App Instance Id", "Web Id", @"Site admins\owners", 
                                               "ViewsRecent", "viewsRecentUnique", "viewsLifetime", "viewsLifetimeUnique" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));

                AccessAppScanData scanData;
                while (accessAppScanner.AccessAppResults.TryPop(out scanData))
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, ToCsv(scanData.SiteUrl), ToCsv(scanData.ParentSiteUrl), ToCsv(scanData.SiteColUrl), ToCsv(scanData.WebTitle), 
                        ToCsv(scanData.WebTemplate), scanData.WebCreatedDate ,scanData.LastAccessedDate, scanData.LastModifiedByUserDate, scanData.AppInstanceStatus, scanData.AppInstanceId, scanData.WebId, ToCsv(scanData.SiteAdmins),
                        scanData.ViewsRecent, scanData.ViewsRecentUnique, scanData.ViewsLifetime, scanData.ViewsLifetimeUnique)));
                }
            }
            #endregion

            #region Error reporting
            if (accessAppScanner.AccessAppScanErrors.Count > 0)
            {
                string errorfile = string.Format("{0}\\errors.csv", accessAppScanner.OutputFolder);
                // Dump scan errors
                Console.WriteLine("Outputting errors to {0}", errorfile);
                outputHeaders = new string[] { "Site Url", "Site Collection Url", "Error" };
                System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));
                AccessAppScanError error;
                while (accessAppScanner.AccessAppScanErrors.TryPop(out error))
                {
                    System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(options.Separator, ToCsv(error.SiteUrl), ToCsv(error.SiteColUrl), ToCsv(error.Error))));
                }
            }
            #endregion

            #region Scanner data
            if (accessAppScanner.ScannedSites > 0)
            {
                outputfile = string.Format("{0}\\ScannerSummary.csv", accessAppScanner.OutputFolder);
                Console.WriteLine("Outputting information over the done scan to {0}", outputfile);
                outputHeaders = new string[] { "Site collections scanned", "Webs scanned", "Scan duration", "Scanner version" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));

                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;
                TimeSpan ts = DateTime.Now.Subtract(accessAppScanner.StartTime);
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, !accessAppScanner.UseSearchQuery?accessAppScanner.ScannedSites:0, accessAppScanner.ScannedWebs, $"{ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds", version)));
            }
            #endregion

            Console.WriteLine("=====================================================");
            Console.WriteLine("All done. Took {0} for {1} sites", (DateTime.Now - start).ToString(), accessAppScanner.ScannedSites);
            Console.WriteLine("=====================================================");

        }

        /// <summary>
        /// Drop carriage returns, leading and trailing spaces + escape embedded quotes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ToCsv(string value)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                return $"\"{value.Trim().Replace("\r\n", string.Empty).Replace("\"", "\"\"")}\"";
            }
        }

    }
}
