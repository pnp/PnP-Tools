using CommandLine;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;

namespace SharePoint.PermissiveFile.Scanner
{
    /// <summary>
    /// Commandline options
    /// </summary>
    public class Options : BaseOptions
    {
        // Important:
        // Following chars are already used as shorthand in the base options class: i, s, u, p, f, x, a, t, e, r, v, o, h, z

        [OptionList('l', "filetypes", HelpText = "List of additional (besides html and html) file types to scan (e.g. zip,ica) that you want to get scanned", Separator = ',')]
        public virtual IList<string> FileTypes { get; set; }


        /// <summary>
        /// Validate the provided commandline options, will exit the program when not valid
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public override void ValidateOptions(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(this.GetUsage());
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            // Perform base validation
            base.ValidateOptions(args);
        }

        /// <summary>
        /// Shows the usage information of the scanner
        /// </summary>
        /// <returns>String with the usage information</returns>
        [HelpOption]
        public string GetUsage()
        {
            var help = this.GetUsage("SharePoint PnP Permissive scanner");

            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("See the PnP-Tools repo for more information at:");
            help.AddPreOptionsLine("https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.PermissiveFile.Scanner");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Let the tool figure out your urls (works only for SPO MT):");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("SharePoint.PermissiveFile.Scanner.exe -t <tenant> -i <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. SharePoint.PermissiveFile.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("SharePoint.PermissiveFile.Scanner.exe -t <tenant> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("e.g. SharePoint.PermissiveFile.Scanner.exe -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Specifying url to tenant admin (needed for SPO Dedicated):");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("SharePoint.PermissiveFile.Scanner.exe -a <tenant admin site> -i <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. SharePoint.PermissiveFile.Scanner.exe -a https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("SharePoint.PermissiveFile.Scanner.exe -a <tenant admin site> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("e.g. SharePoint.PermissiveFile.Scanner.exe -a https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd");
            help.AddOptions(this);
            return help;
        }
    }
}
