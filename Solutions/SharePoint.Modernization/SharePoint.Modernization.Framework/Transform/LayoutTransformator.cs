using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;
using SharePoint.Modernization.Framework.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    public class LayoutTransformator: ILayoutTransformator
    {
        private ClientSidePage page;

        public LayoutTransformator(ClientSidePage page)
        {
            this.page = page;
        }

        public virtual void ApplyLayout(PageLayout layout)
        {
            switch (layout)
            {
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
