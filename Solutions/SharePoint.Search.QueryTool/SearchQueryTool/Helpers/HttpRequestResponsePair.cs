using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SearchQueryTool.Helpers
{
    public class HttpRequestResponsePair : Tuple<HttpWebRequest, HttpWebResponse, string>
    {
        public HttpRequestResponsePair(HttpWebRequest request, HttpWebResponse response)
            : this(request, response, null)
        { }

        public HttpRequestResponsePair(HttpWebRequest request, HttpWebResponse response, string requestContent)
            : base(request, response, requestContent)
        { }
    }
}
