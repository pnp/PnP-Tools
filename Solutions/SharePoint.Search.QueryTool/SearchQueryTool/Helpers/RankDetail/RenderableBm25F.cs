using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    internal class RenderableBM25F : RenderableRankingFeature
    {
        // Methods
        public RenderableBM25F(BM25 bm25RankingFeature)
            : base(bm25RankingFeature)
        {
        }

        public override void CreateDocPropsControls(Table tbl)
        {
            TableRow row = new TableRow();
            tbl.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("BM25"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(this.Feature.Score.ToString()));
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
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            base.CreateHNControls(row);
        }

        public void CreatePropDetailControls(Table tbl)
        {
            TableRow row = new TableRow();
            tbl.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Term</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Property</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Weight</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>B</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Original TF</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Weighted TF</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Length</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>AVDL</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>DL/AVDL</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>DL Factor</B>"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("<B>Normalized TF</B>"));
            foreach (QueryTerm term in this.Feature.Terms)
            {
                foreach (Pid pid in term.Pids)
                {
                    row = new TableRow();
                    tbl.Rows.Add(row);
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(term.TermName)));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.Name));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.Weight.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.B.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.TF.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.TFW.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.DL.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.AVDL.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.DLAvdl.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.DLNorm.ToString()));
                    cell = new TableCell();
                    row.Cells.Add(cell);
                    cell.Controls.Add(new LiteralControl(pid.TFNorm.ToString()));
                }
            }
        }

        public void CreateTermDetailControls(Table tbl)
        {
            TableRow row = new TableRow();
            tbl.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(""));
            foreach (QueryTerm term in this.Feature.Terms)
            {
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl("<B>" + HttpUtility.HtmlEncode(term.TermName) + "</B>"));
            }
            row = new TableRow();
            tbl.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("n"));
            cell.CssClass = "q";
            foreach (QueryTerm term2 in this.Feature.Terms)
            {
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(term2.n.ToString()));
            }
            row = new TableRow();
            tbl.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("BM25 Weight"));
            cell.CssClass = "q";
            foreach (QueryTerm term3 in this.Feature.Terms)
            {
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(term3.RW.ToString()));
            }
            row = new TableRow();
            tbl.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("N"));
            cell.CssClass = "q";
            foreach (QueryTerm term4 in this.Feature.Terms)
            {
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(term4.N.ToString()));
            }
            row = new TableRow();
            tbl.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Weighted TF"));
            cell.CssClass = "q";
            foreach (QueryTerm term5 in this.Feature.Terms)
            {
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(term5.TFW.ToString()));
            }
            row = new TableRow();
            tbl.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("Term Score"));
            cell.CssClass = "q";
            foreach (QueryTerm term6 in this.Feature.Terms)
            {
                cell = new TableCell();
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(term6.Score.ToString()));
            }
        }

        // Properties
        protected BM25 Feature
        {
            get
            {
                return (BM25)base._rankingFeature;
            }
        }
    }

}
