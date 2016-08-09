# Sandbox solutions inventory script for SharePoint Online #
This script can be used to analyze what kind of sandbox solutions exists in specific SharePoint Online tenant. You will need to use tenant administrator account to connect to SharePoint Online and script will generate a list of sandbox solutions to separate txt file, which can be imported to Excel for further analyses.

>**Note**: This script is relatively simple and does not use multi-threading, so execution in larger tenants might take a while. We are looking for further enhancing the script with multi-threading support, if there's demand for this. Also community contributions on this side are more than welcome.

Output file has following columns
* URL of the site collection
* Name of the sandbox solution
* Author field from the sandbox solution - who uploaded the file. If value contains a comma, it will be removed in output
* Created field from the sandbox solution - when solution was uploaded
* Status field - 1=Activated, 0=Not activated
* HasAssemblies field - 1=solution has assemblies , 0=solution does not have assemblies
* SolutionHash field - hash of the solution. If solution is exactly the same cross site collections, has is the same. If solution has small adjustments or it's different version regardless of the wsp name matching, it's a different solution

Example output file, which can be easily imported to Excel for further analyses
```PowerShell
SiteURL,WSPName,Author,CreatedDate,Activated,HasAssemblies,SolutionHash
https://contoso.sharepoint.com/sites/635993483643159661,MyCoolWebPart.wsp,Vesa Juvonen,07/27/2016 00:55:22,0,1,IEYyOmzpYRAWRTLIuh0VbtUQFNJEsNxP6vVbECvVIY0=
https://contoso.sharepoint.com/sites/devpub,PnPRocks.wsp,System Account,07/27/2016 01:03:36,0,1,4KmaeOO1r7vw8/MDBuEmBzKMUtXxYhvc42vv/mw+Nnw=
https://contoso.sharepoint.com/sites/team,WhosReadingThese.wsp,Vesa Juvonen,06/07/2016 12:58:05,1,0,l8oO15G6LKX61wbTHZCCGyzNxZhYC3bbz5F+KsMbWYU=
https://contoso.sharepoint.com/sites/team,HiMom.wsp,Vesa Juvonen,06/22/2016 12:41:04,1,1,cdLgVhgD3uZCuoSl0VF/wBBF903ldJQJAG7sZWcAojw=
https://contoso.sharepoint.com/sites/team,WhatsUp.wsp,Vesa Juvonen,07/27/2016 00:47:54,0,0,tRB8KTZc6VTzJ6rGqtYCeFMozMroeIksQO2Bg0RJzcw=
https://contoso.sharepoint.com/sites/team,MrVanRules.wsp,Vesa Juvonen,07/29/2016 22:26:43,1,1,a48UeIRwd8HTReQ+UNnvH9w3NIKu2u4hYXK412riFgg=
https://contoso.sharepoint.com/sites/templatesite,RememberIgniteOnSeptember.wsp,Vesa Juvonen,07/27/2016 00:50:50,0,1,DsZBtAYAHjfNtHL8qytqI3//VwzSL3kR7rgkOG03Hl0=
https://contoso.sharepoint.com/sites/templatesite,WhatsUp.wsp,Vesa Juvonen,07/27/2016 00:56:32,1,0,MZ8BAIfCKdDhUx2rzQi9TlhVZ1lBn6f1F4/7+q0WekE=
https://contoso.sharepoint.com/sites/visiodemos,UsefullForYouGiveFeedback.wsp,Provisioning User 0,03/24/2014 21:44:25,1,0,xoCOeSDyhnYAY6nkXNpDpCL0A5TbLUTU4qX0q25ZFgE=

```

**What will be reported?**
Scripts reports all sandbox solution from solution gallery regardless if they contain code behind or regardless how they were created. Here's different kind of sandbox solutions, which you can have in the report.
* Site templates
* Design Manager Packages
* InfoPath Forms with code behind
* Custom solutions created with Visual Studio (web parts, web templates, event receivers)

 
>**Note**: This is an **Open Source** project, and any contribution from the community
is more than welcome. Feel free to review the code and submit any [Issues](https://github.com/OfficeDev/PnP-Tools/issues) or [Pull Requests](https://github.com/OfficeDev/PnP-Tools/pulls), using GitHub. If you want to assist the community on this journey around sandbox solutions, we are looking for example improvements on identifying sandbox solution type, code behind existence or to add multi-threading support.
 
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
* **Delimiter**: it is an optional parameter, by default is ',' and can be used to specify a different separation character for CSV output.

Here you can see a couple of examples about how to invoke the *Get-SPOnlineSandboxSolutionList.ps1* script:

###EXAMPLE 1
```PowerShell
PS C:\> .\Get-SPOnlineSandboxSolutionList.ps1 -AdminUrl "https://contoso-admin.sharepoint.com"
```

The example above performs as sandbox solution scan in contoso tenant with admin URL https://contoso-admin.sharepoint.com. The user's credentials are not provided, so the cmdlet will directly prompt the user.

###EXAMPLE 2
```PowerShell
PS C:\> .\Get-SPOnlineSandboxSolutionList.ps1 -AdminUrl "https://contoso-admin.sharepoint.com" -Delimiter ';'
```

This example above shows how to specify a custom character as delimiter for CSV output


###EXAMPLE 3
```PowerShell
PS C:\> $creds = Get-Credential
PS C:\> .\Get-SPOnlineSandboxSolutionList.ps1 -AdminUrl "https://contoso-admin.sharepoint.com" -Credentials $creds
```
 
The example above performs as sandbox solution scan in contoso tenant with admin URL https://contoso-admin.sharepoint.com. The user's credentials are  provided through the *$creds* variable.


--

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Following software installation packages have to be downloaded and installed on the machine where script will execute
- [SharePoint Online Client Components SDK](https://www.microsoft.com/en-us/download/details.aspx?id=42038)
- [SharePoint Online Management Shell](https://www.microsoft.com/en-us/download/details.aspx?id=35588)
- [SharePoint PowerShell CmdLets](https://www.microsoft.com/en-us/download/details.aspx?id=35588)

### **Note** ### 
`You will need to use tenant administrator credentials to be able to list site collections from target tenant.  
This account must have as well access to all site collections, in order to be able to access their solutions gallery.`

### Script ###
Script | Author(s)
---------|----------
SharePoint.Sandbox.ListSolutionsFromTenant | Karine Bosch (Microsoft), Vesa Juvonen (Microsoft), Daniel Budimir (Microsoft), Massimo Prota (Rapid Circle)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.3  | August 9th 2016 | Added hash of the solution as output
1.2  | August 4th 2016 | Added possibility to specify CSV delimiter, headers to output, better ClientContext handling
1.1  | August 3rd 2016 | Added HasAssemblies to output
1.0  | August 3rd 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**
