using CommandLine;
using CommandLine.Text;
using OfficeDevPnP.Core.Framework.TimerJobs.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SharePoint.Scanning.Framework
{
    /// <summary>
    /// Base commandline options which are used in all scanners
    /// </summary>
    public class BaseOptions
    {
        #region Private variables
        Parser parser;
        #endregion

        #region Security related options
        [Option('i', "clientid", HelpText = "Client ID of the app-only principal used to scan your site collections", MutuallyExclusiveSet = "A")]
        public string ClientID { get; set; }

        [Option('s', "clientsecret", HelpText = "Client Secret of the app-only principal used to scan your site collections", MutuallyExclusiveSet = "B")]
        public string ClientSecret { get; set; }

        [Option('u', "user", HelpText = "User id used to scan/enumerate your site collections", MutuallyExclusiveSet = "A")]
        public string User { get; set; }

        [Option('p', "password", HelpText = "Password of the user used to scan/enumerate your site collections", MutuallyExclusiveSet = "B")]
        public string Password { get; set; }

        [Option('z', "azuretenant", HelpText = "Azure tenant (e.g. contoso.microsoftonline.com)")]
        public string AzureTenant { get; set; }

        [Option('f', "certificatepfx", HelpText = "Path + name of the pfx file holding the certificate to authenticate")]
        public string CertificatePfx { get; set; }

        [Option('x', "certificatepfxpassword", HelpText = "Password of the pfx file holding the certificate to authenticate")]
        public string CertificatePfxPassword { get; set; }

        [Option('a', "tenantadminsite", HelpText = "Url to your tenant admin site (e.g. https://contoso-admin.contoso.com): only needed when your not using SPO MT")]
        public string TenantAdminSite { get; set; }
        #endregion

        #region Sites to scan
        [Option('t', "tenant", HelpText = "Tenant name, e.g. contoso when your sites are under https://contoso.sharepoint.com/sites. This is the recommended model for SharePoint Online MT as this way all site collections will be scanned")]
        public string Tenant { get; set; }

        [OptionList('r', "urls", HelpText = "List of (wildcard) urls (e.g. https://contoso.sharepoint.com/*,https://contoso-my.sharepoint.com,https://contoso-my.sharepoint.com/personal/*) that you want to get scanned", Separator = ',')]
        public virtual IList<string> Urls { get; set; }

        [Option('o', "includeod4b", HelpText = "Include OD4B sites in the scan", DefaultValue = false)]
        public bool IncludeOD4B { get; set; }

        [Option('v', "csvfile", HelpText = "CSV file name (e.g. input.csv) which contains the list of site collection urls that you want to scan")]
        public virtual string CsvFile { get; set; }
        #endregion

        #region Scanner configuration
        [Option('h', "threads", HelpText = "Number of parallel threads, maximum = 100", DefaultValue = 10)]
        public int Threads { get; set; }
        #endregion

        #region File handling
        [Option('e', "separator", HelpText = "Separator used in output CSV files (e.g. \";\")", DefaultValue = ",")]
        public string Separator { get; set; }
        #endregion

        #region Option validation
        /// <summary>
        /// Are we using SharePoint App-Only?
        /// </summary>
        /// <returns>true if app-only, false otherwise</returns>
        public AuthenticationType AuthenticationTypeProvided()
        {
            if (!String.IsNullOrEmpty(ClientID) && !String.IsNullOrEmpty(ClientSecret))
            {
                return AuthenticationType.AppOnly;
            }
            else if (!String.IsNullOrEmpty(User) && !String.IsNullOrEmpty(Password))
            {
                return AuthenticationType.Office365;
            }
            else if (!String.IsNullOrEmpty(CertificatePfx) && !String.IsNullOrEmpty(CertificatePfxPassword) && !String.IsNullOrEmpty(ClientID) && !String.IsNullOrEmpty(AzureTenant))
            {
                return AuthenticationType.AzureADAppOnly;
            }
            else
            {
                throw new Exception("Clonflicting security parameters provided.");
            }
        }

        /// <summary>
        /// Validate the provided commandline options, will exit the program when not valid
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public virtual void ValidateOptions(string[] args)
        {
            var parser = new Parser(settings =>
            {
                settings.MutuallyExclusive = true;
                settings.HelpWriter = Parser.Default.Settings.HelpWriter;
                settings.CaseSensitive = false;
            });
            this.parser = parser;

            ValidateOptions(args, parser);
        }

        /// <summary>
        /// Validate the provided commandline options, will exit the program when not valid
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <param name="parser">Parser object holding the commadline parsing settings</param>
        public virtual void ValidateOptions(string[] args, Parser parser)
        {
            this.parser = parser;

            if (!parser.ParseArguments(args, this))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            // perform additional validation
            if (!String.IsNullOrEmpty(this.ClientID) && String.IsNullOrEmpty(this.CertificatePfx))
            {
                if (String.IsNullOrEmpty(this.ClientSecret))
                {
                    Console.WriteLine("If you specify a client id you also need to specify a client secret");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            if (!String.IsNullOrEmpty(this.ClientSecret))
            {
                if (String.IsNullOrEmpty(this.ClientID))
                {
                    Console.WriteLine("If you specify a client secret you also need to specify a client id");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }

            if (!String.IsNullOrEmpty(this.CertificatePfx))
            {
                if (String.IsNullOrEmpty(this.CertificatePfxPassword))
                {
                    Console.WriteLine("If you specify a certificate you also need to specify a password for the certificate");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
                if (String.IsNullOrEmpty(this.AzureTenant))
                {
                    Console.WriteLine("If you specify a certificate you also need to specify the Azure Tenant");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
                if (String.IsNullOrEmpty(this.ClientID))
                {
                    Console.WriteLine("If you specify a certificate you also need to specify the clientid of the Azure application");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }

            if (!String.IsNullOrEmpty(this.CertificatePfxPassword))
            {
                if (String.IsNullOrEmpty(this.CertificatePfx))
                {
                    Console.WriteLine("If you specify a certifcate password you also need to specify a certificate");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
                if (String.IsNullOrEmpty(this.AzureTenant))
                {
                    Console.WriteLine("If you specify a certificate password you also need to specify the Azure Tenant");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
                if (String.IsNullOrEmpty(this.ClientID))
                {
                    Console.WriteLine("If you specify a certificate password you also need to specify the clientid of the Azure application");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }

            if (!String.IsNullOrEmpty(this.User))
            {
                if (String.IsNullOrEmpty(this.Password))
                {
                    Console.WriteLine("If you specify a user you also need to specify a password");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            if (!String.IsNullOrEmpty(this.Password))
            {
                if (String.IsNullOrEmpty(this.User))
                {
                    Console.WriteLine("If you specify a password you also need to specify a user");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            if (!String.IsNullOrEmpty(this.CsvFile))
            {
                if (!System.IO.File.Exists(this.CsvFile))
                {
                    Console.WriteLine("Failed to find csv file with urls. Please check file path provided.");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
        }
        #endregion

        #region Usage
        /// <summary>
        /// Returns the scanner usage information
        /// </summary>
        /// <param name="scanner">Name of the scanner</param>
        /// <returns>HelpText instance holding the help information</returns>
        public HelpText GetUsage(string scanner)
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(UrlToFileName(assembly.EscapedCodeBase));
            string version = fvi.FileVersion;

            var help = new HelpText
            {
                Heading = new HeadingInfo(scanner, version),
                Copyright = new CopyrightInfo("SharePoint PnP", DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = 120
            };
            return help;
        }

        /// <summary>
        /// Converts an URI based file name (file:///c:/temp/file.txt) to a regular path + filename
        /// </summary>
        /// <param name="url">File URI</param>
        /// <returns>File path + name</returns>
        public static string UrlToFileName(string url)
        {
            if (url.StartsWith("file://"))
            {
                return new Uri(url).LocalPath;
            }
            else
            {
                return url;
            }
        }
        #endregion
    }

}
