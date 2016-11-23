param(
	[string]$pdir = "",
	[string]$outdir = ""
)

$dest = "${Env:ProgramFiles(x86)}\MSBuild\CommunityExtensions\SharePointPnP\"
$pdir = $pdir.Trim()
$fullOutDir = $pdir + $outdir.Trim()

New-Item -ItemType Directory -Force -Path $dest

Copy-Item -Path "$fullOutDir\SharePointPnP.DeveloperTools.MSBuild.Tasks.dll" -Destination $dest -Force
Copy-Item -Path "$fullOutDir\SharePointPnP.DeveloperTools.Common.dll" -Destination $dest -Force
Copy-Item -Path "$fullOutDir\OfficeDevPnP.Core.dll" -Destination $dest -Force
Copy-Item -Path "$fullOutDir\Microsoft.SharePoint.Client.dll" -Destination $dest -Force
Copy-Item -Path "$fullOutDir\Microsoft.SharePoint.Client.Runtime.dll" -Destination $dest -Force
Copy-Item -Path "$fullOutDir\SharePointPnP.DeveloperTools.Provisioning.targets" -Destination $dest -Force
