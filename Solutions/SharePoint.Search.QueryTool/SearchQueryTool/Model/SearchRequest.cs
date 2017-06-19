using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace SearchQueryTool.Model
{
    public enum HttpMethodType
    {
        Get,
        Post
    }

    public enum AcceptType
    {
        Json,
        Xml
    }

    public enum AuthenticationType
    { 
        CurrentUser,
        Windows,
        Forms,
        SPO,
        Anonymous,
        Forefront        
    }

    public abstract class SearchRequest
    {
        public const int DefaultTimeout = 30; // 30 seconds timout

        public string SharePointSiteUrl { get; set; }
        public string QueryText { get; set; }
        public int? Timeout { get; set; }

        public HttpMethodType HttpMethodType { get; set; }
        public AcceptType AcceptType { get; set; }
        public AuthenticationType AuthenticationType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public SecureString SecurePassword { get; set; }
        [XmlIgnore] 
        public CookieCollection Cookies { get; set; }

        public abstract Uri GenerateHttpGetUri();
        public abstract Uri GenerateHttpPostUri();
        public abstract string GenerateHttpPostBodyPayload();
        
        public override string ToString()
        {
            return GenerateHttpGetUri().ToString();
        }

        protected static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }
    }
}
