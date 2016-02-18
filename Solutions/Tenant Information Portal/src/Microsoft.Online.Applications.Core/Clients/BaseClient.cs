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



using System.Threading.Tasks;
using Microsoft.Online.Applications.Core.Authentication;
using Microsoft.Online.Applications.Core.Configuration;

namespace Microsoft.Online.Applications.Core
{
    public class BaseClient : IClient
    {
        #region Instance Members
        internal readonly AppConfig _appConfig;
        internal readonly IServiceInfoProvider _serviceInfoProvider;
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="appConfig"></param>
        /// <param name="credentialType"></param>
        /// <param name="serviceProvider"></param>
        public BaseClient(AppConfig appConfig, CredentialType credentialType, IServiceInfoProvider serviceProvider = null)
        {
            this._appConfig = appConfig;
            this.CredentialType = credentialType;
            this._serviceInfoProvider = serviceProvider ?? new ServiceProvider();
        }

        public IAuthenticationProvider AuthenticationProvider
        {
            get
            {
                if (this.ServiceInformation != null)
                {
                    return this.ServiceInformation.AuthenticationProvider;
                }
                return null;
            }
        }

        public ServiceInformation ServiceInformation {
            
            get;
            internal set;
        }

        public CredentialType CredentialType { get; private set; }

        public async Task<AccountSession> AuthenticateAsync()
        {
            if(this.ServiceInformation == null)
            {
                this.ServiceInformation = await this._serviceInfoProvider.CreateServiceInfo(this._appConfig, this.CredentialType);
            }

            var authResult = await this.ServiceInformation.AuthenticationProvider.AuthenticateAsync();

            return authResult;
        }
    }
}
