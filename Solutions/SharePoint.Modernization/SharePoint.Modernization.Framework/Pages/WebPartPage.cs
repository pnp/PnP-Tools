using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SharePoint.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Pages
{
    /// <summary>
    /// Analyzes a web part page
    /// </summary>
    public class WebPartPage: BasePage
    {
        #region construction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageTransformation"></param>
        /// <param name="forceCheckout"></param>
        public WebPartPage(ListItem page, PageTransformation pageTransformation) : base(page, pageTransformation)
        {
        }
        #endregion

        /// <summary>
        /// Analyses a webpart page
        /// </summary>
        /// <param name="includeTitleBarWebPart">Include the TitleBar web part</param>
        /// <returns>Information about the analyzed webpart page</returns>
        public Tuple<PageLayout, List<WebPartEntity>> Analyze(bool includeTitleBarWebPart = false)
        {
            List<WebPartEntity> webparts = new List<WebPartEntity>();

            //Load the page
            var wikiPageUrl = page[Constants.FileRefField].ToString();
            var wikiPage = cc.Web.GetFileByServerRelativeUrl(wikiPageUrl);

            // Load page properties
            var pageProperties = wikiPage.Properties;
            cc.Load(pageProperties);

            // Load web parts on wiki page
            var limitedWPManager = wikiPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            cc.Load(limitedWPManager);

            IEnumerable<WebPartDefinition> webParts = cc.LoadQuery(limitedWPManager.WebParts.IncludeWithDefaultProperties(wp => wp.Id, wp => wp.ZoneId, wp => wp.WebPart.ExportMode, wp => wp.WebPart.Title, wp => wp.WebPart.ZoneIndex, wp => wp.WebPart.IsClosed, wp => wp.WebPart.Hidden, wp => wp.WebPart.Properties));
            cc.ExecuteQueryRetry();

            // Check page type
            var layout = GetLayout(pageProperties);
            
            if (webParts.Count() > 0)
            {
                foreach (var foundWebPart in webParts)
                {
                    // Skip Microsoft.SharePoint.WebPartPages.TitleBarWebPart webpart in TitleBar zone
                    if (foundWebPart.ZoneId.Equals("TitleBar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!includeTitleBarWebPart)
                        {
                            continue;
                        }
                    }

                    string webPartXml = "";
                    string webPartType = "";
                    var exportMode = foundWebPart.WebPart.ExportMode;
                    if (foundWebPart.WebPart.ExportMode != WebPartExportMode.All)
                    {
                        // Use different approach to determine type as we can't export the web part XML without indroducing a change
                        webPartType = GetTypeFromProperties(foundWebPart.WebPart.Properties);
                    }
                    else
                    {
                        var result = limitedWPManager.ExportWebPart(foundWebPart.Id);
                        cc.ExecuteQueryRetry();
                        webPartXml = result.Value;
                        webPartType = GetType(webPartXml);
                    }

                    webparts.Add(new WebPartEntity()
                    {
                        Title = foundWebPart.WebPart.Title,
                        Type = webPartType,
                        Id = foundWebPart.Id,
                        ServerControlId = foundWebPart.Id.ToString(),
                        Row = GetRow(foundWebPart.ZoneId, layout),
                        Column = GetColumn(foundWebPart.ZoneId, layout),
                        Order = foundWebPart.WebPart.ZoneIndex,
                        ZoneId = foundWebPart.ZoneId,
                        ZoneIndex = (uint)foundWebPart.WebPart.ZoneIndex,
                        IsClosed = foundWebPart.WebPart.IsClosed,
                        Hidden = foundWebPart.WebPart.Hidden,
                        Properties = Properties(foundWebPart.WebPart.Properties, webPartType, webPartXml),
                    });
                }
            }

            return new Tuple<PageLayout, List<WebPartEntity>>(layout, webparts);
        }

        /// <summary>
        /// Translates the given zone value and page layout to a column number
        /// </summary>
        /// <param name="zoneId">Web part zone id</param>
        /// <param name="layout">Layout of the web part page</param>
        /// <returns>Column value</returns>
        private int GetColumn(string zoneId, PageLayout layout)
        {
            switch (layout)
            {
                case PageLayout.WebPart_HeaderFooterThreeColumns:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("MiddleColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        break;
                    }
                case PageLayout.WebPart_FullPageVertical:
                    {
                        return 1;
                    }
                case PageLayout.WebPart_HeaderLeftColumnBody:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        break;
                    }
                case PageLayout.WebPart_HeaderRightColumnBody:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        break;
                    }
                case PageLayout.WebPart_HeaderFooter2Columns4Rows:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("Row1", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("Row2", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("Row3", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("Row4", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        break;
                    }
                case PageLayout.WebPart_HeaderFooter4ColumnsTopRow:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("CenterLeftColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("TopRow", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("CenterRightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        break;
                    }
                case PageLayout.WebPart_LeftColumnHeaderFooterTopRow3Columns:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("CenterLeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("TopRow", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("CenterColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("CenterRightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }                        
                        break;
                    }
                case PageLayout.WebPart_RightColumnHeaderFooterTopRow3Columns:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("CenterLeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("TopRow", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("CenterColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("CenterRightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        break;
                    }
                case PageLayout.WebPart_Custom:
                    {
                        return 1;
                    }
                default:
                    return 1;
            }

            return 1;
        }

        /// <summary>
        /// Translates the given zone value and page layout to a row number
        /// </summary>
        /// <param name="zoneId">Web part zone id</param>
        /// <param name="layout">Layout of the web part page</param>
        /// <returns>Row value</returns>
        private int GetRow(string zoneId, PageLayout layout)
        {
            switch (layout)
            {
                case PageLayout.WebPart_HeaderFooterThreeColumns:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("MiddleColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase) )
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        break;
                    }
                case PageLayout.WebPart_FullPageVertical:
                    {
                        return 1;                        
                    }
                case PageLayout.WebPart_HeaderLeftColumnBody:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        break;
                    }
                case PageLayout.WebPart_HeaderRightColumnBody:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        break;
                    }
                case PageLayout.WebPart_HeaderFooter2Columns4Rows:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("Row1", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("Row2", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        else if (zoneId.Equals("Row3", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 4;
                        }
                        else if (zoneId.Equals("Row4", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 5;
                        }
                        else if (zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 6;
                        }
                        break;
                    }
                case PageLayout.WebPart_HeaderFooter4ColumnsTopRow:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("TopRow", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("CenterLeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("CenterRightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        else if (zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 4;
                        }
                        break;
                    }
                case PageLayout.WebPart_LeftColumnHeaderFooterTopRow3Columns:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("LeftColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("TopRow", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("CenterLeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("CenterColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("CenterRightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        else if (zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 4;
                        }
                        break;
                    }
                case PageLayout.WebPart_RightColumnHeaderFooterTopRow3Columns:
                    {
                        if (zoneId.Equals("Header", StringComparison.InvariantCultureIgnoreCase) ||
                            zoneId.Equals("RightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 1;
                        }
                        else if (zoneId.Equals("TopRow", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                        else if (zoneId.Equals("CenterLeftColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("CenterColumn", StringComparison.InvariantCultureIgnoreCase) ||
                                 zoneId.Equals("CenterRightColumn", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 3;
                        }
                        else if (zoneId.Equals("Footer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 4;
                        }
                        break;
                    }
                case PageLayout.WebPart_Custom:
                    {
                        return 1;
                    }
                default:
                    return 1;
            }

            return 1;
        }

        /// <summary>
        /// Determines the used web part page layout
        /// </summary>
        /// <param name="pageProperties">Properties of the web part page file</param>
        /// <returns>Used layout</returns>
        private PageLayout GetLayout(PropertyValues pageProperties)
        {
            if (pageProperties.FieldValues.ContainsKey("vti_setuppath"))
            {
                var setupPath = pageProperties["vti_setuppath"].ToString();
                if (!string.IsNullOrEmpty(setupPath))
                {
                    if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd1.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_FullPageVertical;
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd2.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_HeaderFooterThreeColumns; 
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd3.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_HeaderLeftColumnBody;
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd4.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_HeaderRightColumnBody;
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd5.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_HeaderFooter2Columns4Rows;
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd6.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_HeaderFooter4ColumnsTopRow;
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd7.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_LeftColumnHeaderFooterTopRow3Columns;
                    }
                    else if (setupPath.Equals(@"1033\STS\doctemp\smartpgs\spstd8.aspx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return PageLayout.WebPart_RightColumnHeaderFooterTopRow3Columns;
                    }
                }
            }

            return PageLayout.WebPart_Custom;
        }

    }
}
