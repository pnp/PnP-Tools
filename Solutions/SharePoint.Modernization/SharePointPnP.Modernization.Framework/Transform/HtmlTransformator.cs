using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointPnP.Modernization.Framework.Transform
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
            // Instantiate the AngleSharp Html parser, configure to load the Style property as well
            parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true }, Configuration.Default.WithDefaultLoader().WithCss());
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
            // Strip out the "zero width space characters"
            text = text.Replace("\u200B", string.Empty);
            text = text.Replace("\u200b", string.Empty);

            using (var document = this.parser.Parse(text))
            {
                // Process headings: RTE does h2, h3, h4 while wiki does h1, h2, h3, h4. Wiki h4 to h6 will become (formatted) text
                TransformHeadings(document, 6, 0);
                TransformHeadings(document, 5, 0);
                TransformHeadings(document, 4, 0);
                TransformHeadings(document, 3, 4);
                TransformHeadings(document, 2, 3);
                TransformHeadings(document, 1, 2);

                // Process blockquotes
                TransformBlockQuotes(document.QuerySelectorAll("blockquote"), document);

                // Process elements that can hold forecolor, backcolor, fontsize, underline and strikethrough information
                TransformElements(document.QuerySelectorAll("span"), document);
                TransformElements(document.QuerySelectorAll("sup"), document);
                TransformElements(document.QuerySelectorAll("sub"), document);
                TransformElements(document.QuerySelectorAll("strong"), document);
                TransformElements(document.QuerySelectorAll("em"), document);
                TransformElements(document.QuerySelectorAll("p"), document);


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
                    string updatedText = document.DocumentElement.Children[1].InnerHtml;
                    return updatedText;
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
            // Remove the "Zero width space" chars
            text = text.Replace("\u200B", string.Empty);
            text = text.Replace("\u200b", string.Empty);

            // Check for empty text or "Zero width space"
            if (string.IsNullOrEmpty(text) || (text.Length == 1 && (text[0] == '\u200B' || text[0] == '\u200b')))
            {
                return true;
            }

            using (var document = this.parser.Parse(text))
            {
                if (document.Body.InnerHtml.ToLower() == "<p></p>")
                {
                    return true;
                }                
            }

            return false;
        }

        protected virtual void TransformTables(IHtmlCollection<IElement> tables, IHtmlDocument document)
        {
            // TODO: what about nested tables?
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

                                // Rewrite the current cell content where needed
                                if (IsStrikeThrough(tableCell))
                                {

                                    var newElement = document.CreateElement("s");
                                    newElement.InnerHtml = tableCell.InnerHtml;
                                    tableCell.InnerHtml = newElement.OuterHtml;
                                }
                                else if (IsUnderline(tableCell))
                                {
                                    var newElement = document.CreateElement("u");
                                    newElement.InnerHtml = tableCell.InnerHtml;
                                    tableCell.InnerHtml = newElement.OuterHtml;
                                }

                                // Copy over the content, take over html content as cell can have formatting inside
                                //newTableCell.TextContent = tableCell.TextContent;
                                newTableCell.InnerHtml = tableCell.InnerHtml;

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
            // Dictionary that holds the nodes that will be replaced. Replacement node is a span that groups the elements at the given level.
            // We can't immediately replace the nodes as that will break the model where we walk the tree to find the level and top node
            Dictionary<IElement, IElement> replacementList = new Dictionary<IElement, IElement>();

            // Iterate over all blockquote nodes in the html
            foreach (var blockQuote in blockQuotes)
            {
                // Only process block quotes that are used for indentation
                if (blockQuote.OuterHtml.ToLower().Contains("margin:0px 0px 0px 40px"))
                {
                    if ((blockQuote.ChildElementCount > 0 && blockQuote.Children[0].TagName.ToLower() == "blockquote"))
                    {
                        // do nothing if we still see a blockquote as child element
                    }
                    else
                    {
                        // We're at the bottom level, the children of this blockquote node do contain regular content. This is always wrapped inside either P or Div tags
                        bool replacementDone = false;
                        IElement insertionContainer = null;
                        IElement topLevelBlockQuote = null;

                        // Calculate the level this blockquote is at, it's top level blockquote node, the insertion container for that level (if it exists), whether strike-through was used and whether underline was used
                        bool strikeThroughWasUsed = false;
                        bool underLineWasUsed = false;
                        DetectBlockQuoteLevelParentContainer(replacementList, blockQuote, ref level, ref topLevelBlockQuote, ref insertionContainer, ref strikeThroughWasUsed, ref underLineWasUsed);

                        // For the first level we get null as top level blockquote, so assign the current blockquote 
                        if (topLevelBlockQuote == null)
                        {
                            topLevelBlockQuote = blockQuote;
                        }

                        // If we found an insertion container then we'll append converted nodes to that container
                        if (insertionContainer != null)
                        {
                            replacementDone = true;
                        }

                        // Important to do a ToList() to get a copy as the Children list might be affected by the code in the loop!
                        foreach (var nodeToProcess in blockQuote.Children.ToList())
                        {
                            // Indention in realized via P element with the needed pixels as margin-left
                            IElement newElement;

                            // Block elements get their indentation style on the element, others will we wrapped inside a P
                            bool isBlockElement = IsBlockElement(nodeToProcess);
                            string innerHtml = "";
                            if (isBlockElement)
                            {
                                newElement = nodeToProcess;
                                newElement.Style.MarginLeft = $"{level * 40}px";
                                // Store the block element html for later adding
                                innerHtml = nodeToProcess.InnerHtml;
                                // since we're injecting the block element content again we first remove all nodes from it
                                foreach (var child in newElement.ChildNodes.ToList())
                                {
                                    newElement.RemoveChild(child);
                                }
                            }
                            else
                            {
                                newElement = document.CreateElement($"p");
                                newElement.Style.MarginLeft = $"{level * 40}px";
                            }

                            if (strikeThroughWasUsed)
                            {
                                // Since strikethrough was used wrap the contents in an s element
                                var newLineThroughElement = document.CreateElement("s");
                                if (isBlockElement)
                                {
                                    newLineThroughElement.InnerHtml = innerHtml; 
                                }
                                else
                                {
                                    newLineThroughElement.InnerHtml = nodeToProcess.OuterHtml;
                                }
                                newElement.AppendChild(newLineThroughElement);
                            }
                            else if (underLineWasUsed)
                            {
                                // Since underline was used wrap the contents in an u element
                                var newUnderlineElement = document.CreateElement("u");
                                if (isBlockElement)
                                {
                                    newUnderlineElement.InnerHtml = innerHtml;
                                }
                                else
                                {
                                    newUnderlineElement.InnerHtml = nodeToProcess.OuterHtml;
                                }
                                newElement.AppendChild(newUnderlineElement);
                            }
                            else
                            {
                                if (isBlockElement)
                                {
                                    newElement.InnerHtml = innerHtml;
                                }
                                else
                                {
                                    newElement.InnerHtml = nodeToProcess.OuterHtml;
                                }
                            }

                            // if no insertion container was created then create it as span and add the new element
                            if (insertionContainer == null)
                            {
                                insertionContainer = document.CreateElement("span");
                                insertionContainer.AppendChild(newElement);
                            }

                            if (!replacementDone)
                            {
                                // Since we can't simply replace the node (as that would break future blockquote tree traversals) we're storing the "new" node in a list. 
                                // Replacement will happen at the very end of this flow
                                if (!replacementList.ContainsKey(topLevelBlockQuote))
                                {
                                    replacementList.Add(topLevelBlockQuote, insertionContainer);
                                }
                            }
                            else
                            {
                                // There's already an insertion container, so we can simply append our element
                                insertionContainer.AppendChild(newElement);
                            }

                            replacementDone = true;
                        }

                        // reset variables since we're starting a new blockquote
                        level = 1;
                        insertionContainer = null;
                    }
                }
            }

            // Perform the actual replacements 
            foreach (var node in replacementList)
            {
                var parent = node.Key.Parent;
                if (parent.Contains(node.Key))
                {
                    parent.ReplaceChild(node.Value, node.Key);
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

                if (to == 0)
                {
                    // wrap the content inside a div so that further formatting processing will pick it up
                    var newElement = document.CreateElement("div");
                    newElement.InnerHtml = fromNode.InnerHtml;
                    parent.ReplaceChild(newElement, fromNode);
                }
                else
                {
                    var newElement = document.CreateElement($"h{to}");
                    newElement.InnerHtml = fromNode.InnerHtml;

                    // Copy the text alignment style
                    if (fromNode.Style != null && !string.IsNullOrEmpty(fromNode.Style.TextAlign))
                    {
                        newElement.Style.TextAlign = fromNode.Style.TextAlign;
                    }
                    parent.ReplaceChild(newElement, fromNode);
                }
            }
        }

        protected virtual void TransformElements(IHtmlCollection<IElement> elementsToTransform, IHtmlDocument document)
        {
            foreach (var element in elementsToTransform)
            {
                var parent = element.Parent;

                // ================================
                // Rewrite font styles outside of the header styles
                // ================================
                // <span class="ms-rteStyle-Normal">Norm</span>
                //ms-rteStyle-Normal
                //ms-rteStyle-Quote = Italic
                //ms-rteStyle-IntenseQuote = Italic + Underline
                //ms-rteStyle-Emphasis = italic + light blue
                //ms-rteStyle-IntenseEmphasis = italic + light blue + underline
                //ms-rteStyle-References = light gray
                //ms-rteStyle-IntenseReferences = light gray + underline
                //ms-rteStyle-Accent1 = light blue
                //ms-rteStyle-Accent2 = dark blue
                var rtestylenormal = element.ClassList.PartialMatch("ms-rtestyle-normal");
                if (!string.IsNullOrEmpty(rtestylenormal))
                {
                    element.ClassList.Remove(rtestylenormal);
                }

                // Replace "rte styles" with their wiki style alternatives which then can be picked up by the default processing
                var rtestyle = element.ClassList.PartialMatch("ms-rtestyle-");
                bool addItalic = false;
                if (!string.IsNullOrEmpty(rtestyle))
                {
                    // Remove the wiki style
                    element.ClassList.Remove(rtestyle);

                    rtestyle = rtestyle.ToLower().Replace("ms-rtestyle-", "");
                    // Underline style
                    if (rtestyle.Equals("IntenseQuote", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("IntenseEmphasis", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("IntenseReferences", StringComparison.InvariantCultureIgnoreCase) )
                    {
                        // Update current element Style which will be picked up later on 
                        element.Style.TextDecoration = "underline";
                    }

                    // Light blue
                    if (rtestyle.Equals("Emphasis", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("IntenseEmphasis", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("Accent1", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Update current element ClassList which will be picked up later on 
                        element.ClassList.Add("ms-rteforecolor-8");
                    }

                    // Light gray
                    if (rtestyle.Equals("References", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("IntenseReferences", StringComparison.InvariantCultureIgnoreCase) )
                    {
                        // no color to add as default is light gray
                    }

                    // Dark blue
                    if (rtestyle.Equals("Accent2", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Update current element ClassList which will be picked up later on 
                        element.ClassList.Add("ms-rteforecolor-9");
                    }

                    // Italic style
                    if (rtestyle.Equals("Quote", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("IntenseQuote", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("Emphasis", StringComparison.InvariantCultureIgnoreCase) ||
                        rtestyle.Equals("IntenseEmphasis", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // As we need to insert an EM element we can't do that right now. Let's start with setting an indicator 
                        addItalic = true;
                    }
                }

                // ================================
                // rewrite colors, back and fore color + size can be defined as class on a single span element
                // ================================
                // <span class="ms-rteThemeForeColor-5-0">red</span>
                var themeForeColor = element.ClassList.PartialMatch("ms-rtethemeforecolor-");
                if (!string.IsNullOrEmpty(themeForeColor))
                {
                    string newClass = null;

                    // Modern Theme colors
                    // Darker, Dark, Dark Alternate, Primary, Secondary
                    // Neutral Tertiary, Neutral Secondary, Primary alternate, Neutral primary, Neutral Dark
                    if (int.TryParse(themeForeColor.ToLower()[themeForeColor.ToLower().Length - 1].ToString(), out int themeCode))
                    {
                        string colorName = ThemeCodeToForegroundColorName(themeCode);
                        if (!string.IsNullOrEmpty(colorName))
                        {
                            newClass = $"fontColor{colorName}";
                        }
                    }

                    element.ClassList.Remove(themeForeColor);
                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        element.ClassList.Add(newClass);
                    }
                }

                // <span class="ms-rteThemeBackColor-5-0">red</span>
                var rtethemebackcolor = element.ClassList.PartialMatch("ms-rtethemebackcolor-");
                if (!string.IsNullOrEmpty(rtethemebackcolor))
                {
                    // There are no themed back colors in modern, so for now drop the color span and the background color
                    element.ClassList.Remove(rtethemebackcolor);
                }

                //<span class="ms-rteForeColor-2" style="">Red,&#160;</span>
                //<sup class="ms-rteForeColor-10" style=""><strong style="">superscript</strong></sup>
                var rteforecolor = element.ClassList.PartialMatch("ms-rteforecolor-");
                if (!string.IsNullOrEmpty(rteforecolor))
                {
                    // Modern Theme colors
                    // Dark Red, Red, Orange, Yellow, Light green
                    // Green, Light Blue, Blue, Dark Blue, Purple

                    string newClass = null;
                    if (int.TryParse(rteforecolor.ToLower().Replace("ms-rteforecolor-", ""), out int colorCode))
                    {
                        string colorName = ColorCodeToForegroundColorName(colorCode);
                        if (!string.IsNullOrEmpty(colorName))
                        {
                            newClass = $"fontColor{colorName}";
                        }
                    }

                    element.ClassList.Remove(rteforecolor);
                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        element.ClassList.Add(newClass);
                    }
                }

                // <sub class="ms-rteBackColor-2">lowerscript</sub>
                var rtebackcolor = element.ClassList.PartialMatch("ms-rtebackcolor-");
                if (!string.IsNullOrEmpty(rtebackcolor))
                {
                    // Modern Theme colors
                    // Dark Red, Red, Orange, Yellow, Light green
                    // Green, Light Blue, Blue, Dark Blue, Purple

                    string newClass = null;
                    if (int.TryParse(rtebackcolor.ToLower().Replace("ms-rtebackcolor-", ""), out int colorCode))
                    {
                        string colorName = ColorCodeToBackgroundColorName(colorCode);
                        if (!string.IsNullOrEmpty(colorName))
                        {
                            newClass = $"highlightColor{colorName}";
                        }
                    }

                    element.ClassList.Remove(rtebackcolor);
                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        element.ClassList.Add(newClass);
                    }
                }

                // ================================
                // rewrite font size
                // ================================
                var rtefontsize = element.ClassList.PartialMatch("ms-rtefontsize-");
                if (!string.IsNullOrEmpty(rtefontsize))
                {
                    string newClass = null;
                    if (int.TryParse(rtefontsize.ToLower().Replace("ms-rtefontsize-", ""), out int fontsizeCode))
                    {
                        string fontSize = FontCodeToName(fontsizeCode);
                        if (!string.IsNullOrEmpty(fontSize))
                        {
                            newClass = $"fontSize{fontSize}";
                        }
                    }

                    element.ClassList.Remove(rtefontsize);
                    if (!string.IsNullOrEmpty(newClass))
                    {
                        // We mapped a color
                        element.ClassList.Add(newClass);
                    }
                }

                // ================================
                // rewrite fonts
                // ================================
                // <span class="ms-rteFontFace-7">custom font</span>
                var rtefontface = element.ClassList.PartialMatch("ms-rtefontface-");
                if (!string.IsNullOrEmpty(rtefontface))
                {
                    // There are no themed back colors in modern, so for now drop the color span and the background color
                    element.ClassList.Remove(rtefontface);
                }

                // rewrite striked and underline
                // <span style="text-decoration&#58;line-through;">striked</span>
                // <span style="text-decoration&#58;underline;">underline</span>
                bool replacementDone = false;
                bool isStrikeThroughOnElementToKeep = false;
                bool isUnderlineOnElementToKeep = false;
                string elementToKeep = "";
                if (IsStrikeThrough(element))
                {
                    // strike through can be on an element that we're replacing as well (like em)...if so don't
                    // replace em with strike through now, but wait until at the very end 
                    if (element.TagName.Equals("em", StringComparison.InvariantCultureIgnoreCase) ||
                        element.TagName.Equals("strong", StringComparison.InvariantCultureIgnoreCase) ||
                        //element.TagName.Equals("blockquote", StringComparison.InvariantCultureIgnoreCase) ||
                        element.TagName.Equals("sup", StringComparison.InvariantCultureIgnoreCase) ||
                        element.TagName.Equals("sub", StringComparison.InvariantCultureIgnoreCase))
                    {
                        elementToKeep = element.TagName;
                        isStrikeThroughOnElementToKeep = true;
                    }
                    else
                    {
                        ReplaceByRelevantTag(document, element, parent, "s");
                        replacementDone = true;
                    }
                }
                else if (IsUnderline(element))
                {
                    if (element.TagName.Equals("em", StringComparison.InvariantCultureIgnoreCase) ||
                        element.TagName.Equals("strong", StringComparison.InvariantCultureIgnoreCase) ||
                        //element.TagName.Equals("blockquote", StringComparison.InvariantCultureIgnoreCase) ||
                        element.TagName.Equals("sup", StringComparison.InvariantCultureIgnoreCase) ||
                        element.TagName.Equals("sub", StringComparison.InvariantCultureIgnoreCase))
                    {
                        elementToKeep = element.TagName;
                        isUnderlineOnElementToKeep = true;
                    }
                    else
                    {
                        // Don't apply the default logic when we need to insert a EM tag...a node can only be replaced once, so postpone the 
                        // replacement by an U element when we also need to insert an EM tag
                        if (!addItalic)
                        {
                            ReplaceByRelevantTag(document, element, parent, "u");
                            replacementDone = true;
                        }
                    }
                }

                // No need to wrap a span into a new span
                if (element is IHtmlSpanElement)
                {
                    if (addItalic)
                    {
                        // We needed to insert an EM tag as the provided style uses italic
                        var newElement = document.CreateElement("span");
                        newElement.ClassList.Add(element.ClassList.ToArray());
                        var newItalic = document.CreateElement("em");

                        // Transform the piece of html we add in this span, needed to handle spans nested in here as these otherwise would not be found anymore
                        string innerHtml = TransformInnerHtml(element.InnerHtml);

                        // If the implemented style also used underline then add the U node as well
                        if (IsUnderline(element))
                        {
                            var newUnderline = document.CreateElement("u");
                            newUnderline.InnerHtml = innerHtml;
                            newItalic.AppendChild(newUnderline);
                        }
                        else
                        {
                            newItalic.InnerHtml = innerHtml;                         
                        }

                        newElement.AppendChild(newItalic);
                        parent.ReplaceChild(newElement, element);
                    }
                    else
                    {
                        // if we still did not replace the span element, the span has no classes set anymore and the span does not have child elements then we can replace it by text
                        if (!replacementDone && element.ClassList.Length == 0 && element.FirstElementChild == null)
                        {
                            ReplaceChildElementByText(parent, element, document);
                        }
                        else
                        {
                            // drop style attribute if still present
                            if (element.HasAttribute("style"))
                            {
                                element.RemoveAttribute("style");
                            }
                            // if the element has no classes anymore then let's drop the class attribute
                            if (element.ClassList.Count() == 0 && element.HasAttribute("class"))
                            {
                                element.RemoveAttribute("class");
                            }
                        }
                    }
                }
                else
                {
                    // Non span element with styling that was transformed will be wrapped in a span containing the styling which wraps a "clean" element
                    var newElement = document.CreateElement("span");
                    newElement.ClassList.Add(element.ClassList.ToArray());
                    // no point in having an empty class attribute
                    if (element.HasAttribute("class"))
                    {
                        element.RemoveAttribute("class");
                    }

                    if (isStrikeThroughOnElementToKeep) 
                    {
                        var strikeThroughElement = document.CreateElement("s");
                        newElement.AppendChild(strikeThroughElement);                        
                        // Create the element that held the strikethrough style
                        var emElement = document.CreateElement(elementToKeep.ToLower());
                        emElement.InnerHtml = element.InnerHtml;
                        strikeThroughElement.AppendChild(emElement);
                    }
                    else if (isUnderlineOnElementToKeep)
                    {
                        var underlineElement = document.CreateElement("u");
                        newElement.AppendChild(underlineElement);
                        // Create the element that held the underline style
                        var emElement = document.CreateElement(elementToKeep.ToLower());
                        emElement.InnerHtml = element.InnerHtml;
                        underlineElement.AppendChild(emElement);
                    }
                    else
                    {
                        newElement.InnerHtml = element.OuterHtml;
                    }

                    // Only replace the element if it's still available...it could have been replaced earlier on
                    if (parent.Contains(element))
                    {
                        parent.ReplaceChild(newElement, element);
                    }
                }
            }
        }

        /// <summary>
        /// Map wiki table style to a RTE compatible style
        /// </summary>
        /// <param name="tableStyleCode">Code used for the wiki table style</param>
        /// <returns>RTE compatible table style</returns>
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
        /// Translated SharePoint Wiki foreground theme color number (e.g. ms-rteThemeForeColor-6-1) to RTE compatible color name
        /// </summary>
        /// <param name="themeCode">Theme color code</param>
        /// <returns>RTE color string</returns>
        public static string ThemeCodeToForegroundColorName(int themeCode)
        {
            switch (themeCode)
            {
                case 0:
                    {
                        // 0 (light) will go to NeutralPrimary which is the default, hence returning null
                        return null;
                    }
                case 1:
                    {
                        return "ThemeSecondary";
                    }
                case 2:
                    {
                        return "ThemePrimary";
                    }
                case 3:
                    {
                        return "ThemeDarkAlt";
                    }
                case 4:
                    {
                        return "ThemeDark";
                    }
                case 5:
                    {
                        return "ThemeDarker";
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
        /// <summary>
        /// Recursively loop the blockquote elements until we're at the top level, returns needed information to process:
        /// - Level: how many indents where used
        /// - TopLevelBlockQuote: what is the top level blockquote that we'll be using as "replacement node"
        /// - If there already was a container created to store content at this level then let's return that one
        /// - If by walking the blockquote tree we see strike through being used then indicate that
        /// - If by walking the blockquote tree we see underline being used then indicate that
        /// </summary>
        /// <param name="replacementList"></param>
        /// <param name="blockQuote"></param>
        /// <param name="level"></param>
        /// <param name="topLevelBlockQuote"></param>
        /// <param name="insertionContainer"></param>
        /// <param name="strikeThroughWasUsed"></param>
        /// <param name="underLineWasUsed"></param>
        private void DetectBlockQuoteLevelParentContainer(Dictionary<IElement, IElement> replacementList, IElement blockQuote, ref int level, ref IElement topLevelBlockQuote, ref IElement insertionContainer, ref bool strikeThroughWasUsed, ref bool underLineWasUsed)
        {
            if (IsStrikeThrough(blockQuote))
            {
                strikeThroughWasUsed = true;
            }
            if (IsUnderline(blockQuote))
            {
                underLineWasUsed = true;
            }

            if (blockQuote.Parent is IElement)
            {
                if (blockQuote.ParentElement.TagName.ToLower() == "blockquote")
                {
                    topLevelBlockQuote = blockQuote.ParentElement;
                    level++;

                    if (IsStrikeThrough(blockQuote.ParentElement))
                    {
                        strikeThroughWasUsed = true;
                    }
                    if (IsUnderline(blockQuote.ParentElement))
                    {
                        underLineWasUsed = true;
                    }

                    DetectBlockQuoteLevelParentContainer(replacementList, blockQuote.ParentElement, ref level, ref topLevelBlockQuote, ref insertionContainer, ref strikeThroughWasUsed, ref underLineWasUsed);
                }
                else
                {
                    if (insertionContainer == null && topLevelBlockQuote != null && replacementList.ContainsKey(topLevelBlockQuote))
                    {
                        insertionContainer = replacementList[topLevelBlockQuote];
                    }
                }
            }
        }

        private string TransformInnerHtml(string innerHtml)
        {
            // Let's inspect if there's still span's in the html we take over...these spans are not in our current list of spans 
            // and as such will be ignored if we don't handle them.
            using (var documentTemp = this.parser.Parse(innerHtml))
            {
                TransformElements(documentTemp.QuerySelectorAll("span"), documentTemp);
                //TransformElements(documentTemp.QuerySelectorAll("sup"), documentTemp);
                //TransformElements(documentTemp.QuerySelectorAll("sub"), documentTemp);
                //TransformElements(documentTemp.QuerySelectorAll("strong"), documentTemp);
                //TransformElements(documentTemp.QuerySelectorAll("em"), documentTemp);

                if (documentTemp.DocumentElement.Children.Count() > 1)
                {
                    innerHtml = documentTemp.DocumentElement.Children[1].InnerHtml;
                }
            }

            return innerHtml;
        }

        private void ReplaceByRelevantTag(IHtmlDocument document, IElement element, INode parent, string tag)
        {
            var newElement = document.CreateElement(tag);

            // Transform the piece of html we add in this span, needed to handle spans nested in here as these otherwise would not be found anymore
            string innerHtml = TransformInnerHtml(element.InnerHtml);

            // If the element has a class defined then wrap inside a span
            if (element.ClassList.Count() > 0)
            {
                var newSpan = document.CreateElement("span");
                newSpan.ClassList.Add(element.ClassList.ToArray());
                newSpan.InnerHtml = innerHtml;
                newElement.AppendChild(newSpan);
            }
            else
            {
                newElement.InnerHtml = innerHtml;
            }

            parent.ReplaceChild(newElement, element);
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

        private bool IsBlockElement(IElement element)
        {
            var tag = element.TagName.ToLower();
            if (tag == "article" ||
                tag == "div" ||
                tag == "p" ||
                tag == "h1" ||
                tag == "h2" ||
                tag == "h3" ||
                tag == "h3" ||
                tag == "h4" ||
                tag == "h5" ||
                tag == "h6" ||
                tag == "li" ||
                tag == "ol" ||
                tag == "ul" ||
                tag == "address" ||
                tag == "aside" ||
                tag == "canvas" ||
                tag == "dd" ||
                tag == "dl" ||
                tag == "dt" ||
                tag == "fieldset" ||
                tag == "figcaption" ||
                tag == "figure" ||
                tag == "footer" ||
                tag == "form" ||
                tag == "header" ||
                tag == "hr" ||
                tag == "main" ||
                tag == "nav" ||
                tag == "noscript" ||
                tag == "output" ||
                tag == "pre" ||
                tag == "section" ||
                tag == "table" ||
                tag == "tfoot" ||
                tag == "video" ||
                tag == "aside" ||
                tag == "blockquote" )
            {
                return true;
            }

            return false;
        }

        private bool IsStrikeThrough(IElement element)
        {
            if (element.Style != null && (!string.IsNullOrEmpty(element.Style.TextDecoration) && element.Style.TextDecoration.Equals("line-through", StringComparison.InvariantCultureIgnoreCase) ||
                                          !string.IsNullOrEmpty(element.Style.GetPropertyValue("text-decoration-line")) && element.Style.GetPropertyValue("text-decoration-line").Equals("line-through", StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsUnderline(IElement element)
        {
            if (element.Style != null && (!string.IsNullOrEmpty(element.Style.TextDecoration) && element.Style.TextDecoration.Equals("underline", StringComparison.InvariantCultureIgnoreCase) ||
                                          !string.IsNullOrEmpty(element.Style.GetPropertyValue("text-decoration-line")) && element.Style.GetPropertyValue("text-decoration-line").Equals("underline", StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
