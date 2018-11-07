using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SharePointPnP.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointPnP.Modernization.Framework.Transform
{
    public class WikiTransformatorSimple
    {
        private HtmlParser parser;

        #region Construction
        /// <summary>
        /// Default constructor
        /// </summary>
        public WikiTransformatorSimple()
        {
            // Instantiate the AngleSharp Html parser
            parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
        }
        #endregion

        /// <summary>
        /// Replaces embedded images and iframes with respective "fake" image and video web parts. Depending on the 
        /// image/iframe position in the html the wiki text is broken up in multiple wiki text parts intermixed 
        /// with image and/or video parts. Later on these web parts will be transformed to client side web parts
        /// </summary>
        /// <param name="wikiPageWebParts">List of web parts on the page</param>
        /// <returns>Updated list of web parts</returns>
        public List<WebPartEntity> TransformPlusSplit(List<WebPartEntity> wikiPageWebParts, bool handleWikiImagesAndVideos)
        {
            List<WebPartEntity> updatedWebParts = new List<WebPartEntity>(wikiPageWebParts.Count + 10);
            List<WebPartEntity> replacedWebParts = new List<WebPartEntity>(10);

            // Counters used for breaking up wiki text and placing images in between the broken wiki text fragments
            int lastRow = 1;
            int lastColum = 1;
            int extraWebPartCounter = 1;

            // first ensure there's a big gap in the ordering to allow insertion
            foreach (var wp in wikiPageWebParts)
            {
                wp.Order = wp.Order * 1000;
            }

            // Counters used for placing web parts at the end of the page (e.g. image was defined inside a table)
            int lastRow2 = 1;
            int lastColum2 = 1;
            int extraWebPartCounter2 = 1;
            int splitCounter = 1;

            var lastWebPart2 = wikiPageWebParts.OrderByDescending(p => p.Row).ThenByDescending(p => p.Column).ThenByDescending(p => p.Order).FirstOrDefault();
            if (lastWebPart2 != null)
            {
                lastRow2 = lastWebPart2.Row;
                lastColum2 = lastWebPart2.Column;
                extraWebPartCounter2 = lastWebPart2.Order + 100;
            }

            // iterate over all parts found on the wiki page
            foreach (var wp in wikiPageWebParts)
            {
                if (wp.Type == WebParts.WikiText)
                {
                    // Reset the replaced web parts list
                    replacedWebParts = new List<WebPartEntity>(10);

                    // Parse the html
                    using (var document = this.parser.Parse(wp.Properties["Text"]))
                    {
                        // Check if this text requires special handling due to embedded images or iframes...
                        var images = document.QuerySelectorAll("img");
                        var iframes = document.QuerySelectorAll("iframe");
                        var elementsToHandle = images.Union(iframes);

                        // No special handling needed, so skip this wiki text part
                        if (!elementsToHandle.Any())
                        {
                            updatedWebParts.Add(wp);
                            continue;
                        }

                        // Right, we've found a wiki text part with images or iframes...
                        lastRow = wp.Row;
                        lastColum = wp.Column;
                        extraWebPartCounter = wp.Order;

                        // Iterate over each each element, need to check each element to ensure we create the 
                        // replacement web parts in the right order
                        foreach (var element in document.All)
                        {
                            Dictionary<string, string> props = new Dictionary<string, string>();

                            // Img elements might require splitting of wiki text
                            if (element is IHtmlImageElement)
                            {
                                bool split = true;

                                // Only split if the image was not living in a table or list
                                if (handleWikiImagesAndVideos && !InUnSplitableElement(element))
                                {
                                    // Get the current html tree from this element up and add as text part
                                    props.Add("Title", "Wiki text");
                                    props.Add("Text", $"SplitPart{splitCounter}");
                                    splitCounter++;

                                    replacedWebParts.Add(new WebPartEntity()
                                    {
                                        Type = WebParts.WikiText,
                                        Title = "Wiki text",
                                        Row = lastRow,
                                        Column = lastColum,
                                        Order = extraWebPartCounter,
                                        Properties = props
                                    });
                                    extraWebPartCounter++;
                                }
                                else
                                {
                                    split = false;
                                }

                                // Check if this image tag is wrapped inside an Anchor
                                string anchorTag = null;
                                if (element.ParentElement != null && element.ParentElement.TagName.Equals("A", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (element.ParentElement.HasAttribute("href"))
                                    {
                                        anchorTag = element.ParentElement.GetAttribute("href");
                                    }
                                }

                                // Fill properties of the image web part
                                props = new Dictionary<string, string>();
                                if ((element as IHtmlImageElement).Source != null)
                                {
                                    props.Add("Title", "Image in wiki text");
                                    props.Add("Description", "");
                                    props.Add("ImageUrl", (element as IHtmlImageElement).Source.Replace("about://", ""));
                                    props.Add("Width", (element as IHtmlImageElement).DisplayWidth.ToString());
                                    props.Add("Height", (element as IHtmlImageElement).DisplayHeight.ToString());
                                    props.Add("Anchor", anchorTag ?? "");
                                }

                                var alt = (element as IElement).Attributes.Where(p => p.Name.Equals("alt", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (alt != null)
                                {
                                    props.Add("AlternativeText", alt.Value.ToString());
                                }

                                // Add image part
                                replacedWebParts.Add(new WebPartEntity()
                                {
                                    Type = WebParts.WikiImage,
                                    Title = "Image in wiki text",
                                    Row = split ? lastRow : lastRow2,
                                    Column = split ? lastColum : lastColum2,
                                    Order = split ? extraWebPartCounter : extraWebPartCounter2,
                                    Properties = props
                                });

                                if (split)
                                {
                                    // replace img or img nested in A with "splitter"
                                    var splitter = document.CreateElement("span");
                                    splitter.ClassName = "split";

                                    if (element.ParentElement != null)
                                    {
                                        if (element.ParentElement.TagName.Equals("A", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            element.ParentElement.ParentElement.ReplaceChild(splitter, element.ParentElement);
                                        }
                                        else
                                        {
                                            element.ParentElement.ReplaceChild(splitter, element);
                                        }
                                    }
                                    extraWebPartCounter++;
                                }
                                else
                                {
                                    extraWebPartCounter2++;
                                }
                            }
                            // IFrame elements might require splitting of wiki text
                            else if (element is IHtmlInlineFrameElement)
                            {
                                bool split = true;
                                // Only split if the iframe was not living in a table or list
                                if (handleWikiImagesAndVideos && !InUnSplitableElement(element))
                                {
                                    // Get the current html tree from this element up and add as text part
                                    props.Add("Title", "Wiki text");
                                    props.Add("Text", $"SplitPart{splitCounter}");
                                    splitCounter++;

                                    replacedWebParts.Add(new WebPartEntity()
                                    {
                                        Type = WebParts.WikiText,
                                        Title = "Wiki text",
                                        Row = lastRow,
                                        Column = lastColum,
                                        Order = extraWebPartCounter,
                                        Properties = props
                                    });
                                    extraWebPartCounter++;
                                }
                                else
                                {
                                    split = false;
                                }

                                // Fill properties of the video web part
                                props = new Dictionary<string, string>();
                                if ((element as IHtmlInlineFrameElement).Source != null)
                                {
                                    props.Add("Title", "Video in wiki text");
                                    props.Add("Description", "");
                                    props.Add("IFrameEmbed", (element as IElement).OuterHtml);
                                    props.Add("Source", (element as IHtmlInlineFrameElement).Source);
                                }

                                var allowFullScreen = (element as IElement).Attributes.Where(p => p.Name.Equals("allowfullscreen", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (allowFullScreen != null)
                                {
                                    bool.TryParse(allowFullScreen.Value, out bool val);
                                    props.Add("AllowFullScreen", val.ToString());
                                }
                                var size = (element as IElement).Attributes.Where(p => p.Name.Equals("width", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (size != null)
                                {
                                    props.Add("Width", size.Value.ToString());
                                }
                                size = (element as IElement).Attributes.Where(p => p.Name.Equals("height", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (size != null)
                                {
                                    props.Add("Height", size.Value.ToString());
                                }

                                // Add video part
                                replacedWebParts.Add(new WebPartEntity()
                                {
                                    Type = WebParts.WikiVideo,
                                    Title = "Video in wiki text",
                                    Row = split ? lastRow : lastRow2,
                                    Column = split ? lastColum : lastColum2,
                                    Order = split ? extraWebPartCounter : extraWebPartCounter2,
                                    Properties = props
                                });

                                if (split)
                                {
                                    // replace img or img nested in A with "splitter"
                                    var splitter = document.CreateElement("span");
                                    splitter.ClassName = "split";

                                    if (element.ParentElement != null)
                                    {
                                        element.ParentElement.ReplaceChild(splitter, element);
                                    }
                                    extraWebPartCounter++;
                                }
                                else
                                {
                                    extraWebPartCounter2++;
                                }
                            }
                        }

                        Dictionary<string, string> props2 = new Dictionary<string, string>();
                        props2.Add("Title", "Wiki text");
                        props2.Add("Text", $"SplitPart{splitCounter}");
                        splitCounter++;

                        replacedWebParts.Add(new WebPartEntity()
                        {
                            Type = WebParts.WikiText,
                            Title = "Wiki text",
                            Row = lastRow,
                            Column = lastColum,
                            Order = extraWebPartCounter,
                            Properties = props2
                        });
                        extraWebPartCounter++;

                        // Fix up WikiText parts
                        // Step 1: get the html now that we've replaced img/iframe tags with a span
                        string preppedWikiText = "";
                        if (document.DocumentElement.Children.Count() > 1)
                        {
                            preppedWikiText = document.DocumentElement.Children[1].InnerHtml;
                        }
                        // Step 2: split the html text in parts based upon the span we added
                        string[] splitText = preppedWikiText.Split(new string[] { "<span class=\"split\"></span>" }, StringSplitOptions.RemoveEmptyEntries);

                        // Step 3: use AngleSharp to construct valid html from each part and link it back to the WikiText placeholder web part
                        foreach(var replacedWebPart in replacedWebParts.ToList())
                        {
                            if (replacedWebPart.Type == WebParts.WikiText)
                            {
                                if (Int32.TryParse(replacedWebPart.Properties["Text"].Replace("SplitPart", ""), out int index))
                                {
                                    index = index - 1;

                                    if (splitText.Length >= index + 1)
                                    {
                                        using (var docTemp = parser.Parse(splitText[index]))
                                        {
                                            if (docTemp.DocumentElement.Children.Count() > 1)
                                            {
                                                // Remove empty DIV's as that's a net result of the splitting
                                                StripEmptyDivAndPfromHtmlTree(docTemp.DocumentElement.Children[1]);

                                                string updatedText = docTemp.DocumentElement.Children[1].InnerHtml;
                                                replacedWebPart.Properties["Text"] = updatedText;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // no text part for this web part, so delete it. This happens when there was no content anymore below the last img/iframe tag
                                        replacedWebParts.Remove(replacedWebPart);
                                    }
                                }
                            }
                        }

                        // reset counter for next wiki zone
                        splitCounter = 1;

                        // Insert into updated web parts list
                        updatedWebParts.AddRange(replacedWebParts);
                    }
                }
                else
                {
                    // Not a text editor web part, so we simply retain it
                    updatedWebParts.Add(wp);
                }
            }

            // Return the collection of "split" web parts
            return updatedWebParts;
        }

        private void StripEmptyDivAndPfromHtmlTree(IElement newWikiTextHtmlFragment)
        {
            var divs = newWikiTextHtmlFragment.QuerySelectorAll("div");
            var ps = newWikiTextHtmlFragment.QuerySelectorAll("p");
            var list = divs.Union(ps);


            if (list.Any())
            {
                foreach (var el in list)
                {
                    if (!el.HasChildNodes)
                    {
                        el.Remove();
                    }
                }
            }
        }

        private bool InUnSplitableElement(INode node)
        {
            IElement start = null;

            if (!(node is IElement))
            {
                start = node.ParentElement;
            }
            else
            {
                start = node as IElement;
            }

            bool unSplitableElementFound = false;

            while (!unSplitableElementFound)
            {
                if (start.TagName == "TD" || start.TagName == "TR" || start.TagName == "TBODY" || // table
                    start.TagName == "LI" || start.TagName == "UL" || start.TagName == "OL") // lists
                {
                    return true;
                }
                else
                {
                    start = start.ParentElement;
                }

                if (start == null)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
