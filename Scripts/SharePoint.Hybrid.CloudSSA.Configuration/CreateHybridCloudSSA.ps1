<#

This script is designed specifically to deploy a hybrid cloud Search Service Application to a SharePoint Server farm

It requires the following variables:

*SearchApplicationPoolAccountName - this is the account for the search app pool. If there is an existing local SSA that same account can be reused. Note that this account must be a managed SharePoint service account

*DatabaseServer - this is the name of an existing SharePoint database server

*SearchServer1 - this is the name of the first new server for Hybrid Search services.  This server must already be added to the farm and should not host any SharePoint roles other than what was created when the server was added

*SearchServer2 - this is the name of the second new server for Hybrid Search services.  The presence of this server is optional if the customer does not have a highly available SharePoint farm.  This server must already be added to the farm and should not host any SharePoint roles other than what was created when the server was added

The Authors recommend that two servers be used for high availability of the search service application

#> 


 Param( 
    [Parameter(Mandatory=$true)][string] $SearchApplicationPoolAccountName, 
    [Parameter(Mandatory=$true)][string] $DatabaseServer,
    [Parameter(Mandatory=$true)][string] $SearchServer1,
    [Parameter(Mandatory=$false)][string] $SearchServer2
) 

 #==============================================================
           #Set name constants
 #============================================================== 
 Add-PSSnapin Microsoft.SharePoint.Powershell
 $SearchServiceApplicationName = "HybridSearchServiceApplication"
 $DatabaseName = "HybridSearchServiceApplication"

 #==============================================================
           #Search Application Pool
 #==============================================================
 Write-Host -ForegroundColor DarkGray "Checking if Search Application Pool exists"
 $SPServiceApplicationPool = Get-SPServiceApplicationPool -Identity $SearchServiceApplicationName -ErrorAction SilentlyContinue
  
 if (!$SPServiceApplicationPool)
 {
     Write-Host -ForegroundColor Yellow "Creating Search Application Pool"
     $SPServiceApplicationPool = New-SPServiceApplicationPool -Name  $SearchServiceApplicationName -Account $SearchApplicationPoolAccountName -Verbose
 }
  
 #==============================================================
           #Search Service Application
 #==============================================================
 Write-Host -ForegroundColor DarkGray "Checking if SSA exists"
 $SearchServiceApplication = Get-SPEnterpriseSearchServiceApplication -Identity $SearchServiceApplicationName -ErrorAction SilentlyContinue
 if (!$SearchServiceApplication)
 {
     Write-Host -ForegroundColor Yellow "Creating Search Service Application"
     $SearchServiceApplication = New-SPEnterpriseSearchServiceApplication -Name $SearchServiceApplicationName -ApplicationPool $SPServiceApplicationPool.Name -DatabaseServer $DatabaseServer -DatabaseName $DatabaseName -CloudIndex $true
 }
  
 Write-Host -ForegroundColor DarkGray "Checking if SSA Proxy exists"
 $SearchServiceApplicationProxy = Get-SPEnterpriseSearchServiceApplicationProxy -Identity ($SearchServiceApplicationName + "_Proxy") -ErrorAction SilentlyContinue
 if (!$SearchServiceApplicationProxy)
 {
     Write-Host -ForegroundColor Yellow "Creating SSA Proxy"
     New-SPEnterpriseSearchServiceApplicationProxy -Name ($SearchServiceApplicationName + "_Proxy") -SearchApplication $SearchServiceApplicationName
 }
 

#==============================================================
          #Start Search Service Instance on Server1
#==============================================================
$SearchServiceInstanceServer1 = Get-SPEnterpriseSearchServiceInstance $SearchServer1
 Write-Host -ForegroundColor DarkGray "Checking if SSI is Online on SearchServer1"
 if($SearchServiceInstanceServer1.Status -ne 'Online')
 {
   Write-Host -ForegroundColor Yellow "Starting Search Service Instance"
   Start-SPEnterpriseSearchServiceInstance -Identity $SearchServiceInstanceServer1
   While ($SearchServiceInstanceServer1.Status -ne 'Online')
   {
       Start-Sleep -s 5
       Write-Host "Sleeping for 5 seconds"
       $SearchServiceInstanceServer1 = Get-SPEnterpriseSearchServiceInstance -Identity $SearchServer1
   }
   Write-Host -ForegroundColor Yellow "SSI on SearchServer1 is started"
 }
 
 #==============================================================
         #Start Search Service Instance on Server2 (if specified)
 #==============================================================
 If ($SearchServer2)
 {
 
 $SearchServiceInstanceServer2 = Get-SPEnterpriseSearchServiceInstance $SearchServer2
 Write-Host -ForegroundColor DarkGray "Checking if SSI is Online on SearchServer2"
 if($SearchServiceInstanceServer2.Status -ne 'Online')
 {
   Write-Host -ForegroundColor Yellow "Starting Search Service Instance"
   Start-SPEnterpriseSearchServiceInstance -Identity $SearchServiceInstanceServer2
   While ($SearchServiceInstanceServer2.Status -ne 'Online')
   {
       Start-Sleep -s 5
       Write-Host "Sleeping for 5 seconds"
       $SearchServiceInstanceServer2 = Get-SPEnterpriseSearchServiceInstance -Identity $SearchServer2
   }
   Write-Host -ForegroundColor Yellow "SSI on SearchServer2 is started"
 }

 }
  #==============================================================
  #Cannot make changes to topology in Active State.
  #Create new topology to add components
 #==============================================================
  
 $InitialSearchTopology = $SearchServiceApplication | Get-SPEnterpriseSearchTopology -Active 
 $NewSearchTopology = $SearchServiceApplication | New-SPEnterpriseSearchTopology
 
 #==============================================================
         #Search Service Application Components on SearchServer1
         #Creating all components except Index (created later)     
 #==============================================================
 New-SPEnterpriseSearchAnalyticsProcessingComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1
  
 New-SPEnterpriseSearchContentProcessingComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1
  
 New-SPEnterpriseSearchQueryProcessingComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1
  
 New-SPEnterpriseSearchCrawlComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1 
  
 New-SPEnterpriseSearchAdminComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1
 
 #==============================================================
         #Search Service Application Components on SearchServer2 (if specified)
         #Creating all components except Index (created later)
 #==============================================================
 If ($SearchServer2)
 {

 New-SPEnterpriseSearchAnalyticsProcessingComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2

 New-SPEnterpriseSearchContentProcessingComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2
  
 New-SPEnterpriseSearchQueryProcessingComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2
  
 New-SPEnterpriseSearchCrawlComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2 

 New-SPEnterpriseSearchAdminComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2
 
 } 
#==============================================================
         #Index Components with replicas on SearchServer1 and SearchServer2 (if specified)
 #==============================================================
  
 New-SPEnterpriseSearchIndexComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1 -IndexPartition 0
  
 New-SPEnterpriseSearchIndexComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer1 -IndexPartition 1

 If ($SearchServer2)
 {
 
 New-SPEnterpriseSearchIndexComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2 -IndexPartition 0
  
 New-SPEnterpriseSearchIndexComponent -SearchTopology $NewSearchTopology -SearchServiceInstance $SearchServiceInstanceServer2 -IndexPartition 1
  
 }
  
$NewSearchTopology.Activate()

#=================================================


$ssa = Get-SPEnterpriseSearchServiceApplication | ?{$_.CloudIndex -eq $true}

Get-SPEnterpriseSearchTopology -Active -SearchApplication $ssa

Get-SPEnterpriseSearchStatus -SearchApplication $ssa -Text |ft Name, state,Partition,Host -AutoSize

$ssa.CloudIndex 
