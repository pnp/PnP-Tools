using CommandLine;
using SharePoint.Scanning.Framework;
using System;

namespace SharePoint.Modernization.Scanner
{
    /// <summary>
    /// Possible scanning modes
    /// </summary>
    public enum Mode
    {
        Full = 0,
        GroupifyOnly
    }

    /// <summary>
    /// Commandline options
    /// </summary>
    public class Options : BaseOptions
    {
        // Important:
        // Following chars are already used as shorthand in the base options class: i, s, u, p, f, x, a, t, e, r, v, o, h, z

        [Option('m', "mode", HelpText = "Execution mode. Use following modes: full, GroupifyOnly. Omit or use full for a full scan", DefaultValue = Mode.Full, Required = false)]
        public Mode Mode { get; set; }

        [Option('b', "exportwebpartproperties", HelpText = "Export the web part property data", DefaultValue = false, Required = false)]
        public bool ExportWebPartProperties { get; set; }

        [Option('c', "skipusageinformation", HelpText = "Don't use search to get the site/page usage information and don't export that data", DefaultValue = false, Required = false)]
        public bool SkipUsageInformation { get; set; }


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
            var help = this.GetUsage("SharePoint PnP Modernization scanner");

            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("See the PnP-Tools repo for more information at:");
            help.AddPreOptionsLine("https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.Modernization.Scanner");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Let the tool figure out your urls (works only for SPO MT):");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using Azure AD app-only:");
            help.AddPreOptionsLine("SharePoint.Modernization.Scanner.exe -t <tenant> -i <your client id> -z <Azure AD domain> -f <PFX file> -x <PFX file password>");
            help.AddPreOptionsLine("e.g. SharePoint.Modernization.Scanner.exe -t contoso -i e5808e8b-6119-44a9-b9d8-9003db04a882 -z conto.onmicrosoft.com  -f apponlycert.pfx -x pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("SharePoint.Modernization.Scanner.exe -t <tenant> -i <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. SharePoint.Modernization.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("SharePoint.Modernization.Scanner.exe -t <tenant> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("e.g. SharePoint.Modernization.Scanner.exe -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Specifying url to your sites and tenant admin (needed for SPO Dedicated):");
            help.AddPreOptionsLine("=========================================================================");
            help.AddPreOptionsLine("Using Azure AD app-only:");
            help.AddPreOptionsLine("SharePoint.Modernization.Scanner.exe -r <wildcard urls> -a <tenant admin site>  -i <your client id> -z <Azure AD domain> -f <PFX file> -x <PFX file password>");
            help.AddPreOptionsLine("e.g. SharePoint.Modernization.Scanner.exe -r \"https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*\" -a https://contoso-admin.contoso.com -i e5808e8b-6119-44a9-b9d8-9003db04a882 -z conto.onmicrosoft.com  -f apponlycert.pfx -x pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("SharePoint.Modernization.Scanner.exe -r <wildcard urls> -a <tenant admin site> -i <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. SharePoint.Modernization.Scanner.exe -r \"https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*\" -a https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("SharePoint.Modernization.Scanner.exe -r <wildcard urls> -a <tenant admin site> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("e.g. SharePoint.Modernization.Scanner.exe -r \"https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*\" -a https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd");
            help.AddPreOptionsLine("");
            help.AddOptions(this);
            return help;
        }
    }
}

