using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using SharePointPnP.Modernization.Framework.Transform;
using OfficeDevPnP.Core.Pages;
using SharePointPnP.Modernization.Framework.Pages;
using SharePointPnP.Modernization.Framework.Entities;

namespace SharePointPnP.Modernization.Framework.Tests.Transform
{
    [TestClass]
    public class Tests
    {
        class TestLayout : ILayoutTransformator
        {
            public void Transform(PageLayout layout)
            {
                throw new NotImplementedException();
            }
        }

        class TestTransformator : IContentTransformator
        {
            public void Transform(List<WebPartEntity> webParts)
            {
                throw new NotImplementedException();
            }
        }


        #region Test initialization
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {           
            //using (var cc = TestCommon.CreateClientContext())
            //{
            //    // Clean all migrated pages before next run
            //    var pages = cc.Web.GetPages("Migrated_");

            //    foreach (var page in pages.ToList())
            //    {
            //        page.DeleteObject();
            //    }

            //    cc.ExecuteQueryRetry();
            //}
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {

        }
        #endregion

        [TestMethod]
        public void TransformPagesTest()
        {
            // Local functions
            string titleOverride(string title)
            {
                return $"{title}_1";
            }

            ILayoutTransformator layoutOverride(ClientSidePage cp)
            {
                return new TestLayout();
            }

            IContentTransformator contentOverride(ClientSidePage cp, PageTransformation pt)
            {
                return new TestTransformator();
            }

            using (var cc = TestCommon.CreateClientContext())
            {
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


    }
}
