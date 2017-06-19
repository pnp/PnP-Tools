using System.Web.UI;
using System.Web.UI.WebControls;

namespace SearchQueryTool.Helpers
{
    internal class RenderableBucketedStatic : RenderableRankingFeature
    {
        // Methods
        public RenderableBucketedStatic(BucketedStatic bucketedStaticFeature)
            : base(bucketedStaticFeature)
        {
        }

        // Properties
        protected BucketedStatic Feature
        {
            get { return (BucketedStatic) base._rankingFeature; }
        }

        public override void CreateDocPropsControls(Table tbl)
        {
            var row = new TableRow();
            tbl.Rows.Add(row);
            var cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl("BucketedStatic"));
            cell.CssClass = "q";
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(Feature.Property.Name));
            string text = string.Empty;
            if (Feature.Property.Name == "InternalFileType")
            {
                switch (Feature.Val)
                {
                    case 0:
                        text = "Undefined/html";
                        goto Label_0291;

                    case 1:
                        text = "Word Doc";
                        goto Label_0291;

                    case 2:
                        text = "Power Point";
                        goto Label_0291;

                    case 3:
                        text = "Excel";
                        goto Label_0291;

                    case 4:
                        text = "Xml";
                        goto Label_0291;

                    case 5:
                        text = "Text";
                        goto Label_0291;

                    case 6:
                        text = "List Item";
                        goto Label_0291;

                    case 7:
                        text = "Email";
                        goto Label_0291;

                    case 8:
                        text = "Image";
                        goto Label_0291;

                    case 9:
                        text = "Person";
                        goto Label_0291;

                    case 10:
                        text = "Video";
                        goto Label_0291;

                    case 11:
                        text = "SharePoint Site";
                        goto Label_0291;

                    case 12:
                        text = "OneNote";
                        goto Label_0291;

                    case 13:
                        text = "Publisher";
                        goto Label_0291;

                    case 14:
                        text = "Visio";
                        goto Label_0291;

                    case 15:
                        text = "PDF";
                        goto Label_0291;

                    case 0x10:
                        text = "Webpage";
                        goto Label_0291;

                    case 0x11:
                        text = "Zip";
                        goto Label_0291;

                    case 0x12:
                        text = "Blog";
                        goto Label_0291;

                    case 0x13:
                        text = "Discussion Board";
                        goto Label_0291;

                    case 20:
                        text = "Document Library";
                        goto Label_0291;

                    case 0x15:
                        text = "List";
                        goto Label_0291;

                    case 0x16:
                        text = "Picture Library";
                        goto Label_0291;

                    case 0x17:
                        text = "Picture Library Item";
                        goto Label_0291;

                    case 0x18:
                        text = "Survey";
                        goto Label_0291;

                    case 0x19:
                        text = "Wiki";
                        goto Label_0291;

                    case 0x1a:
                        text = "Access";
                        goto Label_0291;

                    case 0x1b:
                        text = "MicroBlog Post";
                        goto Label_0291;

                    case 0x1c:
                        text = "Community";
                        goto Label_0291;

                    case 0x1d:
                        text = "Discussion";
                        goto Label_0291;

                    case 30:
                        text = "Reply";
                        goto Label_0291;
                }
                text = Feature.Val.ToString();
            }
            else
            {
                text = Feature.Val.ToString();
            }
            Label_0291:
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.Controls.Add(new LiteralControl(text));
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
    }
}