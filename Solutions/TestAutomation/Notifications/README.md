# SharePoint Test Automation Notification Engine #

### Summary ###
This application is the notification component used to [send out communications configured via the PnP Test Automation UI](http://testautomation.sharepointpnp.com). It's used by the PnP core team to follow test results but can also be used by you for your internal test automation efforts.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises
-  SharePoint 2016 on-premises

### Solution ###
Solution | Author(s)
---------|----------
PnPTestResultsNotificationJob | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 17th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# PnP Test Automation Notification Engine #
PnP Test automation is project that does run tests of the PnP Core library and PnP PowerShell against various test environments on a daily basis. These test results are stored in a SQL Azure database and to explore those results we've built a small web application which is available for you to re-use for your internal testing needs. This web application depends on background task (typically deployed as AZure web job) that can send out notifications on a daily basis, which is the shared with your via this project

<img src="https://telemetry.sharepointpnp.com/pnp-tools/solutions/TestAutomationNotifications" /> 


