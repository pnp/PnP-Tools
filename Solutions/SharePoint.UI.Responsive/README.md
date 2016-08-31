# SharePoint 2013/2016/Online Responsive UI #
This solution demonstrates how to enable the PnP Responsive UI in Microsoft SharePoint 2013/2016 on-premises or SharePoint Online using the 
PowerShell cmdlet *Enable-SPOResponsiveUI*, and also
provides you the capability to enable a custom Responsive UI for a Site Collection.
If you just want to enable the responsive UI with the default PnP responsive template you
can simply use the PowerShell cmdlet directly.
See the *Enable-SPOResponsiveUI* and *Disable-SPOResponsiveUI* [cmdlets documentation](https://github.com/OfficeDev/PnP-PowerShell) for more information. 
This solution shows how to use the PnP provisioning engine to deploy custom versions of the CSS and JavaScript assets
which are used to create the responsive site, 
in order to make a custom responsive version of the out of the box UI of a classic Team Site (STS#0).

The Responsive UI Package is described in a [blog article](https://dev.office.com/blogs/announcing-responsive-ui-package-for-sharepoint-on-premises-2013-2016),
although this article and its associated video tutorial were written before the PowerShell cmdlet was created,
so it uses script injection to enable the responsive UI.
Moving forward we recommend using the PowerShell cmdlet which makes use of the PnP core library.

If you simply want to have an overview of this solution, you can read the
<a href="#overview">following section</a>. 

![](http://i.imgur.com/I2VYM3a.png)
 
>**Note**: This is an **Open Source** project, and any contribution from the community
is more than welcome. Thus, feel free to review the code and submit any 
<a href="https://github.com/OfficeDev/PnP-Tools/issues">Issues</a> or
<a href="https://github.com/OfficeDev/PnP-Tools/pulls">Pull Requests</a>, using GitHub.
 
# Setup Instructions #
In order to setup the solution and to enable the Responsive UI on a target
Site Collection, you simply need to:
* [Download the files included in this solution](#download)
* [Setup software requirements](#requirements)
* [Execute the *Enable-SPResponsiveUI* script](#execute)

>**Note**: If you are using SharePoint 2013 environment, setup scripts are assuming that you are running at least the April 2015 CU.
 
<a name="download"></a>
## Download the files
You can download the files manually, one by one, or you can download
a ZIP file with all the PnP-Tools, simply following
<a href="https://github.com/OfficeDev/PnP-Tools/archive/master.zip">this link</a>. 
Within the ZIP file, under the /Solutions/SharePoint.UI.Responsive folder, you will
find all the required files.

<a name="requirements"></a>
## Setup software requirements
This solution requires the OfficeDevPnP.PowerShell commands, which you can install
from the following link (or the appropriate version if you are using SharePoint Online
 - see the <a href="https://github.com/OfficeDev/PnP-PowerShell#installation">OfficeDevPnP.PowerShell installation 
instructions</a> for more information): 

* <a href="http://aka.ms/officedevpnpcmdlets15">OfficeDevPnP.PowerShell v15 package</a>

>**Note**: Because this solution targets SharePoint 2013/2016 on-premises, you should refer to
the v15 of the OfficeDevPnP.PowerShell commands. Nevertheless, even the v16 version, which
targets SharePoint Online, is viable to setup this solution. If you are running SharePoint 2013 environment, 
PowerShell CmdLets used by the automation scripts have dependency on April 2015 CU to be installed on server side. 
Technically this is not required for the Responsive UI elements, but for the automation.


<a name="execute"></a>
## Execute the *Enable-SPResponsiveUI* script
Once you have installed the OfficeDevPnP.PowerShell commands, you can open a 
PowerShell console, go to the path where you stored the files and execute the *Enable-SPResponsiveUI.ps1*
script, which is included in the
<a href="./Enable-SPResponsiveUI.ps1">Enable-SPResponsiveUI.ps1</a> script file of this solution.

The *Enable-SPResponsiveUI* script accepts the following parameters:
* **Web**: it is a mandatory parameter, which declares the URL of the Site Collection where the Responsive UI will be enabled. It has to be provided as a full URL, like for example: https://intranet.mydomain.com/sites/targetSite
* **Credentials**: it is an optional parameter, which defines the user credentials that will be used to authenticate against both the target Site Collection. Should be the credentials of a user, who is Site Collection Administrator for the target Site Collections. If you don't provide this parameter, the script will directly prompt you for credentials.

Here you can see a couple of examples about how to invoke the *Enable-SPResponsiveUI* script:

###EXAMPLE 1
```PowerShell
PS C:\> .\Enable-SPResponsiveUI.ps1 -Web "https://intranet.mydomain.com/sites/targetSite"
```

The example above enables the Responsive UI on the target Site Collection with URL https://intranet.mydomain.com/sites/targetSite and uses the same Site Collection for hosting the JavaScript and CSS files. 
The user's credentials are not provided, so the cmdlet will directly prompt the user.

###EXAMPLE 2
```PowerShell
PS C:\> $creds = Get-Credential
PS C:\> .\Enable-SPResponsiveUI.ps1 -Web "https://intranet.mydomain.com/sites/targetSite" -Credentials $creds
```
 
The example above enables the Responsive UI on the target Site Collection with the user's credentials provided through the *$creds* variable.

The PowerShell cmdlet *Enable-SPOResponsiveUI* takes an additional parameter -InfrastructureUrl 
which allows you to specify where the CSS and JavaScript assets are stored to enable a single
copy to be used across multiple site collections. We have not included this parameter in the
script to reduce the risk of accidentally customising multiple site collections when you are working
on a particular site collection. If you need to customise at this level we recommend that you
run the *Enable-SPResponsiveUI* script against the infrastructure site collection, and then use the *Enable-SPOResponsiveUI* cmdlet directly in
each site collection you wish to associate with the infrastructure site collection.

>**Important**: The Responsive UI can be experienced from a mobile device (tablet or smartphone)
only by disabling the "Mobile Browser View" native feature of SharePoint.
Thus, when you execute the *Enable-SPResponsiveUI* cmdlet,
that feature will be disabled on the root Site of the target Site Collection.
However, if you want to leverage the Responsive UI on all the sub-sites of the target
Site Collection, you will have to disable the "Mobile Browser View"
for those sub sites, as well.

<a name="customising"></a>
# Customise the Responsive UI #
In order to customise the responsive UI you 
will need to modify either or both of the files SP-Responsive-UI.js and SP-Responsive-UI.css.
The script will deploy these files after running the *Enable-SPOResponsiveUI* cmdlet.

The SP-Responsive-UI.css file is a copy of the standard PnP responsive CSS file with a modified BODY tag which
sets the background colour to yellow. This is so that you can verify that the custom template has been applied.
You will want to remove that CSS statement when you do your own customisations.

Note that SharePoint Online already has a responsive template, so if you use the default PnP responsive template,
e.g. by using the Enable-SPOResponsiveUI cmdlet,
you may not be able to detect any difference, even though the template has been successfully deployed. 
Using the PowerShell script will enable the template and deploy the customised assets,
and you should see the background is pale yellow after you run the script against a standard team site collection.

<a name="disable"></a>
# Disable the Responsive UI #
If you decide to disable the Responsive UI, use the *Disable-SPOResponsiveUI* cmdlet.

###EXAMPLE
```PowerShell
# connect to site (not needed if you just ran the enable script)...
PS C:\> Connect-SPOnline "https://intranet.mydomain.com/sites/targetSite"  
# use the current web...
PS C:\> Disable-SPOResponsiveUI  
```

See the *Disable-SPOResponsiveUI* [cmdlet documentation](https://github.com/OfficeDev/PnP-PowerShell) for more information. 

If you want to remove your custom CSS and JavaScript you can remove the files from the site collection and then re-run the 
*Enable-SPOResponsiveUI* cmdlet which will re-instate the PnP versions of the files.


<a name="overview"></a>
# Solution Overview #
The solution leverages the PnP PowerShell *Enable-SPOResponsiveUI* cmdlet to enable
JavaScript embedding and CSS overriding to convert the out of 
the box UI of any SharePoint 2013/2016 Team Site (STS#0) into a Responsive UI.
The PnP default Responsive UI supports three rendering models:
* **Desktop**: screen width above 768px
* **Tablet**: screen width between 481px and 768px
* **SmartPhone**: screen width lower than or equal to 480px

In the following screenshots you can see a sample rendering of the Home Page of a 
Team Site, for the three supported rendering models.

![SharePoint 2016 - Desktop Mode](http://i.imgur.com/D1rOLxv.png)

![SharePoint 2016 - Tablet Mode](http://i.imgur.com/zNFKwHN.png)

![SharePoint 2016 - SmartPhone Mode](http://i.imgur.com/PJPGvuP.png)

The Responsive UI is applied to the following pages of the root site of a Site Collection, as well as of any sub-site (as long as you disable the "Mobile Browser View" feature in any specific sub-site):
* Home Page
* Web Part Pages
* Wiki Pages
* Document Libraries
* Lists
* Site Contents
* Site Settings

>**Note**: We tried to do our best to properly behave with any out of the box Web Part, and we tested most of the common page definitions/layouts. However, 
there could be cases in which the Responsive UI could be better. In that case, plese feel free to contribute to this Open Source project either by suggesting 
a Pull Request, or by submitting an Issue.

## Implementation details
When you enable the Responsive UI the solution embeds a custom JavaScript file 
(<a href="./SP-Responsive-UI.js">SP-Responsive-UI.js</a>), which takes care of
loading jQuery, and embedding a custom CSS file (<a href="./SP-Responsive-UI.css">SP-Responsive-UI.css</a>) that overrides 
most of the native CSS styles of SharePoint 2013/2016, 
in order to make it responsive. 
Moreover, the embedded JS file also handles some inner logic, for example to replace TABLE/TR/TD with DIV elements in the Site Settings page, 
or to replace the out of the box Global navigation bar and Current navigation bar with the common and well-known Bootstrap expansible menu. 
Overall the solution plays fairly with the content
of the pages, mainly overriding native CSS styles and using JavaScript and DOM (Document Object Model) rebuilding only when it is really needed.

The script runs the PowerShell cmdlet which uploads the default PnP JS and CSS files into the Style Library of that
Site Collection in a sub-folder with the name "SP.Responsive.UI". The script then uses the 
Apply-SPOProvisioningTemplate to upload the custom JS and CSS files.

If you supply a sub-site URL instead of the URL of a site collection the script and cmdlet will still work,
but the files will be provisioned at the site (web) level,
so you will not be able to navigate to /Style Library/SP.Responsive.UI to see the provisioned files using the SharePoint UI.

It is interesting to notice that the deployment phase of the solution leverages the PnP Remote Provisioning Engine. If you are interested in digging into the PnP Remote Provisioning Engine
you can read the document <a href="https://github.com/OfficeDev/PnP-Guidance/blob/master/articles/Introducing-the-PnP-Provisioning-Engine.md">"Introducing the PnP Provisioning Engine"</a> 
on GitHub, or you can watch the video
<a href="https://channel9.msdn.com/blogs/OfficeDevPnP/Getting-Started-with-PnP-Provisioning-Engine">"Getting Started with PnP Provisioning Engine"</a> on Channel 9.
