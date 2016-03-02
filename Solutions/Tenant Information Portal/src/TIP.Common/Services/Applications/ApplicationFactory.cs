using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Online.Applications.Core;
using TIP.Common.Services.Applications.Internal;

namespace TIP.Common.Services.Applications
{
    public sealed class ApplicationFactory : IApplicationFactory
    {
        #region Private Instance Members
        // Create a singleton static instance
        private static readonly Lazy<ApplicationFactory> _instance = new Lazy<ApplicationFactory>(() => new ApplicationFactory());
        #endregion

        #region Public Memmbers
        /// <summary>
        /// Returns an <see cref="IApplicationFactory"/>
        /// </summary>
        /// <returns></returns>
        public static IApplicationFactory GetInstance()
        {
            return _instance.Value;
        }
        #endregion

        #region IApplicationFactory Members
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public IApplicationManager CreateInstance(IClient client)
        {
            var _manager = new ApplicationManager(client);
            return _manager;

        }
        #endregion

    }
}
