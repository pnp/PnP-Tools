using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.VSTools.Helpers;
using Provisioning.VSTools.Models;
using Microsoft.VisualStudio.PlatformUI;
using System.Threading.Tasks;

namespace Provisioning.VSTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidProvisioning_VSToolsPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class Provisioning_VSToolsPackage : Package
    {

        internal static SimpleInjector.Container Container = null;
        private Services.IProjectService ProjectService = null;
        private Services.ILogService LogService = null;
        
        public Provisioning_VSToolsPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            try
            {
                //init IoC
                Container = IoCBootstrapper.GetContainer(false);
                Container.Register<Services.ILogService, Services.ProjectLogService>(SimpleInjector.Lifestyle.Singleton);
                Container.Register<Services.IProjectService, Services.ProjectService>(SimpleInjector.Lifestyle.Transient);

                Container.Verify();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Provisioning_VSToolsPackage failed: " + ex.Message);
            }
        }

        #region Package Members

        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            try
            {
                //init project service
                this.LogService = Container.GetInstance<Services.ILogService>();
                this.ProjectService = Container.GetInstance<Services.IProjectService>();
                this.ProjectService.Initialize(
                    (IVsUIShell)GetService(typeof(SVsUIShell)), //uishell
                    (DTE2)GetService(typeof(DTE)), //dte
                    (OleMenuCommandService)GetService(typeof(IMenuCommandService)) //mcs
                    );


                this.ProjectService.AddProjectCommands();
                this.ProjectService.AttachFileEventListeners();
            }
            catch (Exception ex)
            {
                LogService.Exception("Provisioning.VSTools Initialization error", ex);
            }
        }
        #endregion

    }
}
