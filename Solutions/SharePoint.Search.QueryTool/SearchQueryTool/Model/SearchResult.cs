using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SearchQueryTool.Model
{
    public abstract class SearchResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string HttpProtocolVersion { get; set; }
        public string ContentType { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public NameValueCollection ResponseHeaders { get; set; }
        public NameValueCollection RequestHeaders { get; set; }
        public string ResponseContent { get; set; }
        public Uri RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public string RequestContent { get; set; }

        public abstract void Process();
    }
}
