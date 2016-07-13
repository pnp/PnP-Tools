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


namespace Microsoft.Online.Applications.Core.Configuration
{
    /// <summary>
    /// Domain Model for the Application Config
    /// </summary>
    public class AppConfig
    {
        #region Properties 
        /// <summary>
        /// Gets or sets the PostLogoutRedirectURI for Active Directory authentication. The Post Logout Redirect Uri is the URL where the user will be redirected after they have signed out
        /// </summary>
        public string PostLogoutRedirectURI { get; set; }
        /// <summary>
        /// Gets or sets the application ID for Active Directory authentication. The Client ID is used by the application to uniquely identify itself to Azure AD.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// Gets or sets the client secret for Active Directory authentication. The ClientSecret is a credential used to authenticate the application to Azure AD.  Azure AD supports password and certificate credentials.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the Tenant Domain
        /// </summary>
        public string TenantDomain { get; set; }

        /// <summary>
        /// Gets or sets if the Application is Multi-Tenant
        /// </summary>
        public bool? IsAppMultiTenent { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string ServiceResource { get; set; }

        #endregion
    }
}
