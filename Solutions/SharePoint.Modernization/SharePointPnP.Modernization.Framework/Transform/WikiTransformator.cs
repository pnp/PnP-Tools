using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using SharePointPnP.Modernization.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointPnP.Modernization.Framework.Transform
{
    /// <summary>
    /// Transforms images/iframe elements embedded in wiki text into separate "fake" web parts. Depending on the 
    /// image/iframe position in the html the wiki text is broken up in multiple wiki text parts intermixed 
    /// with image and/or video parts
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
        /// Replaces embedded images and iframes with respective "fake" image and video web parts. Depending on the 
        /// image/iframe position in the html the wiki text is broken up in multiple wiki text parts intermixed 
        /// with image and/or video parts. Later on these web parts will be transformed to client side web parts
        /// </summary>
        /// <param name="wikiPageWebParts">List of web parts on the page</param>
        /// <returns>Updated list of web parts</returns>
        public List<WebPartEntity> TransformPlusSplit(List<WebPartEntity> wikiPageWebParts)
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

                        // We need to iterate all nodes, not just the elements, so build up a list of all nodes
                        var elements = document.All;
                        List<INode> nodes = new List<INode>(elements.Count() * 4)
                        {
                            elements.First()
                        };
                        RecurseNodes(elements.First(), ref nodes);

                        // Create home for the html that we'll process
                        var newWikiTextHtmlFragment = document.CreateElement($"div");
                        INode lastElementAdded = null;

                        // Iterate over each node in the html
                        foreach (var element in nodes)
                        {
                            Dictionary<string, string> props = new Dictionary<string, string>();

                            // Img elements might require splitting of wiki text
                            if (element is IHtmlImageElement)
                            {
                                bool split = true;

                                // Only split if the image was not living in a table or list
                                if (!InUnSplitableElement(element))
                                {
                                    // Remove empty DIV's as that's a net result of the splitting
                                    StripEmptyDivAndPfromHtmlTree(newWikiTextHtmlFragment);
                                                                       
                                    // Get the current html tree from this element up and add as text part
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

                                    // Do we need to add the current element's parent element tree again? 
                                    // This is important to correctly be able to parent siblings at the level of this image element
                                    if ((element as IHtmlImageElement).NextElementSibling != null)
                                    {
                                        lastElementAdded = RebuildElementTree(newWikiTextHtmlFragment, lastElementAdded, element);
                                    }
                                }
                                else
                                {
                                    split = false;
                                    lastElementAdded = AddNodeToHtmlTree(newWikiTextHtmlFragment, lastElementAdded, element);
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
                                if (!InUnSplitableElement(element))
                                {
                                    // Remove empty DIV's as that's a net result of the splitting
                                    StripEmptyDivAndPfromHtmlTree(newWikiTextHtmlFragment);

                                    // Get the current html tree from this element up and add as text part
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

                                    // Do we need to add the current element's parent element tree again? 
                                    // This is important to correctly be able to parent siblings at the level of this iframe element
                                    if ((element as IHtmlInlineFrameElement).NextElementSibling != null)
                                    {
                                        lastElementAdded = RebuildElementTree(newWikiTextHtmlFragment, lastElementAdded, element);
                                    }
                                }
                                else
                                {
                                    split = false;
                                    lastElementAdded = AddNodeToHtmlTree(newWikiTextHtmlFragment, lastElementAdded, element);
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
                                    extraWebPartCounter++;
                                }
                                else
                                {
                                    extraWebPartCounter2++;
                                }
                            }
                            else
                            {
                                // Store the element in an in memory document as that will be our html fragment living above the image/iframe tag...skip the html head and body we got due to the parser
                                if (!(element is IHtmlHtmlElement || element is IHtmlHeadElement || element is IHtmlBodyElement))
                                {
                                    lastElementAdded = AddNodeToHtmlTree(newWikiTextHtmlFragment, lastElementAdded, element);
                                }
                            }
                        }

                        if (lastElementAdded != null)
                        {
                            // Process the last piece of Html (the html positioned underneath the last image/iframe tag  
                            Dictionary<string, string> props = new Dictionary<string, string>();

                            // Remove empty DIV's as that's a net result of the splitting
                            StripEmptyDivAndPfromHtmlTree(newWikiTextHtmlFragment);

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

            // Return the collection of "split" web parts
            return updatedWebParts;
        }


        #region Helper methods
        private INode RebuildElementTree(IElement newWikiTextHtmlFragment, INode lastElementAdded, INode element)
        {
            List<IElement> elementsToAdd = new List<IElement>();
            bool notBody = true;
            IElement e = element.ParentElement;
            while (notBody)
            {
                // Insert as first as we want to walk this from top to bottom
                elementsToAdd.Insert(0, e);
                if (e.ParentElement != null && !(e.ParentElement is IHtmlBodyElement))
                {
                    e = e.ParentElement;
                }
                else
                {
                    notBody = false;
                }
            }

            foreach (var e1 in elementsToAdd)
            {
                lastElementAdded = AddNodeToHtmlTree(newWikiTextHtmlFragment, lastElementAdded, e1);
            }

            return lastElementAdded;
        }

        private void StripEmptyDivAndPfromHtmlTree(IElement newWikiTextHtmlFragment)
        {
            var divs = newWikiTextHtmlFragment.QuerySelectorAll("div");
            var ps = newWikiTextHtmlFragment.QuerySelectorAll("p");
            var list = divs.Union(ps);


            if (list.Any())
            {
                foreach(var el in list)
                {
                    if (!el.HasChildNodes)
                    {
                        el.Remove();
                    }
                }
            }
        }

        private INode AddNodeToHtmlTree(IElement newWikiTextHtmlFragment, INode lastElementAdded, INode element)
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
                if (lastElementAdded != null)
                {
                    lastElementAdded = lastElementAdded.AppendChild(clone);
                }
                else
                {
                    // We should not end up here that often unless for some empty nodes (carriage returns) at the very end
                    lastElementAdded = newWikiTextHtmlFragment.AppendChild(clone);
                }
            }

            return lastElementAdded;
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

            while(!unSplitableElementFound)
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
                    nodes.Add(node);
                    if ((node is IElement) && node.ChildNodes.Count() > 0)
                    {
                        RecurseNodes((node as IElement), ref nodes);
                    }
                }
            }
        }
        #endregion
    }
}
