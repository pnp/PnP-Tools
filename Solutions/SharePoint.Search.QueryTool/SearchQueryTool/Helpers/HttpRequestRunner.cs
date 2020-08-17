using System;
using System.IO;
using System.Net;
using System.Text;
using SearchQueryTool.Model;
using System.Security;

namespace SearchQueryTool.Helpers
{
    public class HttpRequestRunner
    {
        public static HttpRequestResponsePair RunWebRequest(SearchRequest searchRequest)
        {
            if (searchRequest.HttpMethodType == HttpMethodType.Get)
            {
                return RunGetWebRequest(searchRequest);
            }
            else // POST
            {
                return RunPostWebRequest(searchRequest);
            }
        }

        private static HttpRequestResponsePair RunGetWebRequest(SearchRequest searchRequest)
        {
            var request = CreateWebRequest(searchRequest.GenerateHttpGetUri(), 
                                           searchRequest.AcceptType, 
                                           searchRequest.Timeout.HasValue? searchRequest.Timeout.Value : SearchRequest.DefaultTimeout,
                                           searchRequest.AuthenticationType,
                                           searchRequest.SharePointSiteUrl,
                                           searchRequest.UserName,
                                           searchRequest.Password,
                                           searchRequest.SecurePassword,
                                           searchRequest.Cookies,
                                           searchRequest.Token);
            
            HttpWebResponse response = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException webEx)
            {
                if (webEx.Response == null)
                    throw;

                response = webEx.Response as HttpWebResponse;
            }

            return new HttpRequestResponsePair(request, response);
        }

        private static HttpRequestResponsePair RunPostWebRequest(SearchRequest searchRequest)
        {
            string xrequestDigestValue = GetXRequestDigestForPostRequest(searchRequest);

            var request = CreateWebRequest(searchRequest.GenerateHttpPostUri(),
                                           searchRequest.AcceptType,
                                           searchRequest.Timeout.HasValue ? searchRequest.Timeout.Value : SearchRequest.DefaultTimeout,
                                           searchRequest.AuthenticationType,
                                           searchRequest.SharePointSiteUrl,
                                           searchRequest.UserName,
                                           searchRequest.Password,
                                           searchRequest.SecurePassword,
                                           searchRequest.Cookies,
                                           searchRequest.Token);

            request.Method = "POST";
            request.ContentType = "application/json;odata=verbose;charset=utf-8";
            request.Headers["x-requestdigest"] = xrequestDigestValue;

            string payload = searchRequest.GenerateHttpPostBodyPayload();

            if (!String.IsNullOrEmpty(payload))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(payload);
                request.ContentLength = (long)bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            HttpWebResponse response = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException webEx)
            {
                response = webEx.Response as HttpWebResponse;
            }

            return new HttpRequestResponsePair(request, response, payload);
        }

        private static string GetXRequestDigestForPostRequest(SearchRequest searchRequest)
        {
            string digestValue = "";

            var request = CreateWebRequest(searchRequest.GenerateHttpPostUri(),
                                           searchRequest.AcceptType,
                                           searchRequest.Timeout.HasValue ? searchRequest.Timeout.Value : SearchRequest.DefaultTimeout,
                                           searchRequest.AuthenticationType,
                                           searchRequest.SharePointSiteUrl,
                                           searchRequest.UserName,
                                           searchRequest.Password,
                                           searchRequest.SecurePassword,
                                           searchRequest.Cookies,
                                           searchRequest.Token);
            
            request.Method = "POST";
            request.ContentType = "application/json;odata=verbose;charset=utf-8";
            request.ContentLength = 0;

            try
            {
                using (var response = (request as HttpWebRequest).GetResponse() as HttpWebResponse)
                {
                    foreach (var header in response.Headers.AllKeys)
                    {
                        if (header.ToLower() == "x-requestdigest")
                        {
                            digestValue = response.Headers[header];
                            break;
                        }
                    }
                }
            }
            catch (WebException webex) // expected 403 Forbidden
            {
                using (WebResponse response = webex.Response)
                {
                    HttpWebResponse httpResponse = response as HttpWebResponse;

                    if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        foreach (var header in response.Headers.AllKeys)
                        {
                            if (header.ToLower() == "x-requestdigest")
                            {
                                digestValue = response.Headers[header];
                                break;
                            }
                        }
                    }
                }
            }

            return digestValue;
        }

        private static HttpWebRequest CreateWebRequest(string uri, AcceptType acceptType, 
                                                       int timeout = SearchRequest.DefaultTimeout,
                                                       AuthenticationType authType = AuthenticationType.CurrentUser, 
                                                       string sharePointSiteUrl = null,
                                                       string username = null, 
                                                       string password = null,
                                                       SecureString securePassword = null,
                                                       CookieCollection authCookies = null,
                                                       string accessToken = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.Accept = acceptType == AcceptType.Json ? "application/json;odata=verbose;charset=utf-8" : "application/xml;charset=utf-8";

            request.Timeout = timeout * 1000;
            request.AllowAutoRedirect = true;

            if (authType == AuthenticationType.CurrentUser)
            {
                request.ApplyDefaultCredentials();
            }
            else if (authType == AuthenticationType.Windows)
            {
                if (String.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Parameter Username cannot be empty!");

                if (String.IsNullOrWhiteSpace(password) && securePassword == null)
                    throw new ArgumentException("Parameter Password cannot be empty!");

                request.ApplyWindowsCredentials(username, securePassword);
            }
            else if (authType == AuthenticationType.Forms)
            {
                if (String.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Parameter Username cannot be empty!");

                if (String.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Parameter Password cannot be empty!");

                request.ApplyFormsCredentials(sharePointSiteUrl, username, password);
            }
            else if (authType == AuthenticationType.SPO || authType == AuthenticationType.Forefront)
            {
                request.ApplyCookieCredentials(authCookies);
            }
            else if (authType == AuthenticationType.SPOManagement)
            {
                request.Headers.Add("Authorization", accessToken);
            }
            return request;
        }
    }
}
