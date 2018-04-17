using Microsoft.SharePoint.Client;
using System;

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
        #endregion

    }
}
