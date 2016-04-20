<#
.Synopsis
This cmdlet will list out all the expired service principals 

Goals
    ·   Prevent expired client id


.EXAMPLE

            -------------------------- EXAMPLE 01 --------------------------
            Get-ExpiredServicePrincipals

#>

function Get-ExpiredServicePrincipals {
    #filter out Microsoft and WorkFlow
    $applist = Get-MsolServicePrincipal -all  | Where-Object -FilterScript { ($_.DisplayName -notlike "*Microsoft*") -and ($_.DisplayName -notlike "*WorkFlow*") }
    $today = get-date

    foreach ($appentry in $applist) {
        $pc = Get-MsolServicePrincipalCredential -AppPrincipalId $appentry.AppPrincipalId -ReturnKeyValues $false | Where-Object { ($_.Type -ne "Other") -and ($_.Type -ne "Asymmetric") -and ($_.EndDate  -lt $today)}
        if($pc){
            Write-Host $appentry.DisplayName, $appentry.AppPrincipalId , $pc[0].EndDate.DateTime
        }
   }
}


