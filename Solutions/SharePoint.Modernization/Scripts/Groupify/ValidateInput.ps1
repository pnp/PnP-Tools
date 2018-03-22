
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

function IsGuid
{
    param([string] $owner)

    try
    {
        [GUID]$g = $owner
        $t = $g.GetType()
        return ($t.Name -eq "Guid")
    }
    catch
    {
        return $false
    }
}

function IsGroupConnected
{
    param([string] $owner)

    if (-not [string]::IsNullOrEmpty($owner))
    {
        if ($owner.Length -eq 38)
        {
            
            if ((IsGuid $owner.Substring(0, 36)) -and ($owner.Substring(36, 2) -eq "_o"))
            {
                return $true
            }
        }        
    }

    return $false
}

function ContainsADAttribute
{
    param($PrefixSuffix)

    $ADAttributes = @("[Department]", "[Company]", "[Office]", "[StateOrProvince]", "[CountryOrRegion]", "[Title]")


    foreach($attribute in $ADAttributes)
    {
        if ($PrefixSuffix -like "*$attribute*")
        {
            return $true
        }
    }

    return $false
}

#endregion

#######################################################
# MAIN section                                        #
#######################################################
# Tenant admin url
$tenantAdminUrl = "https://a830edad9050849523e17050400-admin.sharepoint.com"
# If you use credential manager then specify the used credential manager entry, if left blank you'll be asked for a user/pwd
$credentialManagerCredentialToUse = "pnpa830edad9050849523E17050400admin"

#region Setup Logging
$date = Get-Date
$logfile = ((Get-Item -Path ".\" -Verbose).FullName + "\GroupifyInputValidation_log_" + $date.ToFileTime() + ".txt")
$global:strmWrtLog=[System.IO.StreamWriter]$logfile
$global:Errorfile = ((Get-Item -Path ".\" -Verbose).FullName + "\GroupifyInputValidation_error_" + $date.ToFileTime() + ".txt")
$global:strmWrtError=[System.IO.StreamWriter]$Errorfile
#endregion

#region Load needed PowerShell modules
#Ensure PnP PowerShell is loaded
$minimumVersion = New-Object System.Version("2.24.1803.0")
if (-not (Get-InstalledModule -Name SharePointPnPPowerShellOnline -MinimumVersion $minimumVersion -ErrorAction Ignore)) 
{
    Install-Module SharePointPnPPowerShellOnline -MinimumVersion $minimumVersion -Scope CurrentUser -Force
}
Import-Module SharePointPnPPowerShellOnline -DisableNameChecking -MinimumVersion $minimumVersion
#endregion

#region Ensure Azure PowerShell is loaded
$minimumAzurePowerShellVersion = New-Object System.Version("2.0.0.137")
if (-not (Get-InstalledModule -Name AzureADPreview -MinimumVersion $minimumAzurePowerShellVersion -ErrorAction Ignore))
{
    install-module AzureADPreview -MinimumVersion $minimumAzurePowerShellVersion -Scope CurrentUser -Force
}

Import-Module AzureADPreview -MinimumVersion $minimumAzurePowerShellVersion

$siteURLFile = Read-Host -Prompt 'Input name of .CSV file to validate (e.g. sitecollections.csv) ?'

# Get the tenant admin credentials.
$credentials = $null
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

#region Read Azure AD group settings
$groupSettings = (Get-AzureADDirectorySetting | Where-Object -Property DisplayName -Value "Group.Unified" -EQ)

$CheckGroupCreation = $false
$CanCreateGroupsId = $null
$CheckClassificationList = $false
$ClassificationList = $null
$CheckPrefixSuffix = $false
$PrefixSuffix = $null
$CheckDefaultClassification = $false
$DefaultClassification = $null
$CheckCustomBlockedWordsList = $false

if (-not ($groupSettings -eq $null))
{
    if (-not($groupSettings["EnableGroupCreation"] -eq $true))
    {
        # Group creation is restricted to a security group...verify if the current user is part of that group
        # See: https://support.office.com/en-us/article/manage-who-can-create-office-365-groups-4c46c8cb-17d0-44b5-9776-005fced8e618?ui=en-US&rs=en-001&ad=US
        $CheckGroupCreation = $true
        $CanCreateGroupsId = $groupSettings["GroupCreationAllowedGroupId"]
    }

    if (-not ($groupSettings["CustomBlockedWordsList"] -eq ""))
    {
        # Check for blocked words in group name
        # See: https://support.office.com/en-us/article/office-365-groups-naming-policy-6ceca4d3-cad1-4532-9f0f-d469dfbbb552?ui=en-US&rs=en-001&ad=US
        $CheckCustomBlockedWordsList = $true
        $option = [System.StringSplitOptions]::RemoveEmptyEntries
        $CustomBlockedWordsListString = $groupSettings["CustomBlockedWordsList"]
        $CustomBlockedWordsList = $groupSettings["CustomBlockedWordsList"].Split(",", $option)
        
        # Trim array elements
        [int] $arraycounter = 0
        foreach($c in $CustomBlockedWordsList)
        {
            $CustomBlockedWordsList[$arraycounter] = $c.Trim(" ")
            $arraycounter++
        }
    }

    if (-not ($groupSettings["PrefixSuffixNamingRequirement"] -eq ""))
    {
        # Check for prefix/suffix naming - any dynamic tokens beside [groupname] can be problematic since all 
        # groups are created using the user running the bulk groupify
        # See: https://support.office.com/en-us/article/office-365-groups-naming-policy-6ceca4d3-cad1-4532-9f0f-d469dfbbb552?ui=en-US&rs=en-001&ad=US
        $CheckPrefixSuffix = $true
        $PrefixSuffix = $groupSettings["PrefixSuffixNamingRequirement"]
    }

    if (-not ($groupSettings["ClassificationList"] -eq ""))
    {
        # Check for valid classification labels
        # See: https://support.office.com/en-us/article/Manage-Office-365-Groups-with-PowerShell-aeb669aa-1770-4537-9de2-a82ac11b0540 
        $CheckClassificationList = $true

        $option = [System.StringSplitOptions]::RemoveEmptyEntries
        $ClassificationListString = $groupSettings["ClassificationList"]
        $ClassificationList = $groupSettings["ClassificationList"].Split(",", $option)
        
        # Trim array elements
        [int] $arraycounter = 0
        foreach($c in $ClassificationList)
        {
            $ClassificationList[$arraycounter] = $c.Trim(" ")
            $arraycounter++
        }

        if (-not ($groupSettings["DefaultClassification"] -eq ""))
        {        
            $CheckDefaultClassification = $true
            $DefaultClassification = $groupSettings["DefaultClassification"].Trim(" ")
        }
    }    
}
#endregion

#region Validate input
LogWrite "General Azure AD validation"
if ($CheckPrefixSuffix -and (ContainsADAttribute $PrefixSuffix))
{
    $message = "[ERROR] AzureAD Naming policy : $PrefixSuffix does contain AD attributes that are resolved based on the user running the groupify"
    LogWrite $message Red
    LogError $message                         
}

if ($CheckGroupCreation)
{
    $groupToCheck = new-object Microsoft.Open.AzureAD.Model.GroupIdsForMembershipCheck
    $groupToCheck.GroupIds = $CanCreateGroupsId
    $accountToCheck = Get-AzureADUser -SearchString $adminUPN
    $groupsUserIsMemberOf = Select-AzureADGroupIdsUserIsMemberOf -ObjectId $accountToCheck.ObjectId -GroupIdsForMembershipCheck $groupToCheck
    if ($groupsUserIsMemberOf -eq $null)
    {
        $message = "[ERROR] AzureAD Creation policy : $adminUPN is not part of group $CanCreateGroupsId which controls Office 365 Group creation"
        LogWrite $message Red
        LogError $message                         
    }
}

# "approved" aliases
$approvedAliases = @{}

LogWrite "Validating rows in $siteURLFile..."
$csvRows = Import-Csv $siteURLFile

foreach($row in $csvRows)
{
    if($row.Url.Trim() -ne "")
    {
        $siteUrl = $row.Url
        $siteAlias = $row.Alias
        $siteClassification = $row.Classification
        if ($siteClassification -ne $null)
        {
            $siteClassification = $siteClassification.Trim(" ")
        }

        LogWrite "[VALIDATING] $siteUrl with alias [$siteAlias] and classification [$siteClassification]"

        try 
        {
            # First perform validations that do not require to load the site
            if ($siteAlias.IndexOf(" ") -gt 0)
            {
                $message = "[ERROR] $siteUrl : Alias [$siteAlias] contains a space, which not allowed"
                LogWrite $message Red
                LogError $message 
            }
            elseif (($CheckClassificationList -eq $true) -and (-not ($ClassificationList -contains $siteClassification)))
            {
                $message = "[ERROR] $siteUrl : Classification [$siteClassification] does not comply with available AzureAD classifications [$ClassificationListString]"
                LogWrite $message Red
                LogError $message                         
            }     
            elseif (($CheckCustomBlockedWordsList -eq $true) -and ($CustomBlockedWordsList -contains $siteAlias))
            {
                $message = "[ERROR] $siteUrl : Alias [$siteAlias] is in the AzureAD blocked word list [$CustomBlockedWordsListString]"
                LogWrite $message Red
                LogError $message                         
            }                       
            else 
            {
                # try getting the site
                $site = Get-PnPTenantSite -Url $siteUrl -Connection $tenantContext -ErrorAction Ignore
                
                if ($site.Status -eq "Active")
                {
                    if (IsGroupConnected $site.Owner)
                    {
                        $message = "[ERROR] $siteUrl : Site is already connected a group"
                        LogWrite $message Red
                        LogError $message 
                    }
                    else
                    {
                        $aliasIsUsed = Test-PnPOffice365GroupAliasIsUsed -Alias $siteAlias -Connection $tenantContext      
                        if ($aliasIsUsed)
                        {
                            $message = "[ERROR] $siteUrl : Alias [$siteAlias] is already in use"
                            LogWrite $message Red
                            LogError $message   
                        }
                        elseif ($approvedAliases.ContainsKey($siteAlias))
                        {
                            $message = "[ERROR] $siteUrl : Alias [$siteAlias] was already marked as approved alias for another site in this file"
                            LogWrite $message Red
                            LogError $message   
                        }
                        else 
                        {
                            $approvedAliases.Add($siteAlias, $siteAlias)
                            LogWrite "[VALIDATED] $siteUrl with alias [$siteAlias] and classification [$siteClassification]" Green
                        }                        
                    }
                }
                else 
                {
                    $message = "[ERROR] $siteUrl : Site does not exist or is not available (status = $($site.Status))"
                    LogWrite $message Red    
                    LogError $message
                }                
            }
        }
        catch [Exception]
        {
            $ErrorMessage = $_.Exception.Message
            LogWrite "Error: $ErrorMessage" Red
            LogError $ErrorMessage    
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
