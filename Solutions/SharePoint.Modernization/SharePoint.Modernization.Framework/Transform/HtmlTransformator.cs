using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Linq;

namespace SharePoint.Modernization.Framework.Transform
{
    /// <summary>
    /// Transforms the received Html in html that can be displayed and maintained in the modern client side text part
    /// </summary>
    public class HtmlTransformator : IHtmlTransformator
    {
        private HtmlParser parser;

        #region Construction
        /// <summary>
        /// Default constructor
        /// </summary>
        public HtmlTransformator()
        {
            // Instantiate the AngleSharp Html parser
            parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
        }
        #endregion

        /// <summary>
        /// Transforms the passed html to be usable by the client side text part
        /// </summary>
        /// <param name="text">Html to be transformed</param>
        /// <param name="usePlaceHolder">Insert placeholders for images and iframe tags</param>
        /// <returns>Html that can be used and edited via the client side text part</returns>
        public string Transform(string text, bool usePlaceHolder)
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

                // Process image and iframes ==> put a place holder text as these will be dropped by RTE on edit mode
                if (usePlaceHolder)
                {
                    ImageIFramePlaceHolders(document);
                }

                // Process tables
                TransformTables(document.QuerySelectorAll("table"), document);

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

        /// <summary>
        /// Returns true is the passed html is "empty"
        /// </summary>
        /// <param name="text">Html to verify</param>
        /// <returns>True if considered empty, false otherwise</returns>
        public bool IsEmptyParagraph(string text)
        {
            // Check for empty text or "Zero width space"
            if (string.IsNullOrEmpty(text) || (text.Length == 1 && text[0] == '\u200B'))
            {
                return true;
            }

            using (var document = this.parser.Parse(text))
            {
                if (document.Body.InnerHtml.Equals("<p>​</p>", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void TransformTables(IHtmlCollection<IElement> tables, IHtmlDocument document)
        {
            foreach(var table in tables)
            {
                // <div class="canvasRteResponsiveTable">
                var newTableElement = document.CreateElement($"div");
                newTableElement.ClassName = "canvasRteResponsiveTable";

                // <div class="tableCenterAlign tableWrapper">
                var innerDiv = document.CreateElement("div");
                // Possible alignments: tableLeftAlign, tableCenterAlign and tableRightAlign, since wiki does not have this option default to left align
                innerDiv.ClassList.Add(new string[] { "tableLeftAlign", "tableWrapper" });
                newTableElement.AppendChild(innerDiv);

                // <table class="bandedRowTableStyleNeutral" title="Table">
                var tableElement = document.CreateElement("table");
                //ms-rteTable-default: basic grid lines
                string tableClassName = "simpleTableStyleNeutral";
                if (!string.IsNullOrEmpty(table.ClassName))
                {
                    if (table.ClassName.Equals("ms-rteTable-default", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableClassName = "simpleTableStyleNeutral";
                    }
                    else
                    {
                        if (int.TryParse(table.ClassName.ToLower().Replace("ms-rtetable-", ""), out int tableStyleCode))
                        {
                            tableClassName = TableStyleCodeToName(tableStyleCode);
                        }
                    }
                }

                tableElement.ClassName = tableClassName;
                tableElement.SetAttribute("title", "Table");
                innerDiv.AppendChild(tableElement);

                // <tbody>
                var tableBody = document.CreateElement("tbody");
                tableElement.AppendChild(tableBody);

                // Iterate the table rows
                var tableBodyElement = (table as IHtmlTableElement).Bodies[0];
                var rows = tableBodyElement.Children.Where(p => p.TagName.Equals("tr", StringComparison.InvariantCultureIgnoreCase));
                if (rows != null && rows.Count() > 0)
                {
                    // TODO: col and row spans are not yet supported in RTE but do seem to work...verify
                    foreach(var row in rows)
                    {
                        var newRow = document.CreateElement("tr");

                        // check for table headers
                        var tableHeaders = row.Children.Where(p => p.TagName.Equals("th", StringComparison.InvariantCultureIgnoreCase));
                        if (tableHeaders != null && tableHeaders.Count() > 0)
                        {
                            foreach(var tableHeader in tableHeaders)
                            {
                                var tableHeaderValue = document.CreateElement("strong");
                                tableHeaderValue.TextContent = tableHeader.TextContent;

                                var tableHeaderCell = document.CreateElement("td");
                                tableHeaderCell.AppendChild(tableHeaderValue);

                                // take over row and col spans
                                var rowSpan = tableHeader.GetAttribute("rowspan");
                                if (!string.IsNullOrEmpty(rowSpan) && rowSpan != "1")
                                {
                                    tableHeaderCell.SetAttribute("rowspan", rowSpan);
                                }
                                var colSpan = tableHeader.GetAttribute("colspan");
                                if (!string.IsNullOrEmpty(colSpan) && colSpan != "1")
                                {
                                    tableHeaderCell.SetAttribute("colspan", colSpan);
                                }

                                newRow.AppendChild(tableHeaderCell);
                            }
                        }

                        // check for table cells
                        var tableCells = row.Children.Where(p => p.TagName.Equals("td", StringComparison.InvariantCultureIgnoreCase));
                        if (tableCells != null && tableCells.Count() > 0)
                        {
                            foreach (var tableCell in tableCells)
                            {
                                var newTableCell = document.CreateElement("td");
                                newTableCell.TextContent = tableCell.TextContent;

                                // take over row and col spans
                                var rowSpan = tableCell.GetAttribute("rowspan");
                                if (!string.IsNullOrEmpty(rowSpan) && rowSpan != "1")
                                {
                                    newTableCell.SetAttribute("rowspan", rowSpan);
                                }
                                var colSpan = tableCell.GetAttribute("colspan");
                                if (!string.IsNullOrEmpty(colSpan) && colSpan != "1")
                                {
                                    newTableCell.SetAttribute("colspan", colSpan);
                                }

                                newRow.AppendChild(newTableCell);
                            }
                        }
                        tableBody.AppendChild(newRow);
                    }
                }

                // Swap old table with new table
                table.Parent.ReplaceChild(newTableElement, table);
            }
        }

        protected virtual void ImageIFramePlaceHolders(IHtmlDocument document)
        {
            var images = document.QuerySelectorAll("img");
            var iframes = document.QuerySelectorAll("iframe");
            var elements = images.Union(iframes);

            foreach(var element in elements)
            {
                // Add a text content in place of the element
                string webPartType = "";
                string sourceValue = "";
                var source = element.Attributes.Where(p => p.Name.Equals("src", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (source != null)
                {
                    sourceValue = source.Value;
                }
                if (element is IHtmlImageElement)
                {
                    webPartType = "Image";
                }
                else if (element is IHtmlInlineFrameElement)
                {
                    webPartType = "IFrame";
                }

                string placeHolder = $"***{webPartType} placeholder for source {sourceValue}***";

                // Create P element and insert it just before our current image or iframe element
                var newElement = document.CreateElement($"P");
                newElement.TextContent = placeHolder;

                if (element.Parent != null)
                {
                    element.Parent.InsertBefore(newElement, element);
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
                        // Drop P as nested P is not allowed in clean html
                        // TODO: do this in a better way
                        newElement.InnerHtml = blockQuote.InnerHtml.Replace("<p>", "").Replace("</p>", "").Replace("<P>", "").Replace("</P>", "");
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
                                    blockParent.Parent.Parent.ReplaceChild(newElement, blockParent.Parent);
                                    break;
                                }
                            case 4:
                                {
                                    blockParent.Parent.Parent.Parent.ReplaceChild(newElement, blockParent.Parent.Parent);
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
                // TODO: map theme fore and back colors
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
                    if (int.TryParse(span.ClassName.ToLower().Replace("ms-rteforecolor-", ""), out int colorCode))
                    {
                        string colorName = ColorCodeToForegroundColorName(colorCode);
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
                    if (int.TryParse(span.ClassName.ToLower().Replace("ms-rtebackcolor-", ""), out int colorCode))
                    {
                        string colorName = ColorCodeToBackgroundColorName(colorCode);
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
                    if (int.TryParse(span.ClassName.ToLower().Replace("ms-rtefontsize-", ""), out int fontsizeCode))
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
                        // Let's go to default...meaning font size info will be dropped
                        ReplaceChildElementByText(parent, span, document);
                        continue;
                    }
                }
            }
        }

        public static string TableStyleCodeToName(int tableStyleCode)
        {
            //ms-rteTable-default: basic grid lines
            //ms-rteTable-0: no grid
            //ms-rteTable-1: table style 2, light banded: no header, alternating light gray rows, no grid
            //ms-rteTable-2: table style 4, light lines: header, no alternating rows, no grid
            //ms-rteTable-3: table style 5, grid: no header, no alternating rows, grid
            //ms-rteTable-4: table style 6, Accent 1: header, no alternating rows, grid, blue colors
            //ms-rteTable-5: table style 7, Accent 2: header, no alternating rows, grid, light blue colors
            //ms-rteTable-6: table style 3, medium two tones: header with alternating blue colors, no grid
            //ms-rteTable-7: table style 8, Accent 3: header, no alternating rows, grid, green colors
            //ms-rteTable-8: table style 9, Accent 4: header, no alternating rows, grid, brownish colors
            //ms-rteTable-9: table style 10, Accent 5: header, no alternating rows, grid, red colors
            //ms-rteTable-10: table style 11, Accent 6: header, no alternating rows, grid, purple colors

            switch (tableStyleCode)
            {
                case 0:
                case 3:
                    {
                        return "simpleTableStyleNeutral";
                    }
                case 1:
                    {
                        return "bandedRowTableStyleNeutral";
                    }
                case 2:
                    {
                        return "filledHeaderTableStyleNeutral";
                    }
                case 4:
                case 5:
                case 7:
                case 8:
                case 9:
                case 10:
                    {
                        return "filledHeaderTableStyleTheme";
                    }
                case 6:
                    {
                        return "bandedRowTableStyleTheme";
                    }
            }

            return "simpleTableStyleNeutral";
        }


        /// <summary>
        /// Translates SharePoint wiki font size (e.g. ms-rtefontsize-3 means font size 3) to RTE font size name
        /// </summary>
        /// <param name="fontCode">Wiki font size code</param>
        /// <returns>RTE font size name</returns>
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

        /// <summary>
        /// Translated SharePoint Wiki foreground color number (ms-rteforecolor-2 means number 2 is used) to RTE compatible color name
        /// </summary>
        /// <param name="colorCode">Used color number</param>
        /// <returns>RTE color string</returns>
        public static string ColorCodeToForegroundColorName(int colorCode)
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

        /// <summary>
        /// Translated SharePoint Wiki background color number (ms-rtebackcolor-5 means number 5 is used) to RTE compatible color name
        /// </summary>
        /// <param name="colorCode">Used color number</param>
        /// <returns>RTE color string</returns>
        public static string ColorCodeToBackgroundColorName(int colorCode)
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

        #region Helper methods
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
        #endregion
    }
}
