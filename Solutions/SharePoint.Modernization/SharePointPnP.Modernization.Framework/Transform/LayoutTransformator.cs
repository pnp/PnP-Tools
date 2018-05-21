using OfficeDevPnP.Core.Pages;
using SharePointPnP.Modernization.Framework.Pages;

namespace SharePointPnP.Modernization.Framework.Transform
{

    /// <summary>
    /// Transforms the layout of a classic wiki/webpart page into a modern client side page using sections and columns
    /// </summary>
    public class LayoutTransformator: ILayoutTransformator
    {
        private ClientSidePage page;

        #region Construction
        /// <summary>
        /// Creates a layout transformator instance
        /// </summary>
        /// <param name="page">Client side page that will be receive the created layout</param>
        public LayoutTransformator(ClientSidePage page)
        {
            this.page = page;
        }
        #endregion

        /// <summary>
        /// Transforms a classic wiki/webpart page layout into a modern client side page layout
        /// </summary>
        /// <param name="layout">Source wiki/webpart page layout</param>
        public virtual void Transform(PageLayout layout)
        {
            switch (layout)
            {
                // In case of a custom layout let's stick with one column as model
                case PageLayout.Wiki_OneColumn:
                case PageLayout.WebPart_FullPageVertical:
                case PageLayout.Wiki_Custom:
                case PageLayout.WebPart_Custom:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        return;
                    }
                case PageLayout.Wiki_TwoColumns:
                    {
                        page.AddSection(CanvasSectionTemplate.TwoColumn, 1);
                        return;
                    }
                case PageLayout.Wiki_ThreeColumns:
                    {
                        page.AddSection(CanvasSectionTemplate.ThreeColumn, 1);
                        return;
                    }
                case PageLayout.Wiki_TwoColumnsWithSidebar:
                case PageLayout.WebPart_HeaderRightColumnBody:
                    {
                        page.AddSection(CanvasSectionTemplate.TwoColumnLeft, 1);
                        return;
                    }
                case PageLayout.WebPart_HeaderLeftColumnBody:
                    {
                        page.AddSection(CanvasSectionTemplate.TwoColumnRight, 1);
                        return;
                    }
                case PageLayout.Wiki_TwoColumnsWithHeader:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        page.AddSection(CanvasSectionTemplate.TwoColumn, 2);
                        return;
                    }
                case PageLayout.Wiki_TwoColumnsWithHeaderAndFooter:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        page.AddSection(CanvasSectionTemplate.TwoColumn, 2);
                        page.AddSection(CanvasSectionTemplate.OneColumn, 3);
                        return;
                    }
                case PageLayout.Wiki_ThreeColumnsWithHeader:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        page.AddSection(CanvasSectionTemplate.ThreeColumn, 2);
                        return;
                    }
                case PageLayout.Wiki_ThreeColumnsWithHeaderAndFooter:
                case PageLayout.WebPart_HeaderFooterThreeColumns:
                case PageLayout.WebPart_HeaderFooter4ColumnsTopRow:
                case PageLayout.WebPart_HeaderFooter2Columns4Rows:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        page.AddSection(CanvasSectionTemplate.ThreeColumn, 2);
                        page.AddSection(CanvasSectionTemplate.OneColumn, 3);
                        return;
                    }
                case PageLayout.WebPart_LeftColumnHeaderFooterTopRow3Columns:
                case PageLayout.WebPart_RightColumnHeaderFooterTopRow3Columns:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        page.AddSection(CanvasSectionTemplate.OneColumn, 2);
                        page.AddSection(CanvasSectionTemplate.ThreeColumn, 3);
                        page.AddSection(CanvasSectionTemplate.OneColumn, 4);
                        return;
                    }
                default:
                    {
                        page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                        return;
                    }
            }
        }

    }
}
