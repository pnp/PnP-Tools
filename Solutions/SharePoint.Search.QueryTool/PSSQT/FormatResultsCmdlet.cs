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
        [Alias("SummaryHitHighlightColor")]
        public ConsoleColor Color { get; set; } = Helpers.ConsoleFormatter.DefaultSummaryHitHighlightColor;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Title Color."
        )]
        public ConsoleColor TitleColor { get; set; } = Helpers.ConsoleFormatter.DefaultTitleColor;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Number Color."
        )]
        public ConsoleColor NumberColor { get; set; } = Helpers.ConsoleFormatter.DefaultNumberColor;

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = false,
            HelpMessage = "Path Color."
        )]
        public ConsoleColor PathColor { get; set; } = Helpers.ConsoleFormatter.DefaultPathColor;
        #endregion

        #region Methods

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            formatter = new Helpers.ConsoleFormatter(this);
            formatter.SummaryHitHighlightColor = Color;
            formatter.TitleColor = TitleColor;
            formatter.NumberColor = NumberColor;
            formatter.PathColor = PathColor;

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
