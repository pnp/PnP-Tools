using Microsoft.VisualStudio.Shell;
using System;
namespace Provisioning.VSTools.Services
{
    interface IProjectService
    {
        void Initialize(Microsoft.VisualStudio.Shell.Interop.IVsUIShell uiShell, EnvDTE80.DTE2 dte, OleMenuCommandService mcs);
        Microsoft.VisualStudio.Shell.Interop.IVsUIShell UIShell { get; }
        EnvDTE80.DTE2 DTE { get; }
        void AttachFileEventListeners();
        void DetachFileEventListeners();
        void AddProjectCommands();
        void RemoveProjectCommands();
    }
}
