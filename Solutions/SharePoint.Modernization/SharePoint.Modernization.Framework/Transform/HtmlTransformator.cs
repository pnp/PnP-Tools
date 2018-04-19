using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;

namespace SharePoint.Modernization.Framework.Transform
{
    /// <summary>
    /// Transforms the received Html in html that can be displayed and maintained in the modern client side text part
    /// </summary>
    public class HtmlTransformator : IHtmlTransformator
    {
        private HtmlParser parser;

        public HtmlTransformator()
        {
            // Instantiate the AngleSharp Html parser
            parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
        }


        /// <summary>
        /// Transforms the passed html to be usable by the client side text part
        /// </summary>
        /// <param name="text">Html to be transformed</param>
        /// <returns>Html that can be used and edited via the client side text part</returns>
        public string Transform(string text)
        {
            using (var document = this.parser.Parse(text))
            {
                // Drop all <BR>
                DropBRs(document);

                // Process headings: RTE does h2, h3, h4 while wiki does h1, h2, h3, h4
                TransformHeadings(document, 4, 5);
                TransformHeadings(document, 3, 4);
                TransformHeadings(document, 2, 3);
                TransformHeadings(document, 1, 2);

                // Process spans
                TransformSpans(document.QuerySelectorAll("span"), document);

                // Process blockquotes
                TransformBlockQuotes(document.QuerySelectorAll("blockquote"), document);

                // Return the transformed html
                if (document.DocumentElement.Children.Count() > 1)
                {
                    return document.DocumentElement.Children[1].InnerHtml;
                }
                else
                {
                    return text;
                }
            }
        }

        protected virtual void TransformBlockQuotes(IHtmlCollection<IElement> blockQuotes, IHtmlDocument document)
        {
            int level = 1;
            INode blockParent = null;

            foreach (var blockQuote in blockQuotes)
            {
                var parent = blockQuote.Parent;                
                if (blockQuote.OuterHtml.ToLower().Contains("margin:0px 0px 0px 40px"))
                {
                    if (blockQuote.ChildElementCount > 0 && blockQuote.Children[0].TagName.ToLower() == "blockquote")
                    {
                        blockParent = blockQuote;
                        level++;
                    }
                    else
                    {
                        var newElement = document.CreateElement($"p");
                        // Drop P
                        // TODO: do this in a better way
                        newElement.InnerHtml = blockQuote.InnerHtml.Replace("<p>", "").Replace("</p>", "");
                        newElement.SetAttribute($"style", $"margin-left:{level * 40}px;");

                        switch (level)
                        {
                            case 1:
                                {
                                    parent.ReplaceChild(newElement, blockQuote);
                                    break;
                                }
                            case 2:
                                {
                                    blockParent.Parent.ReplaceChild(newElement, blockParent);
                                    break;
                                }
                            case 3:
                                {
                                    blockParent.Parent.ReplaceChild(newElement, blockParent);
                                    break;
                                }
                            case 4:
                                {
                                    blockParent.Parent.ReplaceChild(newElement, blockParent);
                                    break;
                                }
                        }
                        
                        level = 1;
                    }
                }
            }
        }

        protected virtual void DropBRs(IHtmlDocument document)
        {
            var brNodes = document.QuerySelectorAll($"BR");

            foreach(var brNode in brNodes)
            {
                var parent = brNode.Parent;
                parent.RemoveChild(brNode);
            }
        }

        protected virtual void TransformHeadings(IHtmlDocument document, int from, int to)
        {
            var fromNodes = document.QuerySelectorAll($"h{from}");
            foreach(var fromNode in fromNodes)
            {
                var parent = fromNode.Parent;

                if (to == 5)
                {
                    ReplaceChildElementByText(parent, fromNode, document);
                }
                else
                {
                    var newElement = document.CreateElement($"h{to}");
                    newElement.TextContent = fromNode.TextContent;
                    parent.ReplaceChild(newElement, fromNode);
                }
            }
        }

        protected virtual void TransformSpans(IHtmlCollection<IElement> spans, IHtmlDocument document)
        {
            foreach(var span in spans)
            {
                var parent = span.Parent;

                // rewrite normal style
                // <span class="ms-rteStyle-Normal">Norm</span>
                if (span.ClassName != null && span.ClassName.ToLower().Contains("ms-rtestyle-normal"))
                {
                    ReplaceChildElementByText(parent, span, document);
                    continue;
                }

                // rewrite striked
                // <span style="text-decoration&#58;line-through;">striked</span>
                if (span.OuterHtml.ToLower().Contains("text-decoration:line-through;"))
                {
                    var newElement = document.CreateElement("s");
                    newElement.TextContent = span.InnerHtml;
                    parent.ReplaceChild(newElement, span);
                    continue;
                }

                // rewrite underline
                // <span style="text-decoration&#58;underline;">underline</span>
                if (span.OuterHtml.ToLower().Contains("text-decoration:underline;"))
                {
                    var newElement = document.CreateElement("u");
                    newElement.TextContent = span.InnerHtml;
                    parent.ReplaceChild(newElement, span);
                    continue;
                }

                // ================================
                // rewrite colors
                // ================================
                // <span class="ms-rteThemeForeColor-5-0">red</span>
                if (span.ClassName != null && (span.ClassName.ToLower().StartsWith("ms-rtethemeforecolor-") || span.ClassName.ToLower().StartsWith("ms-rtethemebackcolor-")))
                {
                    // Modern Theme colors
                    // Darker, Dark, Dark Alternate, Primary, Secondary
                    // Neutral Tertiary, Neutral Secondary, Primary alternate, Neutral primary, Neutral Dark

                    // For now drop the color span
                    ReplaceChildElementByText(parent, span, document);
                    continue;
                }

                //<span class="ms-rteForeColor-2" style="">Red,&#160;</span>
                if (span.ClassName != null && span.ClassName.ToLower().StartsWith("ms-rteforecolor-"))
                {
                    // Modern Theme colors
                    // Dark Red, Red, Orange, Yellow, Light green
                    // Green, Light Blue, Blue, Dark Blue, Purple

                    string newClass = null;
                    int colorCode = 0;
                    if (int.TryParse(span.ClassName.ToLower().Replace("ms-rteforecolor-", ""), out colorCode))
                    {                        
                        string colorName = ColorCodeToName(colorCode);
                        if (!string.IsNullOrEmpty(colorName))
                        {
                            newClass = $"fontColor{colorName}";
                        }
                    }

                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        span.ClassName = newClass;
                        continue;
                    }
                    else
                    {
                        // Let's go to default...meaning drop color info
                        ReplaceChildElementByText(parent, span, document);
                        continue;
                    }
                }

                if (span.ClassName != null && span.ClassName.ToLower().StartsWith("ms-rtebackcolor-"))
                {
                    // Modern Theme colors
                    // Dark Red, Red, Orange, Yellow, Light green
                    // Green, Light Blue, Blue, Dark Blue, Purple

                    string newClass = null;
                    int colorCode = 0;
                    if (int.TryParse(span.ClassName.ToLower().Replace("ms-rtebackcolor-", ""), out colorCode))
                    {
                        string colorName = ColorCodeToBackGroundColorName(colorCode);
                        if (!string.IsNullOrEmpty(colorName))
                        {
                            newClass = $"highlightColor{colorName}";
                        }
                    }

                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        span.ClassName = newClass;
                        continue;
                    }
                    else
                    {
                        // Let's go to default...meaning drop color info
                        ReplaceChildElementByText(parent, span, document);
                        continue;
                    }
                }

                // ================================
                // rewrite font size
                // ================================
                if (span.ClassName != null && span.ClassName.ToLower().StartsWith("ms-rtefontsize-"))
                {
                    // Modern Theme colors
                    // Dark Red, Red, Orange, Yellow, Light green
                    // Green, Light Blue, Blue, Dark Blue, Purple

                    string newClass = null;
                    int fontsizeCode = 0;
                    if (int.TryParse(span.ClassName.ToLower().Replace("ms-rtefontsize-", ""), out fontsizeCode))
                    {
                        string fontSize = FontCodeToName(fontsizeCode);
                        if (!string.IsNullOrEmpty(fontSize))
                        {
                            newClass = $"fontSize{fontSize}";
                        }
                    }

                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        span.ClassName = newClass;
                        continue;
                    }
                    else
                    {
                        // Let's go to default...meaning font size info
                        ReplaceChildElementByText(parent, span, document);
                        continue;
                    }
                }
            }
        }

        public static string FontCodeToName(int fontCode)
        {
            switch (fontCode)
            {
                case 1: //8pt
                    {
                        return "Small";
                    }
                case 2: //10t
                    {
                        return "Medium";
                    }
                case 3: //12pt
                    {
                        return "MediumPlus";
                    }
                case 4: //18pt 
                    {
                        // Return empty as this will be mapped to default size
                        return "";
                    }
                case 5: //24pt
                    {
                        return "XxLarge";
                    }
                case 6: //36pt
                    {
                        return "XxxLarge";
                    }
                case 7: //48pt
                    {
                        return "XxLargePlus";
                    }
                case 8: //72pt
                    {
                        return "Super";
                    }
            }

            return null;
        }


        public static string ColorCodeToName(int colorCode)
        {
            switch (colorCode)
            {
                case 1:
                    {
                        return "RedDark";
                    }
                case 2:
                    {
                        return "Red";
                    }
                case 3:
                    {
                        return "Yellow";
                    }
                case 4:
                    {
                        return "YellowLight";
                    }
                case 5:
                    {
                        return "GreenLight";
                    }
                case 6:
                    {
                        return "Green";
                    }
                case 7:
                    {
                        return "BlueLight";
                    }
                case 8:
                    {
                        return "Blue";
                    }
                case 9:
                    {
                        return "BlueDark";
                    }
                case 10:
                    {
                        return "Purple";
                    }
            }

            return null;
        }

        public static string ColorCodeToBackGroundColorName(int colorCode)
        {
            switch (colorCode)
            {
                case 1:
                    {
                        return "Maroon";
                    }
                case 2:
                    {
                        return "Red";
                    }
                case 3:
                    {
                        return "Yellow";
                    }
                case 4:
                    {
                        return "Yellow";
                    }
                case 5:
                    {
                        return "Green";
                    }
                case 6:
                    {
                        return "Green";
                    }
                case 7:
                    {
                        return "Aqua";
                    }
                case 8:
                    {
                        return "Blue";
                    }
                case 9:
                    {
                        return "DarkBlue";
                    }
                case 10:
                    {
                        return "Purple";
                    }
            }

            return null;
        }

        private void ReplaceChildElementByText(INode parent, IElement child, IHtmlDocument document)
        {
            if (!string.IsNullOrEmpty(child.TextContent))
            {
                // Add a text content in place of the element
                var newElement = document.CreateTextNode(child.TextContent);
                parent.ReplaceChild(newElement, child);
            }
            else
            {
                // If no content then drop the element
                parent.RemoveChild(child);
            }
        }

    }
}
