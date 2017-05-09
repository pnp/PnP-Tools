using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using System.Reflection;
using System.Diagnostics;

namespace SharePoint.AccessApp.Scanner
{
    /// <summary>
    /// Possible scanning modes
    /// </summary>
    public enum Mode
    {
        Scan = 0,
    }

    /// <summary>
    /// Options available on the command line
    /// </summary>
    public class Options
    {
        [Option('m', "mode", HelpText = "Execution mode. Omit or use scan for a full scan", DefaultValue = Mode.Scan, Required = false)]
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

        [Option('x', "excludeod4b", HelpText = "Exclude OD4B sites from the scan", DefaultValue = false)]
        public bool ExcludeOD4B { get; set; }

        [Option('e', "separator", HelpText = "Separator used in output CSV files (e.g. \";\")", DefaultValue = ",")]
        public string Separator { get; set; }

        [Option('h', "threads", HelpText = "Number of parallel threads, maximum = 100", DefaultValue = 10)]
        public int Threads { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            var help = new HelpText
            {
                Heading = new HeadingInfo("SharePoint AccessApp Scanner tool", version),
                Copyright = new CopyrightInfo("SharePoint PnP", 2017),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = 120
            };
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("See the PnP-Tools repo for more information at:");
            help.AddPreOptionsLine("https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.AccessApp.Scanner");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Let the tool figure out your urls (works only for SPO MT):");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("accessappscanner.exe -t <tenant> -c <your client id> -s <your client secret>");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("e.g. accessappscanner.exe -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("accessappscanner.exe -t <tenant> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("e.g. accessappscanner.exe -m scan -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Specifying your urls to scan + url to tenant admin (needed for SPO Dedicated):");
            help.AddPreOptionsLine("==============================================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("accessappscanner.exe -r <urls> -a <tenant admin site> -c <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. accessappscanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a https://contoso-admin.contoso.com -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("accessappscanner.exe -r <urls> -a <tenant admin site> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("e.g. accessappscanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd");
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

        public List<Mode> GetAllOptions(string[] args)
        {
            System.Collections.Generic.List<Mode> lstmode = new System.Collections.Generic.List<Mode>();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg == "-m" || arg == "-mode")
                {
                    string mode = args[i + 1];
                    if (mode != null)
                    {
                        string[] modes = mode.Split(',');
                        foreach (string m in modes)
                        {
                            switch (m.Trim().ToLower())
                            {
                                default:
                                    if (!lstmode.Contains(Mode.Scan)) lstmode.Add(Mode.Scan);
                                    break;
                            }
                        }

                        return lstmode;
                    }                   
                }
            }

            lstmode.Add(Mode.Scan);

            return lstmode;
        }
    }
}
