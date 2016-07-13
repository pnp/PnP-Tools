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
    public interface IAuthenticationContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<IAuthenticationResult> AcquireTokenAsync(string resource, string clientId);

        /// <summary>
        ///  Acquires security token from the authority. <seealso cref="AuthenticationContext.AcquireToken(string, ClientCredential)"/>.
        /// </summary>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <param name="clientCredential">The client credential to use for token acquisition.</param>
        /// <returns></returns>
        Task<IAuthenticationResult> AcquireTokenAsync(string resource, ClientCredential clientCredential);

        IAuthenticationResult AcquireToken(string resource, ClientCredential clientCredential);

        Guid CorrelationId { get; set; }

    }
}
