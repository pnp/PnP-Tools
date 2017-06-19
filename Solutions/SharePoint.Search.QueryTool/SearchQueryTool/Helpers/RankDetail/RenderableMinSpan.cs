using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    internal class RenderableMinSpan : RenderableRankingFeature
    {
        // Methods
        public RenderableMinSpan(MinSpan minSpanFeature) : base(minSpanFeature)
        {
        }

        // Properties
        private MinSpan Feature
        {
            get { return (MinSpan) base._rankingFeature; }
        }

        public override void CreateDocPropsControls(Table tbl)
        {
            var row = new TableRow();
            tbl.Rows.Add(row);
            var cell = new TableCell();
            row.Cells.Add(cell);
            if (Feature.Type == "1")
            {
                cell.Controls.Add(new LiteralControl("MinSpan - phrase"));
            }
            else
            {
                cell.Controls.Add(new LiteralControl("MinSpan - soft"));
            }
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Property.Name));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Value.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Transformed.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Normalized.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.BestMinSpan.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.BestSpanCount.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.BestDiffTerms.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.RarestTermCount.ToString()));
            base.CreateHNControls(row);
        }
    }
}