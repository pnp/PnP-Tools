<# Disclaimer

 Microsoft provides programming examples for illustration only, without warranty either expressed or
 implied, including, but not limited to, the implied warranties of merchantability and/or fitness 
 for a particular purpose. 
 
 This sample assumes that you are familiar with the programming language being demonstrated and the 
 tools used to create and debug procedures. Microsoft support professionals can help explain the 
 functionality of a particular procedure, but they will not modify these examples to provide added 
 functionality or construct procedures to meet your specific needs. if you have limited programming 
 experience, you may want to contact a Microsoft Certified Partner or the Microsoft fee-based consulting 
 line at (800) 936-5200. 

# HYBRID CONFIGURATION WIZARD # 
# 20/11/2015 - Release v0.9
# 20/11/2015 - Added pre req checks and more graceful exit
# 11/11/2015 - Fixed up dirsync report format; Make http call to spsite after iisreset, single domain name url fixed, debug text cleanup
# 09/04/2015 - Implemented Form.Focus, added cleanup option (not quite ready) and fixed Wizard Exit on Completion
# 20/03/2015 - Changed text boxes to labels. Added Dirsync check and local admin check. Removed IISReset Warning and updated Start/Exit button behaviour
# 09/01/2015 - Fixed problem installing .MSI files
# 18/12/2014 - V0.3 released for testing
# 10/12/2014 - Self Signed Cert bug fixed by implementing a -1 day to start date
# 09/12/2014 - Fixed certificate export bug - missing move to root auth section
# 09/12/2014 - Added code to remove existing certs from my and root if same self signed name is used.
# 08/12/2014 - Added multiple error checking for failed SPO logins
# 05/12/2014 - Fixed naming conventions for functions

# Version Control

#Update this version number for each release
#>

$HWVersion = " v0.9"

#Useful references - PowerShell forms and Controls - http://technet.microsoft.com/en-us/library/ff730941.aspx
Add-PSSnapin Microsoft.SharePoint.PowerShell -EA 0
Add-Type -AssemblyName System.Windows.Forms

#region FunctionDefs

######################################################################
# Import-ModulesAndConnect                                           #
# Prompts for O365 tenant login creds                                #
######################################################################
function Import-ModulesAndConnect(){
Write-Host "Entering Import-Modules and Connect"

Update-WizardProgress "Importing required PowerShell Modules and Connecting to O365" 

# Import MSOnline Modules
# To Add - if msol modules are not available we need to add a section to prompt for download and install

#Import the Microsoft Online Services Sign-In Assistant.
Import-Module -Name MSOnline
#Import the Microsoft Online Services Module for Windows Powershell.
Import-Module MSOnlineExtended –force 
$O365Creds = ""
$ExitConnect = $false
#We are going to use the $error output to track the last error message to see if it corresponds to an invalid login attempt
$error.Clear()
do
{
  $error.Clear()
  $O365Creds = Get-Credential -Message "Tenant Admin credentials"
  Connect-MsolService -Credential $O365Creds
  $Theuser = $O365Creds.UserName
If ($error[0].Exception -match "The user name or password is incorrect")
{
Update-WizardProgress "Login Unsuccessful"
$O365Connected = $false, $Theuser
$ExitConnect = $false
}
else
{
Update-WizardProgress "Logged into MSOL as $TheUser"
$O365Connected = $true, $Theuser
$ExitConnect = $true
}

if ($ExitConnect -eq $false)
{
Update-WizardProgress "Connection to MSOL failed - Try Again?"

$msgBox = Show-MessageBox "You failed to authenticate to Office 365 as a global admin.`nWould you like to try again?" -YesNo

# Checking if user wants to exit the login attempt
    if ($msgbox -eq "No"){ 
        $ExitConnect = $true
        }
    else{
        $ExitConnect = $false
        }
}
}
until ($ExitConnect -eq $true)


#Check for dirsync active - warn if not
#Report last time sync - warn is last dirsync time > 24 hours

$msolcompany = Get-MsolCompanyInformation
$IsDirSyncEnabled = $msolcompany.DirectorySynchronizationEnabled
$LastDirsynctime = $msolcompany.LastDirSyncTime
$thedate = Get-Date

$SyncDelay = (get-date) - ($msolcompany.LastDirSyncTime)
$SyncDelayDays = $SyncDelay.Days
$SyncDelayHours = $SyncDelay.Hours
$SyncDelayMinutes = $SyncDelay.Minutes

if ($IsDirSyncEnabled -eq $true)
{
    Update-WizardProgress "Dirsync is enabled for the O365 Tenancy"
    Write-Host "Dirsync is enabled for the O365 Tenancy"
    Update-WizardProgress "Last Dirsync was $SyncDelayHours hours and $SyncDelayMinutes minutes ago"
    Write-Host "Last Dirsync was $SyncDelayHours hours and $SyncDelayMinutes minutes ago"

    if($SyncDelay.Days -gt 0)
    {
    Show-MessageBox -Title "Dirsync Warning" "Your Dirsync process has not completed for $SyncDelayDays Days and $SyncDelayHours Hours`nYou can continue with the Wizard but it is recommended you investigate and fix and errors with the sync process as soon as possible" -Warning | out-null
    Update-WizardProgress "Dirsync Warning - Over 24 Hours since last sync"
    }
}
else
{
    Show-MessageBox -Title "Dirsync Critcal" "Dirsync is not enabled for the Tenancy!`n`nYou can continue with the Wizard but it is required that you complete this process to enable hybrid user experiences" -Critical | out-null
    Update-WizardProgress "Dirsync is NOT enabled for the O365 Tenancy"
}

return $O365Connected

}

######################################################################
# Register-ServicePrincipalO365                                      #
# Registers the service principal for O365 using the certificate     #
# $spo_appid = Standared SPO App ID GUID                             #
# $cred_value = Base64 encoding of the STS.cer certificate           #
######################################################################
function Register-ServicePrincipalO365($spo_appid, $cred_value){

write-host "Entering Register-ServicePrincipalO365 with parameters" $spo_appid $cred_value

Update-WizardProgress "Registering Service App Principal" 

#Register the On-Premise STS as Service Principal in Office 365

New-MsolServicePrincipalCredential -AppPrincipalId $spo_appid -Type asymmetric -Usage Verify -Value $cred_Value 
$spocontextID = (Get-MsolCompanyInformation).ObjectID
$spoappprincipalID = (Get-MsolServicePrincipal -ServicePrincipalName $spo_appid).ObjectID
$sponameidentifier = "$spoappprincipalID@$spocontextID"

$SPO365 = $spocontextID, $sponameidentifier

return $SPO365
}

######################################################################
# Establish-ACSTrust                                                 #
# Creates the ACS proxy and Adds ACS as security token issues        #
# $sp_site = defines the site for app prinicpal registeration        #
# $spo_name_identifier = SPOID and Tenant ContextID                  #
# $spo_context_ID = Establishes Authentication Realm                 #
######################################################################
function Establish-ACSTrust($SPSite, $spo_name_identifier, $spo_context_ID){

write-host "Entering Establish_ACSTrust with parameters" $spsite $spo_name_identifier $spo_context_ID

Update-WizardProgress "Establishing ACS Trust and Deploying Proxy"

#First we remove old ACS Proxy and SecurityTokenIssues
$OldACSProxy = Get-SPServiceApplicationProxy | ? {$_.typename -match "Azure Access Control Service Application Proxy"} | Remove-SPServiceApplicationProxy
$OldTSTI = Get-SPTrustedSecurityTokenIssuer | ? {$_.name -match "ACS"} | Remove-SPTrustedSecurityTokenIssuer

#Finally Establish in the On-Premise Farm a Trust with the ACS

$rootsite = Get-SPSite $SPSite

$appPrincipal = Register-SPAppPrincipal -site $rootsite.rootweb -nameIdentifier $spo_name_identifier -displayName "SharePoint Online" #Error here
Set-SPAuthenticationRealm -realm $spo_context_ID 
New-SPAzureAccessControlServiceApplicationProxy -Name "ACS" -MetadataServiceEndpointUri "https://accounts.accesscontrol.windows.net/metadata/json/1/" -DefaultProxyGroup
New-SPTrustedSecurityTokenIssuer -MetadataEndpoint "https://accounts.accesscontrol.windows.net/metadata/json/1/" -IsTrustBroker -Name "ACS"
}

######################################################################
# Manage-STSCertificate                                              #
# Sequence of steps to implement selected Certificates Process       #
# Options are SelfSigned, Default or Public                          #
# v1 = SelfSigned only                                               #
# $spoapplid = SPO App ID GUID                                       #
# $spcname = on premises SP domain name for SPN                      #
######################################################################
function Manage-STSCertificate($spoapplid, $SPSite, $CommonName){

Write-Host "Entering manage-stscertificate with parameters $spoapplid $SPSite $commonname"

Update-WizardProgress "Beginning Certificate Management Process"

$indexHostName = $SPSite.IndexOf('://') + 3
        $HostName = $SPSite.Substring($indexHostName)
		$indexHostName = $HostName.IndexOf('/')
		if ($indexhostName -ge 0) {
			$HostName = $HostName.Substring(0,$indexHostName)
		}

$partfqdn = $Hostname.Indexof('.')

# Check for single part domain name, ie http://spweb as this will result in $partfqdn value of -1
if($partfqdn -lt 0)
{
    $Hostname = "*" + $Hostname
}
else
{
    $Hostname = "*" + $Hostname.Substring($partfqdn)   ##substring error here
}

        $NewSPN = '{0}/{1}' -f $spoapplid, $HostName

        $SPAppPrincipal = Get-MsolServicePrincipal -AppPrincipalId $spoapplid
        if ($SPAppPrincipal.ServicePrincipalNames -notcontains $NewSPN) {
            $SPAppPrincipal.ServicePrincipalNames.Add($NewSPN)
            Set-MsolServicePrincipal -AppPrincipalId $SPAppPrincipal.AppPrincipalId -ServicePrincipalNames $SPAppPrincipal.ServicePrincipalNames
        }

$UpdateWizardProgress = "SPN in O365 configured for $Hostname"

Update-WizardProgress $UpdateWizardProgress

if($radiobuttonUsePublicAuthoritySi.Checked)
{
#Use Public Authority Certificate
}

if($radiobuttonUseNewSelfSignedSTSC.Checked){
#Use Auto Generated Self Signed Certificate

Update-WizardProgress "New selfsigned certificate selected"
$CertPass = read-host -Prompt "Please enter the password to secure the self signed certificate" -AsSecureString
Update-WizardProgress "Creating selfsigned certificate and adding to trusted root store" 
$CreateCert = Add-SelfSignedCertificate $CommonName 
Update-WizardProgress "Updating Farm Secure Token Service with new Certificate"
$UpdateFarmSTS = Update-FarmSTS $CommonName $CertPass
Update-WizardProgress "Generating Credentials for O365 S2S trust"
$CredentialValue = Convert-CertsForUpload $CommonName $CertPass
Update-WizardProgress "Registering the Service App Principal"
$RegServiceAppPrin = Register-ServicePrincipalO365 $spoapplid $CredentialValue

$spo_contextID = $RegServiceAppPrin[0]
$spo_nameidentifier = $RegServiceAppPrin[1]
Update-WizardProgress "Setting up the S2S trust and deploying ACS Proxy"
$EstablishACSTrust = Establish-ACSTrust $SPSite  $spo_nameidentifier  $spo_contextID

#At this Point Hybrid identity setup is complete for NewSelfSignedCertificate

Update-WizardProgress "Hybrid Certificate Management process is complete"
Write-Host "Hybrid Certificate Management process is complete"
}

if($radiobuttonUseBuiltInSharePoint.Checked){
#Use Built In STS Certificate

Update-WizardProgress"Built in certificate selected"
#$sp_site = $labelOnPremisesSharePoin.Text  # future where user gets option to choose which on prem site to configure
Update-WizardProgress"Exporting Default STS Cert"
$LocalCertCred = Export-LocalSTSCert
#Register the Service Principal
Update-WizardProgress"Registering the Service App Principal"
$RegServiceAppPrin = Register-ServicePrincipalO365 $spoapplid $LocalCertCred

$spo_contextID = $RegServiceAppPrin[0]
$spo_nameidentifier = $RegServiceAppPrin[1]
Update-WizardProgress"Setting up the S2S trust and deploying ACS Proxy"
$EstablishACSTrust = Establish-ACSTrust $SPSite $spo_nameidentifier $spo_contextID

#At this Point Hybrid identity setup is complete for ExportingTheDefaultCertificate

Update-WizardProgress "Hybrid Certificate Management process is complete"
Write-Host "Hybrid Certificate Management process is complete"

}
}

######################################################################
# Export-LocalSTSCert                                                #
# Exports local STS Cert and converts for use as CredValue           #
# for settings up the trust                                          #
######################################################################
function Export-LocalSTSCert(){
#Uses the local signing key for upload to O365 instead of creating a new one
$SPSigningCert = (Get-SPSecurityTokenServiceConfig).LocalLoginProvider.SigningCertificate
$ExportedCert = $SPSigningCert.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
$CertValue = [System.Convert]::ToBase64String($ExportedCert,[System.Base64formattingoptions]::InsertLineBreaks)

return $CertValue
}

######################################################################
# Update-FarmSTS                                                     #
# Updates the Farm STS with the specified certificate                #
# $commonname = Certificate common name                              #
# $CertSecret = Password to use to secure the certificate            #
######################################################################
function Update-FarmSTS($commonname, $SecCertSecret){

write-host "Entering Update-FarmSTS with parameters $commonname $SecCertSecret"

#$SecCertSecret = ConvertTo-SecureString $CertSecret -AsPlainText -Force

#Export the required certificates for Updating STS and Uploading to ACS

  $certstore = dir Cert:\LocalMachine\Root `
  | where-object {$_.Subject -eq "CN=$commonname"} `
  | foreach-object{
  [system.IO.file]::WriteAllBytes(    "$home\$($_.subject).pfx",     ($_.Export('PFX', $SecCertSecret)) ) 
  [system.IO.file]::WriteAllBytes(    "$home\$($_.subject).cer",     ($_.Export('CER', $SecCertSecret)) )
  }

  $pfxcertname = "$home\CN=$commonname"+".pfx"

  $pfxCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $pfxcertname, $SecCertSecret, 20
  Set-SPSecurityTokenServiceConfig -ImportSigningCertificate $pfxCertificate

   
#Restart IIS so STS Picks up the New Certificate

Update-WizardProgress "The IIS Services and the SharePoint Timer Service are now going to be recycled."

& iisreset
& net stop SPTimerV4
& net start SPTimerV4

#(Get-SPSecurityTokenServiceConfig).LocalLoginProvider.SigningCertificate  #DEBUG ONLY

}

######################################################################
# Convert-CertsforUpload                                             #
# Converts Certificate to Base 64 for use as App Principal Cred      #
# $LocalDN = Certificate common name                              #
# $CertSecret = Password to use to secure the certificate            #
######################################################################
function Convert-CertsForUpload($LocalCN, $CertSecret){
#Do Some Conversions With the Certificates to Base64 
#Return $credValue for use in Adding App Principal

$stscertpfx = "$home\CN=$LocalCN.pfx"
$stscertpassword = $CertSecret
$stscertcer = "$home\CN=$LocalCN.cer"

$pfxCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList $stscertpfx,$stscertpassword
$pfxCertificateBin = $pfxCertificate.GetRawCertData()
$cerCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cerCertificate.Import($stscertcer)
$cerCertificateBin = $cerCertificate.GetRawCertData()
$credValue = [System.Convert]::ToBase64String($cerCertificateBin)

Return $credValue

}

######################################################################
# Add-SelfSignedCertificate                                          #
# Adds a Self Signed Certificate to the Local Machine Personal Store #
# $commonname defines the certificate subject to be used             #
######################################################################
function Add-SelfSignedCertificate($commonname){
    
    #Remove and existing certificates with the same common name from personal and root stores
    #Need to be very wary of this as could break something

    $certs = dir Cert:\LocalMachine\my | ?{$_.Subject -eq "CN=$commonname"}
    $mystore = get-item Cert:\LocalMachine\My
    $mystore.open("ReadWrite")
    Foreach($acert in $certs){
    $mystore.Remove($acert)}
    $mystore.close()

    $certs = dir Cert:\LocalMachine\Root | ?{$_.Subject -eq "CN=$commonname"}
    $rootstore = get-item Cert:\LocalMachine\Root
    $rootstore.open("ReadWrite")
    Foreach($acert in $certs){
    $rootstore.Remove($acert)}
    $rootstore.close()

    $name = new-object -com "X509Enrollment.CX500DistinguishedName.1"
    $name.Encode("CN=$CommonName", 0)

    $key = new-object -com "X509Enrollment.CX509PrivateKey.1"
    $key.ProviderName = "Microsoft RSA SChannel Cryptographic Provider"
    $key.KeySpec = 1
    $key.Length = 2048 # Modified to 2048
    $key.SecurityDescriptor = "D:PAI(A;;0xd01f01ff;;;SY)(A;;0xd01f01ff;;;BA)(A;;0x80120089;;;NS)"
    $key.MachineContext = 1
    $key.ExportPolicy = 1 # This is required to allow the private key to be exported
    $key.Create()

    $serverauthoid = new-object -com "X509Enrollment.CObjectId.1"
    $serverauthoid.InitializeFromValue("1.3.6.1.5.5.7.3.1")
    $ekuoids = new-object -com "X509Enrollment.CObjectIds.1"
    $ekuoids.add($serverauthoid)
    $ekuext = new-object -com "X509Enrollment.CX509ExtensionEnhancedKeyUsage.1"
    $ekuext.InitializeEncode($ekuoids)

    $cert = new-object -com "X509Enrollment.CX509CertificateRequestCertificate.1"
    $cert.InitializeFromPrivateKey(2, $key, "")
    $cert.Subject = $name
    $cert.Issuer = $cert.Subject
    $cert.NotBefore = get-date
    $cert.NotBefore = $cert.NotBefore.AddDays(-1) # Make the certificate valid to the previous day.
    $cert.NotAfter = $cert.NotBefore.AddDays(720)
    $cert.X509Extensions.Add($ekuext)
    $cert.Encode()

    $enrollment = new-object -com "X509Enrollment.CX509Enrollment.1"
    $enrollment.InitializeFromRequest($cert)
    $certdata = $enrollment.CreateRequest(0)
    $enrollment.InstallResponse(2, $certdata, 0, "")

    $newcert = dir Cert:\LocalMachine\my | ?{$_.Subject -eq "CN=$commonname"}
    #$thecert = "Cert:\localmachine\my\$newcert"
    $store = get-item Cert:\LocalMachine\Root
    $store.open("ReadWrite")
    $store.add($newcert)
    $store.close()
}

######################################################################
# Show-MessageBox                                                    #
# Supports alert boxes with various button combinations              #
# $Msg = Message to send                                             #
# $Title = Alert box title                                           #
# Various button option in parameters
######################################################################
function Show-MessageBox(){ 
 
    Param( 
    [Parameter(Mandatory=$True)][Alias('M')][String]$Msg, 
    [Parameter(Mandatory=$False)][Alias('T')][String]$Title = "", 
    [Parameter(Mandatory=$False)][Alias('OC')][Switch]$OkCancel, 
    [Parameter(Mandatory=$False)][Alias('OCI')][Switch]$AbortRetryIgnore, 
    [Parameter(Mandatory=$False)][Alias('YNC')][Switch]$YesNoCancel, 
    [Parameter(Mandatory=$False)][Alias('YN')][Switch]$YesNo, 
    [Parameter(Mandatory=$False)][Alias('RC')][Switch]$RetryCancel, 
    [Parameter(Mandatory=$False)][Alias('C')][Switch]$Critical, 
    [Parameter(Mandatory=$False)][Alias('Q')][Switch]$Question, 
    [Parameter(Mandatory=$False)][Alias('W')][Switch]$Warning, 
    [Parameter(Mandatory=$False)][Alias('I')][Switch]$Informational) 
 
    #Set Message Box Style 
    IF($OkCancel){$Type = 1} 
    Elseif($AbortRetryIgnore){$Type = 2} 
    Elseif($YesNoCancel){$Type = 3} 
    Elseif($YesNo){$Type = 4} 
    Elseif($RetryCancel){$Type = 5} 
    Else{$Type = 0} 
     
    #Set Message box Icon 
    If($Critical){$Icon = 16} 
    ElseIf($Question){$Icon = 32} 
    Elseif($Warning){$Icon = 48} 
    Elseif($Informational){$Icon = 64} 
    Else{$Icon = 0} 
     
    #Loads the WinForm Assembly, Out-Null hides the message while loading. 
    [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") | Out-Null 
 
    #Display the message with input 
    $Answer = [System.Windows.Forms.MessageBox]::Show($MSG , $TITLE, $Type, $Icon) 
     
    #Return Answer 
    Return $Answer 
} 

######################################################################
# Check-FarmAdmin                                                    #
# Checks if current user is a farm admin and returns true or false   #
######################################################################
function Check-FarmAdmin(){
#Must return true else send message to use different user and exit app

#(Get-SPFarm).DefaultServiceAccount.Name 
$FarmAdmin = (Get-Spfarm).CurrentUserIsAdministrator()
Return $FarmAdmin

}

######################################################################
# Get-SPFarmAdministrators                                           #
# Returns the farm adminstrators group membership                    #
######################################################################
function Get-SPfarmAdministrators() {
  $adminwebapp = Get-SPwebapplication -includecentraladministration | where {$_.IsAdministrationWebApplication}
  $adminsite = Get-SPweb($adminwebapp.Url)
  $AdminGroupName = $adminsite.AssociatedOwnerGroup
  $farmAdministratorsGroup = $adminsite.SiteGroups[$AdminGroupName]
  return $farmAdministratorsGroup.users
}
 
######################################################################
# Validate-ServiceApps                                               #
# Checks if the required service applications are deployed           #
# Checks if the required service application instances are enabled   #
# Warning or Error message is delivered to user                      #
######################################################################
 function Validate-ServiceApps(){

#Create Response to Required Service App Check"
$ServiceCheck = ""
$ServiceStatus = 1

#Validate User Profile Service Application Status
$upa=Get-SPServiceApplication | where-object {$_.TypeName -match "User profile"}
if ($upa.status -eq "Online") {
$ServiceCheck = "UPA Service Application Check Successful`r`n"}
else
{$ServiceCheck = "UPA Service Application Check Failed`r`n"
$ServiceStatus = 0}
#$upa | Out-Null

#validate userprofile service instance status
$upaservinst = Get-SPServiceInstance | where-object{$_.typename -match "User Profile Service" -AND $_.status -eq "Online"}
if($upaservinst) {
$ServiceCheck =  $ServiceCheck + "UPA Service Instance Check Successful`r`n"}
else
{$ServiceCheck =  $ServiceCheck + "UPA Service Instance Check Failed`r`n"
$ServiceStatus = 0}

#validate user profile sync service instance status
$upasyncinst = Get-SPServiceInstance | where-object{$_.typename -match "User Profile Synchronization Service" -AND $_.status -eq "Online"}
if($upasyncinst) {
$ServiceCheck =  $ServiceCheck + "UPA Sync Service Instance Check Successful`r`n"}
else
{if($upa.NoILMUsed -eq $true){
$ServiceCheck =  $ServiceCheck + "Sync Service is disabled, ensure you have run AD import synchronization `r`n"}
else{
#We cannot yet determine the difference between internal and external FIM so do not fail just in case - see windows service on sync machine
$ServiceCheck =  $ServiceCheck + "UPA Sync Service Instance Check Failed - Check UPA Import Config `r`n"}
}

#Validate Search Service Application Status
$ssa=Get-SPServiceApplication | where-object {$_.TypeName -match "Search Service"} 
if ($ssa.status -eq "Online") {
$ServiceCheck = $ServiceCheck + "Search Service Application Check Successful`r`n"}
else
{$ServiceCheck = "Search Service Application Check Failed`r`n"
$ServiceStatus = 0}

#Validate Search Admin Service Application  Status
$sas=Get-SPServiceApplication | where-object {$_.TypeName -match "Search Admin"} 
if ($sas.status -eq "Online") {
$ServiceCheck = $ServiceCheck + "Search Admin Service Application Check Successful`r`n"}
else
{$ServiceCheck = "Search Admin Service Application Check Failed`r`n"
$ServiceStatus = 0}

#validate Search host controller service instance status
$shcservinst = Get-SPServiceInstance | where-object{$_.typename -match "Search Host Controller Service" -AND $_.status -eq "Online"}
if($shcservinst) {
$ServiceCheck =  $ServiceCheck + "Search Host Controller Service Instance Check Successful`r`n"}
else
{$ServiceCheck =  $ServiceCheck + "Search Host Controller Service Instance Check Failed`r`n"
$ServiceStatus = 0}

#validate SharePoint Server Search service instance status
$seaservinst = Get-SPServiceInstance | where-object{$_.typename -match "SharePoint Server Search" -AND $_.status -eq "Online"}
if($seaservinst) {
$ServiceCheck =  $ServiceCheck + "SharePoint Server Search Service Instance Check Successful`r`n"}
else
{$ServiceCheck =  $ServiceCheck + "SharePoint Server Search Service Instance Check Failed`r`n"
$ServiceStatus = 0}

#validate Search Query and Site Settings Service instance status
$sqsservinst = Get-SPServiceInstance | where-object{$_.typename -match "Search Query and Site Settings Service" -AND $_.status -eq "Online"}
if($sqsservinst) {
$ServiceCheck =  $ServiceCheck + "Search Query and Site Settings Service Instance Check Successful`r`n"}
else
{$ServiceCheck =  $ServiceCheck + "Search Query and Site Settings Service Instance Check Failed`r`n"
$ServiceStatus = 0}


#Validate Subscription Settings Application Status
$sss=Get-SPServiceApplication | where-object {$_.TypeName -match "Subscription"} 
if ($sss.status -eq "Online") {
$ServiceCheck = $ServiceCheck + "Subscription Settings Service Application Check Successful`r`n"}
else
{$ServiceCheck = "Subscription Settings Service Application Check Failed`r`n"
$ServiceStatus = 0}

#Validate Microsoft SharePoint Foundation Subscription Settings Service Instance Status
$sfsservinst = Get-SPServiceInstance | where-object{$_.typename -match "Microsoft SharePoint Foundation Subscription Settings Service" -AND $_.status -eq "Online"}
if($sfsservinst) {
$ServiceCheck =  $ServiceCheck + "SharePoint Subscription Settings Service Instance Check Successful`r`n"}
else
{$ServiceCheck =  $ServiceCheck + "SharePoint Subscription Settings Service Instance Check Failed`r`n"
$ServiceStatus = 0}

#Validate App Management Application Status
$app=Get-SPServiceApplication | where-object {$_.TypeName -match "App Management"} 
if ($app.status -eq "Online") {
$ServiceCheck = $ServiceCheck + "App Management Service Application Check Successful`r`n"}
else
{$ServiceCheck = "App Management Service Application Check Failed`r`n"
$ServiceStatus = 0}

#Validate App Management Service Instance Status
$samservinst = Get-SPServiceInstance | where-object{$_.typename -match "App Management Service" -AND $_.status -eq "Online"}
if($samservinst) {
$ServiceCheck =  $ServiceCheck + "App Management Service Instance Check Successful`r`n"}
else
{$ServiceCheck =  $ServiceCheck + "App Management Service Instance Check Failed`r`n"
$ServiceStatus = 0}

#Validate Security Token Service Application Status
$sts=Get-SPServiceApplication | where-object {$_.TypeName -match "Security Token"} 
if ($sts.status -eq "Online") {
$ServiceCheck = $ServiceCheck + "Security Token Service Check Successful`r`n"}
else
{$ServiceCheck = "Security Token Service Check Failed`r`n"
$ServiceStatus = 0}

if ($ServiceStatus -eq 1)
{Show-MessageBox "All Required Services are deployed and Online `r`n$ServiceCheck" | out-null
}
else
{Show-MessageBox "Services not correctly setup `r `n$ServiceCheck" -Critical | out-null

Show-MessageBox "Wizard will now exit!" | Out-Null


}
return $ServiceStatus, $ServiceCheck
}

<#
######################################################################
#               DEPRECATED FUNCTION                                  #
# Prepares the server by deploying the MSOL/ADCRL pieces             #
# Recycles services after deployment                                 #
######################################################################
function Deprecated-Prepare-Environment(){

## Future - look for new version of MSOL/IDCRL online and download/update if required/prompted ##

$scriptFolder = "C:\scripts\Resources"

     $MSOIdCRLRegKey = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\MSOIdentityCRL" -ErrorAction SilentlyContinue
     if($MSOIdCRLRegKey -eq $null)
     {
      Update-WizardProgress "Installing Office Single Sign On Assistant"
      #Install-MSI (Get-AbsolutePath "\msoidcli_64.msi")
      Start-Process "$scriptFolder\msoidcli_64.msi" -ArgumentList " /q /norestart" -Wait
      Update-WizardProgress "Successfully installed!"
     }
    else
    {
      Update-WizardProgress "Office Single Sign On Assistant already Installed"
      }
     $MSOLPSRegKey = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\MSOnlinePowershell" -ErrorAction SilentlyContinue
     if($MSOLPSRegKey -eq $null)
     {
       Update-WizardProgress "Installing Active Directory PowerShell"
       Start-Process "$scriptFolder\AdministrationConfig-EN.msi" -ArgumentList " /q /norestart" -wait
       Update-WizardProgress "Creating Taskbar Shortcuts"
       #New-Shortcut -ShortcutName "AAD PowerShell" -IconPath ($scriptFolder + "\AzurePS.ico") -ApplicationPath "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe  -NoExit -Command ""Import-Module MSOnline"""
       Update-WizardProgress "Successfully installed!"
    }
    else
    {
        Update-WizardProgress "Azure Active Directory Powershell already Installed" 
    }

Update-WizardProgress "Restarting MSO IDCRL Service"
Stop-Service -Name msoidsvc -Force -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
$svc = Get-Service msoidsvc
$svc.WaitForStatus("Stopped")
Start-Service -Name msoidsvc
$svc = Get-Service msoidsvc
$svc.WaitForStatus("Running")
Update-WizardProgress "MSO IDCRL Service Restarted!"

}
#>

######################################################################
# Checks the Server for the AAD PowerShell and Sign In Assistant     #
# Recycles services if they are deployed                             #
# Raises error and exits if they are not installed                   #
######################################################################
function Prepare-Environment(){
    $MSOIdCRLRegKey = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\MSOIdentityCRL" -ErrorAction SilentlyContinue
    if ($MSOIdCRLRegKey -eq $null) {
        Write-Host "Office Single Sign On Assistant required, see http://www.microsoft.com/en-us/download/details.aspx?id=39267." -Foreground Red
        Update-WizardProgress "Office Single Sign on Assistant not found"
    } else {
        Write-Host "Found Office Single Sign On Assistant!" -Foreground Green
        Update-WizardProgress "Office Single Sign on Assistant found"
    }

    $MSOLPSRegKey = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\MSOnlinePowershell" -ErrorAction SilentlyContinue
    if ($MSOLPSRegKey -eq $null) {
        Write-Host "AAD PowerShell required, see http://go.microsoft.com/fwlink/p/?linkid=236297." -Foreground Red
        Update-WizardProgress "AAD PowerShell not found"

    } else {
        Write-Host "Found AAD PowerShell!" -Foreground Green
        Update-WizardProgress "AAD PowerShell found"
    }

    if ($MSOIdCRLRegKey -eq $null -or $MSOLPSRegKey -eq $null) {
        Update-WizardProgress "Please manually install the prerequisites."
        throw "Manual installation of prerequisites required."
    }

    Write-Host "Configuring Azure AD settings..." -Foreground Yellow

    $regkey = "HKLM:\SOFTWARE\Microsoft\MSOnlinePowerShell\Path"
    Set-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\MSOIdentityCRL" -Name "ServiceEnvironment" -Value "Production"
    Set-ItemProperty -Path $regkey -Name "WebServiceUrl" -Value "https://provisioningapi.microsoftonline.com/provisioningwebservice.svc" #$PROVISIONINGAPI_WEBSERVICEURL
    Set-ItemProperty -Path $regkey -Name "FederationProviderIdentifier" -Value "microsoftonline.com"

    Write-Host "Restarting MSO IDCRL Service..." -Foreground Yellow
    Update-WizardProgress "Restarting MSO IDCRL Service"

    # Service takes time to get provisioned, retry restart.
    for ($i = 1; $i -le 10; $i++) {
        try {
            Stop-Service -Name msoidsvc -Force -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
            $svc = Get-Service msoidsvc
            $svc.WaitForStatus("Stopped")
            Start-Service -Name msoidsvc
        } catch {
            Write-Host "Failed to start msoidsvc service, retrying..."
            Update-WizardProgress "Failed to start msoidsvc service, retrying..."
            Start-Sleep -seconds 2
            continue
        }
        Write-Host "Service Restarted!" -Foreground Green
        Update-WizardProgress "Service Restrated"
        break
    }
}

######################################################################
# Validate-MSOLDomain                                                #
# Checks for domains added to the tenant and prompts to use dirsync  #
######################################################################
function Validate-MSOLDomain(){
$domainlist = ""
$domains = get-msoldomain | ?{$_.name -notmatch "onmicrosoft.com"}

 foreach ($domain in $domains)
  {
      $domainlist = $domainlist + $domain.name + "`n"

  }
$updatetxt = "The following domain(s) are associated with your tenant. Please ensure you have completed the Azure AD Sync process for these domains. `n `n$domainlist`n"
Update-WizardProgress $updatetxt

$spodomains = get-msoldomain | ?{$_.name -match "onmicrosoft.com"}
$firstspodomainname=$spodomains[0].name
$thespotenantdomain=$firstspodomainname.split(".")
$SPORootSite="https://" + $thespotenantdomain[0] +".sharepoint.com"
$labelSPOTenantURL.text = $SPORootSite

#Need to isolate a domain for later when adding an SPN - Grab the first returned SPWeb and use that
#Might need to consider adding a selection for the web app into here

 $SharePointWeb = Get-SPSite | Select-Object -First 1 | Get-SPWeb | Select-Object -First 1 | % Url
 $labelOnPremisesSharePoin.Text = $SharePointWeb
 return $SharePointWeb
 }
 
######################################################################
# Creates new query rule and search result source                    #
#                                                                    #
######################################################################
function New-SCSearchResultSourceAndQueryRule($siteUrl, $remoteUrl, $resultSourceName, $queryRuleName){

#$sspApp = Get-SPEnterpriseSearchServiceApplication | select -first 1
#$SearchServiceApplicationName = $sspApp.Name
$RootSiteCollection = Get-SPSite $siteUrl -ErrorAction SilentlyContinue

#-----------------------------------------------------
# Get the Search Service application
#-----------------------------------------------------
# Select the first search service application in case there are multiple - can add choices later
$SearchServiceApplication = Get-SPEnterpriseSearchServiceApplication | select -first 1 # -Identity $SearchServiceApplicationName -ErrorAction SilentlyContinue
$FederationManager = New-Object Microsoft.Office.Server.Search.Administration.Query.FederationManager($SearchServiceApplication)

#--------------------------------------------------------------------------
# The below line creates a Search Object owner at the site collection level
# and this can be changed to Search Application or Site level by passing 
# different SearchObjectLevel argument.
#--------------------------------------------------------------------------
$SearchOwner = New-Object Microsoft.Office.Server.Search.Administration.SearchObjectOwner –ArgumentList @([Microsoft.Office.Server.Search.Administration.SearchObjectLevel]::SPSite,$RootSiteCollection.RootWeb)

$Query = "{searchTerms}"

$ResultSource = $FederationManager.GetSourceByName($resultSourceName,$SearchOwner)

if($ResultSource)
{
    Update-WizardProgress "Result Source : $ResultSourceName exist - appending remote url to name"
    $resultsourcename = $resultsourcename + "-" + $remoteurl  
}

    Update-WizardProgress "Creating Result Source : $ResultSourceName"

    
    $resultSource = $FederationManager.CreateSource($SearchOwner)
    $resultSource.Name = $resultSourceName
    $resultSource.CreateQueryTransform($queryProperties, $query)
    $resultSource.ConnectionUrlTemplate = $remoteUrl
    $resultSource.ProviderId = $FederationManager.ListProviders()["Remote SharePoint Provider"].Id
    $ResultSource.Activate()   
    $resultSource.Commit()


#-------------------------------------------------------------------
# Configure a Query Rule
#-------------------------------------------------------------------

#$QueryRuleConditionTerm = "test"
$QueryRuleManager = New-Object Microsoft.Office.Server.Search.Query.Rules.QueryRuleManager($SearchServiceApplication) 

# Create a search object filter using a $SearchOwner object  (Site collection level - in this case)
$SearchObjectFilter =  New-Object Microsoft.Office.Server.Search.Administration.SearchObjectFilter($SearchOwner) 

$QueryRules = $QueryRuleManager.GetQueryRules($SearchObjectFilter)
ForEach($Rule in $QueryRules)
{
    if($Rule.DisplayName -eq $queryRuleName)
    {
        Update-WizardProgress "Query Rule : $QueryRuleName already exist. Appending remoteurl to name."
        $queryrulename = $queryrulename + "-" + $remoteurl
    }
}

Update-WizardProgress "Creating Query Rule : $QueryRuleName"

$QueryRules = $QueryRuleManager.GetQueryRules($SearchObjectFilter)

# Create a new rule as a active one.
$QueryRule = $QueryRules.CreateQueryRule($QueryRuleName,$null,$null,$true)

# Set the Query Rule condition...
#[string[]] $QueryRuleTerms = @($QueryRuleConditionTerm)
#$QueryRuleConditions = $QueryRule.QueryConditions
#$QueryRuleCondition = $QueryRuleConditions.CreateKeywordCondition($QueryRuleTerms,$true)

#Bind it to the Result Source...
#$QuerySourceContextCondition = $QueryRule.CreateSourceContextCondition($ResultSource)

# Set the Query Condition action to change ranked results...
#$QueryRuleAction = $QueryRule.CreateQueryAction([Microsoft.Office.Server.Search.Query.Rules.QueryActionType]::ChangeQuery)   
#$QueryRuleAction.QueryTransform.OverrideProperties = new-object Microsoft.Office.Server.Search.Query.Rules.QueryTransformProperties
#$QueryRuleAction.QueryTransform.SourceId = $ResultSource.Id

# define a custom sorting - Order by FileName
#$QueryRuleSortCollection = New-Object Microsoft.Office.Server.Search.Query.SortCollection
#$QueryRuleSortCollection.Add("FileName", [Microsoft.Office.Server.Search.Query.SortDirection]::Descending)

#$QueryRule.ChangeQueryAction.QueryTransform.OverrideProperties["SortList"] = [Microsoft.Office.Server.Search.Query.SortCollection]$QueryRuleSortCollection 
#$QueryRule.ChangeQueryAction.QueryTransform.QueryTemplate = "{searchTerms}" 

$difqueryblock = $QueryRule.CreateQueryAction("CreateResultBlock")
$difqueryblock.ResultTitle.DefaultLanguageString = "Results from {searchTerms}"

#look into changing the number of returned items within the result block
$difqueryblock.QueryTransform.SourceId = $resultSource.Id
$difqueryblock.QueryTransform.QueryTemplate = "{searchTerms}"
$difqueryblock.AlwaysShow = $true

$QueryRule.Update()



}

##################################################
#Update-WizardProgress                           #
#Adds status messages to the progress check box  #
##################################################
function Update-WizardProgress($update){

$progress = $textboxWizardProgress.text + "`r`n"
$progress = $progress + $update
$textboxWizardProgress.Text = $progress

$textboxWizardProgress.SelectionStart= $textboxWizardProgress.TextLength
$textboxWizardProgress.ScrollToCaret()
}

##########################################
# CHECK IF CURRENT USER IS A LOCAL ADMIN #
# Todo: check all farm servers           #
##########################################
function Check-IsLocalAdmin {  
    $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()  
      $principal = new-object System.Security.Principal.WindowsPrincipal($identity)  
      $admin = [System.Security.Principal.WindowsBuiltInRole]::Administrator  
      $principal.IsInRole($admin)  
} 

##########################################
# CLEAN UP HYBRID CONFIGURATION          #
# 1. REMOVE SPN                          #
# 2. REMOVE ACS PROXY                    #
# 3. LOOK FOR AND REMOVE DEFAULT RESULT  #
#    SOURCE AND QUERY RULE               #
##########################################

function Remove-HybridConfiguration($SPSite)
{
$app = Get-MsolServicePrincipal -AppPrincipalId "00000003-0000-0ff1-ce00-000000000000"
$app.ServicePrincipalNames

$indexHostName = $SPSite.IndexOf('://') + 3
        $HostName = $SPSite.Substring($indexHostName)
		$indexHostName = $HostName.IndexOf('/')
		if ($indexhostName -ge 0) {
			$HostName = $HostName.Substring(0,$indexHostName)
		}

$partfqdn = $Hostname.Indexof('.')

# Check for single part domain name, ie http://spweb as this will result in $partfqdn value of -1
if($partfqdn -lt 0)
{
    $Hostname = $Hostname #No change since single name website
}
else
{
    $Hostname = "*" + $Hostname.Substring($partfqdn)   
}

# Loop through the service principal names and clean up the one matching the local SharePoint url

for ($i = 0; $i -lt $app.ServicePrincipalNames.count; $i++)
{ 
    if($app[$i] -match $Hostname)
    { 
    $app.ServicePrincipalNames.RemoveAt($i)
    }
}

Set-MsolServicePrincipal -AppPrincipalId $app.AppPrincipalId -ServicePrincipalNames $app.ServicePrincipalNames

# Remove the ACS Proxy - will remove them all so be careful

Get-SPServiceApplicationProxy | ? {$_.TypeName -eq "Azure Access Control Service Application Proxy"} | Remove-SPServiceApplicationProxy
Update-WizardProgress "Azure ACS Proxy has been removed"

#Remove the TrustedSecurityTokenIssuer based on metadata endpoint
Get-SPTrustedSecurityTokenIssuer |?{$_.MetadataEndPoint -eq "https://accounts.accesscontrol.windows.net/metadata/json/1/"} | Remove-SPTrustedSecurityTokenIssuer 


Update-WizardProgress "Trusted Security Token Issuer has been removed"
}

##########################################
# WARMS UP THE ROOT SITE                 #
# After IISRESET                         #
##########################################
function Warmup-SPSite()
{
$Warmup = get-spsite -Limit 1
$warmupurl = $warmup.Url

Update-WizardProgress "Warming up site $warmupurl after IISRESET"
Write-Host "Warming up site $warmupurl after IISRESET"

$WarmItUp = Invoke-WebRequest -Uri $warmupurl -UseDefaultCredentials 


}

#endregion

######################################################################
# mainflow                                                           #
# Main program execution flow                                        #
######################################################################
function mainflow(){

$spsite= ""
$spoappid="00000003-0000-0ff1-ce00-000000000000"
$Update = ""
$wizstatus=$true
$wizstatusmsg = ""
#############################################################
# CHECK IF CURRENT USER IS A LOCAL ADMIN ON ALL FARM SERVERS#
#          REQUIRED                                         #
#############################################################

$Isboxadmin = Check-IsLocalAdmin

If($Isboxadmin -eq $false)
{
Update-WizardProgress "Is User Local Admin returns FALSE"
Show-MessageBox "You are not logged in as a Local Machine administrator account.`nPlease ensure you are logged in with an account that has local machine admin rights and is a SharePoint Farm Administrator then re-start this wizard`n`nThe Hybrid Wizard will now Exit!`n" -Critical | out-null
#Show-MessageBox "Wizard will now exit!" | out-null
$wizstatus=$false
$wizstatusmsg = "`nLocal Admin Check Failed"
}
Else
{
Show-MessageBox "Current User is Local Machine Admin`nPlease ensure the user is a local admin on all other farm servers before proceeding" | out-null
Update-WizardProgress "Is User Local Admin returns TRUE"
}

################################################
#CHECK CURRENT USER IS A MEMBER OF FARM ADMINS.# 
#IF NOT THE DISPLAY LIST OF ADMINS AND EXIT    #
#         REQUIRED                             #
################################################

$IsFarmAdmin = Check-FarmAdmin
if($IsFarmAdmin){}
else{
$farmadmins = Get-SPfarmAdministrators | ? {$_.Name -notmatch "BUILTIN"}

$listoffarmadmins = ""
foreach ($farmadmin in $farmadmins)
{
    $listoffarmadmins = $listoffarmadmins + $farmadmin.Name + "`n"
}
Update-WizardProgress "Is User Farm Admin returns FALSE"
Show-MessageBox "You are not logged in as a farm administrator account. Please login as one of the following accounts and rerun this wizard `n`n$listoffarmadmins" -Critical | out-null

$wizstatus=$false
$wizstatusmsg = $wizstatusmsg + "`nFarm Admin Check Failed"
#Show-MessageBox "Wizard will now exit!" | out-null
#break
}
Update-WizardProgress "Is User Farm Admin returns TRUE"

#######################################################
#VALIDATE ALL REQUIRED SERVICE APPLICATIONS ARE ONLINE#
#IF NOT THE DISPLAY LIST OF APP STATUSES AND QUIT     #
#                 REQUIRED                            #
#######################################################

$validateserviceapps = Validate-ServiceApps

   Update-WizardProgress $validateserviceapps[1]
if($validateserviceapps[0] -eq 0)
    {try
    {
    #$MainForm.Close()
        Update-WizardProgress "Service Application minimum configuration is not setup correctly"
    $wizstatus=$false
    $wizstatusmsg = $wizstatusmsg + "`nService Application Check Failed "
    }
    catch [Exception]
		{ }
    
}
Update-WizardProgress "All Service Application minimum requirements are met"


#######################################################
#CHECK FOR MSOL AND IDCRL INSTALLED                   #
#REQUIREMENTs .NET 3.5SP1 .NET 4.5                    #
#             REQUIRED                                #
#######################################################
try{
$prepareenv = prepare-environment
}
catch
{
    Update-WizardProgress "Prerequisite Software not met. Exiting Wizard. Please refer to the pre requisites section of the documentation"
    Write-Host "Prerequisite Software not met. Exiting Wizard. Please refer to the pre requisites section of the documentation"
    $wizstatus=$false
    $wizstatusmsg = $wizstatusmsg + "`nPrerequisite Software Check Failed "
    
    #$MainForm.close()
    #[environment]::exit(0) 
    #break
}


#######################################################
# Check if all Required Checks Passed or Failed       #
#                                                     #
#######################################################

    if($wizstatus -eq $false)
    {
    Update-WizardProgress $wizstatusmsg
    Write-Host $wizstatusmsg
    Show-MessageBox "Required Checks Failed`n$wizstatusmsg `nThe wizard cannot continue until these errors are remediated" -Critical
    
    $mainform.close()
    
    #[environment]::exit(0)
    pause
    }


#######################################################
#IMPORT MSOL AND IDCRL MODULES                        #
#CONNECT TO O365 TENANT                               #
#######################################################

$connect = Import-ModulesAndConnect

if ($connect[0] -eq $true)
{Update-WizardProgress "continue login success"
}
else {Update-WizardProgress "quit failed login"
}



#######################################################
#VALIDATE MSOLDOMAIN                                  #
#The $SharePointWeb will match the tenant root site   #
#######################################################

$SharePointWeb = Validate-MSOLDomain

if($SharePointWeb -eq ""){break} #Started seeing some odd WCF errors causing blank Urls and we must stop if this happens 

# We can test for clean up here since $SharePointWeb will match the SPN we want to remove (hopefully)

#######################################################
#REMOVE HYBRID CONFIG if Selected                     #
#######################################################

if($checkboxcleanuphybridstatus.checked)
{
Remove-HybridConfiguration $SharePointWeb
Update-WizardProgress "Hybrid Configuration Removed - Exiting Wizard"

Break
}

#######################################################
#MANAGE STS CERTIFICATE                               #
#Lot goes on here to set cert for ACS trust and SPN   #
#######################################################

$spoappid="00000003-0000-0ff1-ce00-000000000000"
$CommonName = $textboxSelfSignedCertCN.Text
$stscertificate = Manage-STSCertificate $spoappid $SharePointWeb $CommonName

#######################################################
#WARM UP WEB APP                                      #
#Hits the first site collection from get-spsite       #
#######################################################

Warmup-spsite

#######################################################
#CREATE RESULT SOURCE                                 #
#Uses Certificate Common Name as ResultSource Name    #
#Uses Certificate Common Name as the Query Rule Name  #
#######################################################

New-SCSearchResultSourceAndQueryRule $labelOnPremisesSharePoin.text $labelSPOTenantURL.Text $textboxSelfSignedCertCN.Text $textboxSelfSignedCertCN.Text 

Update-WizardProgress "Hybrid wizard setup is completed - Please test to validate success"


$buttonStart.Enabled=$true
$buttonStart.Text = "Exit Wizard"



#ALL DONE


#GUI
#Things to Capture
#LocalCN for SelfSignedCert
#Use New SelfSigned STS Cert or Use Existing STS Cert or public
#On Premises Site Url
#SPO Url of Tenant
#Check for Farm Account and Local Admin or prompt for admin creds
}

#region Generate Wizard Form
	
Add-Type -AssemblyName System.Windows.Forms

#region Generated Form Objects
	#----------------------------------------------
	[System.Windows.Forms.Application]::EnableVisualStyles()
	$MainForm = New-Object 'System.Windows.Forms.Form'
	$groupbox1 = New-Object 'System.Windows.Forms.GroupBox'
	$radiobuttonUsePublicAuthoritySi = New-Object 'System.Windows.Forms.RadioButton'
	$radiobuttonUseNewSelfSignedSTSC = New-Object 'System.Windows.Forms.RadioButton'
	$radiobuttonUseBuiltInSharePoint = New-Object 'System.Windows.Forms.RadioButton'
	$labelOnPremisesSharePoint = New-Object 'System.Windows.Forms.Label'
	$labelSPOTenantURLegHttpsw = New-Object 'System.Windows.Forms.Label'
	$labelSPOTenantURL = New-Object 'System.Windows.Forms.Label'
    $labelOnPremisesSharePoin = New-Object 'System.Windows.Forms.Label'
    $textboxSelfSignedCertCN = New-Object 'System.Windows.Forms.TextBox'
    $textboxPubAuthSignedCertCN = New-Object 'System.Windows.Forms.TextBox'
	$labelHybridConfigurationW = New-Object 'System.Windows.Forms.Label'
	$buttonStart = New-Object 'System.Windows.Forms.Button'
	$InitialFormWindowState = New-Object 'System.Windows.Forms.FormWindowState'
    $textboxWizardProgress = New-Object 'System.Windows.Forms.TextBox'
    $checkboxcleanuphybridstatus = new-object 'System.Windows.Forms.CheckBox'
    $labelcleanuphybridstatus = new-object 'System.Windows.Forms.Label'
    $labelcontactinfo = new-object 'System.Windows.Forms.Label'

	#endregion Generated Form Objects

	#----------------------------------------------
	# User Generated Script
	#----------------------------------------------
	
	$OnLoadFormEvent={
	#TODO: Initialize Form Controls here
	$MainForm.Focused
	}

    $Form_FormClosing={
    # Capture form closing event ie user clicked red X. Prompt for validation and cancel event if user has exietd by mistake
    

    If ($wizstatus -eq $false)
        {#Pre requisites failed so kill form
        [environment]::exit(0)
        }


    $closeme = Show-MessageBox "Do you want to close down the Wizard?" -YesNo
    
	If ($closeme -eq "Yes") {}
    Else {$_.Cancel = $true # $_.Cancel actually cancels the FormClosing Event and so FormClosed never fires
    
    Show-MessageBox "Carry on regardless"}
	}

	$buttonStart_Click={

        #$buttonStart.Text = "Exit Wizard" #debug

        if($buttonStart.Text -eq "Exit Wizard")
        {
            $MainForm.close()
            [environment]::exit(0)
            break
        }

        $buttonStart.Text = "Running"
        $buttonStart.Enabled=$false
		mainflow
    
		
	}
    $buttonStart_MouseHover={
        $buttonStart.BackColor = 'Blue'
    }

	
	$Form_StateCorrection_Load=
	{
		#Correct the initial state of the form to prevent the .Net maximized form issue
		$MainForm.WindowState = $InitialFormWindowState
	}
	
	$Form_StoreValues_Closing=
	{
		#Store the control values
		$script:MainForm_radiobuttonUsePublicAuthoritySi = $radiobuttonUsePublicAuthoritySi.Checked
		$script:MainForm_radiobuttonUseNewSelfSignedSTSC = $radiobuttonUseNewSelfSignedSTSC.Checked
		$script:MainForm_radiobuttonUseBuiltInSharePoint = $radiobuttonUseBuiltInSharePoint.Checked
		$script:MainForm_textbox1 = $textbox1.Text
	}
	
	$Form_Cleanup_FormClosed=
	{
		#Remove all event handlers from the controls
		try
		{
			$radiobuttonUseBuiltInSharePoint.remove_CheckedChanged($radiobuttonUseBuiltInSharePoint_CheckedChanged)
			$labelSPOTenantURLegHttpsw.remove_Click($labelSPOTenantURLegHttpsw_Click)
			$buttonStart.remove_Click($buttonStart_Click)
			$MainForm.remove_Load($OnLoadFormEvent)
			$MainForm.remove_Load($Form_StateCorrection_Load)
			$MainForm.remove_Closing($Form_StoreValues_Closing)
			$MainForm.remove_FormClosed($Form_Cleanup_FormClosed)
		}
		catch [Exception]
		{ }
	}
	
   

	
	#region Generated Form Code
	#----------------------------------------------
	$MainForm.SuspendLayout()
	$groupbox1.SuspendLayout()
	#
	#region MainForm
	#
	$MainForm.Controls.Add($groupbox1)
	$MainForm.Controls.Add($labelOnPremisesSharePoint)
	$MainForm.Controls.Add($labelSPOTenantURLegHttpsw)
	$MainForm.Controls.Add($labelSPOTenantURL)
    $MainForm.Controls.Add($labelOnPremisesSharePoin)
	$MainForm.Controls.Add($labelHybridConfigurationW)
	$MainForm.Controls.Add($buttonStart)
    $MainForm.Controls.Add($textboxWizardProgress)
    $MainForm.Controls.Add($checkboxcleanuphybridstatus)
    $MainForm.Controls.Add($labelcleanuphybridstatus)
    $Mainform.Controls.Add($labelcontactinfo)
    $MainForm.Name = "MainForm"
	$MainForm.StartPosition = 'CenterScreen'
	$MainForm.Text = "Hybrid Configuration Wizard" + $HWVersion
	$MainForm.add_Load($OnLoadFormEvent)
    $MainForm.Size = "650,650"
	#
    #endregion Mainform

	#region groupbox1
	#
	$groupbox1.Controls.Add($radiobuttonUsePublicAuthoritySi)
	$groupbox1.Controls.Add($radiobuttonUseNewSelfSignedSTSC)
	$groupbox1.Controls.Add($radiobuttonUseBuiltInSharePoint)
    $groupbox1.Controls.Add($textboxSelfSignedCertCN)
    $groupbox1.Controls.Add($textboxPubAuthSignedCertCN)
	$groupbox1.Location = '13, 193'
	$groupbox1.Name = "groupbox1"
	$groupbox1.Size = '538, 138'
	$groupbox1.TabIndex = 6
	$groupbox1.TabStop = $False
	$groupbox1.Text = "STS Certificate Replacement"
	#
	# radiobuttonUsePublicAuthoritySi
	#
    $radiobuttonUsePublicAuthoritySi.Font = "Microsoft Sans Serif, 8pt"
	$radiobuttonUsePublicAuthoritySi.Location = '15, 87'
	$radiobuttonUsePublicAuthoritySi.Name = "radiobuttonUsePublicAuthoritySi"
	$radiobuttonUsePublicAuthoritySi.Size = '270, 24'
	$radiobuttonUsePublicAuthoritySi.TabIndex = 5
	$radiobuttonUsePublicAuthoritySi.TabStop = $True
	$radiobuttonUsePublicAuthoritySi.Text = "Use Public Authority Signed STS Cert"
	$radiobuttonUsePublicAuthoritySi.UseVisualStyleBackColor = $True
    $radiobuttonUsePublicAuthoritySi.Enabled = $False
	#
	# radiobuttonUseNewSelfSignedSTSC
	#
    $radiobuttonUseNewSelfSignedSTSC.Font = "Microsoft Sans Serif, 8pt"
	$radiobuttonUseNewSelfSignedSTSC.Location = '15, 56'
	$radiobuttonUseNewSelfSignedSTSC.Name = "radiobuttonUseNewSelfSignedSTSC"
	$radiobuttonUseNewSelfSignedSTSC.Size = '270, 24'
	$radiobuttonUseNewSelfSignedSTSC.TabIndex = 4
	$radiobuttonUseNewSelfSignedSTSC.TabStop = $True
	$radiobuttonUseNewSelfSignedSTSC.Text = "Use New Self Signed STS Cert"
	$radiobuttonUseNewSelfSignedSTSC.UseVisualStyleBackColor = $True
    $radiobuttonUseNewSelfSignedSTSC.Checked = $True
	#
	# radiobuttonUseBuiltInSharePoint
	#
    $radiobuttonUseBuiltInSharePoint.Font = "Microsoft Sans Serif, 8pt"
	$radiobuttonUseBuiltInSharePoint.Location = '15, 26'
	$radiobuttonUseBuiltInSharePoint.Name = "radiobuttonUseBuiltInSharePoint"
	$radiobuttonUseBuiltInSharePoint.Size = '270, 24'
	$radiobuttonUseBuiltInSharePoint.TabIndex = 3
	$radiobuttonUseBuiltInSharePoint.TabStop = $True
	$radiobuttonUseBuiltInSharePoint.Text = "Use Built In SharePoint STS Cert"
	$radiobuttonUseBuiltInSharePoint.UseVisualStyleBackColor = $True
	$radiobuttonUseBuiltInSharePoint.add_CheckedChanged($radiobuttonUseBuiltInSharePoint_CheckedChanged)
    #
    # textboxNewSelfSignedCertCN
	#
    $textboxSelfSignedCertCN.Font = "Microsoft Sans Serif, 8pt"
	$textboxSelfSignedCertCN.Location = '300, 56'
	$textboxSelfSignedCertCN.Name = "textboxSelfSignedCertCN"
	$textboxSelfSignedCertCN.Size = '200, 20'
	$textboxSelfSignedCertCN.TabIndex = 8
    $textboxSelfSignedCertCN.Text = "HybridWizard"
    #
    # textboxPubAuthSignedCertCN
	#
	$textboxPubAuthSignedCertCN.Font = "Microsoft Sans Serif, 8pt"
    $textboxPubAuthSignedCertCN.Location = '300, 87'
	$textboxPubAuthSignedCertCN.Name = "textboxPubAuthSignedCertCN"
	$textboxPubAuthSignedCertCN.Size = '200, 20'
	$textboxPubAuthSignedCertCN.TabIndex = 7
    $textboxPubAuthSignedCertCN.Text = "Coming Soon"
    $textboxPubAuthSignedCertCN.Enabled = $False
    #
	# labelOnPremisesSharePoint
	#
    $labelOnPremisesSharePoint.Font = "Microsoft Sans Serif, 8pt"
    $labelOnPremisesSharePoint.Location = '12, 136'
	$labelOnPremisesSharePoint.Name = "labelOnPremisesSharePoint"
	$labelOnPremisesSharePoint.Size = '228, 38'
	$labelOnPremisesSharePoint.TabIndex = 5
	$labelOnPremisesSharePoint.Text = "On Premises SharePoint Url"
	#
	# labelSPOTenantURLegHttpsw
	#
    $labelSPOTenantURLegHttpsw.Font = "Microsoft Sans Serif, 8pt"
	$labelSPOTenantURLegHttpsw.Location = '12, 95'
	$labelSPOTenantURLegHttpsw.Name = "labelSPOTenantURLegHttpsw"
	$labelSPOTenantURLegHttpsw.Size = '228, 38'
	$labelSPOTenantURLegHttpsw.TabIndex = 4
	$labelSPOTenantURLegHttpsw.Text = "SPO Tenant URL"
	#$labelSPOTenantURLegHttpsw.add_Click($labelSPOTenantURLegHttpsw_Click)
	#
	# labelSPOTenantURL
	#
    $labelSPOTenantURL.Font = "Microsoft Sans Serif, 8pt"
	$labelSPOTenantURL.Location = '246, 95'
	$labelSPOTenantURL.Name = "labelSPOTenantURL"
	$labelSPOTenantURL.Size = '260, 20'
	$labelSPOTenantURL.TabIndex = 3
    $labelSPOTenantURL.Text = "Not Connected Yet"
    #
    # labelOnPremisesSharePoin
	#
    $labelOnPremisesSharePoin.Font = "Microsoft Sans Serif, 8pt"
    $labelOnPremisesSharePoin.Location = '246, 130'
	$labelOnPremisesSharePoin.Name = "labelOnPremisesSharePoin"
	$labelOnPremisesSharePoin.Size = '260, 20'
	$labelOnPremisesSharePoin.TabIndex = 6
    $labelOnPremisesSharePoin.Text = "Not Connected Yet"
    #
	# labelHybridConfigurationW
	#
	$labelHybridConfigurationW.Font = "Microsoft Sans Serif, 15.75pt"
	$labelHybridConfigurationW.Location = '12, 9'
	$labelHybridConfigurationW.Name = "labelHybridConfigurationW"
	$labelHybridConfigurationW.Size = '543, 58'
	$labelHybridConfigurationW.TabIndex = 2
	$labelHybridConfigurationW.Text = "Hybrid Configuration Wizard"
	$labelHybridConfigurationW.TextAlign = 'MiddleCenter'
	#
    # labelcleanuphybridstatus
    #
    $labelcleanuphybridstatus.Font = "Microsoft Sans Serif, 8pt"
    $labelcleanuphybridstatus.ForeColor = "Red"
    $labelcleanuphybridstatus.Location = '60,350'
    $labelcleanuphybridstatus.Size = '500,58'
    $labelcleanuphybridstatus.TabIndex = 10
    $labelcleanuphybridstatus.Text = "Select this box to Cleanup Existing Hybrid Config (Coming Soon)"
    #$labelcleanuphybridstatus.TextAlign = 'Left'
   # $labelcleanuphybridstatus.Enabled = $false
    #
    # checkboxcleanuphybridstatus
    $checkboxcleanuphybridstatus.Font = "Microsoft Sans Serif, 8pt"
    $checkboxcleanuphybridstatus.Location = '30,350'
    #
    $checkboxcleanuphybridstatus.Size = '20,20'
    $checkboxcleanuphybridstatus.TabIndex = 10
    $checkboxcleanuphybridstatus.Enabled = $false
    #
    # textboxWizardProgress
    #
    $textboxWizardProgress.Font = "Microsoft Sans Serif, 8pt"
	$textboxWizardProgress.Location = '13, 390'
	$textboxWizardProgress.Name = "textboxWizardProgress"
	$textboxWizardProgress.Size = '543, 150'
	$textboxWizardProgress.TabIndex = 2
	$textboxWizardProgress.Text = "Hybrid Configuration Wizard Progress`r`n"
	$textboxWizardProgress.TextAlign = 'Left'
    $textboxWizardProgress.Multiline = $true
    $textboxWizardProgress.ScrollBars = "Vertical"
    $textboxWizardProgress.AcceptsReturn = "true"
    $textboxWizardProgress.WordWrap = "true"
    $textboxWizardProgress.AutoScrollOffset = 1
    #
	# buttonStart
	#
	$buttonStart.Location = '268, 550'
	$buttonStart.Name = "buttonStart"
	$buttonStart.Size = '75, 20'
	$buttonStart.TabIndex = 0
	$buttonStart.Text = "Start"
	$buttonStart.UseVisualStyleBackColor = $True
	$buttonStart.add_Click($buttonStart_Click)
    $buttonStart.Cursor = "Hand"
	$groupbox1.ResumeLayout()
	$MainForm.ResumeLayout()

    #
    #Contact Info
    #
    $labelcontactinfo 
	$labelcontactinfo.Font = "Microsoft Sans Serif, 10pt"
    $labelcontactinfo.ForeColor = "Red"
    $labelcontactinfo.Location = '20,580'
    $labelcontactinfo.Size = '500,58'
    $labelcontactinfo.TabIndex = 12
    $labelcontactinfo.Text = "Send feedback via the comments on the blog at (https://blogs.msdn.com/spses)"
	

#Save the initial state of the form
	$InitialFormWindowState = $MainForm.WindowState
	#Init the OnLoad event to correct the initial state of the form
	$MainForm.add_Load($Form_StateCorrection_Load)
	#Clean up the control events
	$MainForm.add_FormClosed($Form_Cleanup_FormClosed)
	#Store the control values when form is closing
	$MainForm.add_Closing($Form_FormClosing)
	#Show the Form
	return $MainForm.ShowDialog()
#endregion Generated Form Code
#endregion Generate Wizard Form

