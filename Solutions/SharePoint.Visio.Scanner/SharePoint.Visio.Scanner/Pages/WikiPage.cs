using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SharePoint.Visio.Scanner.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SharePoint.Visio.Scanner.Pages
{

    /// <summary>
    /// Analyzes a wiki page
    /// </summary>
    public class WikiPage: BasePage
    {

        private class WebPartPlaceHolder
        {
            public string Id { get; set; }
            public string ControlId { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
            public int Order { get; set; }
        }

        private const string FileRefField = "FileRef";
        private const string WikiField = "WikiField";
        private HtmlParser parser;

        #region construction
        public WikiPage(ListItem page): base(page)
        {
            this.parser = new HtmlParser();
        }
        #endregion

        /// <summary>
        /// Analyses a wiki page
        /// </summary>
        /// <returns>Information about the analyzed wiki page</returns>
        public List<WebPartEntity> Analyze()
        {
            List<WebPartEntity> webparts = new List<WebPartEntity>();

            //Load the page
            var wikiPageUrl = page[FileRefField].ToString();
            var wikiPage = cc.Web.GetFileByServerRelativeUrl(wikiPageUrl);
            cc.Load(wikiPage);
            cc.ExecuteQueryRetry();

            // Load wiki content in HTML parser
            var pageContents = page.FieldValues[WikiField].ToString();
            var htmlDoc = parser.Parse(pageContents);

            List<WebPartPlaceHolder> webPartsToRetrieve = new List<WebPartPlaceHolder>();

            var rows = htmlDoc.All.Where(p => p.LocalName == "tr");
            int rowCount = 0;

            foreach (var row in rows)
            {
                rowCount++;
                var columns = row.Children.Where(p => p.LocalName == "td" && p.Parent == row);

                int colCount = 0;
                foreach (var column in columns)
                {
                    colCount++;
                    var contentHost = column.Children.Where(p => p.LocalName == "div" && (p.ClassName != null && p.ClassName.Equals("ms-rte-layoutszone-outer", StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault();
                    if (contentHost != null)
                    {
                        var content = contentHost.FirstElementChild;
                        StringBuilder textContent = new StringBuilder();
                        int order = 0;
                        foreach (var node in content.ChildNodes)
                        {
                            // Do we find a web part inside...
                            if (((node as IHtmlElement) != null) && ContainsWebPart(node as IHtmlElement))
                            {
                                Regex regexClientIds = new Regex(@"id=\""div_(?<ControlId>(\w|\-)+)");
                                if (regexClientIds.IsMatch((node as IHtmlElement).OuterHtml))
                                {
                                    foreach (Match webPartMatch in regexClientIds.Matches((node as IHtmlElement).OuterHtml))
                                    {
                                        // Store the web part we need, will be retrieved afterwards to optimize performance
                                        string serverSideControlId = webPartMatch.Groups["ControlId"].Value;
                                        var serverSideControlIdToSearchFor = $"g_{serverSideControlId.Replace("-", "_")}";
                                        webPartsToRetrieve.Add(new WebPartPlaceHolder() { ControlId = serverSideControlIdToSearchFor, Id = serverSideControlId, Row = rowCount, Column = colCount, Order = order });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Bulk load the needed web part information
            if (webPartsToRetrieve.Count > 0)
            {                
                // Load web parts on wiki page
                var limitedWPManager = wikiPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
                cc.Load(limitedWPManager);
                var webParts = limitedWPManager.WebParts;
                cc.Load(webParts, p => p.Include(wp => wp.Id, wp => wp.WebPart.ExportMode, wp => wp.WebPart.Title, wp => wp.WebPart.ZoneIndex, wp => wp.WebPart.IsClosed, wp => wp.WebPart.Hidden, wp => wp.WebPart.Properties));
                cc.ExecuteQueryRetry();

                foreach (var webPartToRetrieve in webPartsToRetrieve)
                {
                    try
                    {
                        // Check if the web part was loaded when we loaded the web parts collection via the web part manager
                        Guid webPartToRetrieveGuid;
                        if (!Guid.TryParse(webPartToRetrieve.Id, out webPartToRetrieveGuid))
                        {
                            // Skip since guid is not valid
                            continue;
                        }

                        var foundWebPart = webParts.Where(p => p.Id.Equals(webPartToRetrieveGuid)).FirstOrDefault();
                        if (foundWebPart == null)
                        {
                            // If not found then try to retrieve the webpart via the GetByControlId method
                            foundWebPart = limitedWPManager.WebParts.GetByControlId(webPartToRetrieve.ControlId);
                            cc.Load(foundWebPart, wp => wp.Id, wp => wp.WebPart.ExportMode, wp => wp.WebPart.Title, wp => wp.WebPart.ZoneIndex, wp => wp.WebPart.IsClosed, wp => wp.WebPart.Hidden, wp => wp.WebPart.Properties);
                            cc.ExecuteQueryRetry();
                        }

                        if (foundWebPart != null)
                        {
                            // Retry to load the properties, sometimes they're not retrieved
                            foundWebPart.EnsureProperties(wp => wp.Id, wp => wp.WebPart.ExportMode, wp => wp.WebPart.Title, wp => wp.WebPart.ZoneIndex, wp => wp.WebPart.IsClosed, wp => wp.WebPart.Hidden, wp => wp.WebPart.Properties);

                            //var changed = false;
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
                                ServerControlId = webPartToRetrieve.Id,
                                Order = webPartToRetrieve.Order,
                                ZoneId = "",
                                ZoneIndex = (uint)foundWebPart.WebPart.ZoneIndex,
                                IsClosed = foundWebPart.WebPart.IsClosed,
                                Hidden = foundWebPart.WebPart.Hidden,
                                Properties = Properties(foundWebPart.WebPart.Properties, webPartType, webPartXml),
                            });
                        }
                    }
                    catch (ServerException)
                    {
                        //Eat exception because we've found a WebPart ID which is not available on the server-side
                    }
                }
            }

            return webparts;
        }

        /// <summary>
        /// Does the tree of nodes somewhere contain a web part?
        /// </summary>
        /// <param name="element">Html content to analyze</param>
        /// <returns>True if it contains a web part</returns>
        private bool ContainsWebPart(IHtmlElement element)
        {
            var doc = parser.Parse(element.OuterHtml);
            var nodes = doc.All.Where(p => p.LocalName == "div");
            foreach (var node in nodes)
            {
                if (((node as IHtmlElement) != null) && (node as IHtmlElement).ClassList.Contains("ms-rte-wpbox"))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
