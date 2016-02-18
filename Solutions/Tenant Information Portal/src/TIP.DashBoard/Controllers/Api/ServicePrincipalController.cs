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
using System.Net;
using System.Web.Http;
using TIP.Common.Configuration;
using TIP.Common.Exceptions;
using TIP.Common.Services.Principals;

namespace TIP.Dashboard.Controllers.Api
{
    /// <summary>
    /// Api to interact with Service Principals
    /// </summary>
    [Authorize]
    public class ServicePrincipalController : ApiController
    {
        #region Instance Members
        AppConfig _appConfig = ConfigurationFactory.Instance.GetApplicationConfiguration();
        #endregion
    
        /// <summary>
        /// Gets a Service Principal by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/servicePrincipal/{id}")]
        public IHttpActionResult GetPrincipalById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id was supplied");
            }

            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);

            try
            {
                ServicePrincipalFactory _f = new ServicePrincipalFactory();
                var _manager = _f.CreateInstance(_client);
                var _servicePrincipals = _manager.GetPrincipalById(id);
                return Ok(_servicePrincipals);
            }
            catch (TIPException _ex)
            {
                var _errorResponse = new ErrorResponse();
                _errorResponse.Error = _ex.Error;
                return Content(HttpStatusCode.InternalServerError, _errorResponse);
            }
        }

        /// <summary>
        /// Gets all Service Principals
        /// </summary>
        /// <returns></returns>
        [Route("api/servicePrincipals")]
        public IHttpActionResult GetAllPrincipals()
        {
            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);

            try
            {
                ServicePrincipalFactory _f = new ServicePrincipalFactory();
                var _manager = _f.CreateInstance(_client);
                var _servicePrincipals = _manager.GetAllPrincipals();
                return Ok(_servicePrincipals);
            }
            catch (TIPException _ex)
            {
                var _errorResponse = new ErrorResponse();
                _errorResponse.Error = _ex.Error;
                return Content(HttpStatusCode.InternalServerError, _errorResponse);
            }

        }

        /// <summary>
        /// Gets all Expired Principals
        /// </summary>
        /// <returns></returns>
        [Route("api/servicePrincipal/getAllExpired")]
        [Authorize]
        public IHttpActionResult GetAllExpired()
        {
            try
            {
                AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);
                ServicePrincipalFactory _f = new ServicePrincipalFactory();
                var _manager = _f.CreateInstance(_client);
                var _servicePrincipals = _manager.GetExpiredPrincipals();
                return Ok(_servicePrincipals);

            }
            catch (TIPException ex)
            {
                var _response = new ErrorResponse();
                _response.Error = ex.Error;
                return Content(HttpStatusCode.InternalServerError, _response);
            }
        }

        /// <summary>
        /// Gets all Expired in Days
        /// </summary>
        /// <param name="InDays"></param>
        /// <returns></returns>
        [Route("api/servicePrincipal/getExpired/{InDays:int}")]
        public IHttpActionResult GetExpiredPrincipalsInDays(int InDays)
        {
            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);
            ServicePrincipalFactory _f = new ServicePrincipalFactory();
            var _manager = _f.CreateInstance(_client);
            var _servicePrincipals = _manager.GetExpiredPrincipalsInDays(InDays);
            return Ok(_servicePrincipals);
        }
    }
}
