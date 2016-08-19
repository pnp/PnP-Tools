using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace SharePoint.SandBoxTool
{
    /// <summary>
    /// Possible scanning modes
    /// </summary>
    public enum Mode
    {
        scan = 0,
        scananddownload,
        scanandanalyze,
    }

    /// <summary>
    /// Options available on the command line
    /// </summary>
    internal class Options
    {
        [Option('m', "mode", HelpText = "Execution mode. Choose scan for a basic scan, scananddownload for also downloading the sandbox solutions or scanandanalyze for downloading + analyzing the SB solutions", DefaultValue = Mode.scananddownload, Required = true)]
        public Mode Mode { get; set; }

        [Option('t', "tenant", HelpText = "Tenant name, e.g. contoso when your sites are under https://contoso.sharepoint.com/sites. This is the recommended model for SharePoint Online MT as this way all site collections will be scanned")]
        public string Tenant { get; set; }

        [OptionList('r', "urls", HelpText = "List of (wildcard) urls (e.g. https://contoso.sharepoint.com/*,https://contoso-my.sharepoint.com,https://contoso-my.sharepoint.com/personal/*) that you want to get scanned. When you specify the --tenant optoin then this parameter is ignored", Separator = ',')]
        public IList<string> Urls { get; set; }

        [Option('c', "clientid", HelpText ="Client ID of the app-only principal used to scan your site collections", MutuallyExclusiveSet = "A")]
        public string ClientID { get; set; }

        [Option('s', "clientsecret", HelpText = "Client Secret of the app-only principal used to scan your site collections", MutuallyExclusiveSet = "B")]
        public string ClientSecret { get; set; }

        [Option('u', "user", HelpText = "User id used to scan/enumerate your site collections", MutuallyExclusiveSet = "A")]
        public string User { get; set; }

        [Option('p', "password", HelpText = "Password of the user used to scan/enumerate your site collections", MutuallyExclusiveSet = "B")]
        public string Password { get; set; }

        [Option('a', "tenantadminsite", HelpText = "Url to your tenant admin site (e.g. https://contoso-admin.contoso.com): only needed when your not using SPO MT")]
        public string TenantAdminSite { get; set; }

        [Option('e', "seperator", HelpText = "Separator used in output CSV files", DefaultValue = ",")]
        public string Separator { get; set; }

        [Option('h', "threads", HelpText = "Number of parallel threads, maximum = 100", DefaultValue = 10)]
        public int Threads { get; set; }

        [Option('d', "duplicates", HelpText = "Download and process all sandbox solutions, not just the unique ones", DefaultValue = false)]
        public bool Duplicates { get; set; }

        [Option('v', "verbose", HelpText = "Show more execution details", DefaultValue = false)]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("SharePoint SandBox Solution tool", "1.0"),
                Copyright = new CopyrightInfo("SharePoint PnP", 2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = 120
            };
            help.AddPreOptionsLine("See the PnP-Tools repo for more information at: https://github.com/OfficeDev/PnP-Tools/tree/master/Solutions");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Let the tool figure out your urls (works only for SPO MT):");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("sandboxtool.exe -m <mode> -t <tenant> -c <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. sandboxtool.exe -m scananddownload -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("sandboxtool.exe -m <mode> -t <tenant> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("e.g. sandboxtool.exe -m scananddownload -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Specifying your urls to scan + url to tenant admin:");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("sandboxtool.exe -m <mode> -r <urls> -a <tenant admin site> -c <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. sandboxtool.exe -m scananddownload -r https://team.contoso.com/*,https://mysites.contoso.com/* -a https://contoso-admin.contoso.com -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("sandboxtool.exe -m <mode> -r <urls> -a <tenant admin site> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("e.g. sandboxtool.exe -m scananddownload -r https://team.contoso.com/*,https://mysites.contoso.com/* -a https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd");
            help.AddOptions(this);
            return help;
        }


        internal bool UsesAppOnly()
        {
            if (!String.IsNullOrEmpty(ClientID) && !String.IsNullOrEmpty(ClientSecret))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool UsesCredentials()
        {
            if (!String.IsNullOrEmpty(User) && !String.IsNullOrEmpty(Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
