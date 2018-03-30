using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Framework.Transform;
using OfficeDevPnP.Core.Pages;
using SharePoint.Modernization.Framework.Pages;

namespace SharePoint.Modernization.Framework.Tests.Transform
{
    [TestClass]
    public class Tests
    {
        class TestLayout : ILayoutTransformator
        {
            public void ApplyLayout(PageLayout layout)
            {
                throw new NotImplementedException();
            }
        }


        #region Test initialization
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {           
            using (var cc = TestCommon.CreateClientContext())
            {
                // Clean all migrated pages before next run
                var pages = cc.Web.GetPages("Migrated_");

                foreach (var page in pages.ToList())
                {
                    page.DeleteObject();
                }

                cc.ExecuteQueryRetry();
            }
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

            using (var cc = TestCommon.CreateClientContext())
            {
                var pages = cc.Web.GetPages("Header");

                foreach (var page in pages)
                {
                    //page.Transform(cc, pageTitleOverride:titleOverride);
                    //page.Transform(cc, layoutTransformatorOverride: layoutOverride);
                    page.Transform(cc);
                }

            }
        }


    }
}
