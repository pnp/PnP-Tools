using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static class WebExtensions
    {
        private const string CAMLQueryByExtension = @"
                <View Scope='Recursive'>
                  <Query>
                    <Where>
                      <Contains>
                        <FieldRef Name='File_x0020_Type'/>
                        <Value Type='text'>aspx</Value>
                      </Contains>
                    </Where>
                  </Query>
                </View>";
        private const string CAMLQueryByExtensionAndName = @"
                <View Scope='Recursive'>
                  <Query>
                    <Where>
                      <And>
                        <Contains>
                          <FieldRef Name='File_x0020_Type'/>
                          <Value Type='text'>aspx</Value>
                        </Contains>
                        <BeginsWith>
                          <FieldRef Name='FileLeafRef'/>
                          <Value Type='text'>{0}</Value>
                        </BeginsWith>
                      </And>
                    </Where>
                  </Query>
                </View>";
        private const string FileRefField = "FileRef";
        private const string FileLeafRefField = "FileLeafRef";


        public static ListItemCollection GetPages(this Web web, string pageNameStartsWith = null)
        {
            // Get pages library
            ListCollection listCollection = web.Lists;
            listCollection.EnsureProperties(coll => coll.Include(li => li.BaseTemplate, li => li.RootFolder));
            var sitePagesLibrary = listCollection.Where(p => p.BaseTemplate == (int)ListTemplateType.WebPageLibrary).FirstOrDefault();
            if (sitePagesLibrary != null)
            {
                CamlQuery query = null;
                if (!string.IsNullOrEmpty(pageNameStartsWith))
                {
                    query = new CamlQuery
                    {
                        ViewXml = string.Format(CAMLQueryByExtensionAndName, pageNameStartsWith)
                    };
                }
                else
                {
                    query = new CamlQuery
                    {
                        ViewXml = CAMLQueryByExtension
                    };
                }

                var pages = sitePagesLibrary.GetItems(query);
                web.Context.Load(pages);
                web.Context.ExecuteQueryRetry();

                return pages;
            }

            return null;
        }
    }
}
