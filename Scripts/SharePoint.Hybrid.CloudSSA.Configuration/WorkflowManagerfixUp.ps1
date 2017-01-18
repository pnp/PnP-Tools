# SUMMARY 
# This script will detect the details of the workflow manager proxy on the farm and recreate the proxy connection
# The proxy connection needs to be recreated due tot he change in AuthentivationRealm ID caused by hybrid deployment.

# EXECUTION
# Run the script as a farm and local admin on a SharePoint server. 


Add-PsSnapin Microsoft.SharePoint.PowerShell -Ea Continue

function Update-WorkflowManagerProxyConnection
{

$workflowproxy = Get-SPWorkflowServiceApplicationProxy
try{
$webapp = get-spwebapplication
}
catch
{}
if ($webapp)
{
    $webappurl = $webapp[0].url
    try
    {
    $Site=get-spsite $webappurl
    }
    catch
    {}
    if ($site)
    {
        $workflowaddress = $workflowproxy.GetWorkflowServiceAddress($site)
        $workflowscopename = $workflowproxy.GetWorkflowScopeName($site)
        $TrimScope = '/'+$workflowscopename+'/'
        $wfmaddress = $workflowaddress.TrimEnd($Trimscope)
     }
     Else
     {
     Write-Warning "There is no site collection at the root of the web application. Create a site collection at $webappurl to fix the workflow manager connection"
     }
}
else
{
Write-Warning "There are no web applications on this farm. Workflow Manager cannot be conencted to a farm with no web applications"
}

write-Warning "Deleting the Workflow proxy connection to $wfmaddress"
$workflowproxy.delete()

try
{
write-output "Recreating the Workflow manager proxy connection to $wfmaddress"
Register-SPWorkflowService -SPSite $Site -WorkflowHostUri $wfmaddress -Force
}
catch
{
Write-Error "Failed to create the proxy connection to $wfmaddress . Please repair this connection manually before attempting to progress any workflow on this farm"
}
}

Update-WorkflowManagerProxyConnection
 
