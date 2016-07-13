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
using Microsoft.Online.Applications.Core.Clients;
using TIP.Common.Services.Principals.Internal;
using Microsoft.Online.Applications.Core;

namespace TIP.Common.Services.Principals
{
    public sealed class ServicePrincipalFactory : IServicePrincipalFactory
    {
        #region Private Instance Members
        // Create a singleton static instance
        private static readonly Lazy<ServicePrincipalFactory> _instance = new Lazy<ServicePrincipalFactory>(() => new ServicePrincipalFactory());
        #endregion

        public static IServicePrincipalFactory GetInstance()
        {
            return _instance.Value;
        }

        public IServicePrincipalManager CreateInstance(IClient client)
        {
            var _manager = new ServicePrincipalManager(client);
            return _manager;
        }


      
    }
}
