using System;
using System.Management.Automation;

namespace PSSQT
{

    [Cmdlet(VerbsCommon.Format, "SPResults")]
    public class FormatResultsCmdlet :
        PSCmdlet
    {
        //private bool doHighlight;
 
        private Helpers.ConsoleFormatter formatter;

        #region ScriptParameters

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            HelpMessage = "The Result object containing title, hithighlightedsummary and path."
        )]
        public PSObject[] Results { get; set; }

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

            formatter = new Helpers.ConsoleFormatter(this);
            formatter.Color = Color;

            formatter.FormatDividerLine();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            formatter.FormatResults(Results);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            formatter.FormatDividerLine();
        }


        #endregion
    }
}
