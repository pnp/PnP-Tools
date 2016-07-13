// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2015 Office Developer
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

using Microsoft.Online.Applications.Core;
using Microsoft.Online.Applications.Core.Clients;
using Microsoft.Online.Applications.Core.Configuration;
using System;
using System.Net;
using System.Web.Http;
using TIP.Common.Configuration;
using TIP.Common.Exceptions;
using TIP.Common.Services.Applications;

namespace TIP.Dashboard.Controllers.Api
{
    [Authorize]
    public class MeController : ApiController
    {
        #region Instance Members

        AppConfig _appConfig = ConfigurationFactory.Instance.GetApplicationConfiguration();
        #endregion

        /// <summary>
        /// Gets all Service Principals
        /// </summary>
        /// <returns></returns>
        [Route("api/me")]
        public IHttpActionResult GetMe()
        {
            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);

            try
            {
                var _applicationFactory = new ApplicationFactory();
                var _manager = _applicationFactory.CreateInstance(_client);
                var _app = _manager.GetApplicationById(_client.ServiceInformation.ClientID);
                return Ok(_app);
             
            }
            catch (Exception _ex)
            {
                var _response = new ErrorResponse();
                _response.Error = new Error
                {
                    Code = Common.Constants.ErrorCodes.GENERAL,
                    Message = _ex.Message
                };
                return Content(HttpStatusCode.InternalServerError, _response);
            }
        }
    }
}
