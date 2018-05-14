using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SharePoint.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<WebPartEntity> TransformPlusSplit(List<WebPartEntity> wikiPageWebParts)
        {
            List<WebPartEntity> updatedWebParts = new List<WebPartEntity>(wikiPageWebParts.Count + 10);
            List<WebPartEntity> replacedWebParts = new List<WebPartEntity>(10);

            int lastRow = 1;
            int lastColum = 1;
            int extraWebPartCounter = 1;

            // first ensure there's a big gap in the ordering to allow insertion
            foreach (var wp in wikiPageWebParts)
            {
                wp.Order = wp.Order * 1000;
            }

            // iterate over all parts found on the wiki page
            foreach (var wp in wikiPageWebParts)
            {
                if (wp.Type == WebParts.WikiText)
                {
                    // Parse the html
                    using (var document = this.parser.Parse(wp.Properties["Text"]))
                    {
                        // Check if this text requires special handling due to embedded images or iframes...
                        var images = document.QuerySelectorAll("img");
                        var iframes = document.QuerySelectorAll("iframe");
                        var elementsToHandle = images.Union(iframes);

                        if (!elementsToHandle.Any())
                        {
                            updatedWebParts.Add(wp);
                            continue;
                        }

                        // Right, we've found a wiki text part with images or iframes...
                        lastRow = wp.Row;
                        lastColum = wp.Column;
                        extraWebPartCounter = wp.Order;

                        var elements = document.All;
                        List<INode> nodes = new List<INode>(elements.Count() * 4)
                        {
                            elements.First()
                        };
                        RecurseNodes(elements.First(), ref nodes);

                        var newWikiTextHtmlFragment = document.CreateElement($"div");
                        INode lastElementAdded = null;

                        foreach (var element in nodes)
                        {
                            Dictionary<string, string> props = new Dictionary<string, string>();

                            if (element is IHtmlImageElement)
                            {
                                // Get the Html tree from this element up and add as text part
                                props.Add("Title", "Wiki text");
                                props.Add("Text", newWikiTextHtmlFragment.InnerHtml);

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

                                // Reset the text element 
                                newWikiTextHtmlFragment = document.CreateElement($"div");
                                lastElementAdded = null;

                                // Fill properties
                                props = new Dictionary<string, string>();
                                if ((element as IHtmlImageElement).Source != null)
                                {
                                    props.Add("Title", "Image in wiki text");
                                    props.Add("Description", "");
                                    props.Add("ImageUrl", (element as IHtmlImageElement).Source.Replace("about://", ""));
                                    props.Add("Width", (element as IHtmlImageElement).DisplayWidth.ToString());
                                    props.Add("Height", (element as IHtmlImageElement).DisplayHeight.ToString());
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
                                    Row = lastRow,
                                    Column = lastColum,
                                    Order = extraWebPartCounter,
                                    Properties = props
                                });

                                extraWebPartCounter++;
                            }
                            else if (element is IHtmlInlineFrameElement)
                            {
                                // Get the Html tree from this element up and add as text part
                                props.Add("Title", "Wiki text");
                                props.Add("Text", newWikiTextHtmlFragment.InnerHtml);

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

                                // Reset the text element 
                                newWikiTextHtmlFragment = document.CreateElement($"div");
                                lastElementAdded = null;

                                // Fill properties
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

                                // Add vido part
                                replacedWebParts.Add(new WebPartEntity()
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
                            else
                            {
                                // Store the element in an in memory document as that will be our html fragment living above the image/iframe tag...skip the html head and body we got due to the parser
                                if (!(element is IHtmlHtmlElement || element is IHtmlHeadElement || element is IHtmlBodyElement))
                                {
                                    var clone = element.Clone(false);
                                    
                                    if (lastElementAdded == null)
                                    {
                                        // Our first element being added to the DIV
                                        lastElementAdded = newWikiTextHtmlFragment.AppendChild(clone);                                       
                                    }
                                    else
                                    {
                                        // Set the correct insertion point for the next cloned node based on how the source element is setup
                                        lastElementAdded = SetParentElement(element, lastElementAdded);
                                        // Add the cloned node
                                        lastElementAdded = lastElementAdded.AppendChild(clone);
                                    }
                                }
                            }
                        }

                        if (lastElementAdded != null)
                        {
                            // Process the last piece of Html  
                            Dictionary<string, string> props = new Dictionary<string, string>();
                            props.Add("Title", "Wiki text");
                            props.Add("Text", newWikiTextHtmlFragment.InnerHtml);

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

            return updatedWebParts;
        }

        #region Helper methods
        private IElement SetParentElement(INode sourceElement, INode targetParentElement)
        {
            string sourceElementParentTagName = "";
                        
            if (sourceElement.ParentElement != null)
            {
                sourceElementParentTagName = sourceElement.ParentElement.TagName;
                // Handle this special case since we start with a div and ignore the html, head and body tags
                if (sourceElementParentTagName == "BODY")
                {
                    sourceElementParentTagName = "DIV";
                }
            }            

            // Handling of the other tags
            IElement returnValue = null;
            bool found = false;
            while (!found)
            {
                if (targetParentElement != null)
                {
                    // If we just inserted like an IText then already move up
                    if (!(targetParentElement is IElement))
                    {
                        targetParentElement = targetParentElement.ParentElement;
                    }

                    // Get the level of the element, is needed to correctly find insertion point in case of nested elements of the same type (e.g. DIV's)
                    int sourceLevel = TreeLevel(sourceElement.ParentElement);
                    int targetLevel = TreeLevel((targetParentElement as IElement));

                    // Source level is always one more since source does have HTML->BODY->xx while target has DIV->xx
                    if ((targetParentElement as IElement).TagName == sourceElementParentTagName && (sourceLevel == targetLevel + 1))
                    {
                        returnValue = targetParentElement as IElement;
                        found = true;
                    }
                    else
                    {
                        // Move pointer one up
                        if ((targetParentElement as IElement).ParentElement != null)
                        {
                            targetParentElement = (targetParentElement as IElement).ParentElement;
                        }
                        else
                        {
                            // no parent anymore, so bail out
                            found = true;
                        }
                    }
                }
                else
                {
                    found = true;
                }
            }

            return returnValue;
        }


        private int TreeLevel(IElement element)
        {
            int level = 0;
            var parent = element.ParentElement;

            while (parent != null)
            {
                level++;
                parent = parent.ParentElement;
            }

            return level;
        }

        private void RecurseNodes(IElement parent, ref List<INode> nodes)
        {
            if (parent.ChildNodes.Count() > 0)
            {
                foreach (var node in parent.ChildNodes)
                {
                    if (!(node is IHtmlBreakRowElement))
                    {
                        nodes.Add(node);
                        if ((node is IElement) && node.ChildNodes.Count() > 0)
                        {
                            RecurseNodes((node as IElement), ref nodes);
                        }
                    }
                }
            }
        }
        #endregion

        /** Previous implementation
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
                    var lastWebPart = wikiPageWebParts.OrderByDescending(p => p.Row).ThenByDescending(p => p.Column).ThenByDescending(p => p.Order).FirstOrDefault();
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
        ***/

    }
}
