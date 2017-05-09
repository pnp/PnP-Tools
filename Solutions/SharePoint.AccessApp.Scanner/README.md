# SharePoint Access App scanner #

### Summary ###
Using this command line utility you can scan your tenant for the presence of Access Apps. Access Apps are deprecated and in the future Microsoft will in a first phase prevent you from adding new Access Apps and in the final phase we'll prevent you running your Access Apps. With the data obtained by this scanner you can plan for the needed remediation work.

If you want to learn more about the Access App deprecation then please checkout:
- [Access Services in SharePoint Roadmap](https://support.office.com/en-us/article/Access-Services-in-SharePoint-Roadmap-497fd86b-e982-43c4-8318-81e6d3e711e8?ui=en-US&rs=en-US&ad=US)
- [Introduction to Microsoft PowerApps for Access web apps developers](https://www.microsoft.com/en-us/download/details.aspx?id=54942)
- [Enable and disable Access Apps in your tenant](https://support.office.com/en-us/article/Enable-and-disable-Access-apps-in-your-organization-92e0f958-4b53-411a-8499-52acec00413e?ui=en-US&rs=en-US&ad=US)

### Applies to ###
-  SharePoint Online

### Solution ###
Solution | Author(s)
---------|----------
SharePoint.AccessApp.Scanner | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
0.1 | May 9th 2017 | First beta version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What will this tool do for you? #
The main purpose of this tool is to give you a report of Access 2010 and 2013 Apps that are present in your tenant. Access Apps are deprecated and in the future Microsoft will in a first phase prevent you from adding new Access Apps and in the final phase we'll prevent you running your Access Apps. With the data obtained by this scanner you can plan for the needed remediation work.

# Quick start guide #
## Download the tool ##
You can download the tool from here:
 - [Access App scanner for SharePoint Online](https://github.com/SharePoint/PnP-Tools/blob/master/Solutions/SharePoint.AccessApp.Scanner/Releases/Access%20App%20scanner%20for%20SharePoint%20Online%20v0.1.zip?raw=true)

Once you've downloaded the tool (or alternatively you can also compile it yourself using Visual Studio) you have a folder containing the tool **AccessAppScanner.exe**. Start a (PowerShell) command prompt and navigate to that folder so that you can use the tool.

## Using the scanner for SharePoint Online ##
Since this tool needs to be able to scan all site collections it's recommended to use an app-only principal with tenant scoped permissions for the scan. This approach will ensure the tool has access, if you use an account (e.g. your SharePoint tenant admin account) then the tool can only access the sites where this user also has access.

### Setting up an app-only principal with tenant permissions ###
Below steps show how to setup the app-only principal with the needed permissions. Note that you'll need to he a SharePoint tenant administrator to complete these steps.

#### Create a new principal ####
Navigate to a site in your tenant (e.g. https://contoso.sharepoint.com) and then call the **appregnew.aspx** page (e.g. https://contoso.sharepoint.com/_layouts/15/appregnew.aspx). In this page click on the **Generate** button to generate a client id and client secret and fill the remaining information like shown in the screen-shot below.

![create app-only principal](http://i.imgur.com/zvxkz5c.png)

>**Important**
> - Store the retrieved information (client id and client secret) since you'll need this in the next step!

#### Grant permissions to the created principal ####
Next step is granting permissions to the newly created principal. Since we're granting tenant scoped permissions this granting can only be done via the **appinv.aspx** page on the tenant administration site. You can reach this site via https://contoso-admin.sharepoint.com/_layouts/15/appinv.aspx. Once the page is loaded add your client id and look up the created principal:

![Grant permissions to app-only principal](http://i.imgur.com/cMbU9xX.png)

In order to grant permissions you'll need to provide the permission XML that describes the needed permissions. Since the Access App scanner needs to be able to access all sites + also uses search with app-only it **requires** below permissions:

```xml
<AppPermissionRequests AllowAppOnlyPolicy="true">
  <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```

When you click on **Create** you'll be presented with a permission consent dialog. Press **Trust It** to grant the permissions:

![Trust principal](http://i.imgur.com/6ogrxxV.png)

>**Important**
> - Please safeguard the created client id/secret combination as would it be your administrator account. Using this client id/secret one **can read/update all data in your SharePoint Online environment**!

With the preparation work done let's continue with doing a scan. 

### Scanning SharePoint Online environment ###
Below option is the typical usage of the tool for most customers: you specify a mode, your tenant name and the created client id and secret:

```console
accessappscanner -t <tenant> -c <clientid> -s <clientsecret>
```

A real life sample:

```console
accessappscanner -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

You'll see the following output during the run:

```console
=====================================================
Scanning is starting...9/05/2017 12:07:28
=====================================================
Building a list of site collections...
Processing site https://bertonline.sharepoint.com/sites/templatePROJECTSITE0...
Thread: 9. Processed 3 of 203 site collections (1%). Process running for 0 days, 0 hours, 0 minutes and 15 seconds.
Processing site https://bertonline.sharepoint.com/sites/pnppartnerpack3...
Thread: 9. Processed 5 of 203 site collections (2%). Process running for 0 days, 0 hours, 0 minutes and 16 seconds.
Processing site https://bertonline.sharepoint.com/sites/130047...
Thread: 12. Processed 7 of 203 site collections (3%). Process running for 0 days, 0 hours, 0 minutes and 17 seconds.
Processing site https://bertonline.sharepoint.com/portals/hub...
Processing site https://bertonline.sharepoint.com/sites/130047/blog...
Thread: 9. Processed 7 of 203 site collections (3%). Process running for 0 days, 0 hours, 0 minutes and 17 seconds.
Thread: 12. Processed 8 of 203 site collections (4%). Process running for 0 days, 0 hours, 0 minutes and 17 seconds.
Processing site https://bertonline.sharepoint.com/sites/20140041...
Processing site https://bertonline.sharepoint.com/sites/130064...
Processing site https://bertonline.sharepoint.com/sites/20140031...
Processing site https://bertonline.sharepoint.com/sites/PnP-Partner-Pack-Infrastructure...
Thread: 9. Processed 11 of 203 site collections (5%). Process running for 0 days, 0 hours, 0 minutes and 19 seconds.
Thread: 12. Processed 12 of 203 site collections (6%). Process running for 0 days, 0 hours, 0 minutes and 19 seconds.
Thread: 14. Processed 12 of 203 site collections (6%). Process running for 0 days, 0 hours, 0 minutes and 19 seconds.
Processing site https://bertonline.sharepoint.com/sites/130064/blog...
Processing site https://bertonline.sharepoint.com/sites/20140042...
Thread: 12. Processed 15 of 203 site collections (7%). Process running for 0 days, 0 hours, 0 minutes and 21 seconds.
Processing site https://bertonline.sharepoint.com/sites/20140018...
Thread: 10. Processed 16 of 203 site collections (8%). Process running for 0 days, 0 hours, 0 minutes and 21 seconds.
Processing site https://bertonline.sharepoint.com/sites/20140050...
Thread: 9. Processed 17 of 203 site collections (8%). Process running for 0 days, 0 hours, 0 minutes and 21 seconds.
Thread: 14. Processed 19 of 203 site collections (9%). Process running for 0 days, 0 hours, 0 minutes and 22 seconds.
Processing site https://bertonline.sharepoint.com/sites/130073...
Thread: 16. Processed 20 of 203 site collections (10%). Process running for 0 days, 0 hours, 0 minutes and 22 seconds.
Processing site https://bertonline.sharepoint.com/sites/20140028...
Processing site https://bertonline.sharepoint.com...
...
Thread: 10. Processed 203 of 203 site collections (100%). Process running for 0 days, 0 hours, 1 minutes and 42 seconds.
Processing site https://bertonline.sharepoint.com/sites/130034/blog...
Thread: 10. Processed 203 of 203 site collections (100%). Process running for 0 days, 0 hours, 1 minutes and 42 seconds.
=====================================================
Scanning is done...now dump the results to a CSV file
=====================================================
Outputting Access App scan to 636299284487379655\AccessApps.csv
Outputting information over the done scan to 636299284487379655\ScannerSummary.csv
=====================================================
All done. Took 00:01:42.5555461 for 203 sites
=====================================================
```

After the run you'll find a new sub folder (e.g. 636299284487379655) which contains the following:

Report | Content
---------|----------
**AccessApps.csv** | Lists all the Access Apps that were detected by the scanner.
**Error.csv** | If the scan tool encountered errors then these are logged in this file.
**ScannerSummary.csv** | Logs the number of scanned site collections and webs. It will also contain information on scan duration and used scanner version.

# Report details
## Understanding the AccessApps.csv file
This report contains the following columns:

Column | Description
---------|----------
**Site Url** | Url of the Access App site.
**Parent Site Url** | Url of the site where the Access App was installed.
**Site Collection Url** | Url of the scanned site collection.
**Web Title** | Title of the Access App.
**Web Template** | Web template of the Access App site.
**App Instance Status** | Status of the Access App (only relevant to Access 2013 Apps).
**App Instance Id** | Id (guid) of the Access App (only relevant to Access 2013 Apps).
**Web Id** | Id (guid) of the Access App site.
**ViewsRecent** | Recent (last 14 days) views for the site collection hosting this Access App
**ViewsRecentUnique** | Recent (last 14 days) unique views for the site collection hosting this Access App
**ViewsLifeTime** | Lifetime views for the site collection hosting this Access App (only 500 first search results are processed)
**ViewsLifeTimeUnique** | Lifetime unique views for the site collection hosting this Access App (only 500 first search results are processed)

### Key takeaways from this report
Load the AccessApps.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you a list of all the found Access Apps. If the amount of found Access Apps is zero or the found Access Apps are not relevant anymore then it's strongly recommended to [disable Access Apps for your tenant](https://support.office.com/en-us/article/Enable-and-disable-Access-apps-in-your-organization-92e0f958-4b53-411a-8499-52acec00413e?ui=en-US&rs=en-US&ad=US). 
**Web Template = ACCSVC#0** | Will give you all the Access 2013 Apps, meaning the Access Apps created using Access 2013+ where the data is living in SQL Azure. 
**Web Template = ACCSRV#0** | Will give you all the Access 2010 Apps, meaning the Access Apps created using Access 2010 where the data is living in SharePoint 
lists
**ViewsRecent = 0** | All the Access Apps of which the hosting site collection has not been visited in the last 14 days

# Advanced topics #

## How can I decrease the time it takes to scan the environment?
If you've a very large environment the scan will take a long time since it will have to process all sites in your tenant. Use below tips to speed up the scanning:
- Run the scan on an Azure VM hosted in the same region as your SharePoint Online tenant, this will optimize the network traffic between the scanner and SharePoint Online
- Use the -r (see later in the chapter) parameter to for example only scan the sites that start with A,B,C,D on one machine, E,F,G,H on the next one,...This way you can have parallel scans running each handling a subset of site collections. This however means you'll need to manually combine the resulting CSV files.
- Increase the number of threads, default is 10 but increasing to 20 can help on large environments. It will however not double the performance :-)

## I'm running SharePoint Online dedicated, is this different? ##
In SharePoint Online Dedicated one can have vanity url's like teams.contoso.com which implies that the tool cannot automatically determine the used url's and tenant admin center url. Using below command line switches you can specify the site url's to scan and the tenant admin center url. Note that the urls need to be separated by a comma.

```console
accessappscanner -r <urls> -a <tenantadminsite> -c <clientid> -s <clientsecret>
```

A real life sample:

```console
accessappscanner -r https://team.contoso.com/*,https://mysites.contoso.com/* 
                 -a https://contoso-admin.contoso.com -c 7a5c1615-997a-4059-a784-db2245ec7cc1 
                 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

## I don't want to use app-only, can I use credentials? ##
The best option is to use app-only since that will ensure that the tool can read all site collections but you can also run the tool using credentials.

```console
accessappscanner -t <tenant> -u <user> -p <password>
```

A real life sample:

```console
accessappscanner -t contoso -c admin@contoso.onmicrosoft.com -p mypassword
```

## I only want to scan a few sites, can I do that? ##
Using the urls command line switch you can control which sites are scanned. You can specify one or more url's which can have a wild card. Samples of valid url's are:
 - https://contoso.sharepoint.com/*
 - https://contoso.sharepoint.com/sites/mysite
 - https://contoso-my.sharepoint.com/personal/*

To specify the url's you can use the -r parameter as shown below:

```console
accessappscanner -r https://contoso.sharepoint.com/*,https://contoso.sharepoint.com/sites/mysite,https://contoso-my.sharepoint.com/personal/* 
-c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```


# Complete list of command line switches for the SharePoint Online version #

```Console
SharePoint AccessApp Scanner tool 0.1.0.0
Copyright (C) 2017 SharePoint PnP
==========================================================

See the PnP-Tools repo for more information at:
https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.AccessApp.Scanner

Let the tool figure out your urls (works only for SPO MT):
==========================================================
Using app-only:
accessappscanner.exe -t <tenant> -c <your client id> -s <your client secret>

e.g. accessappscanner.exe -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
accessappscanner.exe -t <tenant> -u <your user id> -p <your user password>

e.g. accessappscanner.exe -m scan -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd

Specifying your urls to scan + url to tenant admin (needed for SPO Dedicated):
==============================================================================
Using app-only:
accessappscanner.exe -r <urls> -a <tenant admin site> -c <your client id> -s <your client secret>
e.g. accessappscanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a
https://contoso-admin.contoso.com -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
accessappscanner.exe -r <urls> -a <tenant admin site> -u <your user id> -p <your user password>
e.g. accessappscanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a
https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd

  -m, --mode               (Default: Scan) Execution mode. Omit or use scan for a full scan

  -t, --tenant             Tenant name, e.g. contoso when your sites are under https://contoso.sharepoint.com/sites.
                           This is the recommended model for SharePoint Online MT as this way all site collections will
                           be scanned

  -r, --urls               List of (wildcard) urls (e.g.
                           https://contoso.sharepoint.com/*,https://contoso-my.sharepoint.com,https://contoso-my.sharepo
                           int.com/personal/*) that you want to get scanned. When you specify the --tenant optoin then
                           this parameter is ignored

  -c, --clientid           Client ID of the app-only principal used to scan your site collections

  -s, --clientsecret       Client Secret of the app-only principal used to scan your site collections

  -u, --user               User id used to scan/enumerate your site collections

  -p, --password           Password of the user used to scan/enumerate your site collections

  -a, --tenantadminsite    Url to your tenant admin site (e.g. https://contoso-admin.contoso.com): only needed when
                           your not using SPO MT

  -x, --excludeod4b        (Default: False) Exclude OD4B sites from the scan

  -e, --separator          (Default: ,) Separator used in output CSV files (e.g. ";")

  -h, --threads            (Default: 10) Number of parallel threads, maximum = 100

  --help                   Display this help screen.
```

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-accessappscanner" /> 


