using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SharePoint.Visio.Scanner.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Visio.Scanner.Pages
{
    /// <summary>
    /// Analyzes a web part page
    /// </summary>
    public class WebPartPage: BasePage
    {
        private const string FileRefField = "FileRef";

        #region construction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public WebPartPage(ListItem page) : base(page)
        {
        }
        #endregion

        /// <summary>
        /// Analyses a webpart page
        /// </summary>
        /// <returns>Information about the analyzed webpart page</returns>
        public List<WebPartEntity> Analyze()
        {
            List<WebPartEntity> webparts = new List<WebPartEntity>();

            //Load the page
            var wikiPageUrl = page[FileRefField].ToString();
            var wikiPage = cc.Web.GetFileByServerRelativeUrl(wikiPageUrl);

            // Load page properties
            var pageProperties = wikiPage.Properties;
            cc.Load(pageProperties);

            // Load web parts on wiki page
            var limitedWPManager = wikiPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            cc.Load(limitedWPManager);

            IEnumerable<WebPartDefinition> webParts = cc.LoadQuery(limitedWPManager.WebParts.IncludeWithDefaultProperties(wp => wp.Id, wp => wp.ZoneId, wp => wp.WebPart.ExportMode, wp => wp.WebPart.Title, wp => wp.WebPart.ZoneIndex, wp => wp.WebPart.IsClosed, wp => wp.WebPart.Hidden, wp => wp.WebPart.Properties));
            cc.ExecuteQueryRetry();

            if (webParts.Count() > 0)
            {
                foreach (var foundWebPart in webParts)
                {
                    // Skip Microsoft.SharePoint.WebPartPages.TitleBarWebPart webpart in TitleBar zone
                    if (foundWebPart.ZoneId.Equals("TitleBar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
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
                        Order = foundWebPart.WebPart.ZoneIndex,
                        ZoneId = foundWebPart.ZoneId,
                        ZoneIndex = (uint)foundWebPart.WebPart.ZoneIndex,
                        IsClosed = foundWebPart.WebPart.IsClosed,
                        Hidden = foundWebPart.WebPart.Hidden,
                        Properties = Properties(foundWebPart.WebPart.Properties, webPartType, webPartXml),
                    });
                }
            }

            return webparts;
        }
    }
}
