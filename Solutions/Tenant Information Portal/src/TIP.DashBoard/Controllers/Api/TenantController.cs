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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TIP.Common.Configuration;
using TIP.Common.Exceptions;
using TIP.Common.Services.Tenant;

namespace TIP.Dashboard.Controllers.Api
{
    /// <summary>
    /// Api to interact with Tenant Information
    /// </summary>
    [Authorize]
    public class TenantController : ApiController
    {
        #region Instance Members
        AppConfig _appConfig = ConfigurationFactory.Instance.GetApplicationConfiguration();
        #endregion

        /// <summary>
        /// Gets all Service Principals
        /// </summary>
        /// <returns></returns>
        [Route("api/tenant")]
        public IHttpActionResult GetTenantInformation()
        {
            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);
            try
            {
                TenantFactory _f = new TenantFactory();
                var _manager = _f.CreateInstance(_client);
                var _tenantInfo = _manager.GetTenantInformation();
                return Ok(_tenantInfo);
            }
            catch (TIPException _ex)
            {
                var _errorResponse = new ErrorResponse();
                _errorResponse.Error = _ex.Error;
                return Content(HttpStatusCode.InternalServerError, _errorResponse);
            }
        }
    }
}
