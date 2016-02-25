<#
.SYNOPSIS
Enables the Responsive UI on a target SharePoint 2013 or SharePoint 2016 on-premises site collection.

.EXAMPLE
PS C:\> .\Enable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite"

.EXAMPLE
PS C:\> $creds = Get-Credential
PS C:\> .\Enable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite" -InfrastructureSiteUrl "https://intranet.mydomain.com/sites/infrastructureSite" -Credentials $creds
#>
[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true, HelpMessage="Enter the URL of the target site collection, e.g. 'https://intranet.mydomain.com/sites/targetSite'")]
    [String]
    $TargetSiteUrl,

    [Parameter(Mandatory = $false, HelpMessage="Enter the URL of the infrastructural site collection, if any. It is an optional parameter. Values are like: 'https://intranet.mydomain.com/sites/infrastructureSite'")]
    [String]
    $InfrastructureSiteUrl,

	[Parameter(Mandatory = $false, HelpMessage="Optional administration credentials")]
	[PSCredential]
	$Credentials
)

# If the Infrastructure Site URL is not provided, we will fallback to the Target Site URL
if ($InfrastructureSiteUrl -eq "") 
{
    $InfrastructureSiteUrl = $TargetSiteUrl
}

if($Credentials -eq $null)
{
	$Credentials = Get-Credential -Message "Enter Admin Credentials"
}

Write-Host -ForegroundColor White "--------------------------------------------------------"
Write-Host -ForegroundColor White "|               Enabling Responsive UI                 |"
Write-Host -ForegroundColor White "--------------------------------------------------------"

Write-Host -ForegroundColor Yellow "Target Site URL: $targetSiteUrl"
Write-Host -ForegroundColor Yellow "Infrastructure Site URL: $InfrastructureSiteUrl"

try
{
    Connect-SPOnline $InfrastructureSiteUrl -Credentials $Credentials
    Apply-SPOProvisioningTemplate -Path .\01-Responsive.UI.Infrastructure.xml -Handlers Files

    Connect-SPOnline $TargetSiteUrl -Credentials $Credentials
    Apply-SPOProvisioningTemplate -Path .\02-Responsive.UI.Template.xml -Handlers CustomActions,Features -Parameters @{"InfrastructureSiteUrl"=$InfrastructureSiteUrl}

    Write-Host -ForegroundColor Green "Responsive UI application succeded"
}
catch
{
    Write-Host -ForegroundColor Red "Exception occurred!" 
    Write-Host -ForegroundColor Red "Exception Type: $($_.Exception.GetType().FullName)"
    Write-Host -ForegroundColor Red "Exception Message: $($_.Exception.Message)"
}