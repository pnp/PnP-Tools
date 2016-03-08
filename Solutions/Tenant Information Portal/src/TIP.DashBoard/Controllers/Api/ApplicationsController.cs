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
    public class ApplicationsController : ApiController
    {
        #region Instance Members
        AppConfig _appConfig = ConfigurationFactory.Instance.GetApplicationConfiguration();
        #endregion

        /// <summary>
        /// Gets all Principals
        /// </summary>
        /// <returns></returns>
        [Route("api/applications")]
        public IHttpActionResult GetApplications()
        {
            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);

            try
            {
                var _applicationFactory = new ApplicationFactory();
                var _manager = _applicationFactory.CreateInstance(_client);
                var _apps = _manager.GetAllApplications();
                return Ok(_apps);
            }
            catch(Exception _ex)
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

        [Route("api/applications/id/{id}")]
        public IHttpActionResult GetApplicationByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id was supplied");
            }

            AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);

            try
            {
                var _applicationFactory = new ApplicationFactory();
                var _manager = _applicationFactory.CreateInstance(_client);
                var _app = _manager.GetApplicationById(id);
                if(_app != null)
                {
                    return Ok(_app);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, string.Format("Application {0} was not found", id));
                }
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
        /// <summary>
        /// Gets all Expired Applications
        /// </summary>
        /// <returns></returns>
        [Route("api/applications/expired")]
        public IHttpActionResult GetExpired()
        {
            try
            {
                AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);
                var _applicationFactory = new ApplicationFactory();
                var _manager = _applicationFactory.CreateInstance(_client);
                var _apps = _manager.GetAllExpired();
                return Ok(_apps);
            }
            catch (TIPException ex)
            {
                var _response = new ErrorResponse();
                _response.Error = ex.Error;
                return Content(HttpStatusCode.InternalServerError, _response);
            }
            catch (Exception ex)
            {
                var _response = new ErrorResponse();
                _response.Error = new Error
                {
                    Code = Common.Constants.ErrorCodes.GENERAL,
                    Message = ex.Message
                };
                return Content(HttpStatusCode.InternalServerError, _response);
            }

        }


        /// <summary>
        /// Gets all Expired in Days
        /// </summary>
        /// <param name="InDays"></param>
        /// <returns></returns>
        [Route("api/applications/getExpired/{InDays:int}")]
        public IHttpActionResult GetExpiredApplicationsInDays(int InDays)
        {
            try
            {
                AdalClient _client = new AdalClient(_appConfig, CredentialType.Client, null);
                var _applicationFactory = new ApplicationFactory();
                var _manager = _applicationFactory.CreateInstance(_client);
                var _apps = _manager.GetExpiredApplicationInDays(InDays);
                return Ok(_apps);
            }
            catch (TIPException ex)
            {
                var _response = new ErrorResponse();
                _response.Error = ex.Error;
                return Content(HttpStatusCode.InternalServerError, _response);
            }
            catch (Exception ex)
            {
                var _response = new ErrorResponse();
                _response.Error = new Error
                {
                    Code = Common.Constants.ErrorCodes.GENERAL,
                    Message = ex.Message
                };
                return Content(HttpStatusCode.InternalServerError, _response);
            }

        }
    }
}
