using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Framework.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Modernization.Framework.Functions
{
    /// <summary>
    /// Set of native, builtin, functions
    /// </summary>
    public class BuiltIn: FunctionsBase
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

        // All functions return a single string, allowed input types are string, int, bool, DateTime and Guid

        #region Generic functions
        /// <summary>
        /// Html encodes a string
        /// </summary>
        /// <param name="text">Text to html encode</param>
        /// <returns>Html encoded string</returns>
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
        public string HtmlEncodeForJson(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return System.Web.HttpUtility.HtmlEncode(text).Replace("&quot;", @"\&quot;").Replace(":", "&#58;");
        }
        #endregion

        #region Text functions
        /// <summary>
        /// Wiki html rewrite to work in RTE
        /// </summary>
        /// <param name="text">Wiki html to rewrite</param>
        /// <returns>Html that's compatible with RTE</returns>
        public string TextCleanup(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return new HtmlTransformator().Transform(text);
        }
        #endregion

        #region List functions, used by XsltListViewWebPart
        /// <summary>
        /// Selector that returns the base type of the list as input for selecting the correct mapping
        /// </summary>
        /// <param name="listId">Id of the list</param>
        /// <returns>Mapping to be used for the given list</returns>
        public string ListSelectorListLibrary(Guid listId)
        {
            if (listId == Guid.Empty)
            {
                return "";
            }
            else
            {
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
        public string ImageServerRelativePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return "";
            }

            var hostUri = new Uri(this.clientContext.Web.Url);
            string host = $"{hostUri.Scheme}://{hostUri.DnsSafeHost}";

            return imagePath.Replace(host, "");
        }

        public Dictionary<string,string> ImageLookup(string serverRelativeImagePath)
        {
            if (string.IsNullOrEmpty(serverRelativeImagePath))
            {
                return null;
            }

            Dictionary<string, string> results = new Dictionary<string, string>();

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
                    // provided file link does not exist...we're eating the exception and the page will end up with a default page header
                    //TODO: log error
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public string ImageFileName(string serverRelativeImagePath)
        {
            if (string.IsNullOrEmpty(serverRelativeImagePath))
            {
                return "";
            }

            return Path.GetFileName(serverRelativeImagePath);

        }
        #endregion  
    }
}
