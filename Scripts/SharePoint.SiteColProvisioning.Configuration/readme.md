# PowerShell to enable site collection creation with CSOM #

### Summary ###
Scripts and instructions that explain how to enable CSOM based site collection creation in on-premises SharePoint 2013 or 2016.

### Applies to ###
- SharePoint 2013 on-premises
- SharePoint 2016 on-premises
- SharePoint 2019 on-premises

### Prerequisites ###
Capability is introduced in April 2014 CU for SP2013 and is included in SharePoint 2016 and SharePoint 2019.

### Solution ###
Solution | Author(s)
---------|----------
Site Collection provisioning scripts | Vesa Juvonen, Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | July 3rd 2018 | Adding notes on the SharePoint 2019
1.0  | February 8th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction
In order to enable site collection using the CSOM CreateSite method you need to run the below two scripts on your SharePoint on-premises farm (SP2013, SP2016, SP2019). 

> **Important.** If you are enabling this capability in SharePoint 2019 farm, you can skip Step #1.

## Step 1: Flag a site as "tenant admin" site (SharePoint 2013, SharePoint 2016)

The `CreateSite` CSOM operation will server side use the multi-tenant CreateSite API's which has as consequence that these API's will need a tenant admin site to function. Luckily you do not to setup the multi-tenant features in your environment, but rather simply **mark** a site as tenant admin site by setting it's `AdministrationSiteType` property to `SPAdministrationSiteType.TenantAdministration`. This site can be **any** site collection, but it's recommended to create a separate site collection with a meaningful name (e.g. TenantAdmin) as this will avoid accidental deletion.

You can do this by running the `definetenantadminsite.ps1` script like shown below:

```PowerShell
.\definetenantadminsite.ps1 -siteColUrl https://portal2016b4327.pnp.com/sites/tenantadmin 
```

This will result in the following output:

```
Loading SharePoint Powershell Snapin
Site https://portal2016b4327.pnp.com/sites/tenantadmin set to AdministrationSiteType TenantAdministration
```


## Step 2: Prepare the web application(s) to enable the CreateSite CSOM method

Each web application for which you want to be able to create site collections using the `CreateSite` CSOM method needs to be prepared by:

- Adding the TenantAdmin CSOM stub to the list of allowed client callable proxy libraries
- Enabling self service site collection creation

You can perform the above 2 steps by running `allowtenantapiv15.ps1` when you're working with SharePoint 2013 or `allowtenantapiv16.ps1` when you're using SharePoint 2016 or SharePoint 2019 version.. 

**Note:**
This will perform an IISReset on the server you run this. **Don't forget to perform an IISReset on the other servers in the farm.**


```PowerShell
 .\allowtenantapiv16.ps1 -WebApplicationUrl https://portal2016b4327.pnp.com
```

This will result in the following output:

```
Successfully added TenantAdmin ServerStub to ClientCallableProxyLibrary for web application https://portal2016b4327.pnp.com
IISReset...
WARNING: Waiting for service 'World Wide Web Publishing Service (W3SVC)' to stop...
WARNING: Waiting for service 'Windows Process Activation Service (WAS)' to stop...
IISReset complete on this server, remember other servers in farm as well.
```
To finalize this step you'll need to perform an IISReset on all the other servers in the farm.

<img src="https://telemetry.sharepointpnp.com/pnp-tools/scripts/SharePoint.SiteColProvisioning.Configuration" /> 
