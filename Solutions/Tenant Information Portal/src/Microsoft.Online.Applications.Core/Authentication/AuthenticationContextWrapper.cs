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
    public class AuthenticationContextWrapper : IAuthenticationContext
    {
        #region Instance Members
        AuthenticationContext _authenticationContext = null;
        Guid _correlationID;
        #endregion

        #region Contructor
        public AuthenticationContextWrapper(string authority)
        {
            this._authenticationContext = new AuthenticationContext(authority);
        }

        /// <summary>
        /// Constructor to create the context with the address of the authority and flag to turn address validation off. Using this constructor, address validation can be turned off. Make sure you are aware of the security implication of not validating the address.
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="validateAuthority"></param>
        public AuthenticationContextWrapper(string authority, bool validateAuthority)
        {
            this._authenticationContext = new AuthenticationContext(authority, validateAuthority);
        }


        #region Properties
        public Guid CorrelationId
        {
            get
            {
                return this._correlationID;
            }

            set
            {
                this._correlationID = value;
            }
        }
        #endregion


        public async Task<IAuthenticationResult> AcquireTokenAsync(string resource, ClientCredential clientCredential)
        {
            this._authenticationContext.CorrelationId = this.CorrelationId;
            var _result = await this._authenticationContext.AcquireTokenAsync(resource, clientCredential);

            return _result == null ? null : new AuthenticationResultWrapper(_result);
        }

        public Task<IAuthenticationResult> AcquireTokenAsync(string resource, string clientId)
        {
            return null;
         
        }

        public IAuthenticationResult AcquireToken(string resource, ClientCredential clientCredential)
        {
            this._authenticationContext.CorrelationId = this.CorrelationId;
            var _result = this._authenticationContext.AcquireToken(resource, clientCredential);

            return _result == null ? null : new AuthenticationResultWrapper(_result);
        }
        #endregion
    }
}
