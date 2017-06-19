using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    internal abstract class RenderableRankingFeature
    {
        // Fields
        protected RankingFeature _rankingFeature;

        // Methods
        public RenderableRankingFeature(RankingFeature rankingFeature)
        {
            this._rankingFeature = rankingFeature;
        }

        public abstract void CreateDocPropsControls(Table tbl);
        protected void CreateHNControls(TableRow tr)
        {
            TableCell cell = new TableCell();
            tr.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(string.Empty));
            for (int i = 0; i < this._rankingFeature.HNWeights.Length; i++)
            {
                cell = new TableCell();
                tr.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl(this._rankingFeature.HNWeights[i].ToString()));
            }
        }

        // Properties
        public float[] HNWeights
        {
            get
            {
                return this._rankingFeature.HNWeights;
            }
        }
    }


}
