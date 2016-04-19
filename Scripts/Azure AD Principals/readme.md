# Azure Active Directory Principals #

### Summary ###
This script will assist you discovery of Client ID's that have expired in your Azure Active Directory Tenant

*work in progress*
 
### Applies to ###
-  SharePoint 2013 on-premises - Low Trust Configuration
-  SharePoint 2016 on-premises - Low Trust Configuration
-  SharePoint Online MT and Dedicated Services

### Prerequisites ###
[Microsoft Online Services Sign-In](https://www.microsoft.com/en-us/download/details.aspx?id=39267) Assistant is installed
[Microsoft Online Services PowerShell Module](http://go.microsoft.com/fwlink/p/?linkid=236297) is installed


### Solution ###
Solution | Author(s)
---------|----------
Principals | Frank Marasco (Microsoft)
 
### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 19th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

## Execution ##

Connect-MSolService

![](http://i.imgur.com/kXMizJt.png)

Get-ExpiredServicePrincipals

![](http://i.imgur.com/4jVYbWP.png)


## Resources ##
Replace an expiring client secret in a SharePoint Add-in](https://msdn.microsoft.com/en-us/library/office/dn726681.aspx)