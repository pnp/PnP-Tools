using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.VSTools.Helpers;
using Provisioning.VSTools.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Services
{
    public class ProjectService : Provisioning.VSTools.Services.IProjectService
    {
        #region variables and properties
        private Services.ILogService LogService = null;
        private Services.IProvisioningService ProvisioningService = null;

        private bool initialized = false;
        private bool isBusyPendingItems = false;
        private static readonly object _lockObject = new object();

        private OleMenuCommand _projectItemDeployCommand;
        private OleMenuCommand _projectFolderDeployCommand;
        private OleMenuCommand _toolsToggleMenuItem;
        private OleMenuCommand _toolsEditConnMenuItem;

        private EnvDTE.ProjectItemsEvents projItemsEvents;
        private EnvDTE.DocumentEvents docEvents;

        private Queue<TemplateItem> pendingItemTemplates = new Queue<TemplateItem>();
        private Queue<TemplateItem> pendingFolderTemplates = new Queue<TemplateItem>();
        private Queue<ProjectItemRequestBase> pendingProjectItemRequests = new Queue<ProjectItemRequestBase>();

        public IVsSolution VSSolution { get; set; }

        private OutputWindowPane _outputWindowPane = null;

        public IVsUIShell UIShell { get; private set; }

        public DTE2 DTE { get; private set; }

        public OleMenuCommandService MenuCommandService { get; private set; }
        #endregion

        #region ctor and initialization
        public ProjectService(Services.ILogService logSvc, Services.IProvisioningService prvSvc)
        {
            this.LogService = logSvc;
            this.ProvisioningService = prvSvc;
        }

        public void Initialize(IVsUIShell uiShell, DTE2 dte, OleMenuCommandService mcs)
        {
            if (!this.initialized)
            {
                this.UIShell = uiShell;
                this.DTE = dte;
                this.MenuCommandService = mcs;
                this.VSSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;

                InitOutputPane();

                this.initialized = true;
            }
        }
        #endregion

        #region project event handlers

        private async void ProjItemAdded(EnvDTE.ProjectItem projectItem)
        {
            if (!IsEnabledForCurrentProject())
            {
                return;
            }

            if (projectItem.Kind != EnvDTE.Constants.vsProjectItemKindPhysicalFile)
            {
                // we handle only files
                // when folder with files is added, event is raised separately for all files as well
                return;
            }

            if (!ProjectHelpers.IncludeFile(projectItem.Name))
            {
                // do not handle .bundle or .map files the other files that are part of the bundle should be handled.
                // others may be defined in Constants.ExtensionsToIgnore
                return;
            }

            try
            {
                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath);
                var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItemFullPath, projectFolderPath, config);

                if (pnpTemplateInfo != null)
                {
                    this.pendingProjectItemRequests.Enqueue(new ProjectItemRequestAdd()
                    {
                        ItemPath = projectItemFullPath,
                        ItemKind = projectItem.Kind,
                        TemplateInfo = pnpTemplateInfo,
                    });

                    await System.Threading.Tasks.Task.Run(() => ProcessPendingProjectItemRequestsWithDelay());
                    //ProcessPendingProjectItemRequests();
                }
            }
            catch (Exception ex)
            {
                LogService.Exception("Error in ItemAdded", ex);
            }
        }

        private async void ProjItemRemoved(EnvDTE.ProjectItem projectItem)
        {
            if (!IsEnabledForCurrentProject())
            {
                return;
            }

            try
            {
                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath, false);
                var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItemFullPath, projectFolderPath, config);
                var projectItemKind = projectItem.Kind;

                if (pnpTemplateInfo != null)
                {
                    this.pendingProjectItemRequests.Enqueue(new ProjectItemRequestRemove()
                    {
                        ItemPath = projectItemFullPath,
                        ItemKind = projectItem.Kind,
                        TemplateInfo = pnpTemplateInfo,
                    });

                    await System.Threading.Tasks.Task.Run(() => ProcessPendingProjectItemRequestsWithDelay());
                    //ProcessPendingProjectItemRequests();
                }
            }
            catch (Exception ex)
            {
                LogService.Exception("Error in ItemRemoved", ex);
            }
        }

        private async void ProjItemRenamed(EnvDTE.ProjectItem projectItem, string oldName)
        {
            if (!IsEnabledForCurrentProject())
            {
                return;
            }

            if (projectItem.Kind != EnvDTE.Constants.vsProjectItemKindPhysicalFile)
            {
                // we handle only files
                // when folder with files is added, event is raised separately for all files as well
                return;
            }

            if (!ProjectHelpers.IncludeFile(projectItem.Name))
            {
                // do not handle .bundle or .map files the other files that are part of the bundle should be handled.
                // others may be defined in Constants.ExtensionsToIgnore
                return;
            }

            try
            {
                var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath, false);
                var projectItemFullPath = ProjectHelpers.GetFullPath(projectItem);
                var pnpTemplateInfo = GetParentProvisioningTemplateInformation(projectItemFullPath, projectFolderPath, config);
                var projectItemKind = projectItem.Kind;

                if (pnpTemplateInfo != null)
                {
                    this.pendingProjectItemRequests.Enqueue(new ProjectItemRequestRename()
                    {
                        ItemPath = projectItemFullPath,
                        ItemKind = projectItem.Kind,
                        TemplateInfo = pnpTemplateInfo,
                        OldName = oldName
                    });

                    await System.Threading.Tasks.Task.Run(() => ProcessPendingProjectItemRequestsWithDelay());
                    //ProcessPendingProjectItemRequests();
                }
            }
            catch (Exception ex)
            {
                LogService.Exception("Error in ItemRenamed", ex);
            }
        }

        private async void DeployFolderMenuItemCallback(object sender, EventArgs e)
        {
            if (!IsEnabledForCurrentProject() || ProvisioningService.IsBusy)
            {
                return;
            }

            List<DeployTemplateItem> deployItems = new List<DeployTemplateItem>();

            while (pendingFolderTemplates.Count > 0)
            {
                var sourceTemplateItem = pendingFolderTemplates.Dequeue();
                var deployItem = sourceTemplateItem.GetDeployItem(this.LogService);
                if (deployItem != null)
                {
                    deployItems.Add(deployItem);
                }
            }

            if (deployItems.Count() > 0)
            {
                ShowOutputPane();
                var result = await ProvisioningService.DeployProvisioningTemplates(deployItems);
            }
        }

        private async void DeployMenuItemCallback(object sender, EventArgs e)
        {
            if (!IsEnabledForCurrentProject() || ProvisioningService.IsBusy)
            {
                return;
            }

            List<DeployTemplateItem> deployItems = new List<DeployTemplateItem>();

            while (pendingItemTemplates.Count > 0)
            {
                var sourceTemplateItem = pendingItemTemplates.Dequeue();
                var deployItem = sourceTemplateItem.GetDeployItem(this.LogService);
                if (deployItem != null)
                {
                    deployItems.Add(deployItem);
                }
            }

            if (deployItems.Count() > 0)
            {
                ShowOutputPane();
                var result = await ProvisioningService.DeployProvisioningTemplates(deployItems);
            }
        }

        //Context menu check for specific file name
        void menuCommand_ProjectItemBeforeQueryStatus(object sender, EventArgs e)
        {
            ResetPendingItems();

            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                // start by assuming that the menu will not be shown
                menuCommand.Visible = true;
                menuCommand.Enabled = false;

                if (!IsEnabledForCurrentProject() || ProvisioningService.IsBusy)
                {
                    return;
                }

                //enqueue items
                ((List<TemplateItem>)GetSelectedItemTempateItems()).ForEach(t => pendingItemTemplates.Enqueue(t));
                if (pendingItemTemplates.Count() > 0)
                {
                    menuCommand.Enabled = true;
                }
            }
        }

        void menuCommand_ProjectFolderBeforeQueryStatus(object sender, EventArgs e)
        {
            ResetPendingItems();

            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {

                // start by assuming that the menu will not be shown
                menuCommand.Visible = true;
                menuCommand.Enabled = false;

                if (!IsEnabledForCurrentProject() || ProvisioningService.IsBusy)
                {
                    return;
                }

                //enqueue items
                ((List<TemplateItem>)GetSelectedItemTempateItems()).ForEach(t => pendingFolderTemplates.Enqueue(t));
                if (pendingItemTemplates.Count() > 0)
                {
                    menuCommand.Enabled = true;
                }
            }
        }

        private void ToggleToolsMenuItemOnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                bool isEnabled = IsEnabledForCurrentProject();
                if (isEnabled || ProvisioningService.IsBusy)
                {
                    menuCommand.Text = Resources.DisablePnPToolsText;
                }
                else
                {
                    menuCommand.Text = Resources.EnablePnPToolsText;
                }
            }
        }

        private void ToggleToolsMenuItemCallback(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                var config = GetConfig(true);

                //if not enabled and user is trying to enable, ensure valid creds before setting enabled to true
                if (config != null)
                {
                    //check if we should get the user credentials
                    if (!config.ToolsEnabled && !config.Deployment.IsValid)
                    {
                        GetUserCreds(config, config.ProjectPath, config.Deployment.Credentials.FilePath);
                    }

                    //toggle if valid, otherwise set to false
                    if (config.Deployment.IsValid)
                    {
                        config.ToolsEnabled = !config.ToolsEnabled;
                    }
                    else
                    {
                        config.ToolsEnabled = false;
                    }

                    SaveConfigForProject(config.ProjectPath, config);
                }

                //update the menu item to reflect enabled status
                if (config != null && config.ToolsEnabled == true)
                {
                    menuCommand.Text = Resources.EnablePnPToolsText;
                }
                else
                {
                    menuCommand.Text = Resources.DisablePnPToolsText;
                }
            }
        }

        private void EditConnMenuItemCallback(object sender, EventArgs e)
        {
            var project = ProjectHelpers.GetActiveProject();
            var projectPath = Helpers.ProjectHelpers.GetProjectFolder(project);
            var configFileCredsPath = Path.Combine(projectPath, Resources.FileNameProvisioningUserCreds);

            var config = GetConfig(projectPath, true);
            GetUserCreds(config, projectPath, configFileCredsPath);
        }
        #endregion

        /// <summary>
        /// Displays a message box dialog
        /// </summary>
        /// <param name="title">Title of the message box dialog</param>
        /// <param name="message">Message for the message box dialog</param>
        private void ShowMessage(string title, string message)
        {
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.UIShell.ShowMessageBox(
                       0,
                       ref clsid,
                       title,
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));
        }

        private void InitOutputPane()
        {
            if (this._outputWindowPane == null)
            {
                OutputWindow outputWindow = (OutputWindow)this.DTE.Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Object;
                this._outputWindowPane = outputWindow.OutputWindowPanes.Add(Constants.OUTPUT_WINDOW_PANE_NAME);

                if (this.LogService is Services.ProjectLogService)
                {
                    ((Services.ProjectLogService)this.LogService).SetOutputWindowPane(this._outputWindowPane);
                }
            }
        }

        private void ShowOutputPane()
        {
            if (this._outputWindowPane != null)
            {
                this._outputWindowPane.Activate();
            }
        }

        public void AttachFileEventListeners()
        {
            try
            {
                //IVsHierarchyEvents
                // ((Events2)dte.Events).SolutionEvents.

                projItemsEvents = (EnvDTE.ProjectItemsEvents)this.DTE.Events.GetObject("CSharpProjectItemsEvents");
                projItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(ProjItemAdded);
                projItemsEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(ProjItemRemoved);
                projItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(ProjItemRenamed);

                docEvents = (EnvDTE.DocumentEvents)this.DTE.Events.DocumentEvents;
                docEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(DocEventsDocSaved);
            }
            catch (System.Exception ex)
            {
                LogService.Exception("Error registering file event handlers", ex);
            }
        }

        public void DetachFileEventListeners()
        {
            try
            {
                //IVsHierarchyEvents
                // ((Events2)dte.Events).SolutionEvents.

                projItemsEvents = (EnvDTE.ProjectItemsEvents)this.DTE.Events.GetObject("CSharpProjectItemsEvents");
                projItemsEvents.ItemAdded -= ProjItemAdded;
                projItemsEvents.ItemRemoved -= ProjItemRemoved;
                projItemsEvents.ItemRenamed -= ProjItemRenamed;

                docEvents = (EnvDTE.DocumentEvents)this.DTE.Events.DocumentEvents;
                docEvents.DocumentSaved -= DocEventsDocSaved;
            }
            catch (System.Exception ex)
            {
                LogService.Exception("Error unregistering file event handlers", ex);
            }
        }

        public void AddProjectCommands()
        {
            if (this.MenuCommandService != null)
            {
                //toosl menu item
                _toolsToggleMenuItem = new OleMenuCommand(ToggleToolsMenuItemCallback, PkgCmdIDList.ToggleToolsCommandID);
                _toolsToggleMenuItem.BeforeQueryStatus += ToggleToolsMenuItemOnBeforeQueryStatus;
                this.MenuCommandService.AddCommand(_toolsToggleMenuItem);

                //edit connection menu item
                _toolsEditConnMenuItem = new OleMenuCommand(EditConnMenuItemCallback, PkgCmdIDList.EditConnCommandID);
                _toolsEditConnMenuItem.Text = Resources.EditConnPnPToolsText;
                this.MenuCommandService.AddCommand(_toolsEditConnMenuItem);

                //item deploy command
                _projectItemDeployCommand = new OleMenuCommand(DeployMenuItemCallback, PkgCmdIDList.DeployItemCommandID);
                _projectItemDeployCommand.BeforeQueryStatus += menuCommand_ProjectItemBeforeQueryStatus;
                this.MenuCommandService.AddCommand(_projectItemDeployCommand);

                //folder deploy command
                _projectFolderDeployCommand = new OleMenuCommand(DeployFolderMenuItemCallback, PkgCmdIDList.DeployFolderCommandID);
                _projectFolderDeployCommand.BeforeQueryStatus += menuCommand_ProjectFolderBeforeQueryStatus;
                this.MenuCommandService.AddCommand(_projectFolderDeployCommand);
            }
        }

        public void RemoveProjectCommands()
        {
            if (this.MenuCommandService != null)
            {
                this.MenuCommandService.RemoveCommand(_projectItemDeployCommand);
                this.MenuCommandService.RemoveCommand(_projectFolderDeployCommand);
                this.MenuCommandService.RemoveCommand(_toolsEditConnMenuItem);
                this.MenuCommandService.RemoveCommand(_toolsToggleMenuItem);
            }
        }

        private ProvisioningTemplateLocationInfo GetParentProvisioningTemplateInformation(string projectItemFullPath, string projectFolderPath, ProvisioningTemplateToolsConfiguration config)
        {
            Models.ProvisioningTemplateLocationInfo templateInfo = null;

            try
            {
                templateInfo = ProjectHelpers.GetParentProvisioningTemplateInformation(projectItemFullPath, projectFolderPath, config);
            }
            catch (Exception ex)
            {
                LogService.Exception("GetParentProvisioningTemplateInformation", ex);
            }

            if (templateInfo == null)
            {
                LogService.Warn("Cannot determine template for the supplied item: " + projectItemFullPath);
            }

            return templateInfo;
        }

        /// <summary>
        /// Process all pending project item requests... note: this should be called asynchronously and uses a Thread.Sleep call to wait for changes to be queued up before processing them.
        /// </summary>
        private void ProcessPendingProjectItemRequestsWithDelay()
        {
            System.Threading.Thread.Sleep(Constants.PROJECT_ITEM_DELAY_IN_MS); //wait a short time so that changes are queued up

            lock (_lockObject)
            {
                if (!isBusyPendingItems)
                {
                    Dictionary<string, LoadedXMLProvisioningTemplate> loadedTemplates = new Dictionary<string, LoadedXMLProvisioningTemplate>();

                    isBusyPendingItems = true;
                    try
                    {
                        GetNextPendingItemRequest(loadedTemplates);

                        //save the changes to disk
                        foreach (var kvp in loadedTemplates)
                        {
                            var loadedTemplate = kvp.Value;
                            loadedTemplate.Provider.Save(loadedTemplate.Template);

                            //LogService.Info("Saved template: " + loadedTemplate.TemplatePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Exception("ProcessPendingProjectItemRequests failed", ex);
                    }
                    finally
                    {
                        isBusyPendingItems = false;
                    }

                    //if there are remaining items, start processing again
                    //this can happen if: 1) the code fails before it reaches the last item, 2) items are added via the primary thread while the background thread is processing and saving the xml files
                    if (this.pendingProjectItemRequests.Count > 0)
                    {
                        ProcessPendingProjectItemRequestsWithDelay();
                    }
                }
            }
        }

        private void GetNextPendingItemRequest(IDictionary<string, LoadedXMLProvisioningTemplate> loadedTemplates)
        {
            while (this.pendingProjectItemRequests.Count > 0)
            {
                var pendingItem = this.pendingProjectItemRequests.Dequeue();

                LoadedXMLProvisioningTemplate loadedTemplate = null;

                if (loadedTemplates.ContainsKey(pendingItem.TemplateInfo.TemplatePath))
                {
                    loadedTemplate = loadedTemplates[pendingItem.TemplateInfo.TemplatePath];
                }
                else
                {
                    loadedTemplate = pendingItem.TemplateInfo.LoadXmlTemplate();
                    if (loadedTemplate.Template != null)
                    {
                        loadedTemplates.Add(pendingItem.TemplateInfo.TemplatePath, loadedTemplate);
                    }
                }

                if (loadedTemplate.Template != null)
                {
                    switch (pendingItem.RequestType)
                    {
                        case ProjectItemRequestType.Add:
                            AddItemToTemplate(pendingItem.ItemPath, loadedTemplate);
                            break;
                        case ProjectItemRequestType.Remove:
                            RemoveItemFromTemplate(pendingItem.ItemPath, pendingItem.ItemKind, loadedTemplate);
                            break;
                        case ProjectItemRequestType.Rename:
                            var renameItem = (ProjectItemRequestRename)pendingItem;
                            RenameTemplateItem(renameItem.ItemPath, renameItem.ItemKind, renameItem.OldName, loadedTemplate);
                            break;
                        default:
                            LogService.Warn("Unknown ProjectItemRequestType: " + pendingItem.RequestType);
                            break;
                    }
                }
                else
                {
                    LogService.Warn("No template loaded for item ({0}) {1}", pendingItem.RequestType, pendingItem.ItemPath);
                }

                GetNextPendingItemRequest(loadedTemplates);
            }
        }

        private bool AddItemToTemplate(string projectItemFullPath, LoadedXMLProvisioningTemplate loadedTemplate)
        {
            if (loadedTemplate != null && loadedTemplate.Template != null)
            {
                // Item is PnP resource. 
                var src = ProvisioningHelper.MakeRelativePath(projectItemFullPath, loadedTemplate.ResourcesPath);
                var targetFolder = String.Join("/", Path.GetDirectoryName(src).Split('\\'));

                loadedTemplate.Template.Files.Add(new OfficeDevPnP.Core.Framework.Provisioning.Model.File()
                {
                    Src = src,
                    Folder = targetFolder,
                    Overwrite = true,
                    Security = null
                });

                LogService.Info("Item added to template: " + src);
                return true;
            }

            return false;
        }

        private bool RemoveItemFromTemplate(string projectItemFullPath, string projectItemKind, LoadedXMLProvisioningTemplate loadedTemplate)
        {
            if (loadedTemplate != null && loadedTemplate.Template != null)
            {
                var src = ProvisioningHelper.MakeRelativePath(projectItemFullPath, loadedTemplate.ResourcesPath);

                if (projectItemKind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                {
                    // Remove all files where src path starts with given folder path
                    loadedTemplate.Template.Files.RemoveAll(f => f.Src.StartsWith(src, StringComparison.InvariantCultureIgnoreCase));
                }
                else if (projectItemKind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                {
                    // Remove all files where src path equals item path
                    loadedTemplate.Template.Files.RemoveAll(f => f.Src.Equals(src, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    return false; //terminate, wrong "kind"
                }

                LogService.Info("Item removed from template: " + src);
                return true;
            }

            return false;
        }

        private bool RenameTemplateItem(string projectItemFullPath, string projectItemKind, string oldName, LoadedXMLProvisioningTemplate loadedTemplate)
        {
            if (loadedTemplate != null && loadedTemplate.Template != null)
            {
                var src = ProvisioningHelper.MakeRelativePath(projectItemFullPath, loadedTemplate.ResourcesPath);

                if (projectItemKind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                {
                    var oldSrc = Path.Combine(src.Substring(0, src.TrimEnd('\\').LastIndexOf('\\')), oldName) + "\\";

                    // Remove all files where src path starts with given folder path
                    var filesToRename = loadedTemplate.Template.Files.Where(f => f.Src.StartsWith(oldSrc, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var file in filesToRename)
                    {
                        file.Src = Regex.Replace(file.Src, Regex.Escape(oldSrc), src, RegexOptions.IgnoreCase);
                    }
                }
                else if (projectItemKind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                {
                    var oldSrc = Path.Combine(Path.GetDirectoryName(src), oldName);

                    var file =
                        loadedTemplate.Template.Files.Where(
                            f => f.Src.Equals(oldSrc, StringComparison.InvariantCultureIgnoreCase))
                            .FirstOrDefault();

                    if (file != null)
                    {
                        file.Src = src;
                    }
                }
                else
                {
                    return false; //terminate, wrong item type
                }

                LogService.Info("Item renamed in template: " + src);
                return true;
            }

            return false;
        }

        private void DocEventsDocSaved(EnvDTE.Document Doc)
        {
            LogService.Info(string.Format("Document Saved : {0}", Doc.Name));
        }

        private IEnumerable<TemplateItem> GetSelectedItemTempateItems()
        {
            List<TemplateItem> templateItems = new List<TemplateItem>();

            try
            {
                var selectedItems = GetSelectedProjectItems();
                if (selectedItems != null)
                {
                    foreach (var projectItem in selectedItems)
                    {
                        var projectFolderPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                        var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath, false);
                        string itemPath = projectItem.Properties.Item("FullPath").Value as string;

                        if (!string.IsNullOrEmpty(itemPath) && ProjectHelpers.IncludeFile(itemPath))
                        {
                            var ti = new TemplateItem(this.ProvisioningService)
                            {
                                ItemPath = itemPath,
                                ProjectPath = projectFolderPath,
                                ProvisioningConfig = config,
                            };

                            ti.TemplateInfo = GetParentProvisioningTemplateInformation(ti.ItemPath, ti.ProjectPath, ti.ProvisioningConfig);

                            if (ti != null && ti.TemplateInfo != null)
                            {
                                templateItems.Add(ti);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Exception("Error getting selected item info", ex);
            }

            return templateItems;
        }

        private void ResetPendingItems()
        {
            pendingItemTemplates.Clear();
            pendingFolderTemplates.Clear();
        }

        private bool IsEnabledForCurrentProject()
        {
            var project = ProjectHelpers.GetActiveProject();

            return IsEnabledForProject(project);
        }

        private bool IsEnabledForProject(Project project)
        {
            bool isEnabled = false;
            try
            {
                var projectFolderPath = Helpers.ProjectHelpers.GetProjectFolder(project);

                var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath, false);
                if (config != null && config.ToolsEnabled)
                {
                    isEnabled = true;
                }
            }
            catch (Exception ex)
            {
                LogService.Exception("Error checking if enabled", ex);
            }

            return isEnabled;
        }

        private void GetUserCreds(ProvisioningTemplateToolsConfiguration config, string projectFolderPath, string credsFilePath)
        {
            ProvisioningCredentials creds = null;

            //prompt for credentials then persist to .user xml file
            VSToolsConfigWindow cfgWindow = new VSToolsConfigWindow();
            cfgWindow.txtSiteUrl.Text = config.Deployment.TargetSite;
            cfgWindow.txtUsername.Text = config.Deployment.Credentials.Username;
            cfgWindow.ShowDialog();

            bool saveNeeded = false;
            if (cfgWindow.DialogResult.HasValue && cfgWindow.DialogResult.Value)
            {
                config.Deployment.TargetSite = cfgWindow.txtSiteUrl.Text;
                creds = new ProvisioningCredentials()
                {
                    Username = cfgWindow.txtUsername.Text,
                };
                creds.SetSecurePassword(cfgWindow.txtPassword.Password);
                config.Deployment.Credentials = creds;

                saveNeeded = true;
            }

            if (saveNeeded)
            {
                //serialize the credentials to a file
                XmlHelpers.SerializeObject(config.Deployment.Credentials, credsFilePath);

                //save the config to a file
                SaveConfigForProject(projectFolderPath, config);

                //reset any cached sp contexts
                ProvisioningService.ResetSPContexts();
            }
        }

        private void AddTemplateToProject(string templatePath)
        {
            try
            {
                var projectItem = this.DTE.Solution.FindProjectItem(templatePath);

                if (projectItem == null)
                {
                    var project = Helpers.ProjectHelpers.GetProject();
                    project.ProjectItems.AddFromFile(templatePath);
                }
            }
            catch { }
        }

        private string EnsureResourcesFolder(string projectPath, string folderRelativePath)
        {
            string folderPath = System.IO.Path.Combine(projectPath, folderRelativePath);

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            try
            {
                var project = Helpers.ProjectHelpers.GetProject();
                project.ProjectItems.AddFolder(folderRelativePath);
            }
            catch { }

            return folderPath;
        }

        private ProvisioningTemplateToolsConfiguration GetConfig(bool createIfNotExists)
        {
            var project = ProjectHelpers.GetActiveProject();
            string projectPath = ProjectHelpers.GetProjectFolder(project);
            return GetConfig(projectPath, createIfNotExists);
        }

        private ProvisioningTemplateToolsConfiguration GetConfig(string projectFolderPath, bool createIfNotExists)
        {
            var configFilePath = Path.Combine(projectFolderPath, Resources.FileNameProvisioningTemplate);
            var config = GetProvisioningTemplateToolsConfiguration(projectFolderPath, createIfNotExists);

            return config;
        }

        private ProvisioningTemplateToolsConfiguration GetProvisioningTemplateToolsConfiguration(string projectFolderPath, bool createIfNotExists = false)
        {
            ProvisioningTemplateToolsConfiguration config = null;
            ProvisioningCredentials creds = null;

            var configFileCredsPath = Path.Combine(projectFolderPath, Resources.FileNameProvisioningUserCreds);
            var configFilePath = Path.Combine(projectFolderPath, Resources.FileNameProvisioningTemplate);
            var pnpTemplateFilePath = Path.Combine(projectFolderPath, Resources.DefaultFileNamePnPTemplate);

            try
            {
                //get the config from file
                config = Helpers.ProjectHelpers.GetConfigFile<ProvisioningTemplateToolsConfiguration>(configFilePath);
                creds = Helpers.ProjectHelpers.GetConfigFile<ProvisioningCredentials>(configFileCredsPath, false);

                if (config == null && createIfNotExists)
                {
                    var resourcesFolder = Resources.DefaultResourcesRelativePath;
                    EnsureResourcesFolder(projectFolderPath, resourcesFolder);
                    config = ProvisioningHelper.GenerateDefaultProvisioningConfig(Resources.DefaultFileNamePnPTemplate, resourcesFolder);
                }

                config.EnsureInitialState();

                //set the deserialized creds
                if (creds != null)
                {
                    config.Deployment.Credentials = creds;
                }

                //set paths
                config.FilePath = configFilePath;
                config.ProjectPath = projectFolderPath;
                config.Deployment.Credentials.FilePath = configFileCredsPath;

                //ensure a default template exists if createIfNotExists is true
                if (config.Templates.Count == 0 && createIfNotExists)
                {
                    var resourcesFolder = Resources.DefaultResourcesRelativePath;
                    EnsureResourcesFolder(projectFolderPath, resourcesFolder);
                    var tempConfig = ProvisioningHelper.GenerateDefaultProvisioningConfig(Resources.DefaultFileNamePnPTemplate, resourcesFolder);
                    config.Templates = tempConfig.Templates;
                    XmlHelpers.SerializeObject(config, configFilePath);

                    //ensure pnp template files
                    foreach (var t in config.Templates)
                    {
                        string templatePath = System.IO.Path.Combine(projectFolderPath, t.Path);
                        if (!System.IO.File.Exists(templatePath))
                        {
                            string resourcesPath = System.IO.Path.Combine(projectFolderPath, Resources.DefaultResourcesRelativePath);
                            ProvisioningHelper.GenerateDefaultPnPTemplate(resourcesPath, templatePath);
                            AddTemplateToProject(templatePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowOutputPane();
                LogService.Exception("Error in GetProvisioningTemplateToolsConfiguration", ex);
            }

            return config;
        }

        private bool SaveConfigForProject(string projectFolderPath, ProvisioningTemplateToolsConfiguration config)
        {
            try
            {
                var configFilePath = Path.Combine(projectFolderPath, Resources.FileNameProvisioningTemplate);
                XmlHelpers.SerializeObject(config, configFilePath);

                var configItem = this.DTE.Solution.FindProjectItem(configFilePath);
                if (configItem == null)
                {
                    var project = Helpers.ProjectHelpers.GetProject();
                    project.ProjectItems.AddFromFile(configFilePath);
                }
            }
            catch (Exception ex)
            {
                LogService.Exception("Error saving to config xml file", ex);
                return false;
            }

            return true;
        }

        public IEnumerable<ProjectItem> GetSelectedProjectItems()
        {
            List<ProjectItem> selectedProjectItems = new List<ProjectItem>();

            UIHierarchy hierarchy = this.DTE.ToolWindows.SolutionExplorer;
            var selectedItems = (Array)hierarchy.SelectedItems;
            if (selectedItems != null && selectedItems.Length > 0)
            {
                foreach (UIHierarchyItem selectedItem in selectedItems)
                {
                    ProjectItem projectItem = selectedItem.Object as ProjectItem;
                    if (projectItem != null)
                    {
                        selectedProjectItems.Add(projectItem);
                        //string filePath = projectItem.Properties.Item("FullPath").Value as string;
                        //if (!string.IsNullOrEmpty(filePath) && ProjectHelpers.IncludeFile(filePath))
                        //{
                        //    selectedItems.Add(filePath);
                        //}
                    }
                }
            }

            return selectedProjectItems;
        }

        //public IEnumerable<string> GetItemSelections(out IVsHierarchy hierarchy, out uint itemid)
        //{
        //    List<string> selections = new List<string>();

        //    hierarchy = null;
        //    itemid = VSConstants.VSITEMID_NIL;
        //    int hr = VSConstants.S_OK;

        //    IVsMultiItemSelect multiItemSelect = null;
        //    IntPtr hierarchyPtr = IntPtr.Zero;
        //    IntPtr selectionContainerPtr = IntPtr.Zero;

        //    try
        //    {
        //        var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
        //        if (monitorSelection == null || this.VSSolution == null)
        //        {
        //            return new List<string>(); //return empty selections
        //        }

        //        hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);
        //        hierarchy = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;

        //        if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL || hierarchy == null)
        //        {
        //            // invalid selection
        //            return new List<string>(); //return empty selections
        //        }

        //        //// there is a hierarchy root node selected, thus it is not a single item inside a project
        //        //if (itemid == VSConstants.VSITEMID_ROOT)
        //        //{
        //        //    return new List<string>(); //return empty selections
        //        //}

        //        // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
        //        Guid guidProjectID = Guid.Empty;
        //        if (ErrorHandler.Failed(this.VSSolution.GetGuidOfProject(hierarchy, out guidProjectID)))
        //        {
        //            return new List<string>(); //return empty selections
        //        }

        //        // if we got this far then there is a single project item selected
        //        //todo - return the selected item paths, relative to the resources folder??
        //    }
        //    finally
        //    {
        //        if (selectionContainerPtr != IntPtr.Zero)
        //        {
        //            System.Runtime.InteropServices.Marshal.Release(selectionContainerPtr);
        //        }

        //        if (hierarchyPtr != IntPtr.Zero)
        //        {
        //            System.Runtime.InteropServices.Marshal.Release(hierarchyPtr);
        //        }
        //    }

        //    return selections;
        //}

    }
}
