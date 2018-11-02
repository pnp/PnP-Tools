using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointPnP.Modernization.Framework.Transform;

namespace SharePointPnP.Modernization.Framework.Tests.Transform.Wp
{
    [TestClass]
    public class WpTests
    {
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
        public void RunWPTest()
        {
            using (var cc = TestCommon.CreateClientContext())
            {
                var pageTransformator = new PageTransformator(cc);

                var pages = cc.Web.GetPages("wp_2");
                foreach (var page in pages)
                {
                    PageTransformationInformation pti = new PageTransformationInformation(page)
                    {
                        // If target page exists, then overwrite it
                        Overwrite = true,

                        // ModernizationCenter options
                        //ModernizationCenterInformation = new ModernizationCenterInformation()
                        //{
                        //    AddPageAcceptBanner = true
                        //},

                        // Migrated page gets the name of the original page
                        //TargetPageTakesSourcePageName = true,

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
