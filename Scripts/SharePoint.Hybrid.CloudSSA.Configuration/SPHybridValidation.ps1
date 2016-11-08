        if ((Get-PSSnapin -Name "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) {
            Add-PSSnapin -Name "Microsoft.SharePoint.PowerShell"
        } 

cls

#Connect to the O365 Tenant
Write-Host "Please Conenct to the Microsoft Online tenant you wish to validate this on premises farm with" -ForegroundColor Yellow
Connect-MsolService 

#Validate Certificate is not expired - checks all Msol Principals not just current farm
Write-Host "Validating MSOL Principals still within valid date range" -ForegroundColor Yellow
Write-Host
Write-Host "Collecting All MSOL Principals" -ForegroundColor Yellow
$MsolSPC = Get-MsolServicePrincipalCredential -AppPrincipalId "00000003-0000-0ff1-ce00-000000000000" -ReturnKeyValues:$false|ft startdate,enddate ,keyid -autosize 
Write-Host
# Validate that the local STS Signing Certificate matches a valid Microsoft Online Service Principal
Write-Host "Validating that the local STS Signing Certificate matches a valid Microsoft Online Service Principal" -ForegroundColor Yellow

$StsThumbprint = (Get-SPSecurityTokenServiceConfig).LocalLoginProvider.SigningCertificate.thumbprint
#Changed to /sharepoint
$StsCertificate = get-item -Path CERT:\localmachine\SharePoint\$StsThumbprint
$StsCertificateBin = $StsCertificate.GetRawCertData()
$StsCredentialValue = [System.Convert]::ToBase64String($StsCertificateBin)
$MsolServicePrincipalCredential = Get-MsolServicePrincipalCredential -AppPrincipalId "00000003-0000-0ff1-ce00-000000000000" -ReturnKeyValues $true | Where-Object {$_.Value -eq $StsCredentialValue}

if($MsolServicePrincipalCredential){
    
    # Check for valid date 

    if($MsolServicePrincipalCredential.EndDate -gt (get-date) -and $MsolServicePrincipalCredential.StartDate -lt (get-date)){
        Write-Host
        write-host "Msol Service Principal is found and is Valid until :"$MsolServicePrincipalCredential.EndDate -ForegroundColor Green
        }
        Else{
        Write-Host
        write-host "Msol Service Principal is found but expired on " + $MsolServicePrincipalCredential.EndDate + " You should update the ACS trust certificates for hybrid workloads to function"  -ForegroundColor Yellow
        }
    } 
    else{
        Write-Host
        Write-host "No matching Msol Service Principal found for the local farm. Hybrid workloads will not function correctly on this farm" -foregroundcolor Red
    }

   
#Validate SPNs setup properly in WAAD
Write-Host
Write-Host "Validating that the Microsoft online Service Principal Names are correct" -ForegroundColor Yellow
$apps = (Get-MsolServicePrincipal -AppPrincipalId "00000003-0000-0ff1-ce00-000000000000").serviceprincipalnames
#$apps
$webapps = Get-SPWebApplication
$foundmatch = $false
Foreach($WebApp in $WebApps){

$webappurl = (($WebApp.url).split(":")).split("//")

Foreach($app in $apps){

$spn = ((($app).split("/")) -replace "\*.", "")

if(($webappurl[3] -match $spn[1]) -and ($spn[1] -ne $null )){
    Write-Host
    write-host "It seems the local farm has a matching SPN and URL combination " $webappurl[3] " matched with " $spn[1] " on this farm" -ForegroundColor Green
    write-host "This is required for hybrid scenarios requiring outbound OAuthN" -ForegroundColor Green
    $foundmatch=$true
    }
    }
}
if($foundmatch = $false){
    Write-Host
    write-host "No matching SPN and URL combinations found on this farm" -ForegroundColor Red
}

# Validate Directory Synchronization
Write-Host "Checking Directory Synchronization is Operational" -ForegroundColor Yellow
$msolcompany = Get-MsolCompanyInformation
$IsDirSyncEnabled = $msolcompany.DirectorySynchronizationEnabled
Write-Host "Directory Sync Enabled Status" $IsDirSyncEnabled -ForegroundColor Green
Write-Host
$LastDirsynctime = $msolcompany.LastDirSyncTime
Write-Host "Last Directory Sync Time" $LastDirsynctime -ForegroundColor Green
Write-Host

Write-Host "Checking license state of users" -ForegroundColor Yellow
$licensed = Get-MsolUser | ?{$_.IsLicensed -eq $true}
$unlicensed = Get-MsolUser | ?{$_.IsLicensed -eq $false}
$licenses = (MSOnline\Get-MsolSubscription).TotalLicenses

Write-Host "Total number of licensed users is " $licensed.count -ForegroundColor Green
Write-Host "Total number of unlicensed users is " $unlicensed.count -ForegroundColor Green
Write-Host "Total number of purchased licenses is " $licenses -ForegroundColor Green
Write-Host

#Validate Service Applications
Write-Host "Checking On Premises Services" -ForegroundColor Yellow
$upa=Get-SPServiceApplication | where-object {$_.TypeName -match "User profile Service Application"} 
if ($upa -ne $null -and $upa.Status -eq "online") {
Write-Host ("User Profile Service Application is available with status ") $upa.status -ForegroundColor Green
}
elseif ($upa -ne $null -and $upa.Status -ne "online"){
Write-Host ("User Profile Service Application is availble but status is ") $upa.status -ForegroundColor Red
}
else{
Write-Host ("User Profile Service Application is not available. ") $upa.status -ForegroundColor Red
}
Write-Host
$app=Get-SPServiceApplication | where-object {$_.TypeName -match "App Management Service Application"} 

if ($app -ne $null -and $app.Status -eq "online") {
Write-Host ("App Management Service Application is available with status ") $app.status -ForegroundColor Green
}
elseif ($app -ne $null -and $app.Status -ne "online"){
Write-Host ("App Management  Service Application is available but status is ") $app.status -ForegroundColor Red
}
else{
Write-Host ("App Management  Service Application is not available. ") $app.status -ForegroundColor Red
}
Write-Host
Write-Host "Checking On Proxies" -ForegroundColor Yellow

#Validate ACS Proxy

$proxy =Get-SPServiceApplicationProxy | ? {$_.TypeName -eq "Azure Access Control Service Application Proxy"}
if ($proxy -ne $null -and $proxy.Status -eq "online") {
Write-Host ("ACS proxy is available with status ") $proxy.status -ForegroundColor Green
}
elseif ($proxy -ne $null -and $proxy.Status -ne "online"){
Write-Host ("ACS proxy is available but status is ") $proxy.status -ForegroundColor Red
}
else{
Write-Host ("ACS proxy is  not available. ") -ForegroundColor Red
}

Write-Host
# Validate SPO proxy
$spoProxy = Get-SPServiceApplicationProxy | ? {$_.TypeName -eq "SharePoint Online Application Principal Management Service Application Proxy"}
if ($spoproxy -ne $null -and $spoproxy.Status -eq "online") {
Write-Host ("SPO proxy is available with status ") $spoProxy.status -ForegroundColor Green
}
elseif ($spoproxy -ne $null -and $spoproxy.Status -ne "online"){
Write-Host ("SPO proxy is available but status is ") $spoProxy.status -ForegroundColor Red
}
else{
Write-Host ("SPO proxy is  not available. ") -ForegroundColor Red
}

