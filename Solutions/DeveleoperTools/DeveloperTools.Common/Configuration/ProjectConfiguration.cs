using System;
using System.Xml.Serialization;

namespace SharePointPnP.DeveloperTools.Common.Configuration
{
	[Serializable]
	public class ProjectConfiguration
	{
		public string Author { get; set; }
		public string ProvisionSiteUrl { get; set; }
	}
}
