# SharePoint "Modern" user interface experience scanner #

### Summary ###
Using this command line utility you can scan your tenant for compatibility with the SharePoint Online "modern" user interface experience. The scanner will give you a detailed view of sites that are not using "modern" pages, lists and libraries that are rendered in "classic" mode and finally it will also give you a report of customizations which are simply ignored in the "modern" user interface. 

If you want to learn more about the "modern" experiences then please checkout our [MSDN guidance](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations):
- [Customizing "modern" lists and libraries](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-lists-and-libraries)
- [Customizing "modern" pages](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-pages)

If you rather watch a video to learn more about this tool then [please consult the PnP Webcast](https://dev.office.com/blogs/sharepoint-modern-user-interface-experience-scanner) on this topic.

### Applies to ###
-  SharePoint Online

### Solution ###
Solution | Author(s)
---------|----------
SharePoint.UIExperience.Scanner | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0 | May 2nd 2017 | First main version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What will this tool do for you? #
The main purpose of this tool is to give you a report of sites, lists, libraries and deployed customizations which will not work as expected when using the SharePoint "modern" user interface experiences. You can use the generated reports to:
- Which sites do not have the ability to create "modern" pages
- Which lists are not presenting themselves using the "modern" user interface and what's causing that: there are many reasons why a list is falling back to showing the "classic" user interface, all of which will be listed in the report.
- Get a better understanding of which customizations in your tenant are not compatible with the "modern" experiences

Using the reports you can streamline the "modern" experience in your tenant: you can increase the amount of sites, libraries and lists that work fine using "modern", but the reports also give you the data that allows you to disable "modern" for a subset of sites which are not yet ready for it.

# Quick start guide #
## Download the tool ##
You can download the tool from here:
 - [UIExperience scanner for SharePoint Online](https://github.com/SharePoint/PnP-Tools/blob/master/Solutions/SharePoint.UIExperience.Scanner/Releases/UI%20Experience%20scanner%20for%20SharePoint%20Online%20v1.0.zip?raw=true)

Once you've downloaded the tool (or alternatively you can also compile it yourself using Visual Studio) you have a folder containing the tool **UIExperienceScanner.exe**. Start a (PowerShell) command prompt and navigate to that folder so that you can use the tool.

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

In order to grant permissions you'll need to provide the permission XML that describes the needed permissions. Since the UI experience scanner needs to be able to access all sites + also uses search with app-only it **requires** below permissions:

```xml
<AppPermissionRequests AllowAppOnlyPolicy="true">
  <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```

When you click on **Create** you'll be presented with a permission consent dialog. Press **Trust It** to grant the permissions:

![Trust principal](http://i.imgur.com/6ogrxxV.png)

>**Important**
> - Please safeguard the created client id/secret combination as would it be your administrator account. Using this client id/secret one **can read/update all data in your SharePoint Online environment**!

With the preparation work done let's continue with doing a scan. Note that depending on the size of your environment a scan can take a long time as it will have to process each library in your tenant. See the options described in the "Advanced topics" section to limit scanning to a subset of sites. 

### Scanning SharePoint Online environment ###
Below option is the typical usage of the tool for most customers: you specify a mode, your tenant name and the created client id and secret:

```console
uiexperiencescanner -t <tenant> -c <clientid> -s <clientsecret>
```

A real life sample:

```console
uiexperiencescanner -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

You'll see the following output during the run:

```console
=====================================================
Scanning is starting...21/03/2017 11:28:25
=====================================================
Building a list of site collections...
Processing site https://contoso.sharepoint.com/sites/pnppartnerpack3...
Processing site https://contoso.sharepoint.com/sites/pnpnoscript...
Page compatability... https://contoso.sharepoint.com/sites/pnpnoscript
Page compatability... https://contoso.sharepoint.com/sites/pnppartnerpack3
List compatability... https://contoso.sharepoint.com/sites/pnpnoscript
List compatability... https://contoso.sharepoint.com/sites/pnppartnerpack3
Processing site https://contoso.sharepoint.com/sites/dev3...
Processing site https://contoso.sharepoint.com/sites/dev5...
Page compatability... https://contoso.sharepoint.com/sites/dev3
List compatability... https://contoso.sharepoint.com/sites/dev3
Page compatability... https://contoso.sharepoint.com/sites/dev5
List compatability... https://contoso.sharepoint.com/sites/dev5
Processing site https://contoso.sharepoint.com/sites/20140021...
AlternateCSS... https://contoso.sharepoint.com/sites/pnpnoscript
Master page... https://contoso.sharepoint.com/sites/pnpnoscript
Processing site https://contoso.sharepoint.com/sites/spc...
Page compatability... https://contoso.sharepoint.com/sites/20140021
List compatability... https://contoso.sharepoint.com/sites/20140021
Custom actions... https://contoso.sharepoint.com/sites/pnpnoscript
Page compatability... https://contoso.sharepoint.com/sites/spc
List compatability... https://contoso.sharepoint.com/sites/spc
Processing site https://contoso.sharepoint.com/sites/130043...
Thread: 10. Processed 8 of 195 site collections (4%). Process running for 0 days, 0 hours, 0 minutes and 14 seconds.
AlternateCSS... https://contoso.sharepoint.com/sites/pnppartnerpack3
...
Custom actions... https://contoso.sharepoint.com/sites/publishing1/de-de
Thread: 10. Processed 195 of 195 site collections (100%). Process running for 0 days, 0 hours, 12 minutes and 5 seconds.
=====================================================
Scanning is done...now dump the results to a CSV file
=====================================================
Outputting list scan results to 636256925056008591\ModernListBlocked.csv
Outputting list custom action scan results to 636256925056008591\IgnoredCustomizations_CustomAction.csv
Outputting alternate css scan to 636256925056008591\IgnoredCustomizations_AlternateCSS.csv
Outputting master page scan to 636256925056008591\IgnoredCustomizations_MasterPage.csv
Outputting ignored customization results to 636256925056008591\IgnoredCustomizations.csv
Outputting modern page blocked scan to 636256925056008591\ModernPagesBlocked.csv
Outputting errors to 636256925056008591\errors.csv
=====================================================
All done. Took 00:12:17.6794971 for 195 sites
=====================================================
```

After the run you'll find a new sub folder (e.g. 636072073126632445) which contains the following:

Report | Content
---------|----------
**ModernPagesBlocked.csv** | Lists all sites which are not able to use "modern" pages. "Modern" pages can be blocked by disabling the "modern" page feature as explained in [MSDN](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-pages#site-level-configuration).
**ModernListBlocked.csv** | Contains all lists which are not using the "modern" experience. There are many reasons why a list is not using the "modern" experience as explained in this [MSDN article](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-lists-and-libraries#when-does-the-built-in-auto-detect-automatically-switch-rendering-back-to-classic).
**IgnoredCustomizations.csv** | Provides an overview of all sites that have customizations which are ignored on "modern" experiences. This report summarizes the details found in the next set of reports. Typically all customizations that affect the page like custom CSS, custom master pages and embedded JavaScript are simply ignored on "modern" pages. Use this report to get a better understanding of which of your customizations require review work in order to be compatible with the "modern" experience. Please consult [our MSDN articles](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations) to better understand what customization options are possible when using "modern" user interfaces.
**IgnoredCustomizations_MasterPage.csv** | This file contains a list of all sites using a non out of the box master page.
**IgnoredCustomizations_CustomAction.csv** | Contains a list of all user custom actions which are ignored on on "modern" experiences.
**IgnoredCustomizations_AlternateCSS.csv** | Listing all the sites that have the `AlternateCssUrl` property set.
**Error.csv** | If the scan tool encountered errors then these are logged in this file.
**ScannerSummary.csv** | Logs the number of scanned site collections, webs and list. It will also contain information on scan duration and used scanner version.

# Report details
## Understanding the ModernPagesBlocked.csv file
This report contains the following columns:

Column | Description
---------|----------
**URL** | Url of the scanned object (site in this case).
**Site Url** | Url of the scanned site.
**Site Collection Url** | Url of the scanned site collection.
**Web Template** | Web template of the scanned site.
**Modern page feature was enabled** | TRUE is the "modern" page feature was enabled by SharePoint Online or manually. Use this column to detect for which sites the feature was turned off afterwards. See [MSDN](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-pages#why-is-my-site-not-having-the-modern-pages-functionality) for more details on when the "modern" page feature should have been turned on.
**Blocked via disabled modern page web feature** | TRUE if the "modern" page experience was blocked because the modern page feature (B6917CB1-93A0-4B97-A84D-7CF49975D4EC) was disabled.

### Key takeaways from this report
Load the ModernPagesBlocked.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you a list of all sites which do not have the option to add "modern" pages. We enabled "modern" pages by default on all sites based on the GROUP#0 template and on almost all STS#0 based sites. if an STS#0 site (so a "classic" team site) did have a high number of webpart pages or wiki pages then the "modern" page feature was not automatically enabled. The same applies to STS#0 sites which had the publishing feature enabled, these are not having the "modern" pages feature activated. 
**Modern page feature was enabled = TRUE** | Will give you all the sites where the "modern" pages feature was deliberately disabled. You can use this data to re-enable the "modern" page feature on those sites.

## Understanding the ModernListBlocked.csv file
This report contains the following columns:

Column | Description
---------|----------
**URL** | Url of the scanned object (list form page url in this case).
**Site Url** | Url of the scanned site.
**Site Collection Url** | Url of the scanned site collection.
**List Title** | Title of the list. 
**Only blocked by OOB reaons** | TRUE if the list is **only** blocked due to reasons which you as customer cannot influence, being blocked due to managed metadata navigation, unsupported list base template, unsupported list view type or unsupported field type being shown. Note that you can use the the **-o (-excludelistsonlyblockedbyoobreaons) parameter** to skip logging lists which are only blocked due to these reasons
**Blocked at site level** | TRUE if the list is blocked because the **site** scoped feature (E3540C7D-6BEA-403C-A224-1A12EAFEE4C4) was enabled.
**Blocked at web level** | TRUE if the list is blocked because the **web** scoped feature (52E14B6F-B1BB-4969-B89B-C4FAA56745EF) was enabled.
**Blocked at list level** | TRUE if the user changed the list experience setting to "classic experience".
**List page render type** | The value of the PageRenderType property as explained in [MSDN](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-lists-and-libraries#programmatically-detecting-if-your-librarylist-will-be-shown-using-modern-or-classic).
**List experience** | The set list experience setting: auto (default), modern or classic. 
**Blocked by not being able to load page** | TRUE if the page associated with the list default view could not be loaded.
**Blocked by not being able to load page exception** | The error that was triggered when the page could not be loaded.
**Blocked by managed metadata navigation** | TRUE if the list is blocked because the web scoped metadata navigation (7201d6a4-a5d3-49a1-8c19-19c4bac6e668) feature was enabled.
**Blocked by view type** | TRUE if the list default view is using a view type which cannot be shown in "modern".
**View type** | The used view type that is not working in "modern".
**Blocked by list base template** | TRUE if the list is blocked because it's based upon a list type which can't be shown in "modern".
**List base template** | Base template of the list. This base template can't be shown in "modern".
**Blocked by zero or multiple web parts** | TRUE if the the list default view page is having more than 1 web part.
**Blocked by JSLink** | TRUE if the the XSLT List View web part, which is showing the list data, has the JSLink property set.
**JSLink** | The JSLink property set on the XSLT List View web part.
**Blocked by XslLink** | TRUE if the XSLT List View web part, which is showing the list data, has the XslLink property set.
**XslLink** | The XslLink property set on the XSLT List View web part. 
**Blocked by Xsl** | TRUE if the XSLT List View web part, which is showing the list data, has the Xsl property set.
**Blocked by JSLink field** | TRUE if a list view field has the JSLink property set.
**JSLink fields** | Collection of fields with JSLink set.
**Blocked by business data field** | TRUE if a list view field is of the type "business data".
**Business data fields** | Collection of fields of type "business data".
**Blocked by task outcome field** | TRUE if a list view field is of the type "task outcome".
**Task outcome fields** | Collection of fields of type "task outcome".
**Blocked by publishingField** | TRUE if a list view field is of the type "publishing".
**Publishing fields** | Collection of fields of type "publishing".
**Blocked by geo location field** | TRUE if a list view field is of the type "geo location".
**Geo location fields** | Collection of fields of type "geo location".
**Blocked by list custom action** | TRUE if the list is having incompatible list scoped user custom actions. Incompatible user custom actions are having ScriptLink as location.
**List custom actions** | Collection of offending list custom action names.

### Key takeaways from this report
Load the ModernListBlocked.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you **all** the lists for which the default view page will not present itself using the "modern" UI. The number of lists can be very high because not all lists are designed to use the "modern" user interface. This data is however useful to assess the impact of upcoming "modern" changes: e.g. this report will tell you that x lists are not showing in modern because feature y is not available...if you know that feature y will become available in the future you can assess how many additional lists will then be able to present themselves using the "modern" user interface.
**Blocked at site level = TRUE or Blocked at web level = TRUE** | Using this filter you'll get all the sites where the "modern" ui blocking site/web scoped feature was enabled. It might be that this was done because certain sites were not yet ready for "modern", using this data you can disable the "modern" ui blocking feature again.
**Blocked at list level = TRUE** | End users do have the option to turn on/off the "modern" experience per list and using this filter you'll learn where that happened. The column "list experience" will show which setting was applied.
**Only blocked by OOB reaons = FALSE** | Will give you all the lists which are not rendering in "modern" because of a reason you can influence. If you want to improve the "modern" user interface compatibility of your SharePoint Online lists then these are the lists you can work with. 
**Only blocked by OOB reaons = FALSE and (Blocked by JSLink = TRUE or Blocked by XslLink = TRUE or Blocked by Xsl = TRUE or Blocked by JSLink field = TRUE)** | These are all the lists which are not showing in modern because their rendering is impacted by the existence of custom JavaScript or XSL. You could remove the references to these JavaScript and XSL files and go back to the out of the box way of list rendering or alternatively you'll need to re-implement the custom rendering [if Microsoft would release a feature](http://aka.ms/spfx-roadmap) that allows you to do so.
**Only blocked by OOB reaons = FALSE and Blocked by list custom action = TRUE** | If a list custom action is embedding JavaScript then that will prevent rendering of the "modern" user interface. You can remove this list custom action or alternatively update the user custom actions accordingly [if Microsoft would release a feature ](http://aka.ms/spfx-roadmap)that allows you to do.
**Business data fields = true or Blocked by task outcome field = true or Blocked by geo location field = true or Blocked by publishingField = true** | Although you can't fix the rendering of these fields you do have the option to remove them from the list default view which will fix the list "modern" rendering.

## Understanding the IgnoredCustomizations.csv file
This report contains the following columns:

Column | Description
---------|----------
**URL** | Url of the scanned object (site in this case).
**Site Url** | Url of the scanned site.
**Site Collection Url** | Url of the scanned site collection.
**Ignored MasterPage** | The site was using a non out of the box master page.
**Ignored Custom MasterPage** | The site was using a non out of the box custom master page.
**Ignored AlternateCSS** | The site was using a custom CSS script.
**Ignored Custom Action** | TRUE if the site was using an incompatible user custom action.

### Key takeaways from this report
Load the IgnoredCustomizations.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Lists all the sites where there's either a master page, alternate CSS or user custom action which is ignored on "modern" pages. Verifying if the ignored customizations are critical for the site's functionality is important as it allows you to either drop the not important customizations or alternatively force a site to render in "classic" if the customizations are business critical

## Understanding the IgnoredCustomizations_MasterPage.csv file
This report contains the following columns:

Column | Description
---------|----------
**URL** | Url of the scanned object (site in this case).
**Site Url** | Url of the scanned site.
**Site Collection Url** | Url of the scanned site collection.
**Web Template** | Web template of the scanned site.
**MasterPage** | Used non out of the box master page. Although a custom master page is not used for "modern" pages the custom master page still applies to the "classic" pages on your site.
**Custom MasterPage** | Used non out of the box custom master page. Although a custom master page is not used for "modern" pages the custom master page still applies to the "classic" pages on your site.

### Key takeaways from this report
Load the IgnoredCustomizations_MasterPage.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Lists all the sites where there's a custom master page used. Custom master pages only work on "classic" pages and depending on what the custom master page is doing you might be able to eliminate the master page
**Web Template = STS#0** | This lists all team sites having a custom master page...for portals having a custom master page is common, but for team sites this typically is not needed. It's recommended to cross check if you really need a custom master page on these sites

## Understanding the IgnoredCustomizations_CustomAction.csv file
This report contains the following columns:

Column | Description
---------|----------
**URL** | Url of the scanned object (site or list url in this case).
**Site Url** | Url of the scanned site.
**Site Collection Url** | Url of the scanned site collection.
**List name** | Name of the list (in case of a list scoped user custom action).
**Title** | User custom action title.
**Name** | Name of the user custom action.
**Location** | Location of the user custom action.
**RegistrationType** | Registration type of the user custom action.
**RegistrationId** | ID of the registration.
**Reason** | Reasons why this user custom action is ignored in "modern".
**ScriptBlock** | Value of the ScriptBlock user custom action value.
**ScriptSrc** | Value of the ScriptSrc user custom action value.
**CommandActions** | Embedded JavaScript found in command actions. Embedding JavaScript in the command action of your user custom action is not compatible with the "modern" experiences.

### Key takeaways from this report
Load the IgnoredCustomizations_CustomAction.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Lists all the sites having a user custom action which is not going to work on "modern" pages. You need to assess how important these user custom actions are: if they are business critical it's better to either move the site back to "classic" or alternatively fix this [if Microsoft would offer the capabilities ](http://aka.ms/spfx-roadmap)for doing so in the future.
**Location = ScriptLink** | We often see customers using user custom actions to embed a JavaScript file on all their sites. Depending on what this JavaScript file is doing there might not be an impact for the "modern" experience. It's recommended to review what the embedded JavaScript is doing and if needed update the JavaScript.

## Understanding the IgnoredCustomizations_AlternateCSS.csv file
This report contains the following columns:

Column | Description
---------|----------
**URL** | Url of the scanned object (site in this case).
**Site Url** | Url of the scanned site.
**Site Collection Url** | Url of the scanned site collection.
**Web Template** | Web template of the scanned site.
**AlternateCSS** | Value of the `AlternateCssUrl` property. Although the alternate CSS is not used for "modern" pages the alternate CSS still applies to the "classic" pages on your site.

### Key takeaways from this report
Load the IgnoredCustomizations_AlternateCSS.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Lists all the sites where there's a custom CSS file hooked up to the site. Custom CSS files only impact "classic" pages and depending on what the custom CSS is doing you might be able to eliminate this custom CSS file
**Web Template = STS#0** | This lists all team sites having a custom CSS file...while there's typically no need for doing this. Verify if using the out of the box branding options is not sufficient for your teams sites and then drop this custom CSS reference.

# Advanced topics #

## I want to do a partial scan...can I?
Using the -m command line parameter you can specify the scan mode. The tool supports three scans as listed below. If the -m command line parameter is omitted all three scans will be executed.
- **BlockedLists**: scans all lists for compatibility with the "modern" list and library experience
- **BlockedPages**: scans all sites for compatibility with the "modern" pages experiences
- **IgnoredCustomizations**: scans all sites and lists for customizations which will be ignored on the "modern" user interface.

```console
uiexperiencescanner -m BlockedPages -t <tenant> -c <clientid> -s <clientsecret>
```

A real life sample:

```console
uiexperiencescanner -m BlockedPages -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

## There's a large amount of lists logged...is this normal?
By default the scanner will output all the lists which cannot render their default view page using modern. Since Microsoft did not implement all possible features in the modern list UI (e.g. managed metadata navigation) these lists are logged as well. If you want to filter out the lists which are not able to show in modern because of out of the box reasons then you've two options:
- Filter the CSV file on "Only blocked by OOB reaons" = FALSE.
- Use the -o (-excludelistsonlyblockedbyoobreaons) command line parameter which will skip logging these lists.

## How can I decrease the time it takes to scan the environment?
If you've a very large environment the scan will take a long time since it will have to process all libraries in your tenant. Use below tips to speed up the scanning:
- Run the scan on an Azure VM hosted in the same region as your SharePoint Online tenant, this will optimize the network traffic between the scanner and SharePoint Online
- Use the -r (see later in the chapter) parameter to for example only scan the sites that start with A,B,C,D on one machine, E,F,G,H on the next one,...This way you can have parallel scans running each handling a subset of site collections. This however means you'll need to manually combine the resulting CSV files.
- Increase the number of threads, default is 10 but increasing to 20 can help on large environments. It will however not double the performance :-)


## I blocked the "modern" experiences at tenant level...does the tool still work?
A tenant administrator could have blocked the ["modern" list and library experience](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-lists-and-libraries#tenant-level-configuration) and the ["modern" page experience](https://msdn.microsoft.com/en-us/pnp_articles/modern-experience-customizations-customize-pages#tenant-level-configuration) at tenant scope. The scanner will not be able to read this tenant level setting and therefore the following will happen:
- If the scan scope included **BlockedPages** then the tool will still list all sites where the "modern" page feature was disabled, but in reality no sites can use the "modern" page feature. This scan however still allows you to understand which sites will not have "modern" pages once it get's turned on.
- If the scan scope included **BlockedLists** then the tool will do a scan of **all** lists and return compatibility data whenever a list will not work in "modern". This will help you understand the impact of turning on "modern" for your tenant...remember that while the tenant setting is off only list and libraries where the listexperience was forcefully set to "modern" are showing the "modern" user experience.

## I'm running SharePoint Online dedicated, is this different? ##
In SharePoint Online Dedicated one can have vanity url's like teams.contoso.com which implies that the tool cannot automatically determine the used url's and tenant admin center url. Using below command line switches you can specify the site url's to scan and the tenant admin center url. Note that the urls need to be separated by a comma.

```console
uiexperiencescanner -r <urls> -a <tenantadminsite> -c <clientid> -s <clientsecret>
```

A real life sample:

```console
uiexperiencescanner -r https://team.contoso.com/*,https://mysites.contoso.com/* 
                    -a https://contoso-admin.contoso.com -c 7a5c1615-997a-4059-a784-db2245ec7cc1 
                    -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

## I don't want to use app-only, can I use credentials? ##
The best option is to use app-only since that will ensure that the tool can read all site collections but you can also run the tool using credentials.

```console
uiexperiencescanner -t <tenant> -u <user> -p <password>
```

A real life sample:

```console
uiexperiencescanner -t contoso -c admin@contoso.onmicrosoft.com -p mypassword
```

## I only want to scan a few sites, can I do that? ##
Using the urls command line switch you can control which sites are scanned. You can specify one or more url's which can have a wild card. Samples of valid url's are:
 - https://contoso.sharepoint.com/*
 - https://contoso.sharepoint.com/sites/mysite
 - https://contoso-my.sharepoint.com/personal/*

To specify the url's you can use the -r parameter as shown below:

```console
uiexperiencescanner -r https://contoso.sharepoint.com/*,https://contoso.sharepoint.com/sites/mysite,https://contoso-my.sharepoint.com/personal/* 
-c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```


# Complete list of command line switches for the SharePoint Online version #

```Console
SharePoint UI Experience Scanner tool 1.0.0.0
Copyright (C) 2017 SharePoint PnP
==========================================================

See the PnP-Tools repo for more information at:
https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.UIExperience.Scanner

Let the tool figure out your urls (works only for SPO MT):
==========================================================
Using app-only:
UIExperienceScanner.exe -m <mode> -t <tenant> -c <your client id> -s <your client secret>

e.g. UIExperienceScanner.exe -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
e.g. UIExperienceScanner.exe -m scan -t contoso -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
e.g. UIExperienceScanner.exe -m customactions,listexperience,modernfeature -t contoso -c
7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
UIExperienceScanner.exe -m <mode> -t <tenant> -u <your user id> -p <your user password>

e.g. UIExperienceScanner.exe -m scan -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd

Specifying your urls to scan + url to tenant admin (needed for SPO Dedicated):
==============================================================================
Using app-only:
UIExperienceScanner.exe -m <mode> -r <urls> -a <tenant admin site> -c <your client id> -s <your client secret>
e.g. UIExperienceScanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a
https://contoso-admin.contoso.com -c 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
UIExperienceScanner.exe -m <mode> -r <urls> -a <tenant admin site> -u <your user id> -p <your user password>
e.g. UIExperienceScanner.exe -m scan -r https://team.contoso.com/*,https://mysites.contoso.com/* -a
https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd

 -m, --mode                                  (Default: Scan) Execution mode. Choose scan to scan all UIExperience. Use
                                             following UIExperience options blockedlists, blockedpages or
                                             ignoredcustomizations for individual scanning. Omit or use scan for a
                                             full scan

 -t, --tenant                                Tenant name, e.g. contoso when your sites are under
                                             https://contoso.sharepoint.com/sites. This is the recommended model for
                                             SharePoint Online MT as this way all site collections will be scanned

 -r, --urls                                  List of (wildcard) urls (e.g.
                                             https://contoso.sharepoint.com/*,https://contoso-my.sharepoint.com,https:/
                                             /contoso-my.sharepoint.com/personal/*) that you want to get scanned. When
                                             you specify the --tenant optoin then this parameter is ignored

 -c, --clientid                              Client ID of the app-only principal used to scan your site collections

 -s, --clientsecret                          Client Secret of the app-only principal used to scan your site
                                             collections

 -u, --user                                  User id used to scan/enumerate your site collections

 -p, --password                              Password of the user used to scan/enumerate your site collections

 -a, --tenantadminsite                       Url to your tenant admin site (e.g. https://contoso-admin.contoso.com):
                                             only needed when your not using SPO MT

 -x, --excludeod4b                           (Default: True) Exclude OD4B sites from the scan

 -o, --excludelistsonlyblockedbyoobreaons    (Default: False) Exclude lists which are blocked due to out of the box
                                             reasons: managed metadata navigation, base template, view type of field
                                             type

 -e, --separator                             (Default: ,) Separator used in output CSV files (e.g. ";")

 -h, --threads                               (Default: 10) Number of parallel threads, maximum = 100

 --help                                      Display this help screen.
```

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-uiexperiencescanner" /> 


