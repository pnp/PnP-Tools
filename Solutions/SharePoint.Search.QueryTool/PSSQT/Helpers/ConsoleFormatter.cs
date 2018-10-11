using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSSQT.Helpers
{
    public class ConsoleFormatter
    {
        private static readonly string[] splitOn = new string[] { "<c0>", "</c0>" };
        private static readonly string wspattern = @"\s+";

        // These are used here and in FormatResultsCmdlet
        public static readonly ConsoleColor DefaultSummaryHitHighlightColor = ConsoleColor.Magenta;
        public static readonly ConsoleColor DefaultTitleColor = ConsoleColor.Cyan;
        public static readonly ConsoleColor DefaultNumberColor = ConsoleColor.Yellow;
        public static readonly ConsoleColor DefaultPathColor = ConsoleColor.DarkGray;

        //private bool doHighlight;
        private int resultNumber = 1;

        private static readonly int titlePresent = 0x1;
        private static readonly int hhsummaryPresent = 0x2;
        private static readonly int pathPresent = 0x4;
        private static readonly int allPresent = titlePresent | hhsummaryPresent | pathPresent;

        public ConsoleColor SummaryHitHighlightColor { get; set; } = DefaultSummaryHitHighlightColor;
        public ConsoleColor TitleColor { get; set; } = DefaultTitleColor;
        public ConsoleColor NumberColor { get; set; } = DefaultNumberColor;
        public ConsoleColor PathColor { get; set; } = DefaultPathColor;

        public PSCmdlet Cmdlet { get; set; }

        public ConsoleFormatter(PSCmdlet cmdlet)
        {
            Cmdlet = cmdlet;
        }


        public void FormatResults(PSObject[] results)
        {
            foreach (var result in results)
            {
                FormatResult(result);
            }

            FormatDividerLine();
        }

        public void FormatResult(PSObject result)
        {
            var presentMask = 0;

            foreach (PSPropertyInfo psPropertyInfo in result.Properties)
            {
                // process title, hithighlightedsummary and path
                Cmdlet.WriteDebug($"Name: {psPropertyInfo.Name} Value: {psPropertyInfo.Value}");

                if (psPropertyInfo.Name.Equals("title", StringComparison.InvariantCultureIgnoreCase))
                {
                    var title = psPropertyInfo.Value as string;

                    presentMask |= titlePresent;

                    FormatTitle(resultNumber++, title);
                }
                else if (psPropertyInfo.Name.Equals("hithighlightedsummary", StringComparison.InvariantCultureIgnoreCase))
                {
                    var hhsummary = psPropertyInfo.Value as string;

                    hhsummary = hhsummary.Replace("<ddd/>", "\u2026");
                    hhsummary = hhsummary.Replace("&amp;", "&");
                    hhsummary = Regex.Replace(hhsummary, wspattern, " ");

                    presentMask |= hhsummaryPresent;

                    FormatHithighlightedSummary(hhsummary);
                }
                else if (psPropertyInfo.Name.Equals("path", StringComparison.InvariantCultureIgnoreCase))
                {
                    var path = psPropertyInfo.Value as string;

                    presentMask |= pathPresent;

                    FormatPath(path);
                }
                else
                {
                    Cmdlet.WriteVerbose($"Ignored Property Name: {psPropertyInfo.Name} Value: {psPropertyInfo.Value}");
                }

            }

            if (presentMask != allPresent)
            {
                if ((presentMask & titlePresent) == 0)
                {
                    Cmdlet.WriteVerbose("title property not found.");
                }
                if ((presentMask & hhsummaryPresent) == 0)
                {
                    Cmdlet.WriteVerbose("hithighlightedsummary property not found.");
                }
                if ((presentMask & pathPresent) == 0)
                {
                    Cmdlet.WriteVerbose("path property not found.");
                }
            }

            FormatDividerLine();
        }

 
        public void FormatDividerLine()
        {
            Cmdlet.WriteInformation(new HostInformationMessage
            {
                Message = "",
                ForegroundColor = Cmdlet.Host.UI.RawUI.ForegroundColor,
                BackgroundColor = Cmdlet.Host.UI.RawUI.BackgroundColor,
                NoNewLine = false
            }, new[] { "PSHOST" });

        }

        private void FormatTitle(int resultNumber, string title)
        {
            Cmdlet.WriteInformation(new HostInformationMessage
            {
                Message = $"{resultNumber.ToString()}. ",
                ForegroundColor = NumberColor,
                BackgroundColor = Cmdlet.Host.UI.RawUI.BackgroundColor,
                NoNewLine = true
            }, new[] { "PSHOST" });

            Cmdlet.WriteInformation(new HostInformationMessage
            {
                Message = title,
                ForegroundColor = TitleColor,
                BackgroundColor = Cmdlet.Host.UI.RawUI.BackgroundColor,
                NoNewLine = false
            }, new[] { "PSHOST" });
        }

        private void FormatHithighlightedSummary(string hhsummary)
        {

            if (String.IsNullOrWhiteSpace(hhsummary))
            {
                hhsummary = "\u2026";
            }

            bool doHighlight = hhsummary.StartsWith("<c0>") ? true : false;

            var tokens = hhsummary.Split(splitOn, StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {

                Cmdlet.WriteInformation(new HostInformationMessage
                {
                    Message = token,
                    ForegroundColor = doHighlight ? SummaryHitHighlightColor : Cmdlet.Host.UI.RawUI.ForegroundColor,
                    BackgroundColor = Cmdlet.Host.UI.RawUI.BackgroundColor,
                    NoNewLine = true
                }, new[] { "PSHOST" });

                doHighlight = !doHighlight;
            }

            Cmdlet.WriteInformation(new HostInformationMessage
            {
                Message = "",
                ForegroundColor = Cmdlet.Host.UI.RawUI.ForegroundColor,
                BackgroundColor = Cmdlet.Host.UI.RawUI.BackgroundColor,
                NoNewLine = false
            }, new[] { "PSHOST" });

        }

        private void FormatPath(string path)
        {
            Cmdlet.WriteInformation(new HostInformationMessage
            {
                Message = path,
                ForegroundColor = PathColor,
                BackgroundColor = Cmdlet.Host.UI.RawUI.BackgroundColor,
                NoNewLine = false
            }, new[] { "PSHOST" });
        }

    }
}
