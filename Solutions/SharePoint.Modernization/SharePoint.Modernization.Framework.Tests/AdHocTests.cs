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
            using (var cc = TestCommon.CreateClientContext("https://bertonline.sharepoint.com/sites/bert1"))
            {
                cc.Web.EnsureProperty(p => p.Description);
                Assert.IsNotNull(cc.Web.Description);
            }

        }
    }
}
