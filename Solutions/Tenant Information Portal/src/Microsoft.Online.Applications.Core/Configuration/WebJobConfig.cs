using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Online.Applications.Core.Configuration
{
	public class WebJobConfig : AppConfig
	{
		/// <summary>
		/// Gets or Sets the Connector Url
		/// </summary>
		public string ConnectorUrl { get; set; }

		/// <summary>
		/// Gets or Sets the URL of the Tenant Information Portal
		/// </summary>
		public string PortalUrl { get; set; }

		/// <summary>
		/// Gets or Sets the notification interval in days
		/// </summary>
		public int NotificationInterval { get; set; }
	}
}
