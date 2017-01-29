using System.IO;
using System.Text;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Microsoft.Build.Utilities;
using SharePointPnP.DeveloperTools.Common.Configuration;
using Microsoft.Build.Framework;
using System;

namespace SharePointPnP.DeveloperTools.MSBuild.Tasks
{
	public class BuildOpenXmlPackage : Task
	{
		[Required]
		public ITaskItem[] ProvisioningTemplates { get; set; }

		[Required]
		public ITaskItem[] TemplateFiles { get; set; }

		public string ProjectDir { get; set; }

		public string ProjectName { get; set; }

		public string OutDir { get; set; }

		public override bool Execute()
		{
			var res = true;
			foreach(var item in ProvisioningTemplates)
			{
				res = BuildPackage(item);
				if(!res)
				{
					break;
				}
			}
			return res;
		}

		private bool BuildPackage(ITaskItem item)
		{
			var res = true;

			try
			{
				var idenity = item.GetMetadata("Identity");
				var filename = Path.GetFileName(idenity);
				var packageName = ProjectName + ".pnp";
				var packedTemplateName = Path.GetFileNameWithoutExtension(filename) + ".xml";

				LogMessage($"Pack started file={filename}, package={packageName}");

				XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(ProjectDir, "");
				var fsConnector = provider.Connector;
				var template = provider.GetTemplate(idenity);

				var configManager = new ConfigurationManager();
				var config = configManager.GetProjectConfiguration(ProjectDir);

				var outFile = Path.Combine(ProjectDir, OutDir, packageName);
				OpenXMLConnector openXml = new OpenXMLConnector(outFile, fsConnector, config.Author);

				//write files
				foreach (var file in template.Files)
				{
					var fileName = Path.GetFileName(file.Src);
					var container = Path.GetDirectoryName(file.Src);
					using (var stream = fsConnector.GetFileStream(fileName, container))
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
			}
			catch(Exception e)
			{
				//TODO trace
				res = false;
			}
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
