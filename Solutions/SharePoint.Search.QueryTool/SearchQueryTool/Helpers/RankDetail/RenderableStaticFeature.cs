using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    internal class RenderableStaticFeature : RenderableRankingFeature
    {
        // Methods
        public RenderableStaticFeature(StaticRank staticFeature)
            : base(staticFeature)
        {
        }

        // Properties
        protected StaticRank Feature
        {
            get { return (StaticRank) base._rankingFeature; }
        }

        public override void CreateDocPropsControls(Table tbl)
        {
            var row = new TableRow();
            tbl.Rows.Add(row);
            var cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Static"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Property.Name));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Val.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.ValT.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.ValN.ToString()));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            base.CreateHNControls(row);
        }
    }
}