using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    public class LayoutTransformator
    {
        private ClientSidePage page;

        public LayoutTransformator(ClientSidePage page)
        {
            this.page = page;
        }

        public void ApplyLayout(string layout)
        {                        
            /***************  
            Wiki pages:          
            * OneColumn
            * TwoColumns
            * TwoColumnsWithSidebar
            * TwoColumnsWithHeader
            * TwoColumnsWithHeaderAndFooter
            * ThreeColumns
            * ThreeColumnsWithHeader
            * ThreeColumnsWithHeaderAndFooter

            Webpart pages:
            * HeaderFooterThreeColumns,
            * FullPageVertical,
            * HeaderLeftColumnBody,
            * HeaderRightColumnBody,
            * HeaderFooter2Columns4Rows,
            * HeaderFooter4ColumnsTopRow,
            * LeftColumnHeaderFooterTopRow3Columns,
            * RightColumnHeaderFooterTopRow3Columns,
            * Custom
            ******************/

            if (layout.Equals("OneColumn", StringComparison.InvariantCultureIgnoreCase) ||
                layout.Equals("Custom", StringComparison.InvariantCultureIgnoreCase) ||
                layout.Equals("FullPageVertical", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.OneColumn, 1);
            }
            else if (layout.Equals("TwoColumns", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.TwoColumn, 1);
            }
            else if (layout.Equals("ThreeColumns", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.ThreeColumn, 1);
            }
            else if (layout.Equals("TwoColumnsWithSidebar", StringComparison.InvariantCultureIgnoreCase) ||
                     layout.Equals("HeaderRightColumnBody", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.TwoColumnLeft, 1);
            }
            else if (layout.Equals("HeaderLeftColumnBody", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.TwoColumnRight, 1);
            }
            else if (layout.Equals("TwoColumnsWithHeader", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                page.AddSection(CanvasSectionTemplate.TwoColumn, 2);
            }
            else if (layout.Equals("TwoColumnsWithHeaderAndFooter", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                page.AddSection(CanvasSectionTemplate.TwoColumn, 2);
                page.AddSection(CanvasSectionTemplate.OneColumn, 3);
            }
            else if (layout.Equals("ThreeColumnsWithHeader", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                page.AddSection(CanvasSectionTemplate.ThreeColumn, 2);
            }
            else if (layout.Equals("ThreeColumnsWithHeaderAndFooter", StringComparison.InvariantCultureIgnoreCase) ||
                     layout.Equals("HeaderFooterThreeColumns", StringComparison.InvariantCultureIgnoreCase) ||
                     layout.Equals("HeaderFooter4ColumnsTopRow", StringComparison.InvariantCultureIgnoreCase) ||
                     layout.Equals("HeaderFooter2Columns4Rows", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                page.AddSection(CanvasSectionTemplate.ThreeColumn, 2);
                page.AddSection(CanvasSectionTemplate.OneColumn, 3);
            }
            else if (layout.Equals("LeftColumnHeaderFooterTopRow3Columns", StringComparison.InvariantCultureIgnoreCase) ||
                     layout.Equals("RightColumnHeaderFooterTopRow3Columns", StringComparison.InvariantCultureIgnoreCase))
            {
                page.AddSection(CanvasSectionTemplate.OneColumn, 1);
                page.AddSection(CanvasSectionTemplate.OneColumn, 2);
                page.AddSection(CanvasSectionTemplate.ThreeColumn, 3);
                page.AddSection(CanvasSectionTemplate.OneColumn, 4);
            }


        }

    }
}
