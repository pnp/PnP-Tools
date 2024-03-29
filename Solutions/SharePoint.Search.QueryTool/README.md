# SharePoint Search Query Tool #

### Summary ###
*Use this tool to test out and debug search queries against the SharePoint 2013/2016/2019/Online Search REST API.*

Learn how to build an HTTP GET/POST query, and how the different parameters should be formatted.

After running the query, you can view all types of result sets returned; Primary Results, Refinement Results, Query Rules Results, Query Suggestions, in addition to the actual raw response received from the Search service.

* More about the Search REST API in SharePoint 2013 on [Nadeem's blog: http://blogs.msdn.com/b/nadeemis/](https://web.archive.org/web/20160219231941/http://blogs.msdn.com/b/nadeemis/)
* Debugging managed properties on [Mikael's blog: https://www.techmikael.com](http://www.techmikael.com/2014/03/debugging-managed-properties-using.html)
* [Reading the rank details page](https://powersearching.wordpress.com/2013/01/25/explain-rank-in-sharepoint-2013-search/)
* [Freshness boost generator under Tools](http://www.techmikael.com/2013/10/adding-freshness-boost-to-sharepoint.html)

### Applies to ###
-  SharePoint Online, SharePoint 2013, SharePoint 2016, SharePoint 2019

### Solution ###
Solution | Author(s)
---------|----------
SharePoint Query Tool | Nadeem Ishqair (**Microsoft**) - Mikael Svenson (@mikaelsvenson) - Maximilian Melcher (@maxmelcher) - Barry Waldbaum (**Microsoft**) - Dan Göran Lunde (@fastlundan) - Petter Skodvin-Hvammen (@pettersh) - Holger Lutz (**Microsoft**)
PSSQT - PowerShell Module | Frode Sivertsen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.6 | May 31st 2017 | Moved to github
2.7 | Aug 24th 2017 | Added new SPO login, removed GQL support
2.8 | Mar 26th 2018 | Removed old SPO login as it fails too often. Fixed ADAL login for viewing all properties.
2.8.2 | Oct 10th 2018 | Re-added web based SPO login for some people. Updated to .Net 4.6.2 to support TLS 1.2
2.8.3 | Oct 18th 2018 | Fix for view all properties
2.8.4 | Jun 11th 2019 | UI fixes with a fluid layout, query history
2.8.5 | Dec 1st 2019 | Added clear history button, added field for custom Properties parameters
2.8.6 | Feb 2nd 2020 | UI hang bug fixes
2.9.0 | Jan 19th 2022 | Added support to add tenant id hint for multi-geo tenants, improved authentication for Online, copy to clipboard button for response, fixed promoted results parsing
2.10.0 | Oct 24th 2022 | Add interleaving support, fix an null ref check, fix for people results

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# What will this tool do for you? #

This tool will help you test search against SharePoint as well as craft GET and POST queries which you can use in your own code.

# Quick start guide #
## Download the tool ##
You can download the tool from here:
 - [SharePoint Search Query Tool](https://github.com/SharePoint/PnP-Tools/releases)

Once you've downloaded the tool (or alternatively you can also compile it yourself using Visual Studio) you have a folder containing the tool **SearchQueryTool.exe**. Double click the file to start it.

## Connect to a server ##
In the Connection pane enter the URL and authentication information for your server/tenant.

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/sharepoint-searchquerytool" /> 


