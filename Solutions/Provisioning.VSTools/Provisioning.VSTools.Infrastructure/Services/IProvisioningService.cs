using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.VSTools.Models;
using System;
using System.Collections.Generic;
namespace Provisioning.VSTools.Services
{
    public interface IProvisioningService
    {
        System.Threading.Tasks.Task<bool> DeployProvisioningTemplates(IEnumerable<DeployTemplateItem> templates);
        //System.Threading.Tasks.Task<bool> DeployProvisioningTemplate(string name, OfficeDevPnP.Core.Framework.Provisioning.Model.ProvisioningTemplate template, Provisioning.VSTools.Models.ProvisioningTemplateToolsConfiguration config);
        IEnumerable<DeployTemplateItem> pendingTemplatesToDeploy { get; }
        void ResetPendingTemplates();
        bool IsBusy { get; }
    }
}
