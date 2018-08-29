using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SharePointPnP.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointPnP.Modernization.Framework.Pages
{
    /// <summary>
    /// Analyzes a publishing page
    /// </summary>
    public class PublishingPage : BasePage
    {

        #region Construction
        /// <summary>
        /// Instantiates a publishing page object
        /// </summary>
        /// <param name="page">ListItem holding the page to analyze</param>
        /// <param name="pageTransformation">Page transformation information</param>
        public PublishingPage(ListItem page, PageTransformation pageTransformation) : base(page, pageTransformation)
        {
        }
        #endregion

        /// <summary>
        /// Analyses a publishing page
        /// </summary>
        /// <returns>Information about the analyzed publishing page</returns>
        public Tuple<PageLayout, List<WebPartEntity>> Analyze()
        {
            List<WebPartEntity> webparts = new List<WebPartEntity>();
            PageLayout layout = PageLayout.PublishingPage_Custom;

            //Load the page
            var webPartPageUrl = page[Constants.FileRefField].ToString();
            var webPartPage = cc.Web.GetFileByServerRelativeUrl(webPartPageUrl);

            // Load page properties
            var pageProperties = webPartPage.Properties;
            cc.Load(pageProperties);

            // Load web parts on web part page
            // Note: Web parts placed outside of a web part zone using SPD are not picked up by the web part manager. There's no API that will return those,
            //       only possible option to add parsing of the raw page aspx file.
            var limitedWPManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            cc.Load(limitedWPManager);

            IEnumerable<WebPartDefinition> webParts = cc.LoadQuery(limitedWPManager.WebParts.IncludeWithDefaultProperties(wp => wp.Id, wp => wp.ZoneId, wp => wp.WebPart.ExportMode, wp => wp.WebPart.Title, wp => wp.WebPart.ZoneIndex, wp => wp.WebPart.IsClosed, wp => wp.WebPart.Hidden, wp => wp.WebPart.Properties));
            cc.ExecuteQueryRetry();

            if (webParts.Count() > 0)
            {
                foreach (var foundWebPart in webParts)
                {
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
                        Row = 1,
                        Column = 1,
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
    }
}
