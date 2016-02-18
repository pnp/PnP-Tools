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
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Online.Applications.Core.Authentication
{
    /// <summary>
    /// TODO
    /// </summary>
    public class AdalClientAuthenticationProvider : AdalAuthenticationProvider
    {
        #region Constructor
        /// <summary>
        /// ToDo
        /// </summary>
        /// <param name="serviceInfo"></param>
        public AdalClientAuthenticationProvider(ServiceInformation serviceInfo, AccountSession accountSession = null) : base(serviceInfo, accountSession)
        {
         
        }
        #endregion

        protected override async Task<IAuthenticationResult> AuthenticateResourceAsync(string resource)
        {
            IAuthenticationResult _result = null;

            var _clientCredential = string.IsNullOrEmpty(this.ServiceInformation.ClientSecret) ? null : new ClientCredential(this.ServiceInformation.ClientID, this.ServiceInformation.ClientSecret);

            try
            {
                this.Authenticator.CorrelationId = this.CorrelationId;
                _result =_clientCredential == null ? await this.Authenticator.AcquireTokenAsync(resource, this.ServiceInformation.ClientID) : await this.Authenticator.AcquireTokenAsync(resource, _clientCredential);
            }
            catch(AdalException _adalException)
            {
                Console.Write(_adalException.ToString());
                throw;
            }
            catch(Exception _exception)
            {
                Console.Write(_exception.ToString());
                throw;
            }

            return _result;
        }

        protected override IAuthenticationResult AuthenticateResource(string resource)
        {
            IAuthenticationResult _result = null;

            var _clientCredential = string.IsNullOrEmpty(this.ServiceInformation.ClientSecret) ? null : new ClientCredential(this.ServiceInformation.ClientID, this.ServiceInformation.ClientSecret);

            try
            {
                this.Authenticator.CorrelationId = this.CorrelationId;
                _result = this.Authenticator.AcquireToken(resource, _clientCredential);
            }
            catch (AdalException _adalException)
            {
                Console.Write(_adalException.ToString());
                throw;
            }
            catch (Exception _exception)
            {
                Console.Write(_exception.ToString());
                throw;
            }

            return _result;
        }
    }
}
