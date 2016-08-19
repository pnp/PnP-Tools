# SharePoint Sandbox Solution scanner #

### Summary ###
Using this command line utility you can scan, download and analyze the sandbox solutions in your SharePoint environment.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises (coming soon!)
-  SharePoint 2016 on-premises (coming soon!)

### Solution ###
Solution | Author(s)
---------|----------
SharePoint.SandBoxTool | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 19th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What will this tool do for you? #
The main purpose of this tool is to give you a detailed view on the sandboxed solutions in your environment. You'll not only be able to see which sites do have sandbox solutions, whether they've an assembly and are activated, but the tool also can download and analyze the solution for you giving you information about what's inside (is it an InfoPath solution, does it contain web parts, does it contain event receivers,...). This information will be helpful in finding the needed remediation guidance and it will help assessing the sandbox remediation needs.

# Quick start guide #
Once you've downloaded the tool (or alternatively you can also compile it yourself using Visual Studio) you have a folder with the tool .exe file + supporting assemblies. Start a (PowerShell) prompt and navigate to that folder so that you can use the tool like is shown in below samples:

## Authentication options ##
Since this tool needs to be able to scan all site collections it's recommended to use an app-only principal with tenant scoped permissions for the scan. This approach will ensure the tool has access, if you use an account (e.g. your SharePoint tenant admin account) then the tool can only access the sites where this user also has access.

### Setting up an app-only principal with tenant permissions ###

## I want to scan and analyze my SharePoint Online MT environment ##

```cmd
sandboxtool -m <mode> -t <tenant> -c <clientid> -s <clientsecret>
```


