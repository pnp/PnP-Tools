using System.IO;
using System.Text;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Microsoft.Build.Utilities;
using SharePointPnP.DeveloperTools.Common.Configuration;

namespace SharePointPnP.DeveloperTools.MSBuild.Tasks
{
	public class BuildOpenXmlPackage : Task
	{
		private const string templateFileName = "sitetemplate.xml";
		public string ProjectDir { get; set; }

		public string ProjectName { get; set; }

		public string OutDir { get; set; }

		public override bool Execute()
		{
			var res = true;
			var packageName = ProjectName + ".pnp";
			var packedTemplateName = ProjectName + ".xml";

			var configManager = new ConfigurationManager();
			var config = configManager.GetTemplateConfiguration(ProjectDir);
			LogMessage($"Pack started DiplayName={config.DisplayName}, ImagePreviewUrl={config.ImagePreviewUrl}");

			XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(ProjectDir, "");
			var fsConnector = provider.Connector;
			var template = provider.GetTemplate(templateFileName);

			//set template properties
			template.DisplayName = config.DisplayName;
			template.ImagePreviewUrl = config.ImagePreviewUrl;
			template.Properties["PnP_Supports_SP2013_Platform"] = config.TargetPlatform.HasFlag(TargetPlatform.SP13).ToString();
			template.Properties["PnP_Supports_SP2016_Platform"] = config.TargetPlatform.HasFlag(TargetPlatform.SP16).ToString();
			template.Properties["PnP_Supports_SPO_Platform"] = config.TargetPlatform.HasFlag(TargetPlatform.SPO).ToString();

			string outFile = Path.Combine(ProjectDir, OutDir, packageName);
			OpenXMLConnector openXml = new OpenXMLConnector(outFile, fsConnector, config.Author);

			//write files
			foreach(var file in template.Files)
			{
				var fileName = Path.GetFileName(file.Src);
				var container = Path.GetDirectoryName(file.Src);
				using(var stream = fsConnector.GetFileStream(fileName, container))
				{
					openXml.SaveFileStream(fileName, container, stream);
				}
			}

			var xml = template.ToXML();
			using (var stream = GetStream(xml))
			{
				openXml.SaveFileStream(packedTemplateName, stream);
			}

			openXml.Commit();

			return res;
		}

		private Stream GetStream(string text)
		{
			var bytes = Encoding.UTF8.GetBytes(text);
			var res = new MemoryStream(bytes);
			return res;
		}

		private void LogMessage(string msg)
		{
			var args = new Microsoft.Build.Framework.BuildMessageEventArgs(msg, string.Empty, "PnPProvisioningTemplate", Microsoft.Build.Framework.MessageImportance.Normal);
			BuildEngine.LogMessageEvent(args);
		}
	}
}
