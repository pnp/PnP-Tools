<#
.SYNOPSIS
Update the Build & Revision numbers of the AssemblyFileVersion in a .NET AssemblyInfo File
.DESCRIPTION
This function opens a specified AssemblyInfo file in a .NET Project and can modify the Build and/or Revision components of the AssemblyFileVersion attribute. The file is designed to be used as a Pre-Build event on a project.
Throws an exception if the update fails.
.EXAMPLE
.\Update-AssemblyVersion.ps1 -assemblyInfoFilePath "C:\Projects\FooBarProject\AssemblyInfo.cs"
	
.NOTES
Author:		Eoin Campbell
	
.LINK
http://trycatch.me/automatically-update-the-assemblyfileversion-attribute-of-a-net-assembly/
#>

param(
    # The fully qualified path to the 
    [string]$assemblyInfoFilePath = $(throw "Mandatory parameter -assemblyInfoFilePath not supplied.")
)
 
#used to make it easier to spot the comments from the script in the Build Output window
$msgPrefix="   |   "
 
 
function AssignVersionValue([string]$oldValue, [string]$newValue) {
    if ($newValue -eq $null -or $newValue -eq "") {
        $oldValue
    } else {
        $newValue
    }
}
 
 
function SetAssemblyFileVersion([string]$pathToFile, [string]$majorVer, [string]$minorVer, [string]$buildVer, [string]$revVer) {
 
    #load the file and process the lines
    $newFile = Get-Content $pathToFile -encoding "UTF8" | foreach-object {
        if ($_.StartsWith("[assembly: AssemblyFileVersion")) {
            $verStart = $_.IndexOf("(")
            $verEnd = $_.IndexOf(")", $verStart)
            $origVersion = $_.SubString($verStart+2, $verEnd-$verStart-3)
            
            $segments=$origVersion.Split(".")
            
            #default values for each segment
            $v1="1"
            $v2="0"
            $v3="0"
            $v4="0"
            
            #assign them based on what was found
            if ($segments.Length -gt 0) { $v1=$segments[0] }
            if ($segments.Length -gt 1) { $v2=$segments[1] } 
            if ($segments.Length -gt 2) { $v3=$segments[2] } 
            if ($segments.Length -gt 3) { $v4=$segments[3] } 
            
            $v1 = AssignVersionValue $v1 $majorVer
            $v2 = AssignVersionValue $v2 $minorVer
            $v3 = AssignVersionValue $v3 $buildVer
            $v4 = AssignVersionValue $v4 $revVer
            
            if ($v1 -eq $null) { throw "Major version CANNOT be blank!" }
            if ($v2 -eq $null) { throw "Minor version CANNOT be blank!" }
            
            $newVersion = "$v1.$v2"
            
            if ($v3 -ne $null) {
                $newVersion = "$newVersion.$v3"
                
                if ($v4 -ne $null) {
                    $newVersion = "$newVersion.$v4"
                }
            }
 
            write-host "$msgPrefix Setting AssemblyFileVersion to $newVersion"
            $_.Replace($origVersion, $newVersion)
        }  else {
            $_
        } 
    }
    
    $newfile | set-Content $pathToFile -encoding "UTF8"
}
 
 
 
if ($assemblyInfoFilePath -eq "" -or $assemblyInfoFilePath -eq $null) {
    throw "You must supply a valid assemblyinfo.cs path"
}
 
# the values here can be whatever your heart desires
$major=$null # $null indicates that whatever value is currently in the file should be used as-is
$minor=$null 
$build=[int32](((get-date).Year-2000)*366)+(Get-Date).DayOfYear
$rev=[int32](((get-date)-(Get-Date).Date).TotalSeconds / 2)
 
SetAssemblyFileVersion $assemblyInfoFilePath $major $minor $build $rev
