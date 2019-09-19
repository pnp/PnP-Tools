# SharePoint Online Visio Web Access deprecation scanner #

### Summary ###
Visio Online replaces Visio Web Access (also called Visio Services) in SharePoint Online. Visio Online is the new way to view, create and share Visio diagrams in SharePoint Online. As part of the Office Online ecosystem, Visio Online includes a modern UI, introduces performance improvements and has richer capabilities that aren’t available in Visio Web Access (see https://developer.microsoft.com/en-us/visio/blogs/migrate-from-visio-web-access-to-visio-online/) for more details on the actual deprecation. To help you find the relevant files and pages that require remediation work a scanner was developed, which is further explained in this article. If you prefer watching a video then checkout our [PnP Shorts - Visio Web Access deprecation video on YouTube](https://youtu.be/i2fKlUkuuHI).

> Important:
> This deprecation only applies to SharePoint Online, SharePoint on-premises is not impacted.

### Applies to ###
-  SharePoint Online

### Solution ###
Solution | Author(s)
---------|----------
SharePointSharePoint.Visio.Scanner | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1 | March 7th 2018 | Also scan SiteAssets library for web part pages + reliability improvements
1.0 | February 1st 2018 | First main version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What will this tool do for you? #
The main purpose of this tool is to give you a set of reports that you can use to:
- Find all the Visio Web Drawing files (.VDW files) as there need to be saved as VSD or VSDX files to be used in Visio Online
- Find all the pages holding the Visio Web Part: these pages need to be remediated to work with Visio Online

# Quick start guide #
## Download the tool ##
You can download the tool from here:
 - [Visio Web Access scanner for SharePoint Online](https://github.com/SharePoint/PnP-Tools/blob/master/Solutions/SharePoint.Visio.Scanner/Releases/SharePoint.Visio.Scanner%20v1.1.zip?raw=true)

Once you've downloaded the tool you have a folder containing the tool **SharePoint.Visio.Scanner.exe**. Start a (PowerShell) command prompt and navigate to that folder so that you can use the tool.

> Note:
> If you want to compile the tool yourself you'll also need to have the SharePoint.Scanning solution available as this tools depends on the SharePoint Scanner framework to compile.

## Using the scanner for SharePoint Online ##
Since this tool needs to be able to scan all site collections it's recommended to use an app-only principal with tenant scoped permissions for the scan. This approach will ensure the tool has access, if you use an account (e.g. your SharePoint tenant admin account) then the tool can only access the sites where this user also has access. You can either use a an Azure AD application or a SharePoint app principal:
 - [Granting access via Azure AD App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread)
 - [Granting access via SharePoint App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azureacs)


Once the preparation work is done, let's continue with doing a scan.

### Scanning SharePoint Online environment ###
Below option is the typical usage of the tool for most customers: you specify the mode, your tenant name and the created client id and secret:

```console
SharePoint.Visio.Scanner.exe -t <tenant> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.Visio.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

You'll see the following output during the run:

```console
=====================================================
Scanning is starting...1/02/2018 19:10:07
=====================================================
Phase 1: Get all the VDW files...
Start search query fileextension=VDW
Found 1 rows...
Retrieving a batch of up to 500 search results
Processing site https://bertonline.sharepoint.com/sites/espctest1...
Start search query path:https://bertonline.sharepoint.com/sites/espctest1 AND fileextension=aspx AND (contentclass=STS_ListItem_WebPageLibrary OR contentclass=STS_Site OR contentclass=STS_Web)
Found 10 rows...
Retrieving a batch of up to 500 search results
Scan of page /sites/espctest1/SitePages/Home.aspx took 2 seconds
Scan of page /sites/espctest1/SitePages/ContentRollup.aspx took 2 seconds
Scan of page /sites/espctest1/SitePages/forms.aspx took 0 seconds
Scan of page /sites/espctest1/SitePages/mediaandcontent.aspx took 0 seconds
Scan of page /sites/espctest1/SitePages/socialcollab.aspx took 1 seconds
Scan of page /sites/espctest1/SitePages/search.aspx took 4 seconds
Visio web part found on page /sites/espctest1/SitePages/other.aspx
Scan of page /sites/espctest1/SitePages/other.aspx took 1 seconds
Scan of page /sites/espctest1/SitePages/others.aspx took 1 seconds
Processing site https://bertonline.sharepoint.com/sites/espctest1/dummy...
Scan of page /sites/espctest1/dummy/SitePages/Home.aspx took 1 seconds
Scan of page /sites/espctest1/dummy/SitePages/How To Use This Library.aspx took 0 seconds
Thread: 5. Processed 1 of 1 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 24 seconds.
=====================================================
Scanning is done...now dump the results to a CSV file
=====================================================
Outputting errors to 636531090073499869\errors.csv
Outputting information over the done scan to 636531090073499869\ScannerSummary.csv
Outputting scan results to 636531090073499869\VisioVDWResults.csv
Outputting scan results to 636531090073499869\VisioWebPartResults.csv
=====================================================
All done. Took 00:00:25.2779469 for 1 sites
=====================================================
```

After the run you'll find a new sub folder (e.g. 636531090073499869) which contains the following:

Report | Content
---------|----------
**VisioVDWResults.csv** | This report contains an overview of all found Visio Web Drawing files (.vdw files).
**VisioWebPartResults.csv** | Holds a list of pages (wiki page, web part page) that contain one or more instances of the Visio Web Access web part. The Visio Web Access web parts are exported with all properties, for the other web parts on the page only basic information is exported
**Error.csv** | If the scan tool encountered errors then these are logged in this file.
**ScannerSummary.csv** | Logs the number of scanned site collections, webs and list. It will also contain information on scan duration and used scanner version.


# Report details
## Understanding the VisioVDWResults.csv file
This report contains the following columns:

Column | Description
---------|----------
**SiteCollectionUrl** | Url of the scanned site collection.
**SiteUrl** | Url of the scanned web.
**OriginalPath** | Full url to the found Visio Web Drawing file.

### Key takeaways from this report
Load the VisioVDWResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per found Visio Web Drawing file. These files need to be saved as VSD or VSDX files if you want to keep using them in Visio Online.

## Understanding the VisioWebPartResults.csv file
This report contains the following columns:

Column | Description
---------|----------
**SiteCollectionUrl** | Url of the scanned site collection.
**SiteUrl** | Url of the scanned web.
**PageUrl** | Server relative URL of the scanned page.
**Library** | Server relative URL of the pages library holding the scanned page.
**ModifiedBy** | Last person that modified this page.
**ModifiedAt** | Last modification date and time for this page.
**ViewsRecent** | Number of views this page received in the last 14 days.
**ViewsRecentUniqueUsers** | Number of unique visitors for this page in the last 14 days.
**ViewsLifeTime** | Number of views this page received during it's lifetime.
**ViewsLifeTimeUniqueUsers** | Number of unique visitors for this page during it's lifetime.
**WPType1-15** | Up to 15 columns indicating the web part type.
**WPTitle1-15** | Up to 15 columns indicating the web part title.
**WPData1-15** | Up to 15 columns holding the exported web part data (see upcoming chapter for details).

### Key takeaways from this report
Load the VisioWebPartResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per page holding one or more Visio Web Access web parts
**ViewsRecent > 0** | Pages which have been recently accessed

### WPData details
For each exported web part the same base JSON structure is used as shown below. Important to note are:
- The "Properties" property holds the web part specific properties in case of a Visio Web Access web part

```JSON
{
	"Type":"<fully qualified web part type>",
	"Id":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ServerControlId":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ZoneId":"FullPage",
	"Hidden":false,
	"IsClosed":false,
	"Title":"<web part title>",
	"Order":0,
	"ZoneIndex":0,
	"Properties":{"<web part specific properties>"}
}
```

Below you can find a complete example:

```JSON
{
	"Type":"Microsoft.Office.Visio.Server.WebControls.VisioWebAccess, Microsoft.Office.Visio.Server, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c",
	"Id":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ServerControlId":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ZoneId":"FullPage",
	"Hidden":false,
	"IsClosed":false,
	"Title":"Visio Web Access",
	"Order":0,
	"ZoneIndex":0,
	"Properties":
	{
		"Title":"Visio Web Access",
		"Description":"Enables viewing and refreshing of Visio Web Drawings.", 
		"Width":"", 
		"Height":"400px", 
		"ShapeDataNames":"", 
		"ShowBackground":"True", 
		"ShowShapeInfoButton":"True", 
		"ShowPageNavigation":"True", 
		"DisableSelection":"False", 
		"FitViewToShapes":"False", 
		"AlwaysRaster":"False", 
		"AutoRefreshInterval":"0", 
		"ShowRefresh":"True", 
		"DiagramPath":"/sites/espctest1/Shared Documents/Drawing1.vsdx", 
		"OverrideViewSettings":"True", 
		"ShowZoomControl":"True", 
		"ViewSettings":"", 
		"DisableZoom":"False", 
		"DefaultPageShown":"1", 
		"ShowOpenInVisio":"True", 
		"DisableHyperlink":"False"
	}
}
```

# Advanced topics #

## I only want to find the Visio Web Drawing files, can I do that? ##
You might already have an inventory of all the pages holding a Visio Web Access web part from a previous run or from another scanner (e.g. the [modernization scanner](https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.Modernization)) and only want to get the Visio Web Drawing files report. This is possible by specifying the -m VdwOnly parameter:

```console
SharePoint.Visio.Scanner.exe -m VdwOnly -t <tenant> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.Visio.Scanner.exe -m VdwOnly -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

## I'm running SharePoint Online dedicated, is this different? ##
In SharePoint Online Dedicated one can have vanity url's like teams.contoso.com which implies that the tool cannot automatically determine the used url's and tenant admin center url. Using below command line switches you can specify the site url's to scan and the tenant admin center url. Note that the urls need to be separated by a comma.

```console
SharePoint.Visio.Scanner -a <tenantadminsite> -r <wildcard urls> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.Visio.Scanner -a https://contoso-admin.contoso.com 
                         -r "https://teams.contoso.com/sites/*,https://portal.contoso.com/*"
                         -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

```

## I want to use an Azure AD app to authenticate, how do I that?
This scanner, like all scanners built using the SharePoint Scanner framework, do support Azure AD App-Only:

```console
SharePoint.Visio.Scanner -t <tenant> -i <Azure App ID> -z <Azure AD Domain> 
                         -f "<Path to PFX file holding your certificate" -x <Password for the PFX file>
```

A real life sample:

```console
SharePoint.Visio.Scanner -t contoso -i e4108e9b-9865-44a9-c6e1-9003db04a775 -z contoso.onmicrosoft.com  
                         -f "C:\scanning\AzureADAppOnlyScanning.pfx" -x pwd
```

## I don't want to use app-only, can I use credentials? ##
The best option is to use app-only since that will ensure that the tool can read all site collections but you can also run the tool using credentials.

```console
SharePoint.Visio.Scanner -t <tenant> -u <user> -p <password>
```

A real life sample:

```console
SharePoint.Visio.Scanner -t contoso -c admin@contoso.onmicrosoft.com -p mypassword
```

# Complete list of command line switches for the SharePoint Online version #

```Console
SharePoint Visio scanner 1.0.0.0
Copyright (C) 2018 SharePoint PnP
==========================================================

See the PnP-Tools repo for more information at:
https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.Visio.Scanner

Let the tool figure out your urls (works only for SPO MT):
==========================================================
Using Azure AD app-only:
SharePoint.Visio.Scanner.exe -t <tenant> -i <your client id> -z <Azure AD domain> -f <PFX file> -x <PFX file password>
e.g. SharePoint.Visio.Scanner.exe -t contoso -i e5808e8b-6119-44a9-b9d8-9003db04a882 -z conto.onmicrosoft.com  -f
apponlycert.pfx -x pwd

Using app-only:
SharePoint.Visio.Scanner.exe -t <tenant> -i <your client id> -s <your client secret>
e.g. SharePoint.Visio.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
SharePoint.Visio.Scanner.exe -t <tenant> -u <your user id> -p <your user password>

e.g. SharePoint.Visio.Scanner.exe -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd

Specifying url to your sites and tenant admin (needed for SPO Dedicated):
=========================================================================
Using Azure AD app-only:
SharePoint.Visio.Scanner.exe -r <wildcard urls> -a <tenant admin site>  -i <your client id> -z <Azure AD domain> -f
<PFX file> -x <PFX file password>
e.g. SharePoint.Visio.Scanner.exe -r "https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*" -a
https://contoso-admin.contoso.com -i e5808e8b-6119-44a9-b9d8-9003db04a882 -z contoso.onmicrosoft.com  -f apponlycert.pfx
-x pwd

Using app-only:
SharePoint.Visio.Scanner.exe -r <wildcard urls> -a <tenant admin site> -i <your client id> -s <your client secret>
e.g. SharePoint.Visio.Scanner.exe -r "https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*" -a
https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
SharePoint.Visio.Scanner.exe -r <wildcard urls> -a <tenant admin site> -u <your user id> -p <your user password>
e.g. SharePoint.Visio.Scanner.exe -r "https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*" -a
https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd


  -m, --mode                      (Default: Full) Execution mode. Use following modes: full, VdwOnly. Omit or use full
                                  for a full scan

  -i, --clientid                  Client ID of the app-only principal used to scan your site collections

  -s, --clientsecret              Client Secret of the app-only principal used to scan your site collections

  -u, --user                      User id used to scan/enumerate your site collections

  -p, --password                  Password of the user used to scan/enumerate your site collections

  -z, --azuretenant               Azure tenant (e.g. contoso.microsoftonline.com)

  -f, --certificatepfx            Path + name of the pfx file holding the certificate to authenticate

  -x, --certificatepfxpassword    Password of the pfx file holding the certificate to authenticate

  -a, --tenantadminsite           Url to your tenant admin site (e.g. https://contoso-admin.contoso.com): only needed
                                  when your not using SPO MT

  -t, --tenant                    Tenant name, e.g. contoso when your sites are under
                                  https://contoso.sharepoint.com/sites. This is the recommended model for SharePoint
                                  Online MT as this way all site collections will be scanned

  -r, --urls                      List of (wildcard) urls (e.g.
                                  https://contoso.sharepoint.com/*,https://contoso-my.sharepoint.com,https://contoso-my.
                                  sharepoint.com/personal/*) that you want to get scanned

  -o, --includeod4b               (Default: False) Include OD4B sites in the scan

  -v, --csvfile                   CSV file name (e.g. input.csv) which contains the list of site collection urls that
                                  you want to scan

  -h, --threads                   (Default: 10) Number of parallel threads, maximum = 100

  -e, --separator                 (Default: ,) Separator used in output CSV files (e.g. ";")

  --help                          Display this help screen.
```

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-visioscanner" /> 


