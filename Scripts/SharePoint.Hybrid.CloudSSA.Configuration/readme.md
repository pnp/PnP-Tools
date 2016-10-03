# Cloud Search Service Application Configuration #

### Summary ###
This script is designed specifically for use with FastTrack service offerings

It requires the following variables:

*SearchApplicationPoolAccountName - this is the account for the search app pool. If there is an existing local SSA that same account can be reused. Note that this account must be a managed SharePoint service account.

*DatabaseServer - this is the name of an existing SharePoint database server.

*SearchServer1 - this is the name of the first new server for Hybrid Search services.  This server must already be added to the farm and should not host any SharePoint roles other than what was created when the server was added.

*SearchServer2 - this is the name of the second new server for Hybrid Search services.  The presence of this server is optional if the customer does not have a highly available SharePoint farm. This server must already be added to the farm and should not host any SharePoint roles other than what was created when the server was added.

FastTrack recommends that two servers be used to ensure high availability of the service application.
    

*work in progress*
 
### Applies to ###
-  SharePoint 2016 on-premises
-  SharePoint 2013 on-premises

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Hybrid Cloud Search Service Application Configuration Wizard | Neil Hodgkinson (Microsoft), Marcus Tiongson (Microsoft), Manas Biswas (Microsoft)
 
### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 3rd 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# For detailed description of scenarios and steps, please refer to following documentation in the repository.#

TBD
