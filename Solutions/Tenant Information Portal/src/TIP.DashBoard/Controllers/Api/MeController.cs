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
using TIP.Common.Services.Applications;

namespace TIP.Dashboard.Controllers.Api
{
    /// <summary>
    /// 
    /// </summary>
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
