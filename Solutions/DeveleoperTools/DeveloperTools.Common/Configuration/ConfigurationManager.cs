using SharePointPnP.DeveloperTools.Common.Helpers;
using System.IO;

namespace SharePointPnP.DeveloperTools.Common.Configuration
{
	public class ConfigurationManager
	{
		private const string templateConfigFileName = "pnptemplate.config";

		public ProjectConfiguration GetProjectConfiguration(string path)
		{
			var res = LoadConfigurationFromFile<ProjectConfiguration>(path);
			if(res == null)
			{
				//get default config
				res = new ProjectConfiguration();
				res.ProvisionSiteUrl = "http://portal";
			}
			return res;
		}

		public void SetProjectConfiguration(string path, ProjectConfiguration config)
		{
			SaveConfigurationToFile(path, config);
		}

		private string GetFullConfigPath(string path)
		{
			var res = Path.Combine(path, templateConfigFileName);
			return res;
		}

		private T LoadConfigurationFromFile<T>(string path) 
			where T: class
		{
			T res = default(T);
			var fullPath = GetFullConfigPath(path);
			res = XmlHelper.ReadXml<T>(fullPath);
			return res;
		}

		private void SaveConfigurationToFile<T>(string path, T config)
			where T: class
		{
			var fullPath = GetFullConfigPath(path);
			XmlHelper.WriteXml(config, fullPath);
		}
	}
}
