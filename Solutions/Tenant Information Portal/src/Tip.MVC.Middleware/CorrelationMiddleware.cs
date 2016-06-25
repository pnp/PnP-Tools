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

        private void HandleCorrelationHeaders(IOwinContext context)
        {
            Guid _correlationID;

            if(context.Request.Headers.ContainsKey(Constants.HTTPHeaders.CORRELATION_ID_HEADER))
            {
                if (Guid.TryParse(context.Request.Headers.Get(Constants.HTTPHeaders.CORRELATION_ID_HEADER), out _correlationID)) { }         
            }
            else
            {
                _correlationID = new Guid();
                context.Request.Headers.Set(Constants.HTTPHeaders.CORRELATION_ID_HEADER, _correlationID.ToString());
            }

        }
    }
}
