
function Load-PnPModernizationFramework
{
    param(
        [string] $PathToModernizationBinaries
    )

    begin
    {
    }

    process
    {
        Add-Type -Path "$PathToModernizationBinaries\SharePoint.Modernization.Framework.dll"
    }

    end
    {
        return $PathToModernizationBinaries
    }
}

function Execute-PnPModernizationPageTransformation
{
    param(
        [string] $PathToModernizationBinaries,
        $Page,
        [bool] $Overwrite = $false,
        [bool] $HandleWikiImagesAndVideos = $true,
        [bool] $ReplaceHomePageWithDefaultHomePage = $false
    )

    begin
    {

    }

    process
    {
        [bool] $transformOK = $true
        try 
        {            
            # Create the PageTransformationInformation object and populate it
            $pageTransformationInformation = New-Object -TypeName SharePoint.Modernization.Framework.Transform.PageTransformationInformation -ArgumentList $Page
            $pageTransformationInformation.Overwrite = $Overwrite
            $pageTransformationInformation.HandleWikiImagesAndVideos = $HandleWikiImagesAndVideos
            $pageTransformationInformation.ReplaceHomePageWithDefaultHomePage = $ReplaceHomePageWithDefaultHomePage

            # Instantiate the page transformator
            $pageTransformator = New-Object -TypeName SharePoint.Modernization.Framework.Transform.PageTransformator -ArgumentList $Page.Context, "$PathToModernizationBinaries\webpartmapping.xml"
            # Transform
            $pageTransformator.Transform($pageTransformationInformation)  
        }
        catch [Exception]
        {
            Write-Host $_.Exception.Message -ForegroundColor Red
            $transformOK = $false
        }

    }

    end
    {
        return $transformOK
    }
    
}

#######################################################
# MAIN section                                        #
#######################################################
# variables
$CAMLQueryByExtension = "<View Scope='Recursive'><Query><Where><Contains><FieldRef Name='File_x0020_Type'/><Value Type='text'>aspx</Value></Contains></Where></Query></View>"
$CAMLQueryByExtensionAndName = "<View Scope='Recursive'><Query><Where><And><Contains><FieldRef Name='File_x0020_Type'/><Value Type='text'>aspx</Value></Contains><BeginsWith><FieldRef Name='FileLeafRef'/><Value Type='text'>{0}</Value></BeginsWith></And></Where></Query></View>"
$binaryFolder = "C:\github\BertPnPTools\Solutions\SharePoint.Modernization\SharePoint.Modernization.Framework\bin\Debug"

# Load the SharePoint Modernization framework
Load-PnPModernizationFramework $binaryFolder

# Connect to site
Connect-PnPOnline -Url https://bertonline.sharepoint.com/sites/espctest2 -Verbose

# Get aspx page(s)
[string] $query = [string]::Format($CAMLQueryByExtensionAndName, "webparts.aspx")
$pages = Get-PnPListItem -List SitePages -Query $query 

# Modernize the found pages
foreach($page in $pages)
{
    Write-Host "Modernizing " $page.FieldValues["FileLeafRef"] "..."    
    if (Execute-PnPModernizationPageTransformation $binaryFolder $page $true)
    {
        Write-Host "Done!" -ForegroundColor Green
    }
}
