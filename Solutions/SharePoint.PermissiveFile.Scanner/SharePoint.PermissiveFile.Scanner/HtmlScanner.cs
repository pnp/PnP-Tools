using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.PermissiveFile.Scanner
{
    /// <summary>
    /// Result of the Html scanner
    /// </summary>
    public class HtmlScannerResult
    {
        public int LinkReferences { get; set; }
        public int LocalHtmlLinkReferences { get; set; }
        public int ScriptReferences { get; set; }
    }

    /// <summary>
    /// Html scanner class...will inspect a html file for links and script
    /// </summary>
    public class HtmlScanner
    {
        
        /// <summary>
        /// Scans the given string
        /// </summary>
        /// <param name="fileContents">String content of the html file to scan</param>
        /// <returns>Information about the scanned file</returns>
        public HtmlScannerResult Scan(string fileContents)
        {
            HtmlScannerResult result = new HtmlScannerResult();
            try
            {
                var parser = new HtmlParser();
                var doc = parser.Parse(fileContents);

                var linkTags = doc.All.Where(p => p.HasAttribute("href"));

                // Links information
                result.LinkReferences = linkTags.Count();
                if (linkTags.Any())
                {
                    int relativeHtmlLinks = 0;
                    foreach (var link in linkTags)
                    {
                        string href = link.GetAttribute("href");

                        Uri resultUri;
                        if (!Uri.TryCreate(href, UriKind.Absolute, out resultUri))
                        {
                            if (href.ToLower().Contains(".html") || href.ToLower().Contains(".htm"))
                            {
                                relativeHtmlLinks++;
                            }
                        }
                    }

                    result.LocalHtmlLinkReferences = relativeHtmlLinks;
                }

                // Script information
                var scriptTags = doc.All.Where(p => p.LocalName == "script");
                result.ScriptReferences = scriptTags.Count();
            }
            catch(Exception)
            {
                // Eat this exception as we don't want to stop to prevent logging this item if this part of the scan fails
            }

            return result;
        }

    }
}
