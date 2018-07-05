using AngleSharp.Parser.Html;
using Microsoft.SharePoint.Client;
using SharePointPnP.Modernization.Framework.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SharePointPnP.Modernization.Framework.Functions
{
    /// <summary>
    /// Set of native, builtin, functions
    /// </summary>
    public partial class BuiltIn: FunctionsBase
    {

        #region Construction
        /// <summary>
        /// Instantiates the base builtin function library
        /// </summary>
        /// <param name="clientContext">ClientContext object for the site holding the page being transformed</param>
        public BuiltIn(ClientContext clientContext): base(clientContext)
        {
        }
        #endregion

        // All functions return either a single string or a Dictionary<string,string> with key value pairs. 
        // Allowed input parameter types are string, int, bool, DateTime and Guid

        #region Generic functions
        /// <summary>
        /// Html encodes a string
        /// </summary>
        /// <param name="text">Text to html encode</param>
        /// <returns>Html encoded string</returns>
        [FunctionDocumentation(Description = "Returns the html encoded value of this string.",
                               Example = "{EncodedText} = HtmlEncode({Text})")]
        [InputDocumentation(Name = "{Text}", Description = "Text to html encode")]
        [OutputDocumentation(Name = "{EncodedText}", Description = "Html encoded text")]
        public string HtmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return System.Web.HttpUtility.HtmlEncode(text);
        }

        /// <summary>
        /// Html encodes string for inclusion in JSON
        /// </summary>
        /// <param name="text">Text to html encode</param>
        /// <returns>Html encoded string for inclusion in JSON</returns>
        [FunctionDocumentation(Description = "Returns the json html encoded value of this string.",
                               Example = "{JsonEncodedText} = HtmlEncodeForJson({Text})")]
        [InputDocumentation(Name = "{Text}", Description = "Text to html encode for inclusion in json")]
        [OutputDocumentation(Name = "{JsonEncodedText}", Description = "Html encoded text for inclusion in json file")]
        public string HtmlEncodeForJson(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return System.Web.HttpUtility.HtmlEncode(text).Replace("&quot;", @"\&quot;").Replace(":", "&#58;").Replace("@", "%40");
        }

        /// <summary>
        /// Return true
        /// </summary>
        /// <returns>True</returns>
        [FunctionDocumentation(Description = "Simply returns the string true.",
                               Example = "{UsePlaceHolders} = ReturnTrue()")]
        [OutputDocumentation(Name = "{UsePlaceHolders}", Description = "Value true")]
        public string ReturnTrue()
        {
            return "true";
        }

        /// <summary>
        /// Return false
        /// </summary>
        /// <returns>False</returns>
        [FunctionDocumentation(Description = "Simply returns the string false.", 
                               Example = "{UsePlaceHolders} = ReturnFalse()")]
        [OutputDocumentation(Name = "{UsePlaceHolders}", Description = "Value false")]
        public string ReturnFalse()
        {
            return "false";
        }

        /// <summary>
        /// Transforms the incoming path into a server relative path
        /// </summary>
        /// <param name="path">Path to transform</param>
        /// <returns>Server relative path</returns>
        [FunctionDocumentation(Description = "Transforms the incoming path into a server relative path.",
                               Example = "{ServerRelativePath} = ReturnServerRelativePath({Path})")]
        [InputDocumentation(Name = "{Path}", Description = "Path to transform")]
        [OutputDocumentation(Name = "{ServerRelativePath}", Description = "Server relative path")]
        public string ReturnServerRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            var hostUri = new Uri(this.clientContext.Web.Url);
            string host = $"{hostUri.Scheme}://{hostUri.DnsSafeHost}";

            return path.Replace(host, "");
        }

        /// <summary>
        /// Returns the filename of the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>File name</returns>
        [FunctionDocumentation(Description = "Returns the filename of the given path.",
                               Example = "{FileName} = ReturnFileName({Path})")]
        [InputDocumentation(Name = "{Path}", Description = "Path to analyze")]
        [OutputDocumentation(Name = "{FileName}", Description = "File name with extension from the given path")]
        public string ReturnFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            return Path.GetFileName(path);
        }

        [FunctionDocumentation(Description = "Concatenates 2 strings.",
                               Example = "{CompleteString} = Concatenate({String1},{String2})")]
        [InputDocumentation(Name = "{String1}", Description = "First string")]
        [InputDocumentation(Name = "{String2}", Description = "Second string")]
        [OutputDocumentation(Name = "{CompleteString}", Description = "Concatenation of the passed strings")]
        public string Concatenate(string string1, string string2)
        {
            if (string1 == null)
            {
                string1 = "";
            }
            if (string2 == null)
            {
                string2 = "";
            }

            return string1 + string2;
        }
        #endregion

        #region Text functions
        /// <summary>
        /// Selector to allow to embed a spacer instead of an empty text
        /// </summary>
        /// <param name="text">Text to evaluate</param>
        /// <returns>Text if text needs to be inserted, Spacer if text was empty and you want a spacer</returns>
        [SelectorDocumentation(Description = "Allows for option to include a spacer for empty text wiki text parts.",
                               Example = "TextSelector({CleanedText})")]
        [InputDocumentation(Name = "{CleanedText}", Description = "Client side text part compliant html (cleaned via TextCleanup function)")]
        [OutputDocumentation(Name = "Text", Description = "Will be output if the provided wiki text was not considered empty" )]
        [OutputDocumentation(Name = "Spacer", Description = "Will be output if the provided wiki text was considered empty")]
        public string TextSelector(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "Text";
            }

            var empty = new HtmlTransformator().IsEmptyParagraph(text);

            if (empty)
            {
                return "Spacer";
            }
            else
            {
                return "Text";
            }
        }

        /// <summary>
        /// Wiki html rewrite to work in RTE
        /// </summary>
        /// <param name="text">Wiki html to rewrite</param>
        /// <returns>Html that's compatible with RTE</returns>
        [FunctionDocumentation(Description = "Rewrites wiki page html to be compliant with the html supported by the client side text part.",
                               Example = "{CleanedText} = TextCleanup({Text},{UsePlaceHolders})")]
        [InputDocumentation(Name = "{Text}", Description = "Original wiki html content")]
        [InputDocumentation(Name = "{UsePlaceHolders}", Description = "Parameter indicating if placeholders must be included for unsupported img/iframe elements inside wiki html")]
        [OutputDocumentation(Name = "{CleanedText}", Description = "Html compliant with client side text part")]
        public string TextCleanup(string text, string usePlaceHolders)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            bool usePlaceHolder = true;

            bool.TryParse(usePlaceHolders, out usePlaceHolder);

            return new HtmlTransformator().Transform(text, usePlaceHolder);
        }


        [FunctionDocumentation(Description = "Rewrites summarylinks web part html to be compliant with the html supported by the client side text part.",
                               Example = "{CleanedText} = TextCleanUpSummaryLinks({Text})")]
        [InputDocumentation(Name = "{Text}", Description = "Original wiki html content")]
        [OutputDocumentation(Name = "{CleanedText}", Description = "Html compliant with client side text part")]
        public string TextCleanUpSummaryLinks(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return new SummaryLinksHtmlTransformator().Transform(text, false);
        }
        #endregion

        #region List functions, used by XsltListViewWebPart
        /// <summary>
        /// Selector that returns the base type of the list as input for selecting the correct mapping
        /// </summary>
        /// <param name="listId">Id of the list</param>
        /// <returns>Mapping to be used for the given list</returns>
        [SelectorDocumentation(Description = "Analyzes a list and returns the list base type.",
                               Example = "ListSelectorListLibrary({ListId})")]
        [InputDocumentation(Name = "{ListId}", Description = "Guid of the list to use")]
        [InputDocumentation(Name = "{ViewXml}", Description = "Definition of the selected view")]
        [OutputDocumentation(Name = "Library", Description = "The list is a document library")]
        [OutputDocumentation(Name = "List", Description = "The list is a document list")]
        [OutputDocumentation(Name = "Issue", Description = "The list is an issue list")]
        [OutputDocumentation(Name = "DiscussionBoard", Description = "The list is a discussion board")]
        [OutputDocumentation(Name = "Survey", Description = "The list is a survey")]
        [OutputDocumentation(Name = "Undefined", Description = "The list base type is undefined")]
        public string ListSelectorListLibrary(Guid listId, string viewXml)
        {
            if (listId == Guid.Empty)
            {
                return "";
            }
            else
            {
                if (!string.IsNullOrEmpty(viewXml))
                {
                    if (viewXml.IndexOf("Type=\"CALENDAR\"", StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        return "Calendar";
                    }
                }

                var list = this.clientContext.Web.GetListById(listId);
                list.EnsureProperties(p => p.BaseType);

                if (list.BaseType == BaseType.DocumentLibrary)
                {
                    return "Library";
                }
                else if (list.BaseType == BaseType.GenericList)
                {
                    return "List";
                }
                else if (list.BaseType == BaseType.Issue)
                {
                    return "Issue";
                }
                else if (list.BaseType == BaseType.DiscussionBoard)
                {
                    return "DiscussionBoard";
                }
                else if (list.BaseType == BaseType.Survey)
                {
                    return "Survey";
                }

                return "Undefined";
            }
        }

        /// <summary>
        /// Function that returns the server relative url of the given list
        /// </summary>
        /// <param name="listId">Id of the list</param>
        /// <returns>Server relative url of the list</returns>
        [FunctionDocumentation(Description = "Returns the server relative url of a list.",
                               Example = "{ListServerRelativeUrl} = ListAddServerRelativeUrl({ListId})")]
        [InputDocumentation(Name = "{ListId}", Description = "Guid of the list to use")]
        [OutputDocumentation(Name = "{ListServerRelativeUrl}", Description = "Server relative url of the list")]
        public string ListAddServerRelativeUrl(Guid listId)
        {
            if (listId == Guid.Empty)
            {
                return "";
            }
            else
            {
                var list = this.clientContext.Web.GetListById(listId);
                list.EnsureProperties(p => p.RootFolder.ServerRelativeUrl);
                return list.RootFolder.ServerRelativeUrl;
            }
        }

        /// <summary>
        /// Function that returns the web relative url of the given list
        /// </summary>
        /// <param name="listId">Id of the list</param>
        /// <returns>Web relative url of the list</returns>
        [FunctionDocumentation(Description = "Returns the web relative url of a list.",
                               Example = "{ListWebRelativeUrl} = ListAddWebRelativeUrl({ListId})")]
        [InputDocumentation(Name = "{ListId}", Description = "Guid of the list to use")]
        [OutputDocumentation(Name = "{ListWebRelativeUrl}", Description = "Web relative url of the list")]
        public string ListAddWebRelativeUrl(Guid listId)
        {
            if (listId == Guid.Empty)
            {
                return "";
            }
            else
            {
                var list = this.clientContext.Web.GetListById(listId);
                list.EnsureProperties(p => p.RootFolder.ServerRelativeUrl);
                this.clientContext.Web.EnsureProperty(p => p.ServerRelativeUrl);
                return list.RootFolder.ServerRelativeUrl.Replace(this.clientContext.Web.ServerRelativeUrl.TrimEnd('/'), "");
            }
        }

        /// <summary>
        /// Tries to find the id of the view used to configure the web part
        /// </summary>
        /// <param name="listId">Id of the list</param>
        /// <param name="xmlDefinition">Webpart view definition</param>
        /// <returns>Id of the detected view if found or otherwise the id of the default list view</returns>
        [FunctionDocumentation(Description = "Detects the list view id that was used by the webpart by mapping the web part xmldefinition to the list views. If no view found the list default view id is returned.",
                               Example = "{ListViewId} = ListDetectUsedView({ListId},{XmlDefinition})")]
        [InputDocumentation(Name = "{ListId}", Description = "Guid of the list to analyze")]
        [InputDocumentation(Name = "{XmlDefinition}", Description = "XmlDefinition attribute of the XSLTListViewWebPart")]
        [OutputDocumentation(Name = "{ListViewId}", Description = "Id of the view to be used")]
        public string ListDetectUsedView(Guid listId, string xmlDefinition)
        {
            if (listId == Guid.Empty || string.IsNullOrEmpty(xmlDefinition))
            {
                return "";
            }

            // Grab the list and the needed properties
            var list = this.clientContext.Web.GetListById(listId);
            list.EnsureProperties(l=>l.DefaultView, l => l.Views.Include(v => v.Hidden, v => v.Id, v => v.ListViewXml));

            // Get the "identifying" elements from the webpart view xml definition
            var webPartViewElement = XElement.Parse(xmlDefinition);

            // Analyze the views in the list to determine a possible mapping
            foreach (var view in list.Views.AsEnumerable().Where(view => !view.Hidden && view.ListViewXml != null))
            {
                var viewElement = XElement.Parse(view.ListViewXml);

                // Compare Query
                if (webPartViewElement.Descendants("Query").FirstOrDefault() != null && viewElement.Descendants("Query").FirstOrDefault() != null)
                {
                    var equalNodes = XmlComparer.AreEqual(webPartViewElement.Descendants("Query").FirstOrDefault(), viewElement.Descendants("Query").FirstOrDefault());
                    if (!equalNodes.Success)
                    {
                        continue;
                    }
                }
                else
                {
                    if (!(webPartViewElement.Descendants("Query").FirstOrDefault() == null && viewElement.Descendants("Query").FirstOrDefault() != null))
                    {
                        continue;
                    }
                }

                // Compare viewFields
                if (webPartViewElement.Descendants("ViewFields").FirstOrDefault() != null && viewElement.Descendants("ViewFields").FirstOrDefault() != null)
                {
                    var equalNodes = XmlComparer.AreEqual(webPartViewElement.Descendants("ViewFields").FirstOrDefault(), viewElement.Descendants("ViewFields").FirstOrDefault());
                    if (!equalNodes.Success)
                    {
                        continue;
                    }
                }
                else
                {
                    if (!(webPartViewElement.Descendants("ViewFields").FirstOrDefault() == null && viewElement.Descendants("ViewFields").FirstOrDefault() != null))
                    {
                        continue;
                    }
                }

                // Compare RowLimit
                if (webPartViewElement.Descendants("RowLimit").FirstOrDefault() != null && viewElement.Descendants("RowLimit").FirstOrDefault() != null)
                {
                    var equalNodes = XmlComparer.AreEqual(webPartViewElement.Descendants("RowLimit").FirstOrDefault(), viewElement.Descendants("RowLimit").FirstOrDefault());
                    if (!equalNodes.Success)
                    {
                        continue;
                    }
                }
                else
                {
                    if (!(webPartViewElement.Descendants("RowLimit").FirstOrDefault() == null && viewElement.Descendants("RowLimit").FirstOrDefault() != null))
                    {
                        continue;
                    }
                }

                // Yeah, we're still here so we found the matching view!
                return view.Id.ToString();
            }

            // No matching view found, so proceed with the default view
            return list.DefaultView.Id.ToString();
        }

        #endregion

        #region Image functions
        /// <summary>
        /// Does return image properties based on given server relative image path
        /// </summary>
        /// <param name="serverRelativeImagePath">Server relative path of the image</param>
        /// <returns>A set of image properties</returns>
        [FunctionDocumentation(Description = "Does lookup a file based on the given server relative path and return needed properties of the file. Returns null if file was not found.",
                               Example = "ImageLookup({ServerRelativeFileName})")]
        [InputDocumentation(Name = "{ServerRelativeFileName}", Description = "Server relative file name of the image")]
        [OutputDocumentation(Name = "{ImageListId}", Description = "Id of the list holding the file")]
        [OutputDocumentation(Name = "{ImageUniqueId}", Description = "UniqueId of the file")]
        public Dictionary<string,string> ImageLookup(string serverRelativeImagePath)
        {

            bool stop = false;
            if (string.IsNullOrEmpty(serverRelativeImagePath))
            {
                stop = true;
            }

            this.clientContext.Web.EnsureProperties(p => p.ServerRelativeUrl);

            // Check if this url is pointing to content living in this site
            if (!stop && !serverRelativeImagePath.StartsWith(this.clientContext.Web.ServerRelativeUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                // TODO: add handling of files living in another web
                stop = true;
            }

            Dictionary<string, string> results = new Dictionary<string, string>();

            if (stop)
            {
                results.Add("ImageListId", "");
                results.Add("ImageUniqueId", "");
                return results;
            }

            try
            {
                var pageHeaderImage = this.clientContext.Web.GetFileByServerRelativeUrl(serverRelativeImagePath);
                this.clientContext.Load(pageHeaderImage, p => p.UniqueId, p => p.ListId);
                this.clientContext.ExecuteQueryRetry();

                results.Add("ImageListId", pageHeaderImage.ListId.ToString());
                results.Add("ImageUniqueId", pageHeaderImage.UniqueId.ToString());
                return results;
            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    // Provided image was not found, should not happen
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region First party web parts hosted on classic pages
        /// <summary>
        /// Extracts the client side web part properties so they can be reused
        /// </summary>
        /// <param name="clientSideWebPartHtml">Html defining the client side web part hosted on a classic page</param>
        /// <returns>Client side web part properties ready for reuse</returns>
        [FunctionDocumentation(Description = "Extracts the client side web part properties so they can be reused.",
                               Example = "{JsonProperties} = ExtractWebpartProperties({ClientSideWebPartData})")]
        [InputDocumentation(Name = "{ClientSideWebPartData}", Description = "Web part data defining the client side web part configuration")]
        [OutputDocumentation(Name = "{JsonProperties}", Description = "Json properties to configure the client side web part")]
        public string ExtractWebpartProperties(string clientSideWebPartHtml)
        {
            if (string.IsNullOrEmpty(clientSideWebPartHtml))
            {
                return "{}";
            }

            HtmlParser parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
            using (var document = parser.Parse(clientSideWebPartHtml))
            {
                return document.Body.FirstElementChild.GetAttribute("data-sp-webpartdata");
            }
        }
        #endregion

        #region DocumentEmbed functions
        [FunctionDocumentation(Description = "Does lookup a file based on the given server relative path and return needed properties of the file. Returns null if file was not found.",
                               Example = "DocumentEmbedLookup({ServerRelativeFileName})")]
        [InputDocumentation(Name = "{ServerRelativeFileName}", Description = "Server relative file name")]
        [OutputDocumentation(Name = "{DocumentListId}", Description = "Id of the list holding the file")]
        [OutputDocumentation(Name = "{DocumentUniqueId}", Description = "UniqueId of the file")]
        [OutputDocumentation(Name = "{DocumentAuthor}", Description = "User principal name of the document author")]
        [OutputDocumentation(Name = "{DocumentAuthorName}", Description = "Name of the file author")]
        public Dictionary<string,string> DocumentEmbedLookup(string serverRelativeUrl)
        {
            bool stop = false;
            if (string.IsNullOrEmpty(serverRelativeUrl))
            {
                stop = true;
            }

            this.clientContext.Web.EnsureProperties(p => p.ServerRelativeUrl);

            // Check if this url is pointing to content living in this site
            if (!stop && !serverRelativeUrl.StartsWith(this.clientContext.Web.ServerRelativeUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                // TODO: add handling of files living in another web
                stop = true;
            }

            Dictionary<string, string> results = new Dictionary<string, string>();

            if (stop)
            {
                results.Add("DocumentListId", "");
                results.Add("DocumentUniqueId", "");
                results.Add("DocumentAuthor", "");
                results.Add("DocumentAuthorName", "");
                return results;
            }

            try
            {
                var document = this.clientContext.Web.GetFileByServerRelativeUrl(serverRelativeUrl);
                this.clientContext.Load(document, p => p.UniqueId, p => p.ListId, p => p.Author);
                this.clientContext.ExecuteQueryRetry();

                string[] authorParts = document.Author.LoginName.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                results.Add("DocumentListId", document.ListId.ToString());
                results.Add("DocumentUniqueId", document.UniqueId.ToString());
                results.Add("DocumentAuthor", authorParts.Length == 3 ? authorParts[2] : "");
                results.Add("DocumentAuthorName", document.Author.Title);

                return results;
            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    // provided file is not retrievable...we're eating the exception this file not be used in the target web part
                    //TODO: log error
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region Content Embed functions
        [SelectorDocumentation(Description = "Analyzes sourcetype and return recommended mapping.",
                               Example = "ContentEmbedSelectorSourceType({SourceType})")]
        [InputDocumentation(Name = "{SourceType}", Description = "Sourcetype of the viewed page in pageviewerwebpart")]
        [OutputDocumentation(Name = "WebPage", Description = "The embedded content is a page")]
        [OutputDocumentation(Name = "ServerFolderOrFile", Description = "The embedded content points to a server folder or file")]
        public string ContentEmbedSelectorSourceType(string sourceType)
        {
            if (sourceType == "4")
            {
                return "WebPage";
            }

            return "ServerFolderOrFile";
        }

        [SelectorDocumentation(Description = "If ContentLink is set (content editor) then return Link, otherwise return Content.",
                               Example = "ContentEmbedSelectorContentLink({ContentLink})")]
        [InputDocumentation(Name = "{ContentLink}", Description = "Link value if set")]
        [OutputDocumentation(Name = "Link", Description = "If the link was not empty")]
        [OutputDocumentation(Name = "Content", Description = "If no link was specified")]
        public string ContentEmbedSelectorContentLink(string contentLink)
        {
            if (!string.IsNullOrEmpty(contentLink))
            {
                return "Link";
            }
            else
            {
                return "Content";
            }
        }
        #endregion

        #region HighlightedContent functions
        /// <summary>
        /// Maps content by query web part data into a properties collection for the contentrollup (= Highlighted Content) web part
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="listGuid"></param>
        /// <param name="listName"></param>
        /// <param name="serverTemplate"></param>
        /// <param name="contentTypeBeginsWithId"></param>
        /// <param name="filterField1"></param>
        /// <param name="filter1ChainingOperator"></param>
        /// <param name="filterField1Value"></param>
        /// <param name="filterOperator1"></param>
        /// <param name="filterField2"></param>
        /// <param name="filter2ChainingOperator"></param>
        /// <param name="filterField2Value"></param>
        /// <param name="filterOperator2"></param>
        /// <param name="filterField3"></param>
        /// <param name="filterField3Value"></param>
        /// <param name="filterOperator3"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortByDirection"></param>
        /// <param name="groupBy"></param>
        /// <param name="groupByDirection"></param>
        /// <param name="itemLimit"></param>
        /// <param name="displayColumns"></param>
        /// <param name="dataMappings"></param>
        /// <returns>Properties collection for the contentrollup (= Highlighted Content) web part</returns>
        [FunctionDocumentation(Description = "Maps content by query web part data into a properties collection for the contentrollup (= Highlighted Content) web part", 
                               Example = "{JsonProperties} = ContentByQueryToHighlightedContentProperties({WebUrl},{ListGuid},{ListName},{ServerTemplate},{ContentTypeBeginsWithId},{FilterField1},{Filter1ChainingOperator},{FilterDisplayValue1},{FilterOperator1},{FilterField2},{Filter2ChainingOperator},{FilterDisplayValue2},{FilterOperator2},{FilterField3},{FilterDisplayValue3},{FilterOperator3},{SortBy},{SortByDirection},{GroupBy},{GroupByDirection},{ItemLimit},{DisplayColumns},{DataMappings})")]
        [InputDocumentation(Name = "{WebUrl}", Description = "")]
        [InputDocumentation(Name = "{ListGuid}", Description = "")]
        [InputDocumentation(Name = "{ListName}", Description = "")]
        [InputDocumentation(Name = "{ServerTemplate}", Description = "")]
        [InputDocumentation(Name = "{ContentTypeBeginsWithId}", Description = "")]
        [InputDocumentation(Name = "{FilterField1}", Description = "")]
        [InputDocumentation(Name = "{Filter1ChainingOperator}", Description = "")]
        [InputDocumentation(Name = "{FilterField1Value}", Description = "")]
        [InputDocumentation(Name = "{FilterOperator1}", Description = "")]
        [InputDocumentation(Name = "{FilterField2}", Description = "")]
        [InputDocumentation(Name = "{Filter2ChainingOperator}", Description = "")]
        [InputDocumentation(Name = "{FilterField2Value}", Description = "")]
        [InputDocumentation(Name = "{FilterOperator2}", Description = "")]
        [InputDocumentation(Name = "{FilterField3}", Description = "")]
        [InputDocumentation(Name = "{FilterField3Value}", Description = "")]
        [InputDocumentation(Name = "{FilterOperator3}", Description = "")]
        [InputDocumentation(Name = "{SortBy}", Description = "")]
        [InputDocumentation(Name = "{SortByDirection}", Description = "")]
        [InputDocumentation(Name = "{GroupBy}", Description = "")]
        [InputDocumentation(Name = "{GroupByDirection}", Description = "")]
        [InputDocumentation(Name = "{ItemLimit}", Description = "")]
        [InputDocumentation(Name = "{DisplayColumns}", Description = "")]
        [InputDocumentation(Name = "{DataMappings}", Description = "")]
        [OutputDocumentation(Name = "JsonProperties", Description = "Properties collection for the contentrollup (= Highlighted Content) web part")]
        public string ContentByQueryToHighlightedContentProperties(string webUrl, string listGuid, string listName, string serverTemplate, string contentTypeBeginsWithId,
                                                                   string filterField1, string filter1ChainingOperator, string filterField1Value, string filterOperator1,
                                                                   string filterField2, string filter2ChainingOperator, string filterField2Value, string filterOperator2,
                                                                   string filterField3, string filterField3Value, string filterOperator3,
                                                                   string sortBy, string sortByDirection, string groupBy, string groupByDirection, string itemLimit, int displayColumns,
                                                                   string dataMappings)
        {
            ContentByQuery cbq = new ContentByQuery()
            {
                WebUrl = webUrl,
                ListGuid = listGuid,
                ListName = listName,
                ServerTemplate = serverTemplate,
                ContentTypeBeginsWithId = contentTypeBeginsWithId,

                FilterField1 = filterField1,
                Filter1ChainingOperator = (FilterChainingOperator)Enum.Parse(typeof(FilterChainingOperator), filter1ChainingOperator, true),
                FilterField1Value = filterField1Value,
                FilterOperator1 = (FilterFieldQueryOperator)Enum.Parse(typeof(FilterFieldQueryOperator), filterOperator1, true),
                FilterField2 = filterField2,
                Filter2ChainingOperator = (FilterChainingOperator)Enum.Parse(typeof(FilterChainingOperator), filter2ChainingOperator, true),
                FilterField2Value = filterField2Value,
                FilterOperator2 = (FilterFieldQueryOperator)Enum.Parse(typeof(FilterFieldQueryOperator), filterOperator2, true),
                FilterField3 = filterField3,
                FilterField3Value = filterField3Value,
                FilterOperator3 = (FilterFieldQueryOperator)Enum.Parse(typeof(FilterFieldQueryOperator), filterOperator3, true),

                SortBy = sortBy,
                SortByDirection = (SortDirection)Enum.Parse(typeof(SortDirection), sortByDirection, true),
                GroupBy = groupBy,
                GroupByDirection = (SortDirection)Enum.Parse(typeof(SortDirection), groupByDirection, true),

                ItemLimit = Convert.ToInt32(itemLimit),
                DisplayColumns = displayColumns,

                DataMappings = dataMappings
            };

            ContentByQuerySearchTransformator cqs = new ContentByQuerySearchTransformator(this.clientContext);
            var res = cqs.TransformContentByQueryWebPartToHighlightedContent(cbq);
            return res;
        }
        #endregion

    }
}
