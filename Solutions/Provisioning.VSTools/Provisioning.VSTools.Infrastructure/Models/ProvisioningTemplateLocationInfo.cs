using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Models
{
    public class ProvisioningTemplateLocationInfo
    {
        public string TemplateFolderPath { get; set; }

        public string TemplateFileName { get; set; }
        public string TemplatePath
        {
            get
            {
                return System.IO.Path.Combine(TemplateFolderPath, TemplateFileName);
            }
        }

        public string ResourcesPath { get; set; }

        public LoadedXMLProvisioningTemplate LoadXmlTemplate(string containerName = "")
        {
            LoadedXMLProvisioningTemplate loadedTemplate = new LoadedXMLProvisioningTemplate()
            {
                ResourcesPath = this.ResourcesPath,
                TemplateFileName = this.TemplateFileName,
                TemplateFolderPath = this.TemplateFolderPath,
            };

            try
            {
                loadedTemplate.Provider = new OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml.XMLFileSystemTemplateProvider(this.TemplateFolderPath, containerName);
                loadedTemplate.Template = loadedTemplate.Provider.GetTemplate(this.TemplateFileName);
                loadedTemplate.Template.Connector = new OfficeDevPnP.Core.Framework.Provisioning.Connectors.FileSystemConnector(this.ResourcesPath, containerName);
            }
            catch (Exception ex)
            {
                var logService = IoCBootstrapper.GetLoggerInstance();
                logService.Info(string.Format("Error parsing Provisioning Template: {0}, {1}", ex.Message, ex.StackTrace));
            }
            
            return loadedTemplate;
        }
    }
}
