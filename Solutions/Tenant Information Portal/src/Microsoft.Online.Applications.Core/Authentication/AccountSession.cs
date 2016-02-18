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

namespace Microsoft.Online.Applications.Core.Authentication
{
    /// <summary>
    /// TODO
    /// </summary>
    public class AccountSession
    {
        #region Properties
        /// <summary>
        /// Gets the Access Token requested. 
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets the type of the Access Token returned.
        /// </summary>
        public string AccessTokenType { get; set; }

        public AccountType AccountType { get; set; }
        
        /// <summary>
        /// Gets the ClientId that is associated with the Access Token requested.
        /// </summary>
        public string ClientId { get;  set; }

        /// <summary>
        /// Gets the point in time in which the Access Token returned in the AccessToken property ceases to be valid. This value is calculated based on current UTC time measured locally and the value expiresIn received from the service.
        /// </summary>
        public DateTimeOffset ExpiresOnUtc{ get; set; }

        /// <summary>
        /// Gets the Refresh Token associated with the requested Access Token. Not  all operations will return a Refresh Token.
        /// </summary>
        public string RefreshToken { get; set; }

        public bool isExpiring
        {
            get
            {
                return this.ExpiresOnUtc <= DateTimeOffset.Now.UtcDateTime.AddMinutes(5);
            }
        }


        #endregion
    }


}
