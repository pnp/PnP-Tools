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

namespace Microsoft.Online.Applications.Core
{
    public static class Constants
    {
        public static class Authentication
        {
            /// <summary>
            /// Common end-point for Microsoft Online Services. You should no longer use https://login.windows.net
            /// </summary>
            public const string CommonAuthority = "https://login.microsoftonline.com/common/";

            /// <summary>
            /// Endpoint for the Microsoft Azure AD endpoint
            /// </summary>
            public const string GraphServiceUrl = "https://graph.windows.net";

            internal const string ActiveDirectoryAuthenticationServiceUrl = "https://login.microsoftonline.com/common/oauth2/authorize";

            internal const string ActiveDirectorySignOutUrl = "https://login.microsoftonline.com/common/oauth2/logout";

            internal const string ActiveDirectoryTokenServiceUrl = "https://login.microsoftonline.com/common/oauth2/token";
     
        }

    }
}
