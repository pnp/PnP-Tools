using System;
using CommandLine;

namespace SharePoint.UIExperience.Scanner
{
    /// <summary>
    /// UIExperience scanning tool
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
            UIExperienceScanner uiExpScanner = new UIExperienceScanner();
            uiExpScanner.UseThreading = true;
            uiExpScanner.Modes = lstmode;
            uiExpScanner.MaximumThreads = options.Threads;
            uiExpScanner.TenantAdminSite = options.TenantAdminSite;
            uiExpScanner.OutputFolder = DateTime.Now.Ticks.ToString();
            uiExpScanner.Verbose = options.Verbose;
            uiExpScanner.Separator = options.Separator;

            // temp debug
            //uiExpScanner.UseThreading = false;

            // set scan creds
            if (options.UsesAppOnly())
            {
                uiExpScanner.UseAppOnlyAuthentication(options.ClientID, options.ClientSecret);
            }
            else if (options.UsesCredentials())
            {
                uiExpScanner.UseOffice365Authentication(options.User, options.Password);
            }

            uiExpScanner.ExcludeOD4B = options.ExcludeOD4B;

            // set scan urls
            if (!String.IsNullOrEmpty(options.Tenant))
            {
                uiExpScanner.AddSite(string.Format("https://{0}.sharepoint.com/*", options.Tenant));
                uiExpScanner.AddSite(string.Format("https://{0}-my.sharepoint.com/*", options.Tenant));
            }
            else
            {
                foreach (var url in options.Urls)
                {
                    uiExpScanner.AddSite(url);
                }
            }

            DateTime start = DateTime.Now;
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is starting...{0}", start.ToString());
            Console.WriteLine("=====================================================");
            Console.WriteLine("Building a list of site collections...");

            // Launch the job
            uiExpScanner.Run();

            // Dump scan results
            Console.WriteLine("=====================================================");
            Console.WriteLine("Scanning is done...now dump the results to a CSV file");
            Console.WriteLine("=====================================================");
            System.IO.Directory.CreateDirectory(uiExpScanner.OutputFolder);
            string outputfile = null;
            string[] outputHeaders = null;

            #region Modern List blocking reports
            if (lstmode.Contains(Mode.Scan) || lstmode.Contains(Mode.BlockedLists))
            {
                // output summary report
                outputfile = string.Format("{0}\\ModernListBlocked.csv", uiExpScanner.OutputFolder);
                Console.WriteLine("Outputting list scan results to {0}", outputfile);
                outputHeaders = new string[] { "Url", "SiteURL", "List Title",
                                               "Blocked at site level", "Blocked at web level", "Blocked at list level", "List page render type", "List experience", "Blocked by not being able to load Page", "Blocked by not being able to load page exception",
                                               "Blocked by managed metadata navigation", "Blocked by view type", "View type", "Blocked by list base template", "List base template",
                                               "Blocked by zero or multiple web parts", "Blocked by JSLink", "JSLink", "Blocked by XslLink", "XslLink", "Blocked by Xsl", 
                                               "Blocked by JSLink field", "JSLink fields", "Blocked by business data field", "Business data fields", "Blocked by task outcome field", "Task outcome fields",
                                               "Blocked by publishingField", "Publishing fields", "Blocked by geo location field", "Geo location fields", "Blocked by list custom action", "List custom actions"  };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));

                foreach (var list in uiExpScanner.ListResults)
                {
                    if (list.Value.BlockedByNotBeingAbleToLoadPageException==null)
                    {
                        list.Value.BlockedByNotBeingAbleToLoadPageException = "";
                    }

                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n",
                        string.Join(options.Separator, $"\"{list.Key}\"", $"\"{list.Value.SiteUrl}\"", $"\"{list.Value.ListTitle.Replace("\"", "\"\"")}\"",
                                                       list.Value.BlockedAtSiteLevel, list.Value.BlockedAtWebLevel, list.Value.BlockedAtListLevel, list.Value.PageRenderType, list.Value.ListExperience, list.Value.BlockedByNotBeingAbleToLoadPage, $"\"{list.Value.BlockedByNotBeingAbleToLoadPageException.Trim().Replace("\r\n", string.Empty).Replace("\"", "\"\"")}\"",
                                                       list.Value.XsltViewWebPartCompatibility.BlockedByManagedMetadataNavFeature, list.Value.XsltViewWebPartCompatibility.BlockedByViewType, list.Value.XsltViewWebPartCompatibility.ViewType, list.Value.XsltViewWebPartCompatibility.BlockedByListBaseTemplate, list.Value.XsltViewWebPartCompatibility.ListBaseTemplate,
                                                       list.Value.BlockedByZeroOrMultipleWebParts, list.Value.XsltViewWebPartCompatibility.BlockedByJSLink, $"\"{list.Value.XsltViewWebPartCompatibility.JSLink}\"", list.Value.XsltViewWebPartCompatibility.BlockedByXslLink, $"\"{list.Value.XsltViewWebPartCompatibility.XslLink}\"", list.Value.XsltViewWebPartCompatibility.BlockedByXsl,
                                                       list.Value.XsltViewWebPartCompatibility.BlockedByJSLinkField, $"\"{list.Value.XsltViewWebPartCompatibility.JSLinkFields}\"", list.Value.XsltViewWebPartCompatibility.BlockedByBusinessDataField, $"\"{list.Value.XsltViewWebPartCompatibility.BusinessDataFields}\"", list.Value.XsltViewWebPartCompatibility.BlockedByTaskOutcomeField, $"\"{list.Value.XsltViewWebPartCompatibility.TaskOutcomeFields}\"",
                                                       list.Value.XsltViewWebPartCompatibility.BlockedByPublishingField, $"\"{list.Value.XsltViewWebPartCompatibility.PublishingFields}\"", list.Value.XsltViewWebPartCompatibility.BlockedByGeoLocationField, $"\"{list.Value.XsltViewWebPartCompatibility.GeoLocationFields}\"", list.Value.XsltViewWebPartCompatibility.BlockedByListCustomAction, $"\"{list.Value.XsltViewWebPartCompatibility.ListCustomActions}\"")));
                }
            }
            #endregion

            #region Ignored customisations reports
            if (lstmode.Contains(Mode.Scan) || lstmode.Contains(Mode.IgnoredCustomizations))
            {
                // output detailed reports
                // custom actions
                outputfile = string.Format("{0}\\IgnoredCustomizations_CustomAction.csv", uiExpScanner.OutputFolder);
                Console.WriteLine("Outputting list custom action scan results to {0}", outputfile);
                outputHeaders = new string[] {"Url", "SiteURL", "List Name", "Title", "Name", "Location", "RegistrationType", "RegistrationId", "Reason", /*"ImageMaps",*/ "ScriptBlock", "ScriptSrc", "CommandActions" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));
                CustomActionsResult item1;
                while (uiExpScanner.CustomActionScanResults.TryPop(out item1))
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, $"\"{item1.Url}\"", $"\"{item1.SiteUrl}\"", $"\"{item1.ListTitle}\"",
                        $"\"{item1.Title}\"", $"\"{item1.Name}\"", $"\"{item1.Location}\"", $"\"{item1.RegistrationType}\"",
                        $"\"{item1.RegistrationId}\"", $"\"{item1.Problem}\"", /*item1.ImageMaps,*/
                        $"\"{item1.ScriptBlock.Trim().Replace("\r\n", string.Empty).Replace("\"", "\"\"")}\"", 
                        $"\"{item1.ScriptSrc.Trim().Replace("\r\n", string.Empty).Replace("\"", "\"\"")}\"", 
                        item1.CommandActions)));
                }

                // alternate css
                outputfile = string.Format("{0}\\IgnoredCustomizations_AlternateCSS.csv", uiExpScanner.OutputFolder);
                Console.WriteLine("Outputting alternate css scan to {0}", outputfile);
                outputHeaders = new string[] { "Url", "SiteURL", "AlternateCSS" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));
                AlternateCSSResult item2;
                while (uiExpScanner.AlternateCSSResults.TryPop(out item2))
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, $"\"{item2.Url}\"", $"\"{item2.SiteUrl}\"", $"\"{item2.AlternateCSS}\"")));
                }

                // master pages
                outputfile = string.Format("{0}\\IgnoredCustomizations_MasterPage.csv", uiExpScanner.OutputFolder);
                Console.WriteLine("Outputting master page scan to {0}", outputfile);
                outputHeaders = new string[] { "Url", "SiteURL", "MasterPage", "Custom MasterPage" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));
                MasterPageResult item3;
                while (uiExpScanner.MasterPageResults.TryPop(out item3))
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, $"\"{item3.Url}\"", $"\"{item3.SiteUrl}\"", $"\"{item3.MasterPage}\"", $"\"{item3.CustomMasterPage}\"")));
                }

                // output summary report
                outputfile = string.Format("{0}\\IgnoredCustomizations.csv", uiExpScanner.OutputFolder);
                Console.WriteLine("Outputting ignored customization results to {0}", outputfile);
                outputHeaders = new string[] { "Url", "SiteURL", "Ignored MasterPage", "Ignored Custom MasterPage", "Ignored AlternateCSS", "Ignored Custom Action" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));

                foreach (var item in uiExpScanner.CustomizationResults)
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, $"\"{item.Key}\"", $"\"{item.Value.SiteUrl}\"", $"\"{item.Value.IgnoredMasterPage}\"", $"\"{item.Value.IgnoredCustomMasterPage}\"", $"\"{item.Value.IgnoredAlternateCSS}\"", $"\"{item.Value.IgnoredCustomAction}\"")));
                }
            }
            #endregion

            #region Modern page blocking report
            if (lstmode.Contains(Mode.Scan) || lstmode.Contains(Mode.BlockedPages))
            {
                // output summary report
                outputfile = string.Format("{0}\\ModernPagesBlocked.csv", uiExpScanner.OutputFolder);
                Console.WriteLine("Outputting modern page blocked scan to {0}", outputfile);
                outputHeaders = new string[] { "Url", "SiteURL", "Blocked via disabled modern page web feature" };
                System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));

                foreach(var item in uiExpScanner.PageResults)
                {
                    System.IO.File.AppendAllText(outputfile, string.Format("{0}\r\n", string.Join(options.Separator, $"\"{item.Key}\"", $"\"{item.Value.SiteUrl}\"", $"\"{item.Value.BlockedViaDisabledModernPageWebFeature}\"")));
                }
            }
            #endregion

            #region Error reporting
            if (uiExpScanner.UIExpScanErrors.Count > 0)
            {
                string errorfile = string.Format("{0}\\errors.csv", uiExpScanner.OutputFolder);
                // Dump scan errors
                Console.WriteLine("Outputting errors to {0}", errorfile);
                outputHeaders = new string[] { "SiteURL", "Error" };
                System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(options.Separator, outputHeaders)));
                UIExperienceScanError error;
                while (uiExpScanner.UIExpScanErrors.TryPop(out error))
                {
                    System.IO.File.AppendAllText(errorfile, string.Format("{0}\r\n", string.Join(options.Separator, error.SiteURL, $"\"{error.Error.Trim().Replace("\r\n", string.Empty).Replace("\"", "\"\"")}\"")));
                }
            }
            #endregion

            Console.WriteLine("=====================================================");
            Console.WriteLine("All done. Took {0} for {1} sites", (DateTime.Now - start).ToString(), uiExpScanner.ScannedSites);
            Console.WriteLine("=====================================================");

        }
    }
}
