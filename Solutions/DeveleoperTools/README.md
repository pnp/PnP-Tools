# DeveloperTools for SharePointPnP #

### Summary ###
Solution implements set of extentions for Visual Studio and MSBuild that simplify developement with SharePoint PnP Core library.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Prerequisites ###
Visual Studio SDK – this is a beta version of the extension package and requires the SDK to properly install.

###  Author(s) ###
Ivan Vagunin

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

# Intalling and debugging #
## Install custom MSBuild tasks
1) Build DeveloperTools.MSBuild.Tasks project in Visual studio.
2a) Post build event will trigger Install.ps1 (located in root folder) that will copy required dll files to MSBuild folder.
2b) If Install.ps1 fails (e.g. insufficient permissions) copy following dll manually to {ProgramFiles(x86)}\MSBuild\CommunityExtensions\SharePointPnP\
-  SharePointPnP.DeveloperTools.MSBuild.Tasks.dll
-  SharePointPnP.DeveloperTools.Common.dll
-  OfficeDevPnP.Core.dll
-  Microsoft.SharePoint.Client.dll
-  Microsoft.SharePoint.Client.Runtime.dll
-  SharePointPnP.DeveloperTools.Provisioning.targets

## Install Visual Studio extensions ##

To debug the extension package:
1) Navigate to the properties of DeveloperTools.VisualStudio project.
2) On Debug tab select 'Start external program' and set the path to your copy of visual studio (devenv.exe)
3) Set "Start Options" to: /rootsuffix Exp
4) Set DeveloperTools.VisualStudio project as startup project
5) Press F5 or click 'Start'