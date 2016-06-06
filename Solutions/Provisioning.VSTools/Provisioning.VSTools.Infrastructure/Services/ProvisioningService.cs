using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.VSTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Services
{
    /// <summary>
    /// Responsible for provisioning activities to the target SharePoint sites.
    /// </summary>
    public class ProvisioningService : Provisioning.VSTools.Services.IProvisioningService
    {
        private Queue<DeployTemplateItem> pendingTemplatesToDeploy = new Queue<DeployTemplateItem>();
        
        public bool IsBusy { get; set; }

        private Services.ILogService LogService = null;

        public ProvisioningService(Services.ILogService logSvc)
        {
            this.LogService = logSvc;
        }

        public void ResetPendingTemplates()
        {
            if (!IsBusy)
            {
                pendingTemplatesToDeploy.Clear();
            }
        }

        private void AddToPendingTemplates(IEnumerable<DeployTemplateItem> templates)
        {
            if (templates != null)
            {
                templates.ToList().ForEach(t => pendingTemplatesToDeploy.Enqueue(t));
            }
        }

        public async System.Threading.Tasks.Task<bool> DeployProvisioningTemplates(IEnumerable<DeployTemplateItem> templates)
        {
            if (IsBusy)
            {
                LogService.Warn("DeployProvisioningTemplates is busy processing pending requests.");
                return false;
            }

            bool success = true;

            IsBusy = true;

            try
            {
                this.ResetPendingTemplates();
                this.AddToPendingTemplates(templates);

                await System.Threading.Tasks.Task.Run(() =>
                {
                    while (pendingTemplatesToDeploy.Count > 0)
                    {
                        var deployItem = pendingTemplatesToDeploy.Dequeue();
                        LogService.Info(string.Format("Start {1:t} - {0}...", deployItem.Title, System.DateTime.Now));
                        var siteUrl = deployItem.Config.Deployment.TargetSite;
                        var login = deployItem.Config.Deployment.Credentials.Username;

                        try
                        {
                            using (ClientContext clientContext = new ClientContext(siteUrl))
                            {
                                LogService.Info("Signing in - " + siteUrl);
                                clientContext.Credentials = new SharePointOnlineCredentials(login, deployItem.Config.Deployment.Credentials.GetSecurePassword());

                                Web web = clientContext.Web;
                                clientContext.Load(web);
                                clientContext.ExecuteQuery();

                                ProvisioningTemplateApplyingInformation ptai = new ProvisioningTemplateApplyingInformation();
                                ptai.ProgressDelegate = delegate(string message, int step, int total)
                                {
                                    LogService.Info(string.Format("Deploying {0}, Step {1}/{2}", message, step, total));
                                };

                                LogService.Info("Applying template...");
                                clientContext.Web.ApplyProvisioningTemplate(deployItem.Template, ptai);
                            }

                            success = true;
                        }
                        catch (Exception ex)
                        {
                            LogService.Info("Error during provisioning: " + ex.Message);
                            success = false;
                        }

                        LogService.Info(string.Format("End {2:t} (success={1}) - {0}", deployItem.Title, success, System.DateTime.Now));
                    }
                });
            }
            catch (Exception ex)
            {
                success = false;
                LogService.Exception("DeployProvisioningTemplates", ex);
            }
            finally
            {
                IsBusy = false;
            }

            return success;
        }
    }
}
