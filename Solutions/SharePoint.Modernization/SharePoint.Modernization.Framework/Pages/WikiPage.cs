using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SharePoint.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SharePoint.Modernization.Framework.Pages
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

        private HtmlParser parser;

        #region construction
        public WikiPage(ListItem page, PageTransformation pageTransformation): base(page, pageTransformation)
        {
            this.parser = new HtmlParser();
        }
        #endregion

        /// <summary>
        /// Analyses a wiki page
        /// </summary>
        /// <returns>Information about the analyzed wiki page</returns>
        public Tuple<PageLayout,List<WebPartEntity>> Analyze()
        {
            List<WebPartEntity> webparts = new List<WebPartEntity>();

            //Load the page
            var wikiPageUrl = page[Constants.FileRefField].ToString();
            var wikiPage = cc.Web.GetFileByServerRelativeUrl(wikiPageUrl);
            cc.Load(wikiPage);
            cc.ExecuteQueryRetry();

            // Load wiki content in HTML parser
            var pageContents = page.FieldValues[Constants.WikiField].ToString();
            var htmlDoc = parser.Parse(pageContents);
            var layout = GetLayout(htmlDoc);
            if (string.IsNullOrEmpty(pageContents))
            {
                layout = PageLayout.Wiki_OneColumn;
            }

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
                                // first insert text part (if it was available)
                                if (!string.IsNullOrEmpty(textContent.ToString()))
                                {
                                    order++;
                                    webparts.Add(CreateWikiTextPart(textContent.ToString(), rowCount, colCount, order));
                                    textContent.Clear();
                                }

                                // then process the web part
                                order++;
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
                            else
                            {
                                if (!string.IsNullOrEmpty(node.TextContent.Trim()) && node.TextContent.Trim() == "\n")
                                {
                                    // ignore, this one is typically added after a web part
                                }
                                else
                                {
                                    if (node.HasChildNodes)
                                    {
                                        textContent.AppendLine((node as IHtmlElement).OuterHtml);
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(node.TextContent.Trim()))
                                        {
                                            textContent.AppendLine(node.TextContent.Trim());
                                        }
                                    }
                                }
                            }
                        }

                        // there was only one text part
                        if (!string.IsNullOrEmpty(textContent.ToString()))
                        {
                            // insert text part to the web part collection
                            order++;
                            webparts.Add(CreateWikiTextPart(textContent.ToString(), rowCount, colCount, order));
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
                                Row = webPartToRetrieve.Row,
                                Column = webPartToRetrieve.Column,
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

            // Somehow the wiki was not standard formatted, so lets wrap its contents in a text block
            if (webparts.Count == 0 && !String.IsNullOrEmpty(htmlDoc.Source.Text))
            {
                webparts.Add(CreateWikiTextPart(htmlDoc.Source.Text, 1, 1, 1));
            }

            return new Tuple<PageLayout, List<WebPartEntity>>(layout, webparts);
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

        /// <summary>
        /// Stores text content as a fake web part
        /// </summary>
        /// <param name="wikiTextPartContent">Text to store</param>
        /// <param name="row">Row of the fake web part</param>
        /// <param name="col">Column of the fake web part</param>
        /// <param name="order">Order inside the row/column</param>
        /// <returns>A web part entity to add to the collection</returns>
        private WebPartEntity CreateWikiTextPart(string wikiTextPartContent, int row, int col, int order)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("Text", wikiTextPartContent.Trim().Replace("\r\n", string.Empty));

            return new WebPartEntity()
            {
                Title = "WikiText",
                Type = "SharePointPnP.Modernization.WikiTextPart",
                Id = Guid.Empty,
                Row = row,
                Column = col,
                Order = order,
                Properties = properties,
            };
        }

        /// <summary>
        /// Analyzes the wiki page to determine which layout was used
        /// </summary>
        /// <param name="doc">html object</param>
        /// <returns>Layout of the wiki page</returns>
        private PageLayout GetLayout(IHtmlDocument doc)
        {
            string spanValue = "";
            var spanTags = doc.All.Where(p => p.LocalName == "span" && p.HasAttribute("id"));
            if (spanTags.Any())
            {
                foreach(var span in spanTags)
                {
                    if (span.GetAttribute("id").Equals("layoutsdata", StringComparison.InvariantCultureIgnoreCase))
                    {
                        spanValue = span.InnerHtml.ToLower();

                        if (spanValue == "false,false,1")
                        {
                            return PageLayout.Wiki_OneColumn;
                        }
                        else if (spanValue == "false,false,2")
                        {
                            var tdTag = doc.All.Where(p => p.LocalName == "td" && p.HasAttribute("style")).FirstOrDefault();
                            if (tdTag != null)
                            {
                                if (tdTag.GetAttribute("style").Equals("width:49.95%;", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    return PageLayout.Wiki_TwoColumns;
                                }
                                else if (tdTag.GetAttribute("style").Equals("width:66.6%;", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    return PageLayout.Wiki_TwoColumnsWithSidebar;
                                }
                            }
                        }
                        else if (spanValue == "true,false,2")
                        {
                            return PageLayout.Wiki_TwoColumnsWithHeader;
                        }
                        else if (spanValue == "true,true,2")
                        {
                            return PageLayout.Wiki_TwoColumnsWithHeaderAndFooter;
                        }
                        else if (spanValue == "false,false,3")
                        {
                            return PageLayout.Wiki_ThreeColumns;
                        }
                        else if (spanValue == "true,false,3")
                        {
                            return PageLayout.Wiki_ThreeColumnsWithHeader;
                        }
                        else if (spanValue == "true,true,3")
                        {
                            return PageLayout.Wiki_ThreeColumnsWithHeaderAndFooter;
                        }
                    }
                }
            }

            // Oops, we're still here...let's try to deduct a layout as some pages (e.g. from community template) do not add the proper span value
            if (spanValue.StartsWith("false,false,") || spanValue.StartsWith("true,true,") || spanValue.StartsWith("true,false,"))
            {
                // false,false,&#123;0&#125; case..let's try to count the columns via the TD tag data
                var tdTags = doc.All.Where(p => p.LocalName == "td" && p.HasAttribute("style"));
                if (spanValue.StartsWith("false,false,"))
                {
                    if (tdTags.Count() == 1)
                    {
                        return PageLayout.Wiki_OneColumn;
                    }
                    else if (tdTags.Count() == 2)
                    {
                        if (tdTags.First().GetAttribute("style").Equals("width:49.95%;", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return PageLayout.Wiki_TwoColumns;
                        }
                        else if (tdTags.First().GetAttribute("style").Equals("width:66.6%;", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return PageLayout.Wiki_TwoColumnsWithSidebar;
                        }
                    }
                    else if (tdTags.Count() == 3)
                    {
                        return PageLayout.Wiki_ThreeColumns;
                    }
                }
                else if (spanValue.StartsWith("true,true,"))
                {
                    if (tdTags.Count() == 2)
                    {
                        return PageLayout.Wiki_TwoColumnsWithHeaderAndFooter;
                    }
                    else if (tdTags.Count() == 3)
                    {
                        return PageLayout.Wiki_ThreeColumnsWithHeaderAndFooter;
                    }
                }
                else if (spanValue.StartsWith("true,false,"))
                {
                    if (tdTags.Count() == 2)
                    {
                        return PageLayout.Wiki_TwoColumnsWithHeader;
                    }
                    else if (tdTags.Count() == 3)
                    {
                        return PageLayout.Wiki_ThreeColumnsWithHeader;
                    }
                }
            }

            return PageLayout.Wiki_Custom;
        }

    }
}
