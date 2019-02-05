using SearchQueryTool.Model;

namespace PSSQT
{
    public static class SearchSuggestionsRequestExtension
    {
        public static void CopyFrom(this SearchSuggestionsRequest searchSuggestionsRequest, SearchQueryRequest searchQueryRequest)
        {
            searchSuggestionsRequest.AcceptType = searchQueryRequest.AcceptType;
            searchSuggestionsRequest.AuthenticationType = searchQueryRequest.AuthenticationType;
            searchSuggestionsRequest.Cookies = searchQueryRequest.Cookies;
            // searchSuggestionsRequest.Culture = int.Parse(searchQueryRequest.Culture);    // TODO: Hm..
            // searchSuggestionsRequest.HttpMethodType = searchQueryRequest.HttpMethodType; // DO NOT COPY. Always uses GET. 
            searchSuggestionsRequest.Password = searchQueryRequest.Password;
            searchSuggestionsRequest.QueryText = searchQueryRequest.QueryText;
            searchSuggestionsRequest.SecurePassword = searchQueryRequest.SecurePassword;
            searchSuggestionsRequest.SharePointSiteUrl = searchQueryRequest.SharePointSiteUrl;
            searchSuggestionsRequest.Timeout = searchQueryRequest.Timeout;
            searchSuggestionsRequest.Token = searchQueryRequest.Token;
            searchSuggestionsRequest.UserName = searchQueryRequest.UserName;
        }

    }
}
