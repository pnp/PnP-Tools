<#
.SYNOPSIS
Enables the Responsive UI on a target SharePoint 2013, SharePoint 2016 on-premises or SharePoint Online site collection.

.EXAMPLE
PS C:\> .\Enable-SPResponsiveUI.ps1 -Web "https://intranet.mydomain.com/sites/targetSite"

.EXAMPLE
PS C:\> $creds = Get-Credential
PS C:\> .\Enable-SPResponsiveUI.ps1 -Web "https://intranet.mydomain.com/sites/targetSite" -Credentials $creds
#>
[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true, HelpMessage="Enter the URL of the target site collection, e.g. 'https://intranet.mydomain.com/sites/targetSite'")]
    [String]
    $Web,

    [Parameter(Mandatory = $false, HelpMessage="Optional administration credentials")]
    [PSCredential]
    $Credentials
)

if($Credentials -eq $null)
{
	$Credentials = Get-Credential -Message "Enter Admin Credentials"
}

Write-Host -ForegroundColor White "--------------------------------------------------------"
Write-Host -ForegroundColor White "|               Enabling Responsive UI                 |"
Write-Host -ForegroundColor White "--------------------------------------------------------"
Write-Host

try
{
    Write-Host -ForegroundColor Yellow "Connecting to target site URL: $Web"
    Connect-SPOnline $Web -Credentials $Credentials
    Write-Host -ForegroundColor Yellow "Enabling responsive UI on target site"
    Enable-SPOResponsiveUI 
    Write-Host -ForegroundColor Yellow "Uploading custom responsive UI assets to target site"
    Apply-SPOProvisioningTemplate -Path .\Responsive.UI.Infrastructure.xml -Handlers Files
    Write-Host -ForegroundColor Green "Responsive UI application succeeded"
}
catch
{
    Write-Host -ForegroundColor Red "Exception occurred!" 
    Write-Host -ForegroundColor Red "Exception Type: $($_.Exception.GetType().FullName)"
    Write-Host -ForegroundColor Red "Exception Message: $($_.Exception.Message)"
}