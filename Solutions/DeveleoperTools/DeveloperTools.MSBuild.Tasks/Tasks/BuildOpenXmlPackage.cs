using System.IO;
using System.Text;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Microsoft.Build.Utilities;
using SharePointPnP.DeveloperTools.Common.Configuration;
using Microsoft.Build.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

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

				LogMessage($"Packing template={filename}, package={packageName}");

				XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(ProjectDir, "");
				var fsConnector = provider.Connector;
				var template = provider.GetTemplate(idenity);

				var configManager = new ConfigurationManager();
				var config = configManager.GetProjectConfiguration(ProjectDir);

				var outFile = Path.Combine(ProjectDir, OutDir, packageName);
				OpenXMLConnector openXml = new OpenXMLConnector(outFile, fsConnector, config.Author);

				//write files
				var files = template.Files != null ? template.Files.Select(f => f.Src.ToLower()).ToList() : new List<string>();
				if(TemplateFiles?.Length > 0)
				{
					files = files.Union(TemplateFiles.Select(t => t.GetMetadata("Identity")?.ToLower())).ToList();
				}

				foreach (var file in files)
				{
					LogMessage($"Packing file={file}, package={packageName}");
					var fileName = Path.GetFileName(file);
					var container = Path.GetDirectoryName(file);
					using (var stream = fsConnector.GetFileStream(fileName, container))
					{
						if(stream != null)
						{
							openXml.SaveFileStream(fileName, container, stream);
						}
						else
						{
							throw new FileNotFoundException($"Not found: {Path.Combine(ProjectDir, file)}");
						}
					}
				}

				var xml = template.ToXML();
				using (var stream = GetStream(xml))
				{
					openXml.SaveFileStream(packedTemplateName, stream);
				}

				openXml.Commit();
				LogMessage($"Packed successfully.");
			}
			catch (Exception e)
			{
				LogError(e);
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
			Log.LogMessage(MessageImportance.High, msg);
		}
		private void LogError(Exception e)
		{
			Log.LogErrorFromException(e);
		}
	}
}
