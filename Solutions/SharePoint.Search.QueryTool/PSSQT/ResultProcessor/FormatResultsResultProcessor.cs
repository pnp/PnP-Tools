using System.Management.Automation;

namespace PSSQT.ResultProcessor
{
    public class FormatResultsResultProcessor : PrimaryResultsResultProcessor
    {
        private Helpers.ConsoleFormatter formatter;

        public FormatResultsResultProcessor(SearchSPIndexCmdlet cmdlet) :
            base(cmdlet)
        {
            if (IsEndOfPipeline())
            {
                formatter = new Helpers.ConsoleFormatter(cmdlet);
                formatter.FormatDividerLine();
            }
        }

        protected override void EnsurePropertiesPresent()
        {
            base.EnsurePropertiesPresent();

            Cmdlet.AddSelectProperty("title");
            Cmdlet.AddSelectProperty("HitHighlightedSummary");    // case matters!!!
            Cmdlet.AddSelectProperty("path");
        }

        protected override void PrimaryResultWriteItem(PSObject item)
        {
            if (IsEndOfPipeline())
            {
                formatter.FormatResult(item);    
            }
            else
            {
                base.PrimaryResultWriteItem(item);
            }
        }
    }
}
