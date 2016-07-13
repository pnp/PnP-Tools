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
using Microsoft.Online.Applications.Core.Configuration;
using Microsoft.Online.Applications.Core.Authentication;

namespace Microsoft.Online.Applications.Core
{
    /// <summary>
    /// Interface for working with Service Providers
    /// </summary>
    public interface IServiceInfoProvider
    {
        /// <summary>
        /// Gets the Authentication Provider
        /// </summary>
        IAuthenticationProvider AuthenticationProvider { get; }

        /// <summary>
        /// Creates a <seealso cref="ServiceInformation"/> that is used to interact with Office 365 Servcies
        /// </summary>
        /// <param name="appConfig">The configuration information for the service</param>
        /// <param name="credentialType">The Type of Credentials that is used to interact with the service</param>
        /// <returns></returns>
        Task<ServiceInformation> CreateServiceInfo(AppConfig appConfig, CredentialType credentialType);
    }
}
