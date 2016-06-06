using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.VSTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Helpers
{
    public static class ProvisioningHelper
    {
        public static string MakeRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static void GenerateDefaultPnPTemplate(string resourcesPath, string templatePath, string containerName = "")
        {
            if (string.IsNullOrEmpty(containerName))
            {
                containerName = "DefaultContainer";
            }

            string templateFilename = System.IO.Path.GetFileName(templatePath);

            XMLTemplateProvider provider = new XMLFileSystemTemplateProvider(resourcesPath, containerName);
            FileSystemConnector fileConnector = new FileSystemConnector(resourcesPath, containerName);
            ProvisioningTemplate template = new ProvisioningTemplate(fileConnector);
            provider.SaveAs(template, templatePath);
        }

        public static ProvisioningTemplateToolsConfiguration GenerateDefaultProvisioningConfig(string pnpTemplatePath, string resourceFolderName)
        {
            var config = new ProvisioningTemplateToolsConfiguration();
            config.ToolsEnabled = true;
            config.Templates.Add(new Template()
            {
                Path = pnpTemplatePath,//Resources.DefaultFileNamePnPTemplate,
                ResourcesFolder = resourceFolderName,
            });
            config.Deployment.TargetSite = "https://yourtenant.sharepoint.com/sites/testsite";
            config.Deployment.Credentials = new ProvisioningCredentials();

            return config;
        }
    }
}
