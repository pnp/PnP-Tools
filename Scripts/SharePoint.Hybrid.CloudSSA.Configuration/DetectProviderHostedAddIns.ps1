<#
.Synopsis
Script locates Provider Hosted AddIn Instance on the local farm and reports the AddIn name and site url .
.Description
Searches for provider hosted addins and need to be ran with a farm admin account
.example
.\DetectProviderHosterAddIns.ps1
#>

if ((Get-PSSnapin -Name "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{            
    Add-PSSnapin -Name "Microsoft.SharePoint.PowerShell"
}
cls
　
function Find-AppInstances
{

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
            catch
            {
                # Typically this scenario would only arise when the Admin executing the script is not a farm admin, or the site is a Fast Site creation Master template site which is inaccessible by default.
                Write-Output  ""
                write-warning "Unable to enumerate webs at Site: $site.url : If this is expected then ignore this message"
                Write-Output  ""
            }
        } 
    }
}

Find-AppInstances

if($AppsforRemediation.Count -gt 0)
{
    $AppCount = $AppsforRemediation.Count
    Write-Output ""
    Write-Output "Provider Hosted App Instances Discovered: $AppCount Total PHAs"
  
    for ($i = 0; $i -lt $AppCount; $i++)
    { 
    
        $appTitle = $AppsforRemediation[$i][0]
        $clientID = $AppsforRemediation[$i][1]
        $targetWeb = Get-SPWeb $AppsforRemediation[$i][2]
        $appweb = $targetWeb.Url

        Write-Output ""
        Write-Output "Provider Hosted App $appTitle discovered on site $appweb"
    } 
}
Else
{
    Write-Output ""
    Write-Output "No Provider Hosted App Instances Discovered: Remediation not required"
}