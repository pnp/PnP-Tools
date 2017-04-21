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
using System.Configuration;
using Microsoft.Online.Applications.Core.Configuration;

namespace TIP.Common.Configuration.Internal
{
	/// <summary>
	/// Handles reading from the configuration file or Azure configuration
	/// </summary>
	internal class ConfigManager
	{
		/// <summary>
		/// Returns a Domain Model which represents the application config
		/// </summary>
		/// <returns><see cref="AppConfig"/></returns>
		public AppConfig GetApplicationConfig()
		{
			//TODO LOGGING
			var _config = new AppConfig
			{
				ClientID = this.ReadConfiguration(Constants.Configuration.CLIENT_ID_KEY),
				ClientSecret = this.ReadConfiguration(Constants.Configuration.CLIENT_SECRET_KEY),
				PostLogoutRedirectURI = this.ReadConfiguration(Constants.Configuration.POST_LOGOUTREDIRECTURI_KEY),
				TenantDomain = this.ReadConfiguration(Constants.Configuration.TENANT_KEY)
			};
			return _config;
		}

		/// <summary>
		/// Returns a Domain Model which represents the WebJob config
		/// </summary>
		/// <returns><see cref="WebJobConfig"/></returns>
		public WebJobConfig GetWebJobConfig()
		{
			//TODO LOGGING
			var _appConfig = GetApplicationConfig();
			var _config = new WebJobConfig
			{
				ClientID              = _appConfig.ClientID,
				ClientSecret          = _appConfig.ClientSecret,
				PostLogoutRedirectURI = _appConfig.PostLogoutRedirectURI,
				TenantDomain          = _appConfig.TenantDomain,
				ConnectorUrl          = this.ReadConfiguration(Constants.Configuration.CONNECTOR_URL_KEY),
				PortalUrl             = this.ReadConfiguration(Constants.Configuration.PORTAL_URL_KEY),
				NotificationInterval  = Convert.ToInt32(this.ReadConfiguration(Constants.Configuration.NOTIFICATION_INTERVAL_KEY))
			};
			return _config;
		}

		private string ReadConfiguration(object cONNECTOR_URL_KEY)
		{
			throw new NotImplementedException();
		}

		#region Private Members
		/// <summary>
		/// Gets the configuration item by a specific key. 
		/// returns <see cref="string.Empty"/> if the value is not set.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private string ReadConfiguration(string key)
		{
			var _result = string.Empty;

			if (!string.IsNullOrEmpty(key))
			{
				_result = ConfigurationManager.AppSettings[key];
			}
			return _result;

		}
		#endregion
	}
}
