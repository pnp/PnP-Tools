# Test Automation #

This folder contains our [PnP test automation](http://testautomation.sharepointpnp.com) applications:
 - The **UI** folder contains the MVC web application that we use to present the test results to you
 - The **Notifications** folder contains an Azure web job that we use to send out test result summary mails to the ones that have subscribed to our daily mail

Next to that we also have the **test engine project** which is the component that actually executes the test cases. This component resides in the [PnP Sites Core repository](https://github.com/OfficeDev/PnP-Sites-Core/tree/dev/Core/Tools/OfficeDevPnP.Core.Tools.UnitTest).

