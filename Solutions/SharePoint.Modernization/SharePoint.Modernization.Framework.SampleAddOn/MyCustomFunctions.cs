using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Framework.Functions;
using System;

namespace SharePoint.Modernization.Framework.SampleAddOn
{
    public class MyCustomFunctions: FunctionsBase
    {
        #region Construction
        public MyCustomFunctions(ClientContext clientContext) : base(clientContext)
        {
        }
        #endregion

        public string MyListAddServerRelativeUrl(Guid listId)
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

    }
}
