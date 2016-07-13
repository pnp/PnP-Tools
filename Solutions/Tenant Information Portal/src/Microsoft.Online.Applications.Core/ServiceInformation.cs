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

using Microsoft.Online.Applications.Core.Authentication;

namespace Microsoft.Online.Applications.Core
{
    /// <summary>
    /// Class to define the Service Information
    /// </summary>
    public class ServiceInformation
    {
        /// <summary>
        /// Gets or sets the type of the current user account.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Gets or sets the ClientID of the current application.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// Gets or sets the client secret of the current application.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the base URL for the authentication service.
        /// </summary>
        public string AuthenticationServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the base URL for the authentication token service.
        /// </summary>
        public string TokenServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource that is going to be invoked
        /// </summary>
        public string ServiceResource { get; set; }
       
        /// <summary>
        /// Gets or sets the tenant
        /// </summary>
        public string Tenant { get; set; }
        /// <summary>
        /// Gets or sets the service is MultiTenenat
        /// </summary>
        public bool? IsMultiTenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAuthenticationProvider"/> for authenticating service requests
        /// </summary>
        public IAuthenticationProvider AuthenticationProvider { get; set; }

    }
}
