using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    public class RankValuesList : WebControl
    {
        // Fields
        internal RenderableBM25F _bm25Feature;

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
                                ID = "Terms"
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
            cell.Attributes.Add("class", "ch");
            cell.Attributes.Add("bgColor", "#ccddee");
            cell.Controls.Add(new LiteralControl("<FONT size=\"-1\">"));
            if (_bm25Feature != null)
            {
                _bm25Feature.CreateTermDetailControls(child);
            }
        }
    }
}