using Microsoft.SharePoint.Client;
using SharePointPnP.Modernization.Framework.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.Modernization.Framework.Profiler
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var cc = new ClientContext("https://bertonline.sharepoint.com/sites/espctest2"))
            {                
                SharePointOnlineCredentials creds = new SharePointOnlineCredentials("bert.jansen@bertonline.onmicrosoft.com", ConvertToSecureString(""));
                cc.Credentials = creds;

                var pageTransformator = new PageTransformator(cc);

                //complexwiki
                //demo1
                //wikitext
                //wiki_li
                //webparts.aspx
                //contentbyquery1.aspx
                //how to use this library.aspx
                var pages = cc.Web.GetPages("webparts.aspx");

                foreach (var page in pages)
                {
                    PageTransformationInformation pti = new PageTransformationInformation(page)
                    {
                        // If target page exists, then overwrite it
                        Overwrite = true,

                        // Migrated page gets the name of the original page
                        //TargetPageTakesSourcePageName = false,

                        // Give the migrated page a specific prefix, default is Migrated_
                        //TargetPagePrefix = "Yes_",

                        // Configure the page header, empty value means ClientSidePageHeaderType.None
                        //PageHeader = new ClientSidePageHeader(cc, ClientSidePageHeaderType.None, null),

                        // If the page is a home page then replace with stock home page
                        //ReplaceHomePageWithDefaultHomePage = true,

                        // Replace embedded images and iframes with a placeholder and add respective images and video web parts at the bottom of the page
                        //HandleWikiImagesAndVideos = false,

                        // Callout to your custom code to allow for title overriding
                        //PageTitleOverride = titleOverride,

                        // Callout to your custom layout handler
                        //LayoutTransformatorOverride = layoutOverride,

                        // Callout to your custom content transformator...in case you fully want replace the model
                        //ContentTransformatorOverride = contentOverride,
                    };

                    pageTransformator.Transform(pti);
                }

            }
        }

        private static SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
