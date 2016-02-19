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

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.Data.OData;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Online.Applications.Core;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using TIP.Common.Exceptions;

namespace TIP.Common.Services.Principals.Internal
{
    internal class ServicePrincipalManager : ActiveDirectoryAbstractService, IServicePrincipalManager
    {
        #region Constructor
        internal ServicePrincipalManager(IClient client)
        {
            this.Client = client;
        }
        #endregion

        public IList<IAzureServicePrincipal> GetAllPrincipals()
        {
            List<IAzureServicePrincipal> _spPrincipals = new List<IAzureServicePrincipal>();
            var _client = this.GetActiveDirectoryClient();
            ///Get principals
            /// 
            IPagedCollection<IServicePrincipal> _principals = null;
            try
            {
                _principals = _client.ServicePrincipals.Take(999).ExecuteAsync().Result;
                if (_principals != null)
                {
                    do
                    {
                        List<IServicePrincipal> princList = _principals.CurrentPage.ToList();
                        foreach (IServicePrincipal princ in princList)
                        {
                            var _spPrincipal = new Internal.ServicePrincipal();
                            _spPrincipal.PrincipalNames = princ.ServicePrincipalNames;
                            _spPrincipal.AppId = princ.AppId;
                            _spPrincipal.DisplayName = princ.DisplayName;
                            _spPrincipal.ReplyUrls = princ.ReplyUrls;

                            var _creds = princ.KeyCredentials;
                            if (_creds.Count != 0)
                            {
                                var _keyCredential = _creds.FirstOrDefault();
                                _spPrincipal.EndDate = _keyCredential.EndDate;
                            }
                            _spPrincipals.Add(_spPrincipal);
                        }

                        _principals = _principals.GetNextPageAsync().Result;
                    } while (_principals != null);
                }
            }
            catch (Exception _ex)
            {
                //TOO LOGGING
                throw;
            }
            return _spPrincipals;
        }

        public IList<IAzureServicePrincipal> GetExpiredPrincipals()
        {
            List<IAzureServicePrincipal> _spPrincipals = new List<IAzureServicePrincipal>();
            var _client = this.GetActiveDirectoryClient();

            ///Get principals
            IPagedCollection<IServicePrincipal> _principals = null;
            try
            {
                _principals = _client.ServicePrincipals.Take(999).ExecuteAsync().Result;
                if (_principals != null)
                {
                    do
                    {
                        List<IServicePrincipal> princList = _principals.CurrentPage.Where(
                            x => x.KeyCredentials.Count > 0).ToList();
                        foreach (IServicePrincipal princ in princList)
                        {
                            var _spPrincipal = new Internal.ServicePrincipal();
                            _spPrincipal.PrincipalNames = princ.ServicePrincipalNames;
                            _spPrincipal.AppId = princ.AppId;
                            _spPrincipal.DisplayName = princ.DisplayName;

                            var _creds = princ.KeyCredentials.Where(kc => kc.EndDate < DateTime.Now).ToList();
                            if (_creds.Count != 0)
                            {
                                var _keyCredential = _creds.FirstOrDefault();
                                _spPrincipal.EndDate = _keyCredential.EndDate;
                                _spPrincipal.ReplyUrls = princ.ReplyUrls;
                                _spPrincipals.Add(_spPrincipal);
                            }

                        }
                        _principals = _principals.GetNextPageAsync().Result;
                    } while (_principals != null);
                }
            }

            catch(AggregateException ae)
            {
                //TOO LOGGING
                //       ae.Handle(HandleException);

                this.ExceptionHandler(ae);             
            }
            return _spPrincipals;

        }

        public IList<IAzureServicePrincipal> GetExpiredPrincipalsInDays(double numberOfDays)
        {
            List<IAzureServicePrincipal> _spPrincipals = new List<IAzureServicePrincipal>();
            var _client = this.GetActiveDirectoryClient();

            IPagedCollection<IServicePrincipal> _principals = null;
            try
            {
                _principals = _client.ServicePrincipals.Take(999).ExecuteAsync().Result;
                if (_principals != null)
                {
                    do
                    {
                        List<IServicePrincipal> princList = _principals.CurrentPage.Where(
                         x => x.KeyCredentials.Count > 0).ToList();
                        foreach (IServicePrincipal princ in princList)
                        {
                            var _spPrincipal = new Internal.ServicePrincipal();
                            _spPrincipal.PrincipalNames = princ.ServicePrincipalNames;
                            _spPrincipal.AppId = princ.AppId;
                            _spPrincipal.DisplayName = princ.DisplayName;

                            var _creds = princ.KeyCredentials.Where(kc => kc.EndDate >= DateTime.Now && kc.EndDate <= DateTime.Now.AddDays(numberOfDays)).ToList();
                            if (_creds.Count != 0)
                            {
                                var _keyCredential = _creds.FirstOrDefault();
                                _spPrincipal.EndDate = _keyCredential.EndDate;
                                _spPrincipal.ReplyUrls = princ.ReplyUrls;
                                _spPrincipals.Add(_spPrincipal);
                            }
                        }
                        _principals = _principals.GetNextPageAsync().Result;
                    } while (_principals != null);
                }
            }
            catch (Exception _ex)
            {
                //TOO LOGGING
                throw;
            }
            return _spPrincipals;
        }

        public IAzureServicePrincipal GetPrincipalById(string id)
        {
            var _client = this.GetActiveDirectoryClient();

            try
            {
                var _principals = _client.ServicePrincipals.Where(p => p.AppId == id).ExecuteAsync().Result;

                var _principal = _principals.CurrentPage.FirstOrDefault();
                if (_principal != null)
                {
                    var _spPrincipal = new Internal.ServicePrincipal();
                    _spPrincipal.AppId = _principal.AppId;
                    _spPrincipal.DisplayName = _principal.DisplayName;
                    _spPrincipal.PrincipalNames = _principal.ServicePrincipalNames;

                    var _creds = _principal.KeyCredentials.ToList();
                    if (_creds.Count != 0)
                    {
                        var _keyCredential = _creds.FirstOrDefault();
                        _spPrincipal.EndDate = _keyCredential.EndDate;
                    }
                    _spPrincipal.ReplyUrls = _principal.ReplyUrls;
                    return _spPrincipal;
                }
                else
                { return null; }

            }
            catch (Exception _ex)
            {
                //TOO LOGGING
                throw;
            }
        }

        public IAzureServicePrincipal GetPrincipalByName(string name)
        {
            var _client = this.GetActiveDirectoryClient();

            try
            {
                var _principals = _client.ServicePrincipals.Where(p => p.AppDisplayName.Contains(name)).ExecuteAsync().Result;

                var _principal = _principals.CurrentPage.FirstOrDefault();
                if (_principal != null)
                {
                    var _spPrincipal = new Internal.ServicePrincipal();
                    _spPrincipal.AppId = _principal.AppId;
                    _spPrincipal.DisplayName = _principal.DisplayName;
                    _spPrincipal.PrincipalNames = _principal.ServicePrincipalNames;

                    var _creds = _principal.KeyCredentials.ToList();
                    if (_creds.Count != 0)
                    {
                        var _keyCredential = _creds.FirstOrDefault();
                        _spPrincipal.EndDate = _keyCredential.EndDate;
                    }

                    _spPrincipal.ReplyUrls = _principal.ReplyUrls;
                    return _spPrincipal;
                }
                else
                { return null; }

            }
            catch (Exception _ex)
            {
                //TOO LOGGING
                throw;
            }
        }

        internal void ExceptionHandler (Exception exception)
        {
            var _exception = exception.GetBaseException() as ODataErrorException;
            AdalServiceException _serviceException = exception.GetBaseException() as AdalServiceException;

            var _innerException = _exception.InnerException;

            if (_exception != null)
            {
                throw new TIPException(
                        new Error
                        {
                            Code = _exception.Error.ErrorCode,
                            Message = _exception.Message,
                        },
                        _exception.InnerException);
           }

            throw new TIPException(
                 new Error
                 {
                     Code = _exception.Error.ErrorCode,
                     Message = _exception.Message,
                 }, _exception.InnerException);
        }

        internal bool HandleException(Exception exception)
        {

            Nullable<HttpStatusCode> httpStatusCode = null;

            DataServiceRequestException requestException = exception as DataServiceRequestException;
            if (requestException != null)
            {
                OperationResponse opResponse = requestException.Response.FirstOrDefault();
                httpStatusCode = opResponse != null
                    ? (HttpStatusCode)opResponse.StatusCode
                    : (HttpStatusCode)requestException.Response.BatchStatusCode;
            }

            DataServiceClientException clientException = exception as DataServiceClientException;
            if (!httpStatusCode.HasValue && clientException != null)
            {
                httpStatusCode = (HttpStatusCode)clientException.StatusCode;
            }

            DataServiceQueryException queryException = exception as DataServiceQueryException;
            if (!httpStatusCode.HasValue && queryException != null)
            {
                httpStatusCode = (HttpStatusCode)queryException.Response.StatusCode;
            }

            ODataErrorException odataException = exception as ODataErrorException;
            if (!httpStatusCode.HasValue && odataException != null)
            {
                var _errorCode = odataException.Error;
            }

            if (exception is AdalException || exception is ODataErrorException)
            {
               
                return false;
            }

            return true;
        }

    }
}
