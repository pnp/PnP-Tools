using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Pages
{
    public enum PageLayout
    {
        Wiki_OneColumn = 0,
        Wiki_TwoColumns = 1,
        Wiki_TwoColumnsWithSidebar = 2,
        Wiki_TwoColumnsWithHeader = 3,
        Wiki_TwoColumnsWithHeaderAndFooter = 4,
        Wiki_ThreeColumns = 5,
        Wiki_ThreeColumnsWithHeader = 6,
        Wiki_ThreeColumnsWithHeaderAndFooter = 7,
        Wiki_Custom = 8,
        WebPart_HeaderFooterThreeColumns = 20,
        WebPart_FullPageVertical = 21,
        WebPart_HeaderLeftColumnBody = 22,
        WebPart_HeaderRightColumnBody = 23,
        WebPart_HeaderFooter2Columns4Rows = 24,
        WebPart_HeaderFooter4ColumnsTopRow = 25,
        WebPart_LeftColumnHeaderFooterTopRow3Columns = 26,
        WebPart_RightColumnHeaderFooterTopRow3Columns = 27,
        WebPart_Custom = 28
    }
}
