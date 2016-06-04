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
    }
}
