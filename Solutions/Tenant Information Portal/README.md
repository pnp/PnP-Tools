# Tenant Information Portal

### Summary
This solution is used to display information regarding your Azure Active Directory Tenant specifically related to Service Principals that are only surfaced via PowerShell and Azure Applications in a single consolidated view. When you register an add-in with appregnew/appinv, these service principals are not displayed in the Azure Portal and the default expiration is 1 year. This solution will also assist you with identifying when the service principals and applications that are expired or may be expiring soon.

This application was built using AngularJS/MVC that is invoking a web API that is secured using Azure AD to query the Graph API.
The application uses the Active Directory Authentication Library (ADAL) to obtain a JWT access token through the OAuth 2.0 protocol. The access token is sent to the ASP.NET 5 Web API, which authenticates the user using the OWIN OAuth Bearer Authentication middleware.


![SharePoint Patterns and Practices logo](http://i.imgur.com/I2VYM3a.png)

This is open source repository, so feedback, issues and pull requests are absolutely welcome.

### Features ###
- Dashboard that provides a visual indicator on Service Principals and Applications that are expired or that may be expiring in 30, 60, and 90 Days.
- Displays/Exports all Service Principals and Applications that are registered within your Azure Active Directory Tenant.
- Displays Tenant Last Directory Synchronization time
- WebJob to send expiration notifications to Office 365 Group/Team via Incoming Webhook connector

### Applies to
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013/2016 on-premises with low trust established

### Prerequisites
- An Azure subscription (a free trial is sufficient). If you don't already have an Azure subscription, you may get a free subscription by signing up at [https://azure.microsoft.com](https://azure.microsoft.com).  All of the Azure AD features used in this application are available free of charge.

### Solution ###
Solution | Author(s)
---------|----------
Tenant Information Portal (TIP) | Frank Marasco (Microsoft)
TIP - WebJob | [Paul Schaeflein](https://github.com/pschaeflein)

### Version history
Version  | Date | Comments
---------| -----| --------
1.0.0  | September 28, 2015 | Initial version
1.0.1 | February 23, 2016 | Date Format
1.0.2 | March 7, 2016 | Azure AD Applications Functionality
1.0.3 | March 23, 2016 | UX Fabric
1.0.4 | June 6, 2016 | Error Controller, Owin Middleware, Azure Resource Deployment (Simon J.K. Pedersen)
2.0.0 | April 2017 | Added WebJob 

See [Change Log](docs/changelog.md) for detailed information.


### Disclaimer
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

## Running the CODE

### Landing Page
![Landing page picture where service principals and applications are shown](http://i.imgur.com/y1gvCfx.png)

### Expired Applications Report

![List of expired applications with name and expiration date details](http://i.imgur.com/Si5u2Kz.png)


### All Principals Report

![List of all principals](http://i.imgur.com/TcYzpMZ.png)

### Expired Principals
![List of expired principals](http://i.imgur.com/Bglwvyg.png)



## Set Up

### Step 1:  Register the application with your Azure Active Directory tenant

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Active Directory in the left hand nav.
3. Click the directory tenant where you wish to register the sample application.
4. Click the Applications tab.
5. In the drawer, click Add.
6. Click "Add an application my organization is developing".
7. Enter a friendly name for the application, for example "Tenant Information Portal", select "Web Application and/or Web API", and click next.
8. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44300/`.
9. For the App ID URI, enter `https://<your_tenant_name>TIP`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.  Click OK to complete the registration.
10. While still in the Azure portal, click the Configure tab of your application.
11. Find the Client ID value and copy it aside, you will need this later when configuring your application.
12. Create a new key for the application.  Save the configuration so you can view the key value.  Save this aside for when you configure the project in Visual Studio.
13. In permissions to other applications Select Windows Azure Active Directory under Application Permissions select "Read directory data

### Step 2:  Configure the application to use your Azure AD tenant

1. Open the solution in Visual Studio 2015.
2. Open the `web.config` file.
3. Find the app key `ida:Tenant` and replace the value with your AAD tenant name. (i.e. contoso.com)
4. Find the app key `ida:ClientId` and replace the value with the Client ID for Tenant Information Portal from the Azure portal.
5. Find the app key `ida:ClientSecret` and replace the value with the key for Tenant Information Portal from the Azure portal.

### Azure Deployment
To deploy the TIP solution to Azure Web Sites, you will create 1 web site with a SQL Server instance, publish the project to the web site.
#### Automatic Deployment with Azure Resource Manager Template
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https:%2F%2Fraw.githubusercontent.com%2Fofficedev%2FPnP-Tools%2Fmaster%2FSolutions%2FTenant%20Information%20Portal%2Fsrc%2FTIP.Azure.ResourceGroup%2FTemplates%2FWebSite.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

#### Manual Deployment
1. Navigate to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Web Sites in the left hand nav.
3. Click New in the bottom left hand corner, select Compute --> Web Site --> Custom Create, select the hosting plan and region, and give your web site a name, e.g. tenantinfoportal.azurewebsites.net.  Select a database to use, or create a new one.  Click Create Web Site.
4. Click Configure and create 3 app settings
  - ida:ClientId and supply your ClientId
  - ida:ClientSecret and supply your ClientSecret
  - ida:Tenant and supply your tenant AAD tenant. e.g. contoso.onmicrosoft.com

5. Right click within Visual Studio and publish to the Azure Web Site.

## WebJob

The solution contains an Azure WebJob that can post to a conversation in an Office 365 Group or Team. The post contains a connector card detailing applications and service principals that will expire soon.

![Connector Card](https://cloud.githubusercontent.com/assets/8008013/24919645/f9c5d73a-1ea9-11e7-8ddb-92ea56574f9e.gif)

### Group/Team setup

1. In the Office 365 Group or Team, navigate to the Connector tab.
2. Scroll to the "Incoming Webhook" Connector. Click Add.
3. Provide a name and image for the connector. Click Create.
4. Note the URL that is displayed. This URL must be added to the WebJob configuration. Click Done.

### WebJob Configuration

The WebJob configuration can be accomplished in the app.config file or using the Configuration blade in the Azure Portal. Some settings are the same as the Portal website. In the Azure Portal, these settings are specified only once.
(The WebJob configuration is not part of the Azure Deployment Template.)

To update the app.config file:
1. Open the solution in Visual Studio 2015.
2. Expand the `webjob` folder.
3. Expand the `TIP.WebJob` project.
4. Open the `app.config` file.
5. Find the app key `ida:Tenant` and replace the value with your AAD tenant name. (i.e. contoso.com)
6. Find the app key `ida:ClientId` and replace the value with the Client ID for Tenant Information Portal from the Azure portal.
7. Find the app key `ida:ClientSecret` and replace the value with the key for Tenant Information Portal from the Azure portal.
8. Find the app key `ConnectorUrl` and replace the value with the URL generated when creating the Incoming Webhook.
9. Find the app key `PortalUrl` and replace the value with the URl of the Tenant Information Portal Dashboard.
10. Find the app key `NotificationInterval` and replace the value with the interval (in days) for warnings about expiring soon.

To update Azure AppService settings:
- Click `Application settings`
- Set the application to `Always On` (Always On requires Basic plan or higher. Alternatively, a [Scheduler Job Collection in the Standard Tier](https://www.schaeflein.net/re-visiting-azure-search-webjobs-and-scheduler/) can be used.)
- Create `App settings` for the WebJob:
  - ConnectorUrl
  - PortalUrl
  - NotificationInterval
 

### Additonal Information
- [Setup Low Trust for SharePoint on-premises] (https://github.com/OfficeDev/PnP-Tools/tree/master/Scripts/SharePoint.LowTrustACS.Configuration)
- [Replace an expiring client secret in a SharePoint Add-in] (https://msdn.microsoft.com/en-us/library/office/dn726681.aspx)
- [Authentication Scenarios for Azure AD] (https://azure.microsoft.com/en-us/documentation/articles/active-directory-authentication-scenarios/)


<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/Tenant-Information-Portal" />
