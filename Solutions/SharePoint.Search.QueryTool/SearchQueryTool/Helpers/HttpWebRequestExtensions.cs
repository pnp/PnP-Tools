using SearchQueryTool.SPAuthenticationClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SearchQueryTool.Helpers
{
    public static class HttpWebRequestExtensions
    {
        public static void ApplyDefaultCredentials(this HttpWebRequest webRequest)
        {
            webRequest.UseDefaultCredentials = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
        }

        public static void ApplyWindowsCredentials(this HttpWebRequest webRequest, string username, SecureString password)
        {
            if (!String.IsNullOrEmpty(username))
            {
                string[] usernameParts = username.Split(new char[] { '\\' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (usernameParts.Length == 2)
                {
                    webRequest.Credentials = new NetworkCredential(usernameParts[1], password, usernameParts[0]);
                }
                else if (usernameParts.Length == 1)
                {
                    webRequest.Credentials = new NetworkCredential(usernameParts[0], password);
                }

                webRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            }
        }

        public static void ApplyFormsCredentials(this HttpWebRequest webRequest, string sharepointSiteUrl, string username, string password)
        {
            if (!String.IsNullOrWhiteSpace(username))
            {
                if (!String.IsNullOrWhiteSpace(sharepointSiteUrl))
                {
                    string url = sharepointSiteUrl;

                    if (!url.EndsWith("/"))
                        url += "/";

                    url = String.Format("{0}_vti_bin/authentication.asmx", url);

                    EndpointAddress authServiceAddress = new EndpointAddress(url);

                    using (AuthenticationSoapClient client = new AuthenticationSoapClient(new BasicHttpBinding(), authServiceAddress))
                    {
                        using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                        {
                            var result = client.Login(username, password.ToString());
                            if (result.ErrorCode == SPAuthenticationClient.LoginErrorCode.NoError)
                            {
                                HttpResponseMessageProperty respProp = (HttpResponseMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpResponseMessageProperty.Name];

                                foreach (string headerName in respProp.Headers.AllKeys)
                                {
                                    if (headerName == "Set-Cookie")
                                    {
                                        string authCookie = respProp.Headers[headerName];
                                        webRequest.Headers.Add(HttpRequestHeader.Cookie, authCookie);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                string errorMsg = String.Format("Login request to authentication service at {0} returned error code: {1}", url, result.ErrorCode.ToString());
                                throw new InvalidOperationException(errorMsg);
                            }
                        }
                    }
                }
            }
        }

        public static void ApplyCookieCredentials(this HttpWebRequest webRequest, CookieCollection authCookies)
        {
            if (authCookies != null)
            {
                webRequest.CookieContainer = new CookieContainer();
                foreach (Cookie cookie in authCookies)
                {
                    webRequest.CookieContainer.Add(cookie);
                }
            }
        }
    }
}
