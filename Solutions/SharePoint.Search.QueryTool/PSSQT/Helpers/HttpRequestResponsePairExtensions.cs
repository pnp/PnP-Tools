using SearchQueryTool.Helpers;
using SearchQueryTool.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT.Helpers
{
    public static class HttpRequestResponsePairExtensions
    {
        public static TSearchResult GetResultItem<TSearchResult>(this HttpRequestResponsePair requestResponsePair) where TSearchResult : SearchResult, new()
        {
            TSearchResult searchResult;
            var request = requestResponsePair.Item1;

            using (var response = requestResponsePair.Item2)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(String.Format("HTTP {0}: {1}", (int)response.StatusCode, response.StatusDescription));
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();

                    NameValueCollection requestHeaders = new NameValueCollection();
                    foreach (var header in request.Headers.AllKeys)
                    {
                        requestHeaders.Add(header, request.Headers[header]);
                    }

                    NameValueCollection responseHeaders = new NameValueCollection();
                    foreach (var header in response.Headers.AllKeys)
                    {
                        responseHeaders.Add(header, response.Headers[header]);
                    }

                    string requestContent = "";
                    if (request.Method == "POST")
                    {
                        requestContent = requestResponsePair.Item3;
                    }


                    searchResult = new TSearchResult {

                        RequestUri = request.RequestUri,
                        RequestMethod = request.Method,
                        RequestContent = requestContent,
                        ContentType = response.ContentType,
                        ResponseContent = content,
                        RequestHeaders = requestHeaders,
                        ResponseHeaders = responseHeaders,
                        StatusCode = response.StatusCode,
                        StatusDescription = response.StatusDescription,
                        HttpProtocolVersion = response.ProtocolVersion.ToString()
                    };

                    searchResult.Process();
                }
            }

            return searchResult;
        }

    }
}
