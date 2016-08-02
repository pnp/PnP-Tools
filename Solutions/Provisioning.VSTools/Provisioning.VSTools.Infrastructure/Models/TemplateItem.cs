using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Models
{
    public class TemplateItem
    {
        Services.IProvisioningService ProvisioningService = null;
        public TemplateItem(Services.IProvisioningService provSvc)
        {
            this.ProvisioningService = provSvc;
        }

        public string ItemPath { get; set; }
        public string ProjectPath { get; set; }
        public ProvisioningTemplateToolsConfiguration ProvisioningConfig { get; set; }
        public ProvisioningTemplateLocationInfo TemplateInfo { get; set; }
        public DeployTemplateItem GetDeployItem(Services.ILogService logService)
        {
            LoadedXMLProvisioningTemplate loadedTemplate = null;
            try
            {
                //load the template xml file
                loadedTemplate = this.TemplateInfo.LoadXmlTemplate();
            }
            catch (Exception ex)
            {
                logService.Exception("Error reading template file " + this.ItemPath, ex);
                return null;
            }

            if (loadedTemplate.Template != null)
            {
                if (this.TemplateInfo.TemplatePath == this.ItemPath)
                {
                    //deploy complete template
                    var deployItem = new DeployTemplateItem()
                    {
                        Config = this.ProvisioningConfig,
                        Template = loadedTemplate.Template,
                        TemplateName = loadedTemplate.TemplateFileName,
                        IsCompleteTemplate = true,
                    };
                    return deployItem;
                }
                else
                {
                    //deploy specific file(s)
                    var src = Helpers.ProvisioningHelper.MakeRelativePath(this.ItemPath, this.TemplateInfo.ResourcesPath);

                    var files = loadedTemplate.Template.Files.Where(
                        f => f.Src.StartsWith(src, StringComparison.InvariantCultureIgnoreCase)
                        ).ToList();

                    if (files.Count > 0)
                    {
                        //create a new template for the specified file
                        var filesUnderFolderTemplate = new OfficeDevPnP.Core.Framework.Provisioning.Model.ProvisioningTemplate(loadedTemplate.Template.Connector);
                        filesUnderFolderTemplate.Files.AddRange(files);

                        var deployItem = new DeployTemplateItem()
                        {
                            Config = this.ProvisioningConfig,
                            Template = filesUnderFolderTemplate,
                            TemplateName = loadedTemplate.TemplateFileName,
                            IsCompleteTemplate = false,
                        };
                        return deployItem;
                    }
                }
            }

            return null;
        }
    }
}
