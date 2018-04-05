using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Functions
{
    public class BuiltIn: FunctionsBase
    {

        #region Construction
        public BuiltIn(ClientContext clientContext): base(clientContext)
        {
        }
        #endregion

        // All functions return a single string, allowed input types are string, int, bool, DateTime and Guid

        #region Generic functions
        #endregion

        #region List functions
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



        public string EncodeGuid(string viewContentTypeId)
        {
            return $"Hi there {viewContentTypeId}";
        }

        public string EncodeGuid2(string viewContentTypeId, string pageType)
        {
            return $"Hi there {viewContentTypeId} - {pageType}";
        }

        public string EncodeGuid3(Guid viewContentTypeId)
        {
            return $"ok";
        }

        public string DoublePageSize(int pageSize)
        {
            return (pageSize * 2).ToString();
        }

    }
}
