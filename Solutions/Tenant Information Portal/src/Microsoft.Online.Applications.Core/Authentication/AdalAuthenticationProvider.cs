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



using System;
using System.Threading.Tasks;

namespace Microsoft.Online.Applications.Core.Authentication
{
    /// <summary>
    /// Abstract class for AdalAutehnticationProviders
    /// </summary>
    public abstract class AdalAuthenticationProvider : IAuthenticationProvider
    {
        #region Instance Members
        internal AuthenticationContextWrapper _authenticationContextWrapper;
        protected ServiceInformation _serviceInfo;
        Guid _correlationID;
        #endregion

        #region Constructor
        protected AdalAuthenticationProvider(ServiceInformation serviceinfo, AccountSession accountSession = null)
        {
            this.ServiceInformation = serviceinfo;
            this.AccountSession = accountSession;
        }
        #endregion


        #region Properties
        public Guid CorrelationId
        {
            get
            {
                this._correlationID = this._correlationID != Guid.Empty ? this._correlationID : Guid.NewGuid();
                return this._correlationID;
            }
            set { this._correlationID = value; }
        }
        #endregion


        /// <summary>
        /// Gets or sets the <seealso cref="ServiceInformation"/>
        /// </summary>
        internal ServiceInformation ServiceInformation
        {
            get { return this._serviceInfo; }
            set
            {

                this._serviceInfo = value;
                this._serviceInfo.AuthenticationServiceUrl = _serviceInfo.AuthenticationServiceUrl.Replace("common", _serviceInfo.Tenant);
                string authority = _serviceInfo.AuthenticationServiceUrl.Replace("common", _serviceInfo.Tenant);
                this._authenticationContextWrapper = new AuthenticationContextWrapper(this._serviceInfo.AuthenticationServiceUrl, false);
            }
        }

        internal AuthenticationContextWrapper Authenticator
        { 
            get { return this._authenticationContextWrapper; } 
            set { this._authenticationContextWrapper = value; }
        }

        public AccountSession AccountSession { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<AccountSession> AuthenticateAsync()
        {
            if(this.AccountSession != null && !this.AccountSession.isExpiring)
            {
                return this.AccountSession;
            }

            var _authenticationResult = await this.AuthenticateResourceAsync(this.ServiceInformation.ServiceResource);

            if(_authenticationResult == null)
            {
                this.AccountSession = null;
                return this.AccountSession;
            }

            this.AccountSession = new AdalSession
            {
                AccessToken = _authenticationResult.AccessToken,
                AccessTokenType = _authenticationResult.AccessTokenType,
                ExpiresOnUtc = _authenticationResult.ExpiresOn,
                AccountType = AccountType.ActiveDirectory,
                ClientId = this.ServiceInformation.ClientID,
                RefreshToken = _authenticationResult.RefreshToken,
            };

            return this.AccountSession;
        }

        public AccountSession Authenticate()
        {
            if (this.AccountSession != null && !this.AccountSession.isExpiring)
            {
                return this.AccountSession;
            }

            var _authenticationResult = this.AuthenticateResource(this.ServiceInformation.ServiceResource);

            if (_authenticationResult == null)
            {
                this.AccountSession = null;
                return this.AccountSession;
            }

            this.AccountSession = new AdalSession
            {
                AccessToken = _authenticationResult.AccessToken,
                AccessTokenType = _authenticationResult.AccessTokenType,
                ExpiresOnUtc = _authenticationResult.ExpiresOn,
                AccountType = AccountType.ActiveDirectory,
                ClientId = this.ServiceInformation.ClientID,
                RefreshToken = _authenticationResult.RefreshToken,
            };

            return this.AccountSession;
        }

        protected abstract Task<IAuthenticationResult> AuthenticateResourceAsync(string resource);
        protected abstract IAuthenticationResult AuthenticateResource(string resource);

    }
}
