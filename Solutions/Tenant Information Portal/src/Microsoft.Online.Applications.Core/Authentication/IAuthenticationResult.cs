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
    /// <summary>
    /// Contains the results of one token acquisition operation.
    /// </summary>
    public interface IAuthenticationResult
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the type of the access token.
        /// </summary>
        string AccessTokenType { get; }

        /// <summary>
        /// Gets the point in time in which the access token expires.
        /// This value is calculated based on current UTC time.
        /// </summary>
        DateTimeOffset ExpiresOn { get; }

        /// <summary>
        /// Gets the ID token.
        /// </summary>
        string IdToken { get; }

        /// <summary>
        /// Gets a value indicating whether or not the refresh token can be used for requesting
        /// access tokens for other resources.
        /// </summary>
        bool IsMultipleResourceRefreshToken { get; }

        /// <summary>
        /// Gets the refresh token for the current access token.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Gets an identifier for the tenant from which the access token was acquired.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Gets user information, such as user ID.
        /// </summary>
        UserInfo UserInfo { get; }
    }
}
