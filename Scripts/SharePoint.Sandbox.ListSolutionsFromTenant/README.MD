# Generate list of sandbox solutions in SharePoint Online tenant #
This script can be used to generate list of sandbox solutions in SharePoint Online tenant. You will need to use tenant administrator account to connect to SharePoint Online and script will generate a list of sandbox solutions to seperate txt file, which can be imported to Excel for further analyses.

>**Note**: This script is relatively simple and does not use multi-threading, so execution in larger tenants might take a while. We are looking for further enhancing the script with multi-threading support, if there's demand for this. Also community contributions on this side are more than welcome.

Output file has following columns
* URL of the site collection
* Name of the sandbox solution
* Author field from the sandbox solution - who uploaded the file
* Created field from the sandbox solution - when solution was uploaded
* Status field - 1=Activated, 0=Not activated

Example output file, which can be easily imported to Excel for further analyses
```PowerShell
https://contoso.sharepoint.com/sites/635993483643159661,Fuubar.wsp,Vesa Juvonen,07/27/2016 00:55:22,0
https://contoso.sharepoint.com/sites/devpub,Dev Pub-1.0.wsp,System Account,07/27/2016 01:03:36,0
https://contoso.sharepoint.com/sites/team,SPrello.wsp,Vesa Juvonen,06/07/2016 12:58:05,1
https://contoso.sharepoint.com/sites/team,Template.wsp,Vesa Juvonen,06/22/2016 12:41:04,1
https://contoso.sharepoint.com/sites/team,SharePointProject1.wsp,Vesa Juvonen,07/27/2016 00:47:54,0
https://contoso.sharepoint.com/sites/team,EmptySandbox.wsp,Vesa Juvonen,07/29/2016 22:26:43,1
https://contoso.sharepoint.com/sites/templatesite,SharePointProject1.wsp,Vesa Juvonen,07/27/2016 00:50:50,0
https://contoso.sharepoint.com/sites/templatesite,Fuubar.wsp,Vesa Juvonen,07/27/2016 00:56:32,1
https://contoso.sharepoint.com/sites/visiodemos,UpdateVSDXDataConnections.wsp,Provisioning User 0,03/24/2014 21:44:25,1

```

**What will be reported?**
Scripts reports all sandbox solution from solution gallery regardless if they contain code behind or regardless ho they were created. Here's different kind of sandbox solutions, which you can have in the report.
* Site templates
* Design Manager Packages
* InfoPath Forms with code behind
* Custom solutions created with Visual Studio (web parts, web templates, event receivers)

 
>**Note**: This is an **Open Source** project, and any contribution from the community
is more than welcome. Feel free to review the code and submit any [Issues](a href="https://github.com/OfficeDev/PnP-Tools/issues) or [Pull Requests](https://github.com/OfficeDev/PnP-Tools/pulls), using GitHub. If you want to assist the community on this journey around sandbox solutions, we are looking for example improvements on identifying sandbox solution type, code behind existance or to add multi-threading support.
 
# Setup Instructions #
In order to get list of sandbox solutions using this script, you'll need to:
* [Download the files included in this solution](#download)
* [Setup software requirements](#requirements)
* [Execute the *Get-SPOnlineSandboxSolutionList* cmdlet](#execute)

<a name="download"></a>
## Download the files
You can download the script file manually or you can download
a ZIP file with all the content from PnP-Tools repository, simply following
<a href="https://github.com/OfficeDev/PnP-Tools/archive/master.zip">this link</a>. 
Within the ZIP file, under the /Scripts/SharePoint.Sandbox.ListSolutionsFromTenant folder, you will
find all the required files.

<a name="requirements"></a>
## Setup software requirements
This scripts requires SharePoint Online PowerShell commands, which you can install
from the following link:

* [SharePoint PowerShell commands](https://www.microsoft.com/en-us/download/details.aspx?id=35588) 

<a name="execute"></a>
## Execute the *Get-SPOnlineSandboxSolutionList* cmdlet
Once you have installed the SharePoint PowerShell commands, you can  simply open a 
PowerShell console, go to the path where you stored the files and execute the *Get-SPOnlineSandboxSolutionList*
cmdlet, which is included in the 
<a href="./Get-SPOnlineSandboxSolutionList.ps1">Get-SPOnlineSandboxSolutionList.ps1</a> script file of this solution.

The *Get-SPOnlineSandboxSolutionList* cmdlet accepts the following three parameters:
* **AdminUrl**: it is a mandatory parameter, which declares the URL of the admin site for the tenant where the sandbox solution scan will be performed. It has to be provided as a full URL, like for example: https://contoso-admin.sharepoint.com
* **Credentials**: it is an optional parameter, which defines the user credentials that will be used to authenticate against both the target Site Collection and the infrastructure Site Collection, if any. Should be the credentials of a user, who is Site Collection Administrator for the target Site Collections. If you don't provide this parameter, the script will directly prompt you for credentials.

Here you can see a couple of examples about how to invoke the *Get-SPOnlineSandboxSolutionList.ps1* script:

###EXAMPLE 1
```PowerShell
PS C:\> .\Get-SPOnlineSandboxSolutionList.ps1 -AdminUrl "https://contoso-admin.sharepoint.com"
```

The example above performs as sandbox solution scan in contoso tenant with admin URL https://contoso-admin.sharepoint.com. The user's credentials are not provided, so the cmdlet will directly prompt the user.

###EXAMPLE 2
```PowerShell
PS C:\> $creds = Get-Credential
PS C:\> .\Get-SPOnlineSandboxSolutionList.ps1 -AdminUrl https://contoso-admin.sharepoint.com -Credentials $creds
```
 
The example above performs as sandbox solution scan in contoso tenant with admin URL https://contoso-admin.sharepoint.com. The user's credentials are  provided through the *$creds* variable.


--

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
- You will need to have [SharePoint PowerShell CmdLets](https://www.microsoft.com/en-us/download/details.aspx?id=35588) installed on the machine where script is executed. 
- You will need to use tenant administrator credentials to be able to list site collections from target tenant

### Script ###
Script | Author(s)
---------|----------
SharePoint.Sandbox.ListSolutionsFromTenant | Karine Bosch (Microsoft), Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 3rd 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
