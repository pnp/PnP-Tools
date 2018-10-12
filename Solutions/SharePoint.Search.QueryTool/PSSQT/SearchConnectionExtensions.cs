using SearchQueryTool.Model;
using System;

namespace PSSQT
{
    public static class SearchConnectionExtension
    {
        public static void CopyFrom(this SearchConnection searchConnection, SearchQueryRequest searchQueryRequest)
        {
            searchConnection.Accept = Enum.GetName(typeof(AcceptType), searchQueryRequest.AcceptType);
            searchConnection.HttpMethod = Enum.GetName(typeof(HttpMethodType), searchQueryRequest.HttpMethodType);
            searchConnection.SpSiteUrl = searchQueryRequest.SharePointSiteUrl;
            searchConnection.Timeout = searchQueryRequest.Timeout.HasValue ? searchQueryRequest.Timeout.Value.ToString() : null;
            searchConnection.Username = searchQueryRequest.UserName;
            searchConnection.AuthMethodIndex = 0;
            searchConnection.AuthTypeIndex = 0;
        }

    }
}
