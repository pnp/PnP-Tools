using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using SharePoint.Modernization.Framework.Transform;

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

        [TestMethod]
        public void TestMethod2()
        {
            using (var cc = TestCommon.CreateClientContext())
            {
                ContentByQuerySearchTransformator cqs = new ContentByQuerySearchTransformator(cc);
                var res = cqs.HighlightedContentProperties();
            }
        }


        [TestMethod]
        public void TestMethod3()
        {
            using (var cc = TestCommon.CreateClientContext())
            {
                // sample config from web part filtering data from document library
                ContentByQuery cbq = new ContentByQuery()
                {
                    WebUrl = "~sitecollection",
                    ListGuid = "c3f3860d-53c9-4118-b985-8701af1505b0", // done
                    ListName = "Documents", // done
                    ServerTemplate = "101", 
                    ContentTypeBeginsWithId = "0x0101",

                    
                    // Done for Title
                    FilterField1 = "Title",
                    FilterField1Value = "value1",
                    FilterOperator1 = FilterFieldQueryOperator.BeginsWith,
                    Filter1ChainingOperator = FilterChainingOperator.And,
                    FilterField2 = "FileLeafRef",
                    FilterField2Value = "value2",
                    FilterOperator2 = FilterFieldQueryOperator.Eq,
                    Filter2ChainingOperator = FilterChainingOperator.And,
                    FilterField3 = "Title",
                    FilterField3Value = "value3",
                    FilterOperator3 = FilterFieldQueryOperator.Contains,

                    SortBy = "FileLeafRef", // Done, not usable unless managed property
                    SortByDirection = SortDirection.Asc, // Done, not usable unless managed property
                    GroupBy = "", // Done, not usable today
                    GroupByDirection = SortDirection.Desc, // Done, not usable today

                    ItemLimit = 15, // done
                    DisplayColumns = 1, // done 

                    DataMappings = "Description:|ImageUrl:|Title:{fa564e0f-0c70-4ab9-b863-0177e6ddd247},Title,Text;|LinkUrl:{94f89715-e097-4e8b-ba79-ea02aa8b7adb},FileRef,Lookup;|"
                };
                
                ContentByQuerySearchTransformator cqs = new ContentByQuerySearchTransformator(cc);
                var res = cqs.TransformContentByQueryWebPartToHighlightedContent(cbq);
                
            }
        }

    }
}
