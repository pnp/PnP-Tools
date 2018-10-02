<#
.synopsis
Script determines whether A Workflow Manager Service Application Proxy Connection is deployed to the local farm.
.example
.\DetectWorkFlowConnection.ps1
#>
if ((Get-PSSnapin -Name "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null)
{            
    Add-PSSnapin -Name "Microsoft.SharePoint.PowerShell"
}
cls

$workflowproxy = Get-SPWorkflowServiceApplicationProxy

if ($workflowproxy)
{
    Write-Output "Workflow Manager Connection Detected. Use documented remediation steos to update it after hybrid deployment"
}
else
{
    Write-Output "No Workflow Manager Connection Detected. Remediation not required"
}