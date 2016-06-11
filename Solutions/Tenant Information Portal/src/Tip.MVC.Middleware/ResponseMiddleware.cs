// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2016 Office Developer
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
// ------------------------------------------------------------------------------

using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Owin;

namespace Tip.Mvc.Middleware
{
    /// <summary>
    /// Middleware to add ElapsedMilliseconds in the response headers
    /// </summary>
    public class ResponseMiddleware : OwinMiddleware
    {
        #region Constructor
        /// <summary>
        /// Default Contructor
        /// </summary>
        /// <param name="next"></param>
        public ResponseMiddleware(OwinMiddleware next) : base(next)
        {
            
        }
        #endregion

        #region
        /// <summary>
        /// Adds X-RequestDuration in the response header for the request duration in Milliseconds
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext context)
        {
            this.HandleDurationResponseHeader(context);
            await Next.Invoke(context);
        }

        /// <summary>
        /// Sets the Response Duration Header
        /// </summary>
        /// <param name="context"></param>
        private void HandleDurationResponseHeader(IOwinContext context)
        {
            var _sw = Stopwatch.StartNew();
            context.Response.OnSendingHeaders((state) =>
            {
                _sw.Stop();
                var _responseTime = string.Format("{0}{1}", _sw.ElapsedMilliseconds.ToString(), "ms");
                context.Response.Headers.Add(Constants.HTTPHeaders.DURATION_RESPONSE_HEADER, new string[] { _responseTime });

            }, null);

        }
        #endregion
    }
}
