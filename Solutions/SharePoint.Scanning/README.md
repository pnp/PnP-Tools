# SharePoint Scanner framework #

### Summary ###
This solution is a framework solution targeted to speed up the development of SharePoint scanners. This framework will handle all the "plumbing" and therefore allows you to focus on writing the actual scanning logic. 


### Applies to ###
-  SharePoint Online

### Solution ###
Solution | Author(s)
---------|----------
SharePoint.Scanning | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1 | March 7th 2018 | Using March PnP Sites Core version + improved reliability/output writing in sample scanner
1.0 | December 10nd 2017 | First main version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What will this framework do for you? #
This framework is starting basis for writing your own scanner and will provide you with:
- A multi-threaded SharePoint scanning approach
- A model that supports app-only and credential based authentication, including Azure AD app based authentication
- A model to filter/scope the sites to be scanned using either wildcard urls, search, a input CSV file or a custom mechanism of your choice
- A model for iterating over sub sites
- A model for storing scan results in a thread safe manner
- A model for persisting the scan results and errors as CSV files
- A sample scanner implementation that shows:
	- A basic site collection scoped scanner
	- A scanner that uses search to build the list of sites to scan
	- A scanner that iterates sub sites

## How to use the framework
The recommended approach is to include the SharePoint.Scanning.Framework project in your project, but you can also copy over the .dll file and reference that.

# Quick start guide #
## Using the scanning framework for SharePoint Online ##
Since this tool needs to be able to scan all site collections it's recommended to use an app-only principal with tenant scoped permissions for the scan. This approach will ensure the tool has access, if you use an account (e.g. your SharePoint tenant admin account) then the tool can only access the sites where this user also has access.

## Setting up an app-only principal with tenant permissions ###
The framework supports both SharePoint App-Only app principals or Azure AD-Only app principals. Below links contain the needed steps to setup these:
- [Granting access via Azure AD App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread)
- [Granting access using SharePoint App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azureacs)


## Typical use and default command line options provided by the framework ###
Below option is the typical usage of the framework: you specify your tenant name and the created client id and secret:

```console
myscanner.exe -t <tenant> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
myscanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

# Complete list of command line switches offered by the framework #

```Console
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
```

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-scannerframework" /> 


