using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{

    [Cmdlet(VerbsCommon.Format, "SPSuggestions")]
    public class FormatSuggestionsCmdlet :
        PSCmdlet
    {
        private static readonly string[] splitOn = new string[] { "<B>", "</B>" };
        private bool doHighlight;

        #region ScriptParameters

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            HelpMessage = "The Query containing the highlighting tags (<B> and </B>)."
        )]
        public string[] Query { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Highlight Color."
        )]
        public ConsoleColor Color { get; set; } = ConsoleColor.Green;

        #endregion

        #region Methods

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            doHighlight = false;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (var query in Query)
            {
                doHighlight = query.StartsWith("<B>") ? true : false;

                var tokens = query.Split(splitOn, StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    WriteInformation(new HostInformationMessage
                    {
                        Message = token,
                        ForegroundColor = doHighlight ?  Color : Host.UI.RawUI.ForegroundColor,
                        BackgroundColor = Host.UI.RawUI.BackgroundColor,
                        NoNewLine = true
                    }, new[] { "PSHOST" });

                    doHighlight = !doHighlight;
                }

                WriteInformation(new HostInformationMessage
                {
                    Message = "",
                    ForegroundColor = Host.UI.RawUI.ForegroundColor,
                    BackgroundColor = Host.UI.RawUI.BackgroundColor,
                    NoNewLine = false
                }, new[] { "PSHOST" });
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            WriteInformation(new HostInformationMessage
            {
                Message = "",
                ForegroundColor = Host.UI.RawUI.ForegroundColor,
                BackgroundColor = Host.UI.RawUI.BackgroundColor,
                NoNewLine = false
            }, new[] { "PSHOST" });
        }

        #endregion
    }
}
