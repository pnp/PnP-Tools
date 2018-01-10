# SharePoint permissive file scanner #

### Summary ###
Most of the SharePoint Online tenants handles the file open experience using the strict model. As a result, all files which can potentially cause harm (e.g. a html file having embedded script) are not executed in the browser but downloaded or shown as raw content (html preview in the modern user experience). If your tenant is configured using the permissive model then the file open experience will execute the file, for example a html file in a document library does get executed and page is shown in the browser. Checkout the [Migrating from permissive to strict tenant setting](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-permissivesetting) article on docs.microsoft.com to learn more.

Using this command line utility you can scan your tenant for files that would be impacted if your tenant switches from the permissive model to the strict model.

### Applies to ###
-  SharePoint Online

### Solution ###
Solution | Author(s)
---------|----------
SharePoint.PermissiveFile.Scanner | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1 | January 10th 2018 | Authenticode signed executable + using 2018 01 version of the scanning framework
1.0 | December 10th 2017 | First main version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What will this tool do for you? #
The main purpose of this tool is to give you a report of the files impacted when you switch your tenant from the permissive setting to strict.

You can easily verify if your tenant is in permissive mode using below [Office 365 PowerShell for SharePoint Online](https://technet.microsoft.com/en-us/library/fp161362.aspx):

```PowerShell
Connect-SPOService -url https://bertonline-admin.sharepoint.com
$tenant = get-spotenant
$tenant.PermissiveBrowserFileHandlingOverride
```

If this results in False then your tenant is using strict, if this is set to True then your tenant is using the permissive setting.

# Quick start guide #
## Download the tool ##
You can download the tool from here:
 - [Permissive file scanner for SharePoint Online](https://github.com/SharePoint/PnP-Tools/blob/master/Solutions/SharePoint.PermissiveFile.Scanner/Releases/SharePoint.PermissiveFile.Scanner%20v1.1.zip?raw=true)

Once you've downloaded the tool you have a folder containing the tool **SharePoint.PermissiveFile.Scanner.exe**. Start a (PowerShell) command prompt and navigate to that folder so that you can use the tool.

> Note:
> If you want to compile the tool yourself you'll also need to have the SharePoint.Scanning solution available as this tools depends on the SharePoint Scanner framework to compile.

## Using the scanner for SharePoint Online ##
Since this tool needs to be able to scan all site collections it's recommended to use an app-only principal with tenant scoped permissions for the scan. This approach will ensure the tool has access, if you use an account (e.g. your SharePoint tenant admin account) then the tool can only access the sites where this user also has access. You can either use a an Azure AD application or a SharePoint app principal:
 - [Granting access via Azure AD App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread)
 - [Granting access via SharePoint App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azureacs)


Once the preparation work is done, let's continue with doing a scan.

### Scanning SharePoint Online environment ###
Below option is the typical usage of the tool for most customers: you specify a mode, your tenant name and the created client id and secret:

```console
SharePoint.PermissiveFile.Scanner.exe -t <tenant> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.PermissiveFile.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

You'll see the following output during the run:

```console
=====================================================
Scanning is starting...26/11/2017 10:14:59
=====================================================
Processing site https://contoso.sharepoint.com/sites/130021...
Processing site https://contoso.sharepoint.com/sites/dev...
Processing site https://contoso.sharepoint.com/sites/bert2...
Processing site https://contoso.sharepoint.com/sites/devinfopath3...
Processing site https://contoso.sharepoint.com/sites/espctest2...
Processing site https://contoso.sharepoint.com/sites/clientsidepagesdevteamsite...
Thread: 8. Processed 6 of 12 site collections (50%). Process running for 0 days, 0 hours, 0 minutes and 7 seconds.
Processing site https://contoso.sharepoint.com/sites/bert1...
Thread: 6. Processed 7 of 12 site collections (58%). Process running for 0 days, 0 hours, 0 minutes and 7 seconds.
Processing site https://contoso.sharepoint.com/sites/teamsitedemo...
Thread: 10. Processed 8 of 12 site collections (67%). Process running for 0 days, 0 hours, 0 minutes and 8 seconds.
Processing site https://contoso.sharepoint.com/sites/devclr...
Thread: 5. Processed 9 of 12 site collections (75%). Process running for 0 days, 0 hours, 0 minutes and 9 seconds.
Processing site https://contoso.sharepoint.com/sites/130020...
Thread: 9. Processed 10 of 12 site collections (83%). Process running for 0 days, 0 hours, 0 minutes and 9 seconds.
Processing site https://contoso.sharepoint.com/sites/demogroup1...
Thread: 10. Processed 11 of 12 site collections (92%). Process running for 0 days, 0 hours, 0 minutes and 10 seconds.
Thread: 8. Processed 11 of 12 site collections (92%). Process running for 0 days, 0 hours, 0 minutes and 11 seconds.
Thread: 6. Processed 11 of 12 site collections (92%). Process running for 0 days, 0 hours, 0 minutes and 11 seconds.
Thread: 5. Processed 11 of 12 site collections (92%). Process running for 0 days, 0 hours, 0 minutes and 11 seconds.
Thread: 7. Processed 11 of 12 site collections (92%). Process running for 0 days, 0 hours, 0 minutes and 13 seconds.
Processing site https://bertonline.sharepoint.com/sites/dev2...
Thread: 9. Processed 12 of 12 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 13 seconds.
Thread: 7. Processed 12 of 12 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 14 seconds.
=====================================================
Scanning is done...now dump the results to a CSV file
=====================================================
Outputting errors to 636472880994285644\errors.csv
Outputting information over the done scan to 636472880994285644\ScannerSummary.csv
Outputting scan results to 636472880994285644\PermissiveScanResults.csv
=====================================================
All done. Took 00:00:15.0324218 for 12 sites
=====================================================
```

After the run you'll find a new sub folder (e.g. 636472880994285644) which contains the following:

Report | Content
---------|----------
**PermissiveScanResults.csv** | Lists of files which are identified as not working in strict mode. For html/htm files the scanner wil give additional information
**Error.csv** | If the scan tool encountered errors then these are logged in this file.
**ScannerSummary.csv** | Logs the number of scanned site collections, webs and list. It will also contain information on scan duration and used scanner version.

# Report details
## Understanding the PermissiveScanResults.csv file
This report contains the following columns:

Column | Description
---------|----------
**Site Collection Url** | Url of the scanned site collection.
**Site Url** | Url of the scanned site.
**File extension** | Extension of the scanned file.
**File name** | The url to the scanned file.
**Link count** | For html/html files only: shows the number of links in this file.
**Embedded html link count** | For html/html files only: shows the number of local html/htm links in the file
**Script tag count** | For html/html files only: shows the number of script tags in the file.
**Site admins and owners** | Email addresses of the site collection admins and site owners.

### Key takeaways from this report
Load the PermissiveScanResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you a list of scanned files which will not execute anymore whenever the tenant is switched to strict
**FileExtension = html** | Will give you all the html files, the most common impacted files when moving to strict.
**FileExtension = html AND ScriptTagCount > 0** | Will give you all the html files having script tags (note that the scanner does not see inline JavaScript), which you can use to identify files without script as these could potentially be migrated differently (so no rename to aspx).

# Advanced topics #

## I'm running SharePoint Online dedicated, is this different? ##
In SharePoint Online Dedicated one can have vanity url's like teams.contoso.com which implies that the tool cannot automatically determine the used url's and tenant admin center url. Using below command line switches you can specify the site url's to scan and the tenant admin center url. Note that the urls need to be separated by a comma.

```console
SharePoint.PermssiveFile.Scanner -a <tenantadminsite> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.PermssiveFile.Scanner -a https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 
                                 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

## I want to use an Azure AD app to authenticate, how do I that?
This scanner, like all scanners built using the SharePoint Scanner framework, do support Azure AD App-Only:

```console
SharePoint.PermssiveFile.Scanner -t <tenant> -i <Azure App ID> -z <Azure AD Domain> 
                                 -f "<Path to PFX file holding your certificate" -x <Password for the PFX file>
```

A real life sample:

```console
SharePoint.PermssiveFile.Scanner -t contoso -i e4108e9b-9865-44a9-c6e1-9003db04a775 -z contoso.onmicrosoft.com  
                                 -f "C:\scanning\AzureADAppOnlyScanning.pfx" -x pwd
```

## I don't want to use app-only, can I use credentials? ##
The best option is to use app-only since that will ensure that the tool can read all site collections but you can also run the tool using credentials.

```console
SharePoint.PermssiveFile.Scanner -t <tenant> -u <user> -p <password>
```

A real life sample:

```console
SharePoint.PermssiveFile.Scanner -t contoso -c admin@contoso.onmicrosoft.com -p mypassword
```

# Complete list of command line switches for the SharePoint Online version #

```Console
SharePoint PnP Permissive scanner 1.0.0.0
Copyright (C) 2017 SharePoint PnP
==========================================================

See the PnP-Tools repo for more information at:
https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.PermissiveFile.Scanner

Let the tool figure out your urls (works only for SPO MT):
==========================================================
Using app-only:
SharePoint.PermissiveFile.Scanner.exe -t <tenant> -i <your client id> -s <your client secret>
e.g. SharePoint.PermissiveFile.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
SharePoint.PermissiveFile.Scanner.exe -t <tenant> -u <your user id> -p <your user password>

e.g. SharePoint.PermissiveFile.Scanner.exe -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd

Specifying your urls to scan + url to tenant admin (needed for SPO Dedicated):
==============================================================================
Using app-only:
SharePoint.PermissiveFile.Scanner.exe -r <urls> -a <tenant admin site> -i <your client id> -s <your client secret>
e.g. SharePoint.PermissiveFile.Scanner.exe -r https://team.contoso.com/*,https://mysites.contoso.com/* -a
https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
SharePoint.PermissiveFile.Scanner.exe -r <urls> -a <tenant admin site> -u <your user id> -p <your user password>
e.g. SharePoint.PermissiveFile.Scanner.exe -r https://team.contoso.com/*,https://mysites.contoso.com/* -a
https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd


  -l, --filetypes                 List of additional (besides html and html) file types to scan (e.g. zip,ica) that you
                                  want to get scanned

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

  -h, --threads                   (Default: 10) Number of parallel threads, maximum = 100

  -e, --separator                 (Default: ,) Separator used in output CSV files (e.g. ";")

  --help                          Display this help screen.
```

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-permissivefilescanner" /> 


