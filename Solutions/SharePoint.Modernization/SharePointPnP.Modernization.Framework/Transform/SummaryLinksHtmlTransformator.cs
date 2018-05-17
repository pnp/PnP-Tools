using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System.Linq;

namespace SharePointPnP.Modernization.Framework.Transform
{
    /// <summary>
    /// This class is used to rewrite the html coming out of the SummaryLinks web part
    /// </summary>
    public class SummaryLinksHtmlTransformator: IHtmlTransformator
    {
        private HtmlParser parser;

        #region Construction
        /// <summary>
        /// Default constructor
        /// </summary>
        public SummaryLinksHtmlTransformator()
        {
            // Instantiate the AngleSharp Html parser
            parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
        }
        #endregion


        /// <summary>
        /// Transforms the passed summarylinks html to be usable by the client side text part
        /// </summary>
        /// <param name="text">Summarylinks html to be transformed</param>
        /// <returns>Html that can be used and edited via the client side text part</returns>
        public string Transform(string text, bool usePlaceHolder)
        {            
            using (var document = this.parser.Parse(text))
            {
                using (var newDocument = this.parser.Parse(""))
                {
                    // Iterate over the divs
                    var divs = document.QuerySelectorAll("div").Where(p => p.GetAttribute("title") == "_link");
                    IElement header = null;
                    IElement list = null;
                    foreach (var div in divs)
                    {
                        var summaryLinkHeader = div.Children.Where(p => p.GetAttribute("title") == "_groupstyle").FirstOrDefault();
                        if (summaryLinkHeader != null)
                        {
                            // Header
                            header = newDocument.CreateElement("div");
                            var strong = newDocument.CreateElement("strong");
                            var title = div.Children.Where(p => p.GetAttribute("title") == "_title").FirstOrDefault();
                            if (title != null)
                            {
                                strong.TextContent = title.TextContent;
                            }
                            header.AppendChild(strong);
                            newDocument.DocumentElement.Children[1].AppendChild(header);

                            // reset list
                            list = null;
                        }
                        else
                        {
                            // Link
                            if (list == null)
                            {
                                list = newDocument.CreateElement("ul");
                                header.AppendChild(list);
                            }


                            // Link
                            var title = div.Children.Where(p => p.GetAttribute("title") == "_title").FirstOrDefault();
                            var linkUrl = div.Children.Where(p => p.GetAttribute("title") == "_linkurl").FirstOrDefault();
                            var openInNewWindow = div.Children.Where(p => p.GetAttribute("title") == "_openinnewwindow").FirstOrDefault();

                            if (linkUrl != null && title != null)
                            {
                                // ListItem
                                var item = newDocument.CreateElement("li");

                                var link = newDocument.CreateElement("a");
                                var href = linkUrl.Children.Where(p => p.HasAttribute("href")).FirstOrDefault();
                                link.SetAttribute("href", href != null ? href.GetAttribute("href") : "");
                                link.TextContent = title.TextContent;

                                if (openInNewWindow != null)
                                {
                                    if (bool.TryParse(openInNewWindow.TextContent, out bool openInNewWindowValue))
                                    {
                                        if (openInNewWindowValue)
                                        {
                                            link.SetAttribute("target", "_blank");
                                            link.SetAttribute("data-interception", "off");
                                        }
                                    }
                                }

                                item.AppendChild(link);
                                list.AppendChild(item);
                            }
                        }
                    }

                    // Return the transformed html
                    if (newDocument.DocumentElement.Children.Count() > 1)
                    {
                        return newDocument.DocumentElement.Children[1].InnerHtml;
                    }
                    else
                    {
                        return text;
                    }
                }
            }
        }
    }
}
