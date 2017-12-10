using CommandLine;
using SharePoint.Scanning.Framework;
using System;
using System.Collections.Generic;

namespace SharePoint.Scanning.ReferenceScanner
{
    /// <summary>
    /// Possible scanning modes
    /// </summary>
    public enum Mode
    {
        DefaultMode = 0,
        Mode2,
        Mode3,
    }

    /// <summary>
    /// Commandline options
    /// </summary>
    public class Options: BaseOptions
    {
        public List<Mode> ScanModes { get; set; }

        // Important:
        // Following chars are already used as shorthand in the base options class: i, s, u, p, f, x, a, t, e, r, v, o, h, z

        [Option('m', "mode", HelpText = "Execution mode. Use following modes: mode1, mode2, mode3, mode4. Omit or use scan for a full scan", DefaultValue = Mode.DefaultMode, Required = false)]
        public Mode Mode { get; set; }

        [Option('q', "dontusesearchquery", HelpText = "Use site enumeration instead of search to find the impacted files", DefaultValue = false)]
        public bool DontUseSearchQuery { get; set; }



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

            base.ValidateOptions(args);

            // Add validation for the options added in this child class
            ScanModes = this.GetAllOptions(args);

            if (ScanModes.Count == 0)
            {
                Console.WriteLine("You need at least have one mode set.");
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
        }

        /// <summary>
        /// Shows the usage information of the scanner
        /// </summary>
        /// <returns>String with the usage information</returns>
        [HelpOption]
        public string GetUsage()
        {
            var help = this.GetUsage("SharePoint PnP Reference scanner");

            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("See the PnP-Tools repo for more information at:");
            help.AddPreOptionsLine("https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.Scanning");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Let the tool figure out your urls (works only for SPO MT):");
            help.AddPreOptionsLine("==========================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("referencescanner.exe -m <mode> -t <tenant> -i <your client id> -s <your client secret>");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("e.g. referencescanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("e.g. referencescanner.exe -m mode2 -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("e.g. referencescanner.exe -m mode2,mode3 -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("referencescanner.exe -m <mode> -t <tenant> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("e.g. referencescanner.exe -m mode2 -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Specifying your urls to scan + url to tenant admin (needed for SPO Dedicated):");
            help.AddPreOptionsLine("==============================================================================");
            help.AddPreOptionsLine("Using app-only:");
            help.AddPreOptionsLine("referencescanner.exe -m <mode> -r <urls> -a <tenant admin site> -i <your client id> -s <your client secret>");
            help.AddPreOptionsLine("e.g. referencescanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Using credentials:");
            help.AddPreOptionsLine("referencescanner.exe -m <mode> -r <urls> -a <tenant admin site> -u <your user id> -p <your user password>");
            help.AddPreOptionsLine("e.g. referencescanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd");
            help.AddOptions(this);
            return help;
        }

        /// <summary>
        /// Processes the modes passed via commandline into a list of modes
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        /// <returns>List of passed scan modes</returns>
        public List<Mode> GetAllOptions(string[] args)
        {
            System.Collections.Generic.List<Mode> lstmode = new System.Collections.Generic.List<Mode>();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();

                if (arg == "-m" || arg == "--mode")
                {
                    string mode = args[i + 1];
                    if (mode != null)
                    {
                        string[] modes = mode.Split(',');
                        foreach (string m in modes)
                        {
                            switch (m.Trim().ToLower())
                            {
                                case "mode2":
                                    if (!lstmode.Contains(Mode.Mode2)) { lstmode.Add(Mode.Mode2); }
                                    break;
                                case "mode3":
                                    if (!lstmode.Contains(Mode.Mode3)) lstmode.Add(Mode.Mode3);
                                    break;
                                default:
                                    if (!lstmode.Contains(Mode.DefaultMode)) lstmode.Add(Mode.DefaultMode);
                                    break;
                            }
                        }

                        return lstmode;
                    }
                }
            }

            lstmode.Add(Mode.DefaultMode);

            return lstmode;
        }
    }
}
