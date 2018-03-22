

#region Logging and generic functions
function LogWrite
{
    param([string] $log , [string] $ForegroundColor)

    $global:strmWrtLog.writeLine($log)
    if([string]::IsNullOrEmpty($ForegroundColor))
    {
        Write-Host $log
    }
    else
    {    
        Write-Host $log -ForegroundColor $ForegroundColor
    }
}

function LogError
{
    param([string] $log)
    
    $global:strmWrtError.writeLine($log)
}

function LoginNameToUPN
{
    param([string] $loginName)

    return $loginName.Replace("i:0#.f|membership|", "")
}

function AddToOffice365GroupOwnersMembers
{
    param($groupUserUpn, $groupId, [bool] $Owners)

    # Apply an incremental backoff strategy as after group creation the group is not immediately available on all Azure AD nodes resulting in resource not found errors
    # It can take up to a minute to get all Azure AD nodes in sync
    $retryCount = 5
    $retryAttempts = 0
    $backOffInterval = 2

    LogWrite "Attempting to add $groupUserUpn to group $groupId"  

    while($retryAttempts -le $retryCount)
    {
        try 
        {
            if ($Owners)
            {
                $azureUserId = Get-AzureADUser -ObjectId $groupUserUpn            
                Add-AzureADGroupOwner -ObjectId $groupId -RefObjectId $azureUserId.ObjectId  
                LogWrite "User $groupUserUpn added as group owner"  
            }
            else 
            {
                $azureUserId = Get-AzureADUser -ObjectId $groupUserUpn           
                Add-AzureADGroupMember -ObjectId $groupId -RefObjectId $azureUserId.ObjectId    
                LogWrite "User $groupUserUpn added as group member"  
            }
            
            $retryAttempts = $retryCount + 1;
        }
        catch 
        {
            if ($retryAttempts -lt $retryCount)
            {
                $retryAttempts = $retryAttempts + 1        
                Write-Host "Retry attempt number: $retryAttempts. Sleeping for $backOffInterval seconds..."
                Start-Sleep $backOffInterval
                $backOffInterval = $backOffInterval * 2
            }
            else
            {
                throw
            }
        }
    }
}

function UsageLog
{
    try 
    {
        $cc = Get-PnPContext
        $cc.Load($cc.Web)
        $cc.ClientTag = "SPDev:GroupifyPS"
        $cc.ExecuteQuery()
    }
    catch [Exception] { }
}
#endregion

function GroupifySite
{
    param([string] $siteCollectionUrl, 
          [string] $alias,
          [Boolean] $isPublic,
          [string] $siteClassification,
          $credentials,
          $tenantContext,
          [string] $adminUPN)
    
    
    #region Ensure access to the site collection, if needed promote the calling account to site collection admin
    # Check if we can access the site...if not let's 'promote' ourselves as site admin
    $adminClaim = "i:0#.f|membership|$adminUPN"    
    $adminWasAdded = $false
    $siteOwnersGroup = $null
    $siteContext = $null    
    $siteCollectionUrl = $siteCollectionUrl.TrimEnd("/");

    Try
    {
        LogWrite "User running groupify: $adminUPN"
        LogWrite "Connecting to site $siteCollectionUrl"
        $siteContext = Connect-PnPOnline -Url $siteCollectionUrl -Credentials $credentials -Verbose -ReturnConnection
    }
    Catch [Exception]
    {
        # If Access Denied then use tenant API to add current tenant admin user as site collection admin to the current site
        if ($_.Exception.Response.StatusCode -eq "Unauthorized")
        {
            LogWrite "Temporarily adding user $adminUPN as site collection admin"
            Set-PnPTenantSite -Url $siteCollectionUrl -Owners @($adminUPN) -Connection $tenantContext
            $adminWasAdded = $true
            LogWrite "Second attempt to connect to site $siteCollectionUrl"
            $siteContext = Connect-PnPOnline -Url $siteCollectionUrl -Credentials $credentials -Verbose -ReturnConnection
        }
        else 
        {
            $ErrorMessage = $_.Exception.Message
            LogWrite "Error for site $siteCollectionUrl : $ErrorMessage" Red
            LogError $ErrorMessage
            return              
        }
    }
    #endregion

    Try
    {
        # Groupify steps
        # - [Done] Add current tenant admin as site admin when needed
        # - [Done] Verify site template / publishing feature use and prevent groupify --> align with the logic in the scanner
        # - [Done] Ensure no modern blocking features are enabled...if so fix it
        # - [Done] Ensure the modern page feature is enabled
        # - [Done] Optional: Deploy applications (e.g. application customizer)
        # - [Done] Optional: Add modern home page
        # - [Done] Call groupify API
        # - [Done] Define Site Admins and Site owners as group owners
        # - [Done] Define Site members as group members
        # - []     Have option to "expand" site owners/members if needed
        # - [Done] Remove added tenant admin and site owners from SharePoint admins
        # - [Done] Remove added tenant admin from the Office 365 group

        #region Adding admin
        # Check if current tenant admin is part of the site collection admins, if not add the account        
        $siteAdmins = $null
        if ($adminWasAdded -eq $false)
        {
            try 
            {
                # Eat exceptions here...resulting $siteAdmins variable will be empty which will trigger the needed actions                
                $siteAdmins = Get-PnPSiteCollectionAdmin -Connection $siteContext -ErrorAction Ignore
            }
            catch [Exception] { }
            
            $adminNeedToBeAdded = $true
            foreach($admin in $siteAdmins)
            {
                if ($admin.LoginName -eq $adminClaim)
                {
                    $adminNeedToBeAdded = $false
                    break
                }
            }

            if ($adminNeedToBeAdded)
            {
                LogWrite "Temporarily adding user $adminUPN as site collection admin"
                Set-PnPTenantSite -Url $siteCollectionUrl -Owners @($adminUPN) -Connection $tenantContext
                $adminWasAdded = $true
            }
        }

        UsageLog
        #endregion

        #region Checking for "blockers"
        $publishingSiteFeature = Get-PnPFeature -Identity "F6924D36-2FA8-4F0B-B16D-06B7250180FA" -Scope Site -Connection $siteContext
        $publishingWebFeature = Get-PnPFeature -Identity "94C94CA6-B32F-4DA9-A9E3-1F3D343D7ECB" -Scope Web -Connection $siteContext

        if (($publishingSiteFeature.DefinitionId -ne $null) -or ($publishingWebFeature.DefinitionId -ne $null))
        {
            throw "Publishing feature enabled...can't groupify this site"
        }

        # Grab the web template and verify if it's a groupify blocker
        $web = Get-PnPWeb -Connection $siteContext -Includes WebTemplate,Configuration,Description
        $webTemplate = $web.WebTemplate + $web.Configuration

        if ($webTemplate -eq "BICENTERSITE#0" -or 
            $webTemplate -eq "BLANKINTERNET#0" -or
            $webTemplate -eq "ENTERWIKI#0" -or
            $webTemplate -eq "SRCHCEN#0" -or
            $webTemplate -eq "SRCHCENTERLITE#0" -or
            $webTemplate -eq "POINTPUBLISHINGHUB#0" -or
            $webTemplate -eq "POINTPUBLISHINGTOPIC#0" -or
            $siteCollectionUrl.EndsWith("/sites/contenttypehub"))
        {
            throw "Incompatible web template detected...can't groupify this site"
        }
        #endregion
        
        #region Enable full modern experience by enabling the pages features and disabling "blocking" features
        LogWrite "Enabling modern page feature, disabling modern list UI blocking features"
        # Enable modern page feature
        Enable-PnPFeature -Identity "B6917CB1-93A0-4B97-A84D-7CF49975D4EC" -Scope Web -Force -Connection $siteContext
        # Disable the modern list site level blocking feature
        Disable-PnPFeature -Identity "E3540C7D-6BEA-403C-A224-1A12EAFEE4C4" -Scope Site -Force -Connection $siteContext
        # Disable the modern list web level blocking feature
        Disable-PnPFeature -Identity "52E14B6F-B1BB-4969-B89B-C4FAA56745EF" -Scope Web -Force -Connection $siteContext
        #endregion

        #region Optional: Add SharePoint Framework customizations - sample
        LogWrite "Deploying SPFX application customizer"
        Add-PnPCustomAction -Name "Footer" -Title "Footer" -Location "ClientSideExtension.ApplicationCustomizer" -ClientSideComponentId "edbe7925-a83b-4d61-aabf-81219fdc1539" -ClientSideComponentProperties "{}"
        #endregion

        #region Optional: Add custom home page - sample
        LogWrite "Deploying a custom modern home page"
        $homePage = Get-PnPHomePage -Connection $siteContext
        $newHomePageName = $homePage.Substring($homePage.IndexOf("/") + 1).Replace(".aspx", "_new.aspx")
        $newHomePagePath = $homePage.Substring(0, $homePage.IndexOf("/") + 1)
        $newHomePage = Add-PnPClientSidePage -Name $newHomePageName -LayoutType Article -CommentsEnabled:$false -Publish:$true -Connection $siteContext

        # Add your additional web parts here!
        Add-PnPClientSidePageSection -Page $newHomePage -SectionTemplate OneColumn -Order 1 -Connection $siteContext
        Add-PnPClientSideText -Page $newHomePage -Text "Old home page was <a href=""$siteCollectionUrl/$homePage"">here</a>" -Section 1 -Column 1
        Set-PnPHomePage -RootFolderRelativeUrl ($newHomePagePath + $newHomePageName) -Connection $siteContext
        #endregion        

        #region Prepare for group permission configuration
        # Get admins again now that we've ensured our access
        $siteAdmins = Get-PnPSiteCollectionAdmin -Connection $siteContext
        # Get owners and members before the group claim gets added
        $siteOwnersGroup = Get-PnPGroup -AssociatedOwnerGroup -Connection $siteContext               
        $siteMembersGroup = Get-PnPGroup -AssociatedMemberGroup -Connection $siteContext               
        #endregion

        #region Call groupify API
        LogWrite "Call groupify API with following settings: Alias=$alias, IsPublic=$isPublic, Classification=$siteClassification"
        Add-PnPOffice365GroupToSite -Url $siteCollectionUrl -Alias $alias -DisplayName $alias -Description $web.Description -IsPublic:$isPublic -KeepOldHomePage:$true -Classification $siteClassification -Connection $siteContext
        #endregion

        #region Configure group permissions
        LogWrite "Adding site administrators and site owners to the Office 365 group owners"
        $groupOwners = @{}
        foreach($siteAdmin in $siteAdmins)
        {
            if (($siteAdmin.LoginName).StartsWith("i:0#.f|membership|"))
            {
                $siteAdminUPN = (LoginNameToUPN $siteAdmin.LoginName)
                if (-not ($siteAdminUPN -eq $adminUPN))
                {
                    if (-not ($groupOwners.ContainsKey($siteAdminUPN)))
                    {
                        $groupOwners.Add($siteAdminUPN, $siteAdminUPN)
                    }
                }
            }
            else 
            {
                #TODO: group expansion?    
            }
        }
        foreach($siteOwner in $siteOwnersGroup.Users)
        {
            if (($siteOwner.LoginName).StartsWith("i:0#.f|membership|"))
            {
                $siteOwnerUPN = (LoginNameToUPN $siteOwner.LoginName)
                if (-not ($groupOwners.ContainsKey($siteOwnerUPN)))
                {
                    $groupOwners.Add($siteOwnerUPN, $siteOwnerUPN)
                }
            }
            else 
            {
                #TODO: group expansion?    
            }
        }

        $site = Get-PnPSite -Includes GroupId -Connection $siteContext
        foreach($groupOwner in $groupOwners.keys)
        {
            try 
            {
                AddToOffice365GroupOwnersMembers $groupOwner ($site.GroupId) $true
            }
            catch [Exception]
            {
                $ErrorMessage = $_.Exception.Message
                LogWrite "Error adding user $groupOwner to group owners. Error: $ErrorMessage" Red
                LogError $ErrorMessage
            }
        }

        LogWrite "Adding site members to the Office 365 group members"
        $groupMembers = @{}
        foreach($siteMember in $siteMembersGroup.Users)
        {
            if (($siteMember.LoginName).StartsWith("i:0#.f|membership|"))
            {
                $siteMemberUPN = (LoginNameToUPN $siteMember.LoginName)
                if (-not ($groupMembers.ContainsKey($siteMemberUPN)))
                {
                    $groupMembers.Add($siteMemberUPN, $siteMemberUPN)
                }
            }
            else 
            {
                #TODO: group expansion?    
            }
        }

        foreach($groupMember in $groupMembers.keys)
        {
            try 
            {
                AddToOffice365GroupOwnersMembers $groupMember ($site.GroupId) $false                
            }
            catch [Exception]
            {
                $ErrorMessage = $_.Exception.Message
                LogWrite "Error adding user $groupMember to group members. Error: $ErrorMessage" Red
                LogError $ErrorMessage
            }
        }        
        #endregion

        #region Cleanup updated permissions
        LogWrite "Groupify is done, let's cleanup the configured permissions"
    
        # Remove the added site collection admin - obviously this needs to be the final step in the script :-)
        if ($adminWasAdded)
        {
            #Remove the added site admin from the Office 365 Group owners and members
            LogWrite "Remove $adminUPN from the Office 365 group owners and members"            
            $site = Get-PnPSite -Includes GroupId -Connection $siteContext
            $azureAddedAdminId = Get-AzureADUser -ObjectId $adminUPN
            try 
            {
                Remove-AzureADGroupOwner -ObjectId $site.GroupId -OwnerId $azureAddedAdminId.ObjectId -ErrorAction Ignore
                Remove-AzureADGroupMember -ObjectId $site.GroupId -MemberId $azureAddedAdminId.ObjectId -ErrorAction Ignore                    
            }
            catch [Exception] { }

            LogWrite "Remove $adminUPN from site collection administrators"            
            Remove-PnPSiteCollectionAdmin -Owners @($adminUPN) -Connection $siteContext
}
        #endregion

        LogWrite "Groupify done for site collection $siteCollectionUrl" Green
        
        # Disconnect PnP Powershell from site
        Disconnect-PnPOnline
    }
    Catch [Exception]
    {
        $ErrorMessage = $_.Exception.Message
        LogWrite "Error: $ErrorMessage" Red
        LogError $ErrorMessage

        #region Cleanup updated permissions on error
        # Groupify run did not complete...remove the added tenant admin to restore site permissions as final step in the cleanup
        if ($adminWasAdded)
        {
            # Below logic might fail if the error happened before the groupify API call, but errors are ignored
            $site = Get-PnPSite -Includes GroupId -Connection $siteContext
            $azureAddedAdminId = Get-AzureADUser -ObjectId $adminUPN
            try 
            {
                Remove-AzureADGroupOwner -ObjectId $site.GroupId -OwnerId $azureAddedAdminId.ObjectId -ErrorAction Ignore
                Remove-AzureADGroupMember -ObjectId $site.GroupId -MemberId $azureAddedAdminId.ObjectId -ErrorAction Ignore
                # Final step, remove the added site collection admin
                Remove-PnPSiteCollectionAdmin -Owners @($adminUPN) -Connection $siteContext
            }
            catch [Exception] { }
        }
        #endregion

        LogWrite "Groupify failed for site collection $siteCollectionUrl" Red
    } 

}

#######################################################
# MAIN section                                        #
#######################################################
# Tenant admin url
$tenantAdminUrl = "https://a830edad9050849523e17050400-admin.sharepoint.com"
# If you use credential manager then specify the used credential manager entry, if left blank you'll be asked for a user/pwd
$credentialManagerCredentialToUse = "pnpa830edad9050849523E17050400admin"

#region Setup Logging
$date = Get-Date
$logfile = ((Get-Item -Path ".\" -Verbose).FullName + "\Groupify_log_" + $date.ToFileTime() + ".txt")
$global:strmWrtLog=[System.IO.StreamWriter]$logfile
$global:Errorfile = ((Get-Item -Path ".\" -Verbose).FullName + "\Groupify_error_" + $date.ToFileTime() + ".txt")
$global:strmWrtError=[System.IO.StreamWriter]$Errorfile
#endregion

#region Load needed PowerShell modules
# Ensure PnP PowerShell is loaded
$minimumVersion = New-Object System.Version("2.24.1803.0")
if (-not (Get-InstalledModule -Name SharePointPnPPowerShellOnline -MinimumVersion $minimumVersion -ErrorAction Ignore)) 
{
    Install-Module SharePointPnPPowerShellOnline -MinimumVersion $minimumVersion -Scope CurrentUser
}
Import-Module SharePointPnPPowerShellOnline -DisableNameChecking -MinimumVersion $minimumVersion

# Ensure Azure PowerShell is loaded
$loadAzurePreview = $false
if (-not (Get-Module -ListAvailable -Name AzureAD))
{
    # Maybe the preview AzureAD PowerShell is installed?
    if (-not (Get-Module -ListAvailable -Name AzureADPreview))
    {
        install-module azuread
    }
    else 
    {
        $loadAzurePreview = $true
    }
}

if ($loadAzurePreview)
{
    Import-Module AzureADPreview
}
else 
{
    Import-Module AzureAD   
}
#endregion

#region Gather Groupify run input
# Url of the site collection to remediate
$siteCollectionUrlToRemediate = ""
$siteAlias = ""
$siteIsPublic = $false

# Get the input information
$siteURLFile = Read-Host -Prompt 'Input either single site collection URL (e.g. https://contoso.sharepoint.com/sites/teamsite1) or name of .CSV file (e.g. sitecollections.csv) ?'
if (-not $siteURLFile.EndsWith(".csv"))
{
    $siteCollectionUrlToRemediate = $siteURLFile
    $siteAlias = Read-Host -Prompt 'Input the alias to be used to groupify this site ?'
    $siteIsPublicString = Read-Host -Prompt 'Will the created Office 365 group be a public group ? Enter True for public, False otherwise'
    $siteClassificationLabel = Read-Host -Prompt 'Classification label to use? Enter label or leave empty if not configured'
    try 
    {
        $siteIsPublic = [System.Convert]::ToBoolean($siteIsPublicString) 
    } 
    catch [FormatException]
    {
        $siteIsPublic = $false
    }
}

# Get the tenant admin credentials.
$credentials = $null
$azureADCredentials = $null
$adminUPN = $null
if(![String]::IsNullOrEmpty($credentialManagerCredentialToUse) -and (Get-PnPStoredCredential -Name $credentialManagerCredentialToUse) -ne $null)
{
    $adminUPN = (Get-PnPStoredCredential -Name $credentialManagerCredentialToUse).UserName
    $credentials = $credentialManagerCredentialToUse
    $azureADCredentials = Get-PnPStoredCredential -Name $credentialManagerCredentialToUse -Type PSCredential
}
else
{
    # Prompts for credentials, if not found in the Windows Credential Manager.
    $adminUPN = Read-Host -Prompt "Please enter admin UPN"
    $pass = Read-host -AsSecureString "Please enter admin password"
    $credentials = new-object management.automation.pscredential $adminUPN,$pass
    $azureADCredentials = $credentials
}

if($credentials -eq $null) 
{
    Write-Host "Error: No credentials supplied." -ForegroundColor Red
    exit 1
}
#endregion

#region Connect to SharePoint and Azure
# Get a tenant admin connection, will be reused in the remainder of the script
LogWrite "Connect to tenant admin site $tenantAdminUrl"
$tenantContext = Connect-PnPOnline -Url $tenantAdminUrl -Credentials $credentials -Verbose -ReturnConnection

LogWrite "Connect to Azure AD"
$azureUser = Connect-AzureAD -Credential $azureADCredentials
#endregion

#region Groupify the site(s)
if (-not $siteURLFile.EndsWith(".csv"))
{
    # Remediate the given site collection
    GroupifySite $siteCollectionUrlToRemediate $siteAlias $siteIsPublic $siteClassificationLabel $credentials $tenantContext $adminUPN
}
else 
{
    $csvRows = Import-Csv $siteURLFile
    
    foreach($row in $csvRows)
    {
        if($row.Url.Trim() -ne "")
        {
            $siteUrl = $row.Url
            $siteAlias = $row.Alias
            $siteIsPublicString = $row.IsPublic
    
            try 
            {
                $siteIsPublic = [System.Convert]::ToBoolean($siteIsPublicString) 
            } 
            catch [FormatException] 
            {
                $siteIsPublic = $false
            }    

            $siteClassification = $row.Classification
            if ($siteClassification -ne $null)
            {
                $siteClassification = $siteClassification.Trim(" ")
            }

            GroupifySite $siteUrl $siteAlias $siteIsPublic $siteClassification $credentials $tenantContext $adminUPN
        }
    }
}
#endregion

#region Close log files
if ($global:strmWrtLog -ne $NULL)
{
    $global:strmWrtLog.Close()
    $global:strmWrtLog.Dispose()
}

if ($global:strmWrtError -ne $NULL)
{
    $global:strmWrtError.Close()
    $global:strmWrtError.Dispose()
}
#endregion