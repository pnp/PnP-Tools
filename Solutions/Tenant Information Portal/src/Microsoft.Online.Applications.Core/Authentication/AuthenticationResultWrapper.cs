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
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Online.Applications.Core.Authentication
{
 
    public sealed class AuthenticationResultWrapper : IAuthenticationResult
    {
        private AuthenticationResult _authenticationResult;

        #region Constructors
        public AuthenticationResultWrapper(AuthenticationResult authenticationResult)
        {
            this._authenticationResult = authenticationResult;
        }
        #endregion

        public string AccessToken
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.AccessToken : string.Empty;
            }
        }

        public string AccessTokenType
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.AccessTokenType : string.Empty;
            }
        }

        public DateTimeOffset ExpiresOn
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.ExpiresOn : default(DateTimeOffset); 
            }
        }

        public string IdToken
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.IdToken : string.Empty;
            }
        }

        public bool IsMultipleResourceRefreshToken
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.IsMultipleResourceRefreshToken : false;
            }
        }

        public string RefreshToken
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.RefreshToken : string.Empty;
            }
        }

        public string TenantId
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.TenantId : string.Empty;
            }
        }

        public UserInfo UserInfo
        {
            get
            {
                return (this._authenticationResult != null) ? this._authenticationResult.UserInfo : null;
            }
        }
    }
}
