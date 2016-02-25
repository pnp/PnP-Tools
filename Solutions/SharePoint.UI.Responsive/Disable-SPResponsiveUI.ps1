<#
.SYNOPSIS
Disables the Responsive UI for a target SharePoint 2013 or SharePoint 2016 on-premises site collection.

.EXAMPLE
PS C:\> .\Disable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite"

.EXAMPLE
PS C:\> $creds = Get-Credential
PS C:\> .\Disable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite" -Credentials $creds
#>
[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true, HelpMessage="Enter the URL of the target site collection, e.g. 'https://intranet.mydomain.com/sites/targetSite'")]
    [String]
    $TargetSiteUrl,

	[Parameter(Mandatory = $false, HelpMessage="Optional administration credentials")]
	[PSCredential]
	$Credentials
)

if($Credentials -eq $null)
{
	$Credentials = Get-Credential -Message "Enter Admin Credentials"
}

Write-Host -ForegroundColor White "--------------------------------------------------------"
Write-Host -ForegroundColor White "|              Disabling Responsive UI                 |"
Write-Host -ForegroundColor White "--------------------------------------------------------"

Write-Host -ForegroundColor Yellow "Target Site URL: $TargetSiteUrl"

try
{
    Connect-SPOnline $TargetSiteUrl -Credentials $Credentials
    Remove-SPOCustomAction -Identity "PnPResponsiveUI" -Scope Site -Force

    Write-Host -ForegroundColor Green "Responsive UI removal succeded"
}
catch
{
    Write-Host -ForegroundColor Red "Exception occurred!" 
    Write-Host -ForegroundColor Red "Exception Type: $($_.Exception.GetType().FullName)"
    Write-Host -ForegroundColor Red "Exception Message: $($_.Exception.Message)"
}