 if ((Get-PSSnapin -Name "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) {            
 Add-PSSnapin -Name "Microsoft.SharePoint.PowerShell"        } 
cls
　
function Find-AppInstances{

#SUMMARY Get all app instances in each of the web applications

$AppsforRemediation = @()
$webApplications = Get-SPWebApplication

foreach($webapp in $webapplications)
{
    foreach($site in $webApp.Sites)
    {
    try{
        foreach($web in $site.AllWebs)
        {
        $appInstance = Get-SPAppInstance -Web $web.Url | ? {$_.LaunchUrl -notlike "~appWebUrl*"} | select Title, AppPrincipalId
            if($appInstance -ne $null)
            {
                foreach ($instance in $appInstance)
                {
                  $tmp = $instance.AppPrincipalId.Split('|@',[System.StringSplitOptions]::RemoveEmptyEntries)
                  $appInfo = $instance.Title + " - " + $tmp[$tmp.Count - 2] + " - " + $web.Url 

                  $AppsforRemediation+=(,($instance.Title, $tmp[$tmp.Count - 2], $web.Url))
                          
                }
            }
        }
       }
    catch{

    # Typically this scenario would only arise when the Admin executing the script is not a farm admin, or the site is a Fast Site creation Master template site which is inaccessible by default.
    Write-Output  ""
    write-warning "Unable to enumerate webs at Site: $site.url : If this is expected then ignore this message"
    Write-Output  ""
    }
    } 
}
}

　
function Repair-TrustedSecurityTokenIssuer {

#SUMMARY  Fix up the SPTrustedSecurityTokenIssuers with the new SPAuthenticationRealmID to match the Office365

　
            $NewRealm = Get-SPAuthenticationRealm
            $sts = Get-SPTrustedSecurityTokenIssuer | ? {$_.Name -ne 'EvoSTS-Trust' -and $_.Name -ne 'ACS_STS'} | Select RegisteredIssuerName
            $realm = $sts | ?{$_.RegisteredIssuerName -ne $null} | %{$($($_.RegisteredIssuerName).toString().split('@',2)[1]).toString()} | ?{$_ -ne '*' -and $_ -ne $newRealm}

            if($Realm.count -gt 0) 
            {
            try{
                $TempRealm = '*@$($NewRealm)'
                $Issuers = Get-SPTrustedSecurityTokenIssuer | ?{$_.Name -ne 'EvoSTS-Trust' -and $_.Name -ne 'ACS_STS' -and $_.RegisteredIssuerName -ne $null -and $_.RegisteredIssuerName -notlike '*@`*' -and $_.RegisteredIssuerName -notlike $TempRealm}

                $Guid = [guid]::NewGuid()
                foreach ($Issuer in $Issuers)
                {
                    $NameCopy = $Issuer.Name
                    $NewIssuerName = $Guid
                    $IssuerCertificate = $Issuer.SigningCertificate
                    $OldRegisteredIssuerID = $Issuer.RegisteredIssuerName
                    $IssuerID = $OldRegisteredIssuerID.Split('@')[0]
                    $NewRegisteredIssuerName = $IssuerID + '@' + $NewRealm

                    $NewIssuer = New-SPTrustedSecurityTokenIssuer -Name $NewIssuerName -Certificate $IssuerCertificate -RegisteredIssuerName $NewRegisteredIssuerName -IsTrustBroker
                    Remove-SPTrustedSecurityTokenIssuer $Issuer -Confirm:$false
                    $NewIssuer.Name = $NameCopy
                    $NewIssuer.Update()
                }
            }
            Catch{
            Write-Output ""
            Write-Error "Failed to update SharePoint Trusted Security Token Issuer for $NewIssuerName. Investigate and correct before re-running the script else PHAs depending on this Issuer will fail"
            }
            }
}

　
function Repair-AppPermissions{

$AppCount = $AppsforRemediation.count

for ($i = 0; $i -lt $AppCount; $i++)
{ 
   
$appTitle = $AppsforRemediation[$i][0]
$clientID = $AppsforRemediation[$i][1]
$targetWeb = Get-SPWeb $AppsforRemediation[$i][2]
$appweb = $targetWeb.Url
$Scope = 'Site'
$Right = 'FullControl'
        $authRealm = Get-SPAuthenticationRealm -ServiceContext $targetWeb.Site
        $AppIdentifier = $clientID + '@' + $authRealm
        Write-Output ""
        Write-Output "Repairing Permissions for $appTitle on site $appweb"

        $regprincipal = Register-SPAppPrincipal -NameIdentifier $AppIdentifier -Site $targetWeb -DisplayName $appTitle
        $appPrincipal = Get-SPAppPrincipal -Site $targetWeb -NameIdentifier $AppIdentifier
        Set-SPAppPrincipalPermission -Site $targetWeb -AppPrincipal $appPrincipal -Scope $Scope -Right $Right

}

}

　
　
Find-AppInstances

if($AppsforRemediation.Count -gt 0){
    
    Write-Output ""
    Write-Output "Provider Hosted App Instances Discovered: $AppCount Total PHAs"
   

Write-Output ""
Write-Output "Repairing the SPAuthenticationrealmID"

Repair-TrustedSecurityTokenIssuer

Write-Output ""
Write-Output "Repairing the Individual App Permissions"

Repair-AppPermissions

}
else
{
Write-Output ""
Write-Output "There are no remediation actions required on this farm"
}
 
