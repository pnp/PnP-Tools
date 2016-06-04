using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Models
{
    public class DeployTemplateItem
    {
        public string Title { get; set; }
        public OfficeDevPnP.Core.Framework.Provisioning.Model.ProvisioningTemplate Template { get; set; }
        public ProvisioningTemplateToolsConfiguration Config { get; set; }
    }
}
