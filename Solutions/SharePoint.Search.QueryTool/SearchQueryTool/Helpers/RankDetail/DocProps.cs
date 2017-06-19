using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{

    public class DocProps : WebControl
    {
        // Fields
        internal long _iExternalDocId = -1;
        internal long _iInternalDocId = -1;
        internal string _Language = string.Empty;
        internal Guid _ModelId = Guid.Empty;
        internal string _ModelType = string.Empty;
        internal float _OriginalScore;
        internal string _Path = string.Empty;
        internal float _Score;
        internal string _Title = string.Empty;

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            base.Render(writer);
        }

        // Methods
        protected override void CreateChildControls()
        {
            Table child = new Table
            {
                ID = "DocProps"
            };
            child.Attributes.Add("cellSpacing", "0");
            child.Attributes.Add("cellPadding", "3");
            child.Attributes.Add("width", "100%");
            child.BorderWidth = 0;
            this.Controls.Add(child);
            TableRow row = new TableRow();
            child.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.Attributes.Add("class", "ch");
            cell.Attributes.Add("bgColor", "#ccddee");
            cell.Controls.Add(new LiteralControl("<FONT size=\"-1\">"));
            if (this._Path.Length > 0)
            {
                row = new TableRow();
                child.Rows.Add(row);
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl("URL"));
                cell.CssClass = "q";
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(this._Path)));
            }
            if (this._Title.Length > 0)
            {
                row = new TableRow();
                child.Rows.Add(row);
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl("Title"));
                cell.CssClass = "q";
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(this._Title)));
            }
            if (this._Language.Length > 0)
            {
                row = new TableRow();
                child.Rows.Add(row);
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl("Language"));
                cell.CssClass = "q";
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(this._Language)));
            }
            row = new TableRow();
            child.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("External DocId"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(this._iExternalDocId.ToString()));
            row = new TableRow();
            child.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Internal DocId"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(this._iInternalDocId.ToString()));
            row = new TableRow();
            child.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Normalized Rank"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(this._Score.ToString()));
            row = new TableRow();
            child.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("OriginalScore"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(this._OriginalScore.ToString()));
            row = new TableRow();
            child.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Ranking Model Type"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(this._ModelType.ToString()));
            row = new TableRow();
            child.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Ranking Model ID"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(this._ModelId.ToString())));
        }
    }

}
