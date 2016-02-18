# Tenant infromation Portal 

### Summary 
This solution is used to display information regarding your Azure Active Directory Tenant.

### Features ###
- Dashboard that provids a visual indicator on Service Principals that are expired or that may be expiring in 30, 60, and 90 Days.
- Displays/Exports all service principals that are registered within your Azure Active Directory Tenant.

### Applies to 
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013/2016 on-premises with an low trust established 

### Prerequisites 
- An Azure subscription (a free trial is sufficient) If you don't already have an Azure subscription, you may get a free subscription by signing up at [https://azure.microsoft.com](https://azure.microsoft.com).  All of the Azure AD features used in this applicatons are available free of charge.

### Version history 
Version  | Date | Comments
---------| -----| --------
1.0  | September 28, 2015 | Initial version

----------
### Set Up

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
To deploy the TIP solution to Azure Web Sites, you will create 1 web sites with a SQL Server instance, publish each project to a web site

1. Navigate to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Web Sites in the left hand nav.
3. Click New in the bottom left hand corner, select Compute --> Web Site --> Custom Create, select the hosting plan and region, and give your web site a name, e.g. tenantinfoportal.azurewebsites.net.  Select a database to use, or create a new one.  Click Create Web Site.
4. Click Configure and create 4 app settings
	- ida:ClientId and supply your ClientId
	- ida:ClientSecret and supply your ClientSecret
	- ida:Tenant and supply your tenant AAD tenant. e.g. contoso.onmicrosoft.com


### Additonal Information	
- Setup Low Trust for SharePoint on-premises https://github.com/OfficeDev/PnP-Tools/tree/master/Scripts/SharePoint.LowTrustACS.Configuration

### Disclaimer 
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**




