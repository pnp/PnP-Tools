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
using Microsoft.Online.Applications.Core.Configuration;
using TIP.Common.Configuration.Internal;

namespace TIP.Common.Configuration
{
	/// <summary>
	/// Factory class for working with configuration of the applications
	/// </summary>
	public sealed class ConfigurationFactory
	{
		#region Instance Members
		private static readonly Lazy<ConfigurationFactory> _instance = new Lazy<ConfigurationFactory>(() => new ConfigurationFactory());
		#endregion

		#region Constructor
		private ConfigurationFactory()
		{
		}
		#endregion

		#region Properties

		/// <summary>
		/// Returns an instance of <see cref="ConfigurationFactory"/>
		/// </summary>
		public static ConfigurationFactory Instance
		{
			get
			{
				return _instance.Value;
			}
		}
		#endregion

		#region Public Members

		/// <summary>
		/// Returns an instance of <see cref="AppConfig"/>
		/// </summary>
		/// <returns></returns>
		public AppConfig GetApplicationConfiguration()
		{
			var _configManager = new ConfigManager();
			return _configManager.GetApplicationConfig();
		}

		/// <summary>
		/// Returns an instance of <see cref="WebJobConfig"/> 
		/// </summary>
		/// <returns></returns>
		public WebJobConfig GetWebJobConfiguration()
		{
			var _configManager = new ConfigManager();
			return _configManager.GetWebJobConfig();
		}
		#endregion


	}
}
