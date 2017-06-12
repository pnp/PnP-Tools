using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT
{
    public enum ResultSourceName
    {
        Documents,
        ItemsMatchingContentType,
        ItemsMatchingTag,
        ItemsRelatedToCurrentUser,
        ItemsWithSameKeywordAsThisItem,
        LocalPeopleResults,
        LocalReportsAndDataResults,
        LocalSharePointResults,
        LocalVideoResults,
        Pages,
        Pictures,
        Popular,
        RecentlyChangedItems,
        RecommendedItems,
        Wiki
    }

    public static class ResultSource    {
        public static string Select(ResultSourceName name)
        {
            switch (name)
            {
                case ResultSourceName.Documents: return "e7ec8cee-ded8-43c9-beb5-436b54b31e84";

                case ResultSourceName.ItemsMatchingContentType: return "5dc9f503-801e-4ced-8a2c-5d1237132419";

                case ResultSourceName.ItemsMatchingTag: return "e1327b9c-2b8c-4b23-99c9-3730cb29c3f7";

                case ResultSourceName.ItemsRelatedToCurrentUser: return "48fec42e-4a92-48ce-8363-c2703a40e67d";

                case ResultSourceName.ItemsWithSameKeywordAsThisItem: return "5c069288-1d17-454a-8ac6-9c642a065f48";

                case ResultSourceName.LocalPeopleResults: return "b09a7990-05ea-4af9-81ef-edfab16c4e31";

                case ResultSourceName.LocalReportsAndDataResults: return "203fba36-2763-4060-9931-911ac8c0583b";

                case ResultSourceName.LocalSharePointResults: return "8413cd39-2156-4e00-b54d-11efd9abdb89";

                case ResultSourceName.LocalVideoResults: return "78b793ce-7956-4669-aa3b-451fc5defebf";

                case ResultSourceName.Pages: return "5e34578e-4d08-4edc-8bf3-002acf3cdbcc";

                case ResultSourceName.Pictures: return "38403c8c-3975-41a8-826e-717f2d41568a";

                case ResultSourceName.Popular: return "97c71db1-58ce-4891-8b64-585bc2326c12";

                case ResultSourceName.RecentlyChangedItems: return "ba63bbae-fa9c-42c0-b027-9a878f16557c";

                case ResultSourceName.RecommendedItems: return "ec675252-14fa-4fbe-84dd-8d098ed74181";

                case ResultSourceName.Wiki: return "9479bf85-e257-4318-b5a8-81a180f5faa1";

                default: throw new ArgumentException(String.Format("Unknown result source name: {0}", Enum.GetName(typeof(ResultSourceName), name)));
            }
        }
    }
}
