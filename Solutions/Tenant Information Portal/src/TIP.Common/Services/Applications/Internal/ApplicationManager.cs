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
using Microsoft.Online.Applications.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIP.Common.Services.Applications.Internal
{
    /// <summary>
    /// Internal class to interact with the Graph API and Applications that have been registered in Azure AD.
    /// </summary>
    internal class ApplicationManager : ActiveDirectoryAbstractService, IApplicationManager
    {
        internal ApplicationManager(IClient client)
        {
            this.Client = client;
        }
        #region IApplicationManager Members
        public List<IApplicationInformation> GetAllApplications()
        {
            List<IApplicationInformation> _applications = new List<IApplicationInformation>();
            var _client = this.GetActiveDirectoryClient();

            IPagedCollection<IApplication> _azureApplications = null;

            try
            {
                _azureApplications = _client.Applications.Take(999).ExecuteAsync().Result;
                if(_azureApplications != null)
                {
                    foreach(var _azureApp in _azureApplications.CurrentPage.ToList())
                    {
                        var _appInfo = new ApplicationInformation
                        {
                            
                            AppId = _azureApp.AppId,
                            DiplayName = _azureApp.DisplayName,
                            ReplyUrls = _azureApp.ReplyUrls,
                            IdentifierUris = _azureApp.IdentifierUris
                        };
                  
                       
                        if(_azureApp.PasswordCredentials.Count != 0) {
                            _appInfo.EndDate = _azureApp.PasswordCredentials.FirstOrDefault().EndDate;
                        }

                        if(_azureApp.KeyCredentials.Count != 0) {
                            _appInfo.EndDate = _azureApp.KeyCredentials.FirstOrDefault().EndDate;
                        }
                        _applications.Add(_appInfo);
                    }
                }

                return _applications;

            }
            catch(Exception ex)
            {
                throw;
            }

        }

        public IList<IApplicationInformation> GetAllExpired()
        {
            List<IApplicationInformation> _applications = new List<IApplicationInformation>();
            var _client = this.GetActiveDirectoryClient();
            IPagedCollection<IApplication> _azureApplications = null;

            try
            {
                _azureApplications = _client.Applications.Take(999).ExecuteAsync().Result;
                if (_azureApplications != null)
                {
                    List<IApplication> _apps = _azureApplications.CurrentPage.Where(x => x.PasswordCredentials.Count > 0).ToList();
                    var _expiredApps = _apps.Where(kc => kc.PasswordCredentials.FirstOrDefault().EndDate < DateTime.Now).ToList();

                    foreach (var _expiredApp in _expiredApps)
                    {

                        var _appInfo = new ApplicationInformation
                        {

                            AppId = _expiredApp.AppId,
                            DiplayName = _expiredApp.DisplayName,
                            ReplyUrls = _expiredApp.ReplyUrls,
                            IdentifierUris = _expiredApp.IdentifierUris
                        };

                        if (_expiredApp.PasswordCredentials.Count != 0)
                        {
                            _appInfo.EndDate = _expiredApp.PasswordCredentials.FirstOrDefault().EndDate;
                        }
                        _applications.Add(_appInfo);
                    }
                }
                return _applications;
            }
            catch(Exception _ex)
            {
                throw;
            }
        }

        public IApplicationInformation GetApplicationById(string id)
        {
            var _client = this.GetActiveDirectoryClient();
            ApplicationInformation _appInfo = null;

            try
            {
                var _aadApplications = _client.Applications.
                    Where(app => app.AppId.Equals(id))
                    .ExecuteAsync()
                    .Result
                    .CurrentPage.ToList();
                
                // should only find one with the id 
                if(_aadApplications != null && _aadApplications.Count == 1)
                {
                    var _azureApplication = _aadApplications.First();

                    if (_azureApplication != null)
                    {
                        _appInfo = new ApplicationInformation
                        {
                            AppId = _azureApplication.AppId,
                            DiplayName = _azureApplication.DisplayName,
                            ReplyUrls = _azureApplication.ReplyUrls,
                            IdentifierUris = _azureApplication.IdentifierUris
                        };

                        if (_azureApplication.PasswordCredentials.Count != 0)
                        {
                            _appInfo.EndDate = _azureApplication.PasswordCredentials.FirstOrDefault().EndDate;
                        }

                        if (_azureApplication.KeyCredentials.Count != 0)
                        {
                            _appInfo.EndDate = _azureApplication.KeyCredentials.FirstOrDefault().EndDate;
                        }
                    }
                }

                return _appInfo;
            }
            catch(Exception ex)
            {
                throw;
            }
   
        }

        public IList<IApplicationInformation> GetExpiredApplicationInDays(double numberOfDays)
        {
            List<IApplicationInformation> _applications = new List<IApplicationInformation>();
            var _client = this.GetActiveDirectoryClient();
            IPagedCollection<IApplication> _azureApplications = null;

            try
            {
                _azureApplications = _client.Applications.Take(999).ExecuteAsync().Result;
                if (_azureApplications != null)
                {
                    List<IApplication> _apps = _azureApplications.CurrentPage.Where(x => x.PasswordCredentials.Count > 0).ToList();
                    //var _expiredApps = _apps.Where(x => x.PasswordCredentials.FirstOrDefault().EndDate >= DateTime.Now || x.KeyCredentials.FirstOrDefault().EndDate >= DateTime.Now && 
                    //        x.KeyCredentials.FirstOrDefault().EndDate <= DateTime.Now.AddDays(numberOfDays) ||
                    //        x.PasswordCredentials.FirstOrDefault().EndDate <= DateTime.Now.AddDays(numberOfDays)).ToList();

                    var _expiredApps = _apps.Where(x => x.PasswordCredentials.FirstOrDefault().EndDate >= DateTime.Now  &&  x.PasswordCredentials.FirstOrDefault().EndDate <= DateTime.Now.AddDays(numberOfDays)).ToList();
                    foreach (var _expiredApp in _expiredApps)
                    {
                        var _appInfo = new ApplicationInformation
                        {
                            AppId = _expiredApp.AppId,
                            DiplayName = _expiredApp.DisplayName,
                            ReplyUrls = _expiredApp.ReplyUrls,
                            IdentifierUris = _expiredApp.IdentifierUris
                        };

                        if (_expiredApp.PasswordCredentials.Count != 0)
                        {
                            _appInfo.EndDate = _expiredApp.PasswordCredentials.FirstOrDefault().EndDate;
                        }

                        _applications.Add(_appInfo);
                    }
                }
                return _applications;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
