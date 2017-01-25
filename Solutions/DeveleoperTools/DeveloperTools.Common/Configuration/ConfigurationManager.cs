using SharePointPnP.DeveloperTools.Common.Helpers;
using System.IO;

namespace SharePointPnP.DeveloperTools.Common.Configuration
{
	public class ConfigurationManager
	{
		private const string templateConfigFileName = "pnptemplate.config";

		public TemplateConfiguration GetTemplateConfiguration(string path)
		{
			var res = LoadConfigurationFromFile<TemplateConfiguration>(path);
			if(res == null)
			{
				//get default config
				res = new TemplateConfiguration();
				res.DisplayName = "My Provisioning Template";
				res.ImagePreviewUrl = "https://raw.githubusercontent.com/OfficeDev/PnP-Provisioning-Templates/master/RootFolder/<TemplateCategory>/<TemplateFolder>/TemplatePreview.png";
			}
			return res;
		}

		public void SetTemplateConfiguration(string path, TemplateConfiguration config)
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
