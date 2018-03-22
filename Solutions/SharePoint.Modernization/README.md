# SharePoint Modernization scanner #

### Summary ###

Using this scanner you can prepare your classic team sites for modernization via connecting these sites to an Office 365 group (the "groupify" process). This scanner is a key piece if you want to modernize your classic sites. Checkout the [Modernize your classic sites](https://docs.microsoft.com/en-us/sharepoint/dev/transform/modernize-classic-sites) article on docs.microsoft.com to learn more.

### Applies to ###

- SharePoint Online

### Solution ###

Solution | Author(s)
---------|----------
SharePoint.Modernization.Scanner | Bert Jansen (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.3 | March 16th 2018 | Added site usage information
1.2 | March 7th 2018 | Reliability improvements
1.1 | January 31st 2018 | Performance and stability improvements + Page scanner component integrated
1.0 | January 19th 2018 | First main version

### Disclaimer ###

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# What will this tool do for you? #

The main purpose of this tool is to give you a set of reports that you can use to:

- Assess which sites are ready for "groupify": this report will give you "groupify" warnings and blockers which you can use to scope the sites to "groupify" and plan the needed remediation work
- Learn more about the site pages used in your tenant: knowing which pages you have and their characteristics (type, layout, web part data, usage) is important to prepare for modernizing (a subset of) these pages

# Quick start guide #

## Download the tool ##

You can download the tool from here:

- [Modernization scanner for SharePoint Online](https://github.com/SharePoint/PnP-Tools/blob/master/Solutions/SharePoint.Modernization/Releases/SharePoint.Modernization.Scanner%20v1.3.zip?raw=true)

Once you've downloaded the tool you have a folder containing the tool **SharePoint.Modernization.Scanner.exe**. Start a (PowerShell) command prompt and navigate to that folder so that you can use the tool.

> Note:
> If you want to compile the tool yourself you'll also need to have the SharePoint.Scanning solution available as this tools depends on the SharePoint Scanner framework to compile.

## Using the scanner for SharePoint Online ##

Since this tool needs to be able to scan all site collections it's recommended to use an app-only principal with tenant scoped permissions for the scan. This approach will ensure the tool has access, if you use an account (e.g. your SharePoint tenant admin account) then the tool can only access the sites where this user also has access. You can either use a an Azure AD application or a SharePoint app principal:

- [Granting access via Azure AD App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread)
- [Granting access via SharePoint App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azureacs)

Once the preparation work is done, let's continue with doing a scan.

### Scanning SharePoint Online environment - Groupify scan only ###

Below option is the typical usage of the tool for most customers: you specify the mode, your tenant name and the created client id and secret:

```console
SharePoint.Modernization.Scanner.exe -m GroupifyOnly -t <tenant> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.Modernization.Scanner.exe -m GroupifyOnly -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

You'll see the following output during the run:

```console
=====================================================
Scanning is starting...19/01/2018 16:33:52
=====================================================
Processing site https://bertonline.sharepoint.com/sites/espctest1...
Processing site https://bertonline.sharepoint.com/sites/ediscovery...
Processing site https://bertonline.sharepoint.com/sites/espctest2...
Processing site https://bertonline.sharepoint.com/sites/espcteamsite1...
Thread: 5. Processed 4 of 4 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 15 seconds.
Processing site https://bertonline.sharepoint.com/sites/espctest2/sub1...
Processing site https://bertonline.sharepoint.com/sites/ediscovery/demodavid2...
Processing site https://bertonline.sharepoint.com/sites/espctest1/dummy...
Processing site https://bertonline.sharepoint.com/sites/ediscovery/case1...
Thread: 8. Processed 4 of 4 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 20 seconds.
Thread: 7. Processed 4 of 4 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 20 seconds.
Thread: 6. Processed 4 of 4 site collections (100%). Process running for 0 days, 0 hours, 0 minutes and 20 seconds.
=====================================================
Scanning is done...now dump the results to a CSV file
=====================================================
Outputting errors to 636519764327038722\errors.csv
Outputting information over the done scan to 636519764327038722\ScannerSummary.csv
Outputting scan results to 636519764327038722\ModernizationSiteScanResults.csv
Outputting scan results to 636519764327038722\ModernizationWebScanResults.csv
Outputting scan results to 636519764327038722\ModernizationUserCustomActionScanResults.csv
=====================================================
All done. Took 00:00:22.4154431 for 4 sites
=====================================================
```

After the run you'll find a new sub folder (e.g. 636519019371118441) which contains the following:

Report | Content
---------|----------
**ModernizationSiteScanResults.csv** | The main "groupify" report contains one row per site collection explaining which sites are ready to "groupify" with which warnings. It will also tell which "groupify" blockers it found and provide extensive information on the applied permission model.
**ModernizationWebScanResults.csv** | Having sub sites is a potential "groupify" warning and this report contains "groupify" relevant information about each web. This information is also rolled up to the ModernizationSiteScanResults.csv report, so you only need this report if you want to get more details on the found warnings/blockers.
**ModernizationUserCustomActionScanResults.csv** | When a site is "Groupified" it will get a "modern" home page...and  user custom actions that embed script do not work on modern pages. This report contains all the site/web scoped user custom actions that do not work on modern pages. This information is also rolled up to the ModernizationSiteScanResults.csv report, so you only need this report if you want to get more details on the actual found user custom actions
**Error.csv** | If the scan tool encountered errors then these are logged in this file.
**ScannerSummary.csv** | Logs the number of scanned site collections, webs and list. It will also contain information on scan duration and used scanner version.

### Scanning SharePoint Online environment - Groupify + Pages scan ###

Below option is the typical usage of the tool for most customers: you specify the mode, your tenant name and the created client id and secret:

```console
SharePoint.Modernization.Scanner.exe -t <tenant> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.Modernization.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

You'll see the following output during the run:

```console
=====================================================
Scanning is starting...31/01/2018 14:03:19
=====================================================
Processing site https://bertonline.sharepoint.com/sites/espctest1...
Processing site https://bertonline.sharepoint.com/sites/espctest2...
Start search query path:https://bertonline.sharepoint.com/sites/espctest1 AND fileextension=aspx AND (contentclass=STS_ListItem_WebPageLibrary OR contentclass=STS_Site OR contentclass=STS_Web)
Found 10 rows...
Retrieving a batch of up to 500 search results
Start search query path:https://bertonline.sharepoint.com/sites/espctest2 AND fileextension=aspx AND (contentclass=STS_ListItem_WebPageLibrary OR contentclass=STS_Site OR contentclass=STS_Web)
Found 20 rows...
Retrieving a batch of up to 500 search results
Scan of page /sites/espctest1/SitePages/Home.aspx took 2 seconds
Scan of page /sites/espctest2/SitePages/Home.aspx took 2 seconds
Scan of page /sites/espctest2/SitePages/How To Use This Library.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/espc2017.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/simple.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/bert1234.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/Home2.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/webpartpage1.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/onecolwithsidebar.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/wp1.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/rightcolheaderfootertoprow3col.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/HeaderFooterThreeColumns.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/FullPageVertical.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/HeaderLeftColumnBody.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/HeaderRightColumnBody.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/HeaderFooter2Columns4Rows.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/HeaderFooter4ColumnsTopRow.aspx took 0 seconds
Scan of page /sites/espctest1/SitePages/ContentRollup.aspx took 4 seconds
Scan of page /sites/espctest2/SitePages/LeftColumnHeaderFooterTopRow3Columns.aspx took 0 seconds
Scan of page /sites/espctest2/SitePages/sample_html.aspx took 0 seconds
Processing site https://bertonline.sharepoint.com/sites/espctest2/sub1...
...
=====================================================
Scanning is done...now dump the results to a CSV file
=====================================================
Outputting errors to 636530041937506713\errors.csv
Outputting information over the done scan to 636530041937506713\ScannerSummary.csv
Outputting scan results to 636530041937506713\ModernizationSiteScanResults.csv
Outputting scan results to 636530041937506713\ModernizationWebScanResults.csv
Outputting scan results to 636530041937506713\ModernizationUserCustomActionScanResults.csv
Outputting scan results to 636530041937506713\PageScanResults.csv
Outputting scan results to 636530041937506713\UniqueWebParts.csv
=====================================================
All done. Took 00:01:03.5211127 for 2 sites
=====================================================
```

After the run you'll find a new sub folder (e.g. 636530041937506713) which contains the following:

Report | Content
---------|----------
**ModernizationSiteScanResults.csv** | The main "groupify" report contains one row per site collection explaining which sites are ready to "groupify" with which warnings. It will also tell which "groupify" blockers it found and provide extensive information on the applied permission model.
**ModernizationWebScanResults.csv** | Having sub sites is a potential "groupify" warning and this report contains "groupify" relevant information about each web. This information is also rolled up to the ModernizationSiteScanResults.csv report, so you only need this report if you want to get more details on the found warnings/blockers.
**ModernizationUserCustomActionScanResults.csv** | When a site is "Groupified" it will get a "modern" home page...and  user custom actions that embed script do not work on modern pages. This report contains all the site/web scoped user custom actions that do not work on modern pages. This information is also rolled up to the ModernizationSiteScanResults.csv report, so you only need this report if you want to get more details on the actual found user custom actions.
**PageScanResults.csv** | Contains a row per page in the site pages library of the scanned sites. This contains a ton of details on the scanned page like type, used layout and detailed web part information.
**UniqueWebParts.csv** | Contains a list of uniquely found web parts during the scan.
**Error.csv** | If the scan tool encountered errors then these are logged in this file.
**ScannerSummary.csv** | Logs the number of scanned site collections, webs and list. It will also contain information on scan duration and used scanner version.

# Report details

## Understanding the ModernizationSiteScanResults.csv file

This report contains the following columns:

Column | Description
---------|----------
**Site Collection Url** | Url of the scanned site collection.
**Site Url** | Url of the scanned web.
**ReadyForGroupify** | Can this site be "groupified"? If value is FALSE then it's strongly discouraged to "groupify" this site collection.
**GroupifyBlockers** | Lists the found blocking issues which is either `SiteHasOffice365Group` (site is already "groupified"), `PublishingFeatureEnabled` (publishing features are enabled) or `IncompatibleWebTemplate` (Web template is not ready for changes introduced by "groupify").
**GroupifyWarnings** | List of the found warnings: these indicate non optimal conditions for "groupifying" the site but these are not considered a blocker for "groupify". Following can be potential warnings: `ADGroupWillNotBeExpanded` (site uses AD groups which do not expand inside an Office 365 group), `SiteHasSubSites` (sub sites are discouraged in modern sites), `ModernUIIssues` (parts of the modern UI capability has been disabled or we've incompatible customizations) or `DefaultHomePageImpacted` ("groupify" will add a new modern home page which is too different from the default home page of the used web template).
**GroupMode** | Proposed group mode (private/public) based on the found security setup (presence of EveryOne claims).
**PermissionWarnings** | Consolidates permission related warnings. Following values can be shown: `PrivateGroupButEveryoneUsedOutsideOfAdminOwnerMemberGroups` (Office 365 group is marked as private, but an everyone claim was used making the SharePoint site more open than the Office 365 group), `SharingDisabledForSiteButGroupWillAllowExternalSharing` (External sharing was disabled for the SharePoint site, but an Office 365 group will by default allow external sharing) or `SubSiteWithBrokenPermissionInheritance` (site has sub sites with unique permissions).
**ModernHomePage** | Does the site have a modern home page or not: if not "groupify" will create a default modern home page. Use this indicator to assess which sites you still want to give your customized modern home page before running "groupify"
**ModernUIWarnings** | This is a collection of warnings indicating either some modern UI component was turned off or incompatible features/customizations have been detected. Possible values are `ModernPageFeatureDisabled` (modern pages are disabled for this site), `ModernListsBlockedAtSiteLevel` (modern UI for lists has been purposely been blocked at site collection level), `ModernListsBlockedAtWebLevel` (modern UI for lists has been purposely been blocked at web collection level), `MasterPageUsed` (a custom master page has been used), `AlternateCSSUsed` (alternate CSS was defined), `UserCustomActionUsed` (incompatible user custom actions have been found) and `PublishingFeatureEnabled` (publishing features are enabled).
**WebTemplate** | The web template used by the site.
**Office365GroupId** | If this site is already connected to an Office 365 group is shows the id if that group.
**MasterPage** | Was a custom master page used?
**AlternateCSS** | Was alternate CSS defined?
**UserCustomActions** | Are there incompatible user custom actions used?
**SubSites** | Does the site have sub sites?
**SubSitesWithBrokenPermissionInheritance** | Does the site have sub sites with unique permissions?
**ModernPageWebFeatureDisabled** | Was the modern page feature disabled for this site?
**ModernPageFeatureWasEnabledBySPO** | Was the modern page feature ever enabled by SPO (it was not enabled on all sites ,this allows you to check where it really was turned off versus never enabled in the first place)?
**ModernListSiteBlockingFeatureEnabled** | Was the modern UI for lists purposely blocked at site collection level?
**ModernListWebBlockingFeatureEnabled** | Was the modern UI for lists purposely blocked at web level?
**SitePublishingFeatureEnabled** | Is site scoped publishing feature enabled?
**WebPublishingFeatureEnabled** | Is web scoped publishing feature enabled?
**ViewsRecent** | Number of views this site received in the last 14 days.
**ViewsRecentUniqueUsers** | Number of unique visitors for this site in the last 14 days.
**ViewsLifeTime** | Number of views this site received during it's lifetime.
**ViewsLifeTimeUniqueUsers** | Number of unique visitors for this site during it's lifetime.
**Everyone(ExceptExternalUsers)Claim** | Is the `everyone` or `everyone except external users` claim used at site level?
**UsesADGroups** | Are there AD groups used to grant permissions?
**ExternalSharing** | Lists the external sharing status of the site
**Admins** | A comma delimited list of site administrators.
**AdminContainsEveryone(ExceptExternalUsers)Claim** | Is the `everyone` or `everyone except external users` claim used in the site administrators?
**AdminContainsADGroups** | Are there AD groups used in the site administrators?
**Owners** | A comma delimited list of site owners.
**OwnersContainsEveryone(ExceptExternalUsers)Claim** | Is the `everyone` or `everyone except external users` claim used in the site owners?
**OwnersContainsADGroups** | Are there AD groups used in the site owners?
**Members** | A comma delimited list of site members.
**MembersContainsEveryone(ExceptExternalUsers)Claim** | Is the `everyone` or `everyone except external users` claim used in the site members?
**MembersContainsADGroups** | Are there AD groups used in the site members?
**Visitors** | A comma delimited list of site visitors.
**VisitorsContainsEveryone(ExceptExternalUsers)Claim** | Is the `everyone` or `everyone except external users` claim used in the site viewers?
**VisitorsContainsADGroups** | Are there AD groups used in the site visitors?

### Key takeaways from this report

Load the ModernizationSiteScanResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per site collection in your tenant
**ReadyForGroupify = TRUE** | Will give you all the site collections that can be "groupified". There might still be warnings to check, but we did not find any blocking issues
**ReadyForGroupify = FALSE AND Office365GroupId = ""** | Will give you all the sites which do not yet have an Office 365 group connected and which can't be groupified
**ReadyForGroupify = TRUE AND GroupMode = PUBLIC** | Will give you all the site collections that can be "groupified" and for which we'll default to a public group (based on the presence of the `everyone` or `everyone except external users` in the site members or site owners

## Understanding the ModernizationWebScanResults.csv file ##

This report contains the following columns:

Column | Description
---------|----------
**Site Collection Url** | Url of the scanned site collection.
**Site Url** | Url of the scanned web.
**WebTemplate** | The web template used by this web.
**BrokenPermissionInheritance** | Does this site have unique permissions defined?
**ModernPageWebFeatureDisabled** | Was the modern page feature disabled for this web?
**ModernPageFeatureWasEnabledBySPO** | Was the modern page feature ever enabled by SPO (it was not enabled on all sites ,this allows you to check where it really was turned off versus never enabled in the first place)?
**WebPublishingFeatureEnabled** | Is web scoped publishing feature enabled?
**MasterPage** | Value of the master page if customized
**CustomMasterPage** | Value of the custom master page if customized
**AlternateCSS** | Value of the alternate CSS if set
**UserCustomActions** | Are there incompatible user custom actions used in this web?
**Everyone(ExceptExternalUsers)Claim** | Is the `everyone` or `everyone except external users` claim used at web level?
**UniqueOwners** | Comma delimited list of owners if this web has unique permissions defined.
**UniqueMembers** | Comma delimited list of members if this web has unique permissions defined.
**UniqueVisitors** | Comma delimited list of visitors if this web has unique permissions defined.

### Key takeaways from this report ###

Load the ModernizationWebScanResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per scanned web
**MasterPage <> "" OR CustomMasterPage <> ""** | Will give you the webs having a custom master page set and name of that custom master page
**AlternateCSS <> ""** | Will give you the webs having alternate CSS defined and the name of the configured alternate CSS file

## Understanding the ModernizationUserCustomActionScanResults.csv file ##

This report contains the following columns:

Column | Description
---------|----------
**Site Collection Url** | Url of the scanned site collection.
**Site Url** | Url of the scanned web.
**Title** | User custom action title.
**Name** | Name of the user custom action.
**Location** | Location of the user custom action.
**RegistrationType** | Registration type of the user custom action.
**RegistrationId** | ID of the registration.
**Reason** | Reasons why this user custom action is ignored in "modern".
**ScriptBlock** | Value of the ScriptBlock user custom action value.
**ScriptSrc** | Value of the ScriptSrc user custom action value.

### Key takeaways from this report ##

Load the ModernizationUserCustomActionScanResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per found incompatible user custom action. You need to assess how important these user custom actions are: if they are business critical it's better to either build an alternative [SharePoint Framework extension](https://docs.microsoft.com/en-us/sharepoint/dev/spfx/extensions/overview-extensions) or move the site back to "classic"

## Understanding the PageScanResults.csv file ##

This report contains the following columns:

Column | Description
---------|----------
**Site Collection Url** | Url of the scanned site collection.
**Site Url** | Url of the scanned web.
**PageUrl** | Server relative URL of the scanned page.
**Library** | Server relative URL of the pages library holding the scanned page.
**HomePage** | Is this a site's home page?.
**Type** | Type of the page (`WikiPage`, `WebPartPage`, `ClientSidePage` or `AspxPage`).
**Layout** | Standard layout of the found WikiPage or WebPartPage
**ModifiedBy** | Last person that modified this page.
**ModifiedAt** | Last modification date and time for this page.
**ViewsRecent** | Number of views this page received in the last 14 days.
**ViewsRecentUniqueUsers** | Number of unique visitors for this page in the last 14 days.
**ViewsLifeTime** | Number of views this page received during it's lifetime.
**ViewsLifeTimeUniqueUsers** | Number of unique visitors for this page during it's lifetime.
**WPType1-30** | Up to 30 columns indicating the web part type.
**WPTitle1-30** | Up to 30 columns indicating the web part title.
**WPData1-30** | Up to 30 columns holding the exported web part data (see upcoming chapter for details).


### Key takeaways from this report ##

Load the PageScanResults.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per scanned page
**HomePage = TRUE** | Gives you all the site home pages
**Type = "WebPartPage"** | Gives you all the web part pages
**ViewsRecent > 0** | Pages which have been recently accessed

### WPData details ###

For each exported web part the same base JSON structure is used as shown below. Important to note are:

- The "Row" property indicates the row (=section) this web part should be added into in a new client side page (assuming the layout of the original page was detected)
- The "Column" property indicates the column this web part should be added into in a new client side page (assuming the layout of the original page was detected)
- The "Properties" property holds the web part specific properties

```JSON
{
	"Type":"<fully qualified web part type>",
	"Id":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ServerControlId":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ZoneId":"FullPage",
	"Hidden":false,
	"IsClosed":false,
	"Title":"<web part title>",
	"Row":1,
	"Column":1,
	"Order":0,
	"ZoneIndex":0,
	"Properties":{"<web part specific properties>"}
}
```

Since each web part has different **relevant** properties the properties that are exported for a web part are defined in the `webpartmapping.xml` file: this allows you to add/remove properties based on your needs. Each defined property is exported and stored underneath the "Properties" property as shown in below complete example:

```JSON
{
	"Type":"Microsoft.Office.Visio.Server.WebControls.VisioWebAccess, Microsoft.Office.Visio.Server, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c",
	"Id":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ServerControlId":"6f930c6f-b7d5-44db-b6e7-55d375e4667d",
	"ZoneId":"FullPage",
	"Hidden":false,
	"IsClosed":false,
	"Title":"Visio Web Access",
	"Row":1,
	"Column":1,
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

## Understanding the UniqueWebParts.csv file ##

This report contains the following columns:

Column | Description
---------|----------
**Type** | Fully qualified name of the web part.
**InMappingFile** | Indicates if this web part is listed in the webpartmapping file.

### Key takeaways from this report ###

Load the UniqueWebParts.csv into Microsoft Excel and use below filters to analyze the received data

Filter | Takeaway
---------|----------
**No filter** | Will give you one row per uniquely detected web part
**InMappingFile = FALSE** | Gives you all the web parts which are not part of the webpartmapping.xml file. If these are important for you it's best to define these web parts with the properties you want to retrieve

# Advanced topics #

## I'm running SharePoint Online dedicated, is this different? ##

In SharePoint Online Dedicated one can have vanity url's like teams.contoso.com which implies that the tool cannot automatically determine the used url's and tenant admin center url. Using below command line switches you can specify the site url's to scan and the tenant admin center url. Note that the urls need to be separated by a comma.

```console
SharePoint.Modernization.Scanner -m GroupifyOnly -a <tenantadminsite> -r <wildcard urls> -i <clientid> -s <clientsecret>

SharePoint.Modernization.Scanner -a <tenantadminsite> -r <wildcard urls> -i <clientid> -s <clientsecret>
```

A real life sample:

```console
SharePoint.Modernization.Scanner -m GroupifyOnly
                                 -a https://contoso-admin.contoso.com 
                                 -r "https://teams.contoso.com/sites/*,https://portal.contoso.com/*"
                                 -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

SharePoint.Modernization.Scanner -a https://contoso-admin.contoso.com 
                                 -r "https://teams.contoso.com/sites/*,https://portal.contoso.com/*"
                                 -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=
```

## I want to use an Azure AD app to authenticate, how do I that? ##

This scanner, like all scanners built using the SharePoint Scanner framework, do support Azure AD App-Only:

```console
SharePoint.Modernization.Scanner -m GroupifyOnly -t <tenant> -i <Azure App ID> -z <Azure AD Domain> 
                                 -f "<Path to PFX file holding your certificate" -x <Password for the PFX file>

SharePoint.Modernization.Scanner -t <tenant> -i <Azure App ID> -z <Azure AD Domain> 
                                 -f "<Path to PFX file holding your certificate" -x <Password for the PFX file>
```

A real life sample:

```console
SharePoint.Modernization.Scanner -m GroupifyOnly -t contoso -i e4108e9b-9865-44a9-c6e1-9003db04a775 -z contoso.onmicrosoft.com  
                                 -f "C:\scanning\AzureADAppOnlyScanning.pfx" -x pwd

SharePoint.Modernization.Scanner -t contoso -i e4108e9b-9865-44a9-c6e1-9003db04a775 -z contoso.onmicrosoft.com  
                                 -f "C:\scanning\AzureADAppOnlyScanning.pfx" -x pwd
```

## I don't want to use app-only, can I use credentials? ##

The best option is to use app-only since that will ensure that the tool can read all site collections but you can also run the tool using credentials.

```console
SharePoint.Modernization.Scanner -m GroupifyOnly -t <tenant> -u <user> -p <password>

SharePoint.Modernization.Scanner -t <tenant> -u <user> -p <password>
```

A real life sample:

```console
SharePoint.Modernization.Scanner -m GroupifyOnly -t contoso -c admin@contoso.onmicrosoft.com -p mypassword

SharePoint.Modernization.Scanner -t contoso -c admin@contoso.onmicrosoft.com -p mypassword
```

# Complete list of command line switches for the SharePoint Online version #

```Console
SharePoint PnP Modernization scanner 1.1.0.0
Copyright (C) 2018 SharePoint PnP
==========================================================

See the PnP-Tools repo for more information at:
https://github.com/SharePoint/PnP-Tools/tree/master/Solutions/SharePoint.Modernization.Scanner

Let the tool figure out your urls (works only for SPO MT):
==========================================================
Using Azure AD app-only:
SharePoint.Modernization.Scanner.exe -t <tenant> -i <your client id> -z <Azure AD domain> -f <PFX file> -x <PFX file
password>
e.g. SharePoint.Modernization.Scanner.exe -t contoso -i e5808e8b-6119-44a9-b9d8-9003db04a882 -z conto.onmicrosoft.com
-f apponlycert.pfx -x pwd

Using app-only:
SharePoint.Modernization.Scanner.exe -t <tenant> -i <your client id> -s <your client secret>
e.g. SharePoint.Modernization.Scanner.exe -t contoso -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
SharePoint.Modernization.Scanner.exe -t <tenant> -u <your user id> -p <your user password>

e.g. SharePoint.Modernization.Scanner.exe -t contoso -u spadmin@contoso.onmicrosoft.com -p pwd

Specifying url to your sites and tenant admin (needed for SPO Dedicated):
=========================================================================
Using Azure AD app-only:
SharePoint.Modernization.Scanner.exe -r <wildcard urls> -a <tenant admin site>  -i <your client id> -z <Azure AD
domain> -f <PFX file> -x <PFX file password>
e.g. SharePoint.Modernization.Scanner.exe -r "https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*" -a
https://contoso-admin.contoso.com -i e5808e8b-6119-44a9-b9d8-9003db04a882 -z conto.onmicrosoft.com  -f apponlycert.pfx
-x pwd

Using app-only:
SharePoint.Modernization.Scanner.exe -r <wildcard urls> -a <tenant admin site> -i <your client id> -s <your client
secret>
e.g. SharePoint.Modernization.Scanner.exe -r "https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*" -a
https://contoso-admin.contoso.com -i 7a5c1615-997a-4059-a784-db2245ec7cc1 -s
eOb6h+s805O/V3DOpd0dalec33Q6ShrHlSKkSra1FFw=

Using credentials:
SharePoint.Modernization.Scanner.exe -r <wildcard urls> -a <tenant admin site> -u <your user id> -p <your user
password>
e.g. SharePoint.Modernization.Scanner.exe -r "https://teams.contoso.com/sites/*,https://my.contoso.com/personal/*" -a
https://contoso-admin.contoso.com -u spadmin@contoso.com -p pwd


  -m, --mode                      (Default: Full) Execution mode. Use following modes: full, GroupifyOnly. Omit or use
                                  full for a full scan

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

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-modernizationscanner" /> 