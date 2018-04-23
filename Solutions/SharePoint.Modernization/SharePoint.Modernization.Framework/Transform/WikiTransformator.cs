using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SharePoint.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Transform
{
    /// <summary>
    /// Transforms images/iframe elements embedded in wiki text into separate "fake" web parts
    /// </summary>
    public class WikiTransformator
    {
        private HtmlParser parser;

        #region Construction
        /// <summary>
        /// Default constructor
        /// </summary>
        public WikiTransformator()
        {
            // Instantiate the AngleSharp Html parser
            parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
        }
        #endregion

        /// <summary>
        /// Replaces embedded images and iframes with respective "fake" image and video web parts. Later on these web parts will be transformed to client side web parts
        /// </summary>
        /// <param name="wikiPageWebParts"></param>
        /// <returns></returns>
        public List<WebPartEntity> Transform(List<WebPartEntity> wikiPageWebParts)
        {
            List<WebPartEntity> updatedWebParts = new List<WebPartEntity>(wikiPageWebParts.Count + 10);

            // Find last row and column
            int lastRow = 1;
            int lastColum = 1;
            int extraWebPartCounter = 1;
            var lastWebPart = wikiPageWebParts.OrderByDescending(p => p.Row).OrderByDescending(p => p.Column).OrderByDescending(p => p.Order).FirstOrDefault();
            if (lastWebPart != null)
            {
                lastRow = lastWebPart.Row;
                lastColum = lastWebPart.Column;
                extraWebPartCounter = lastWebPart.Order + 10;
            }
            
            foreach(var wp in wikiPageWebParts)
            {
                updatedWebParts.Add(wp);

                if (wp.Type == WebParts.WikiText)
                {
                    // Parse the html
                    using (var document = this.parser.Parse(wp.Properties["Text"]))
                    {                        
                        var images = document.QuerySelectorAll("img");
                        var iframes = document.QuerySelectorAll("iframe");
                        var elements = images.Union(iframes);

                        foreach(var element in elements)
                        {
                            Dictionary<string, string> props = new Dictionary<string, string>();

                            if (element is IHtmlImageElement)
                            {
                                // Fill properties
                                if ((element as IHtmlImageElement).Source != null)
                                {
                                    props.Add("Title", "Image in wiki text");
                                    props.Add("Description", "");
                                    props.Add("ImageUrl", (element as IHtmlImageElement).Source.Replace("about://", ""));
                                    props.Add("Width", (element as IHtmlImageElement).DisplayWidth.ToString());
                                    props.Add("Height", (element as IHtmlImageElement).DisplayHeight.ToString());
                                }

                                var alt = element.Attributes.Where(p => p.Name.Equals("alt", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (alt != null)
                                {
                                    props.Add("AlternativeText", alt.Value.ToString());
                                }

                                // Add image part
                                updatedWebParts.Add(new WebPartEntity()
                                {
                                    Type = WebParts.WikiImage,
                                    Title = "Image in wiki text",
                                    Row = lastRow,
                                    Column = lastColum,
                                    Order = extraWebPartCounter,
                                    Properties = props
                                });

                                extraWebPartCounter++;
                            }
                            else if (element is IHtmlInlineFrameElement)
                            {
                                // Fill properties
                                if ((element as IHtmlInlineFrameElement).Source != null)
                                {
                                    props.Add("Title", "Video in wiki text");
                                    props.Add("Description", "");
                                    props.Add("IFrameEmbed", element.OuterHtml);
                                    props.Add("Source", (element as IHtmlInlineFrameElement).Source);
                                }

                                var allowFullScreen = element.Attributes.Where(p => p.Name.Equals("allowfullscreen", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (allowFullScreen != null)
                                {
                                    bool.TryParse(allowFullScreen.Value, out bool val);
                                    props.Add("AllowFullScreen", val.ToString());
                                }
                                var size = element.Attributes.Where(p => p.Name.Equals("width", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (size != null)
                                {
                                    props.Add("Width", size.Value.ToString());
                                }
                                size = element.Attributes.Where(p => p.Name.Equals("height", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (size != null)
                                {
                                    props.Add("Height", size.Value.ToString());
                                }

                                // Add vido part
                                updatedWebParts.Add(new WebPartEntity()
                                {
                                    Type = WebParts.WikiVideo,
                                    Title = "Video in wiki text",
                                    Row = lastRow,
                                    Column = lastColum,
                                    Order = extraWebPartCounter,
                                    Properties = props
                                });

                                extraWebPartCounter++;
                            }
                        }
                    }
                }
            }

            return updatedWebParts;
        }

    }
}
