
param
(
    [Parameter(Mandatory=$true)][ValidateNotNullOrEmpty()] [String]$PFXFile,
    [Parameter(Mandatory=$true)][ValidateNotNullOrEmpty()] [String]$PFXPassword,
    [Parameter(Mandatory=$true)][ValidateNotNullOrEmpty()] [String]$ConnectSPFarmToAADModulePath
)

#Load the PS module when needed
$snapin = Get-PSSnapin | Where-Object {$_.Name -eq 'Microsoft.SharePoint.Powershell'}
if ($snapin -eq $null) 
{
    Write-Host "Loading SharePoint Powershell Snapin"
    Add-PSSnapin "Microsoft.SharePoint.Powershell"
} 

#Import the signing certificate and reset the timer service
$stsCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $PFXFile, $PFXPassword, 20
Set-SPSecurityTokenServiceConfig –ImportSigningCertificate $stsCertificate -confirm:$false
iisreset
net stop SPTimerV4
net start SPTimerV4

#Load the custom module with the Connect-SPFarmToAAD command
Import-Module ("{0}\Connect-SPFarmToAAD.psm1" -f $ConnectSPFarmToAADModulePath)
ls function:\ | where {$_.Name -eq "Connect-SPFarmToAAD"}

Write-Host
Write-Host "Use the Connect-SPFarmToAAD cmdlet to realize the low trust connection between your on-premises farm and your Office 365 tenant." -ForegroundColor Green
Write-Host "See below for typical sample usages:"

Write-Host "Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com" -ForegroundColor Cyan
Write-Host "Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com –SharePointWeb https://fabrikam.com" -ForegroundColor Cyan
Write-Host "Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com –SharePointWeb http://northwind.com -AllowOverHttp" -ForegroundColor Cyan
Write-Host "Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com –SharePointWeb http://northwind.com –AllowOverHttp –RemoveExistingACS –RemoveExistingSTS –RemoveExistingSPOProxy –RemoveExistingAADCredentials" -ForegroundColor Cyan
Write-Host "More information can be found at https://msdn.microsoft.com/en-us/library/office/dn155905.aspx"
Write-Host