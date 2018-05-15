using Microsoft.SharePoint.Client;

namespace SharePointPnP.Modernization.Framework.Functions
{
    /// <summary>
    /// Base class for all function libraries
    /// </summary>
    public abstract class FunctionsBase
    {
        protected ClientContext clientContext;

        #region Construction
        /// <summary>
        /// Instantiates a function library class
        /// </summary>
        /// <param name="clientContext">ClientContext object for the site holding the page being transformed</param>
        public FunctionsBase(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }
        #endregion
    }
}
