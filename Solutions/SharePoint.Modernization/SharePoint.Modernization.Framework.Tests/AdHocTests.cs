using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace SharePoint.Modernization.Framework.Tests
{
    [TestClass]
    public class AdHocTests
    {

        [TestMethod]
        public void TestMethod1()
        {
            using (var cc = TestCommon.CreateClientContext())
            {
                var pages = cc.Web.GetPages("Header");

                Assert.IsTrue(pages.Count > 0);

            }
        }
    }
}
