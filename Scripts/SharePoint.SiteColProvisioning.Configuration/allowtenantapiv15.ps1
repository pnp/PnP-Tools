param
(
[Parameter(Mandatory=$true)][ValidateNotNullOrEmpty()]
    [String]$WebApplicationUrl
)
#
# Enable the remote site collection creation for on-prem in web application level
# If this is not done, unknon object exception is raised by the CSOM code
#
$snapin = Get-PSSnapin | Where-Object {$_.Name -eq 'Microsoft.SharePoint.Powershell'}
if ($snapin -eq $null) 
{
    Write-Host "Loading SharePoint Powershell Snapin"
    Add-PSSnapin "Microsoft.SharePoint.Powershell"
}    
 
$webapp=Get-SPWebApplication $WebApplicationUrl
$newProxyLibrary = New-Object "Microsoft.SharePoint.Administration.SPClientCallableProxyLibrary"
$newProxyLibrary.AssemblyName = "Microsoft.Online.SharePoint.Dedicated.TenantAdmin.ServerStub, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
$newProxyLibrary.SupportAppAuthentication = $true
$webapp.ClientCallableSettings.ProxyLibraries.Add($newProxyLibrary)
$webapp.SelfServiceSiteCreationEnabled=$true
$webapp.Update()
Write-Host "Successfully added TenantAdmin ServerStub to ClientCallableProxyLibrary for web application" $WebApplicationUrl
# Reset the memory of the web application
Write-Host "IISReset..."    
Restart-Service W3SVC,WAS -force
Write-Host "IISReset complete on this server, remember other servers in farm as well." 
