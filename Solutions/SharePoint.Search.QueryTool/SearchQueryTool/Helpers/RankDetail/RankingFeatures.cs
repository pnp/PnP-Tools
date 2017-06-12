using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    public class RankingFeatures : WebControl
    {
        // Fields
        internal List<RenderableRankingFeature> _RankingFeatures;

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            base.Render(writer);
        }

        // Methods
        protected override void CreateChildControls()
        {
            var child = new Table
                            {
                                ID = "DocRankProps"
                            };
            child.Attributes.Add("cellSpacing", "0");
            child.Attributes.Add("cellPadding", "3");
            child.Attributes.Add("width", "100%");
            child.BorderWidth = 0;
            Controls.Add(child);
            var row = new TableRow();
            child.Rows.Add(row);
            var cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Feature</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Property</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Value</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Transformed</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Normalized</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>MinSpan Len</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Span Count</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Span Terms</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Rarest Term</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            if (_RankingFeatures != null)
            {
                int length = _RankingFeatures[0].HNWeights.Length;
                for (int i = 0; i < length; i++)
                {
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl("<B>Node " + i.ToString() + "</B>"));
                }
                foreach (RenderableRankingFeature feature in _RankingFeatures)
                {
                    feature.CreateDocPropsControls(child);
                }
            }
        }
    }
}