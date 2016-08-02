using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Models
{
    public class DeployTemplateItem
    {
        public string TemplateName { get; set; }
        public bool IsCompleteTemplate { get; set; }
        public OfficeDevPnP.Core.Framework.Provisioning.Model.ProvisioningTemplate Template { get; set; }
        public ProvisioningTemplateToolsConfiguration Config { get; set; }

        public string GetDeployTitle()
        {
            if (IsCompleteTemplate)
            {
                return string.Format("Deploying complete template \"{0}\"", this.TemplateName);
            }
            else if (Template != null && Template.Files != null)
            {
                if (Template.Files.Count == 1)
                {
                    return string.Format("Deploying file \"{0}\" from template \"{1}\"", Template.Files.First().Src, this.TemplateName);
                }
                else if (Template.Files.Count > 1)
                {
                    return string.Format("Deploying {0} file(s) from template \"{1}\"", Template.Files.Count, this.TemplateName);
                }
            }

            return "Deploying unknown template";
        }
    }
}
