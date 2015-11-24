# title of the PnP script or solution #

### Summary ###
Short summary.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises
-  SharePoint 2016 on-premises

-> Remove platforms if needed

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
solution name | Author

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | March 21st 2016 (to update/remove)| comment
1.0  | November 24th 2015 (to update) | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Doc scenario 1 #
Description
Image


## Sub level 1.1 ##
Description:
Code snippet:
```PowerShell
 #
 # Enable the remote site collection creation for on-prem in web application level
 # If this is not done, unknon object exception is raised by the CSOM code
 #
 $WebApplicationUrl = http://dev.contoso.com
 $snapin = Get-PSSnapin | Where-Object {$_.Name -eq 'Microsoft.SharePoint.Powershell'}
 if ($snapin -eq $null) 
 {
     Write-Host "Loading SharePoint Powershell Snapin"
     Add-PSSnapin "Microsoft.SharePoint.Powershell"
 }    
  
 $webapp=Get-SPWebApplication $WebApplicationUrl  

```

## Sub level 1.2 ##

# Doc scenario 2 #

## Sub level 2.1 ##

## Sub level 2.2 ##

### Note: ###

## Sub level 2.3 ##

# Doc scenario 3#

