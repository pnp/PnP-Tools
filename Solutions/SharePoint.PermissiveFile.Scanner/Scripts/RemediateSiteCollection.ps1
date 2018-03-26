# This script does rename .htm and .html files to .aspx files. Doing so enables these files to be "executed" in SharePoint Online 
# which has it's file handling configured to be strict. See https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-permissivesetting 
# for more details

function PermissiveRemediateASiteCollection
{
    param([string] $siteCollectionUrl, [string] $winCredentialsManagerLabel)
    
    $siteCollectionUrl = $siteCollectionUrl.TrimEnd("/");

    # Gets or Sets the tenant admin credentials.
    $credentials = $null

    if(![String]::IsNullOrEmpty($winCredentialsManagerLabel) -and (Get-PnPStoredCredential -Name $winCredentialsManagerLabel) -ne $null)
    {
        $credentials = $winCredentialsManagerLabel
    }
    else
    {
        # Prompts for credentials, if not found in the Windows Credential Manager.
        $email = Read-Host -Prompt "Please enter admin email"
        $pass = Read-host -AsSecureString "Please enter admin password"
        $credentials = New-Object –TypeName "System.Management.Automation.PSCredential" –ArgumentList $email, $pass
    }

    if($credentials -eq $null) 
    {
        Write-Host "Error: No credentials supplied." -ForegroundColor Red
        exit 1
    }

    Connect-PnPOnline -Url $siteCollectionUrl -Credentials $credentials -Verbose

    Write-Host "Using search to obtain a list of files to remediate..."
    Try
    {
        $searchQuery = "((fileextension=htm OR fileextension=html) AND contentclass=STS_ListItem_DocumentLibrary AND Path:$siteCollectionUrl/*)"
        $htmlFiles = Submit-PnPSearchQuery -Query $searchQuery -TrimDuplicates:$false -All
    }
    Catch [Exception]
    {
       $ErrorMessage = $_.Exception.Message
       Write-Host "Error: Search query to find the files to remediate failed...exiting the script" -ForegroundColor Red 
       Write-Host "Error: $ErrorMessage" -ForegroundColor Red 
       exit 1
    }

    # if no files were found then bail out
    if ($htmlFiles.RowCount -eq 0)
    {
        Write-Host "No files found to remediate...exiting the script" -ForegroundColor Green
        exit 0
    }
    else
    {
        Write-Host "Found" $htmlFiles.RowCount "files to remediate" -ForegroundColor Green
    }

    # Create temp folder if not yet existing
    $path = "$env:TEMP\permissivefix"
    If(!(test-path $path))
    {
          New-Item -ItemType Directory -Force -Path $path
    }

    # iterate over the found files and rename them
    foreach($htmlFile in $htmlFiles.ResultRows)
    {
        Try
        {
            $web = $htmlFile.SPWebUrl
            Write-Host "Connected to $web..."
            Connect-PnPOnline -Url $web -Credentials $credentials

            $fileToRename = $htmlFile.OriginalPath
            Write-Host "Renaming $fileToRename..."

            # Get the server relative path
            $serverRelativePath = $fileToRename.Replace("https://", "")
            $serverRelativePath = $serverRelativePath.Substring($serverRelativePath.IndexOf("/"))
            #Write-Host $serverRelativePath

            # Get new file name and server relative path
            $newFileName = $serverRelativePath.Substring($serverRelativePath.LastIndexOf("/") + 1).ToLower()
            $serverRelativePathNew = $serverRelativePath
            if ($newFileName.EndsWith(".html"))
            {
                $newFileName = $newFileName.Replace(".html", ".aspx")
                $serverRelativePathNew = $serverRelativePathNew.Replace(".html", ".aspx")
            } 
            elseif($newFileName.EndsWith(".htm"))
            {
                $newFileName = $newFileName.Replace(".htm", ".aspx")
                $serverRelativePathNew = $serverRelativePathNew.Replace(".html", ".aspx")
            }
        
            # Perform the file rename
            Rename-PnPFile -ServerRelativeUrl $serverRelativePath -TargetFileName $newFileName -OverwriteIfAlreadyExists -Force
        
            # Download the file once to ensure it works correctly in modern UI
            Get-PnPFile -Url $serverRelativePathNew -Path $path -Filename $newFileName -AsFile -Force
        }
        Catch [Exception]
        {
           $ErrorMessage = $_.Exception.Message
           Write-Host "Error: Rename of file $serverRelativePath failed" -ForegroundColor Red 
           Write-Host "Error: $ErrorMessage" -ForegroundColor Red 
        }
    }

    # Cleanup the temp folder
    Write-Host "Cleaning up the temp folder $path"
    Remove-Item $path -Recurse -ErrorAction Ignore

    Write-Host "Remediation done for site collection $siteCollectionUrl" -BackgroundColor DarkGreen -ForegroundColor White
}

#######################################################
# MAIN section                                        #
#######################################################

# Url of the site collection to remediate
$siteCollectionUrlToRemediate = "https://bertonline.sharepoint.com/sites/espctest1"
# If you use credential manager then specify the used credential manager entry, if left blank you'll be asked for a user/pwd
$credentialManagerCredentialToUse = "bertonline"

# Ensure PnP PowerShell is loaded
if (-not (Get-Module -ListAvailable -Name SharePointPnPPowerShellOnline)) 
{
    Install-Module SharePointPnPPowerShellOnline
}

Import-Module SharePointPnPPowerShellOnline

# Remediate the given site collection
PermissiveRemediateASiteCollection $siteCollectionUrlToRemediate $credentialManagerCredentialToUse

