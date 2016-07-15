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

using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace Tip.Mvc.Middleware
{
    /// <summary>
    /// Adds CorrelationID to Request and Response Headers
    /// </summary>
    public class CorrelationMiddleware : OwinMiddleware
    {
        #region Constructor
        /// <summary>
        /// Default Contructor
        /// </summary>
        /// <param name="next"></param>
        public CorrelationMiddleware(OwinMiddleware next) : base(next)
        {

        }
        #endregion

        public override async Task Invoke(IOwinContext context)
        {
            this.HandleCorrelationHeaders(context);
            await Next.Invoke(context);
        }

        /// <summary>
        /// Handles generating the Headers
        /// </summary>
        /// <param name="context"></param>
        private void HandleCorrelationHeaders(IOwinContext context)
        {
            Guid _correlationId;
            string[] _correlationIdAsString;

            if (context.Request.Headers.TryGetValue(Constants.HTTPHeaders.CORRELATION_ID_HEADER, out _correlationIdAsString))
            {
                _correlationId = Guid.Parse(_correlationIdAsString[0]);
            }
            else
            {
                _correlationId = Guid.NewGuid();
                context.Request.Headers.Append(Constants.HTTPHeaders.CORRELATION_ID_HEADER, _correlationId.ToString());
            }

            context.Response.OnSendingHeaders((state) =>
            {
                context.Response.Headers.Set(Constants.HTTPHeaders.CORRELATION_ID_HEADER,  _correlationId.ToString());
            }, null);

        }
    }
}
