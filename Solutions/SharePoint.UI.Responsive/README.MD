# SharePoint 2013/2016 Responsive UI #
This solution provides you the capability to enable Responsive UI for a 
Site Collection in Microsoft SharePoint 2013/2016 on-premises.
The solution leverages some JavaScript embedding and CSS overriding, 
in order to make responsive the out of the box UI of a classic Team Site (STS#0).

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
* [Execute the *Enable-SPResponsiveUI* cmdlet](#execute)

If you decide to disable this Responsive UI, please refer to the section [Disable the Responsive UI](#disable)

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
simply by following <a href="https://github.com/OfficeDev/PnP-PowerShell#installation">the instructions</a> provided 
<a href="https://github.com/OfficeDev/PnP-PowerShell">here in the dedicate GitHub repository</a>.

>**Note**: Because this solution targets SharePoint 2013/2016 on-premises, you should refer to
the v15 of the OfficeDevPnP.PowerShell commands. Nevertheless, even the v16 version, which
targets SharePoint Online, is viable to setup this solution.

<a name="execute"></a>
## Execute the *Enable-SPResponsiveUI* cmdlet
Once you have installed the OfficeDevPnP.PowerShell commands, you can  simply open a 
PowerShell console, go to the path where you stored the files and execute the *Enable-SPResponsiveUI*
cmdlet, which is included in the
<a href="./Enable-SPResponsiveUI.ps1">Enable-SPResponsiveUI.ps1</a> script file of this solution.

The *Enable-SPResponsiveUI* cmdlet accepts the following three parameters:
* **TargetSiteUrl**: it is a mandatory parameter, which declares the URL of the Site Collection, where the Responsive UI will be enabled. It has to be provided as a full URL, like for example: https://intranet.mydomain.com/sites/targetSite
* **InfrastructureSiteUrl**: it is an optional parameter, which declares the URL of an infrastructural Site Collection, where will be uploaded/updated the JavaScript and CSS files backing the Responsive UI solution. If you don't provide a value for this parameter, the cmdlet will use the target Site Collection to host these files. It has to be provided as a full URL, like for example: https://intranet.mydomain.com/sites/infrastructureSite
* **Credentials**: it is an optional parameter, which defines the user credentials that will be used to authenticate against both the target Site Collection and the infrastructure Site Collection, if any. Should be the credentials of a user, who is Site Collection Administrator for the target Site Collections. If you don't provide this parameter, the script will directly prompt you for credentials.

Here you can see a couple of examples about how to invoke the *Enable-SPResponsiveUI* cmdlet:

###EXAMPLE 1
```PowerShell
PS C:\> .\Enable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite"
```

The example above enables the Responsive UI on the target Site Collection with URL https://intranet.mydomain.com/sites/targetSite and uses the same Site Collection for hosting the JavaScript and CSS files. The user's credentials are not provided, so the cmdlet will directly prompt the user.

###EXAMPLE 2
```PowerShell
PS C:\> $creds = Get-Credential
PS C:\> .\Enable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite" -InfrastructureSiteUrl "https://intranet.mydomain.com/sites/infrastructureSite" -Credentials $creds
```
 
The example above enables the Responsive UI on the target Site Collection with URL https://intranet.mydomain.com/sites/targetSite and uses the Site Collection with URL https://intranet.mydomain.com/sites/infrastructureSite for hosting the JavaScript and CSS files. The user's credentials are  provided through the *$creds* variable.

>**Important**: The Responsive UI can be experienced from a mobile device (tablet or smartphone)
only by disabling the "Mobile Browser View" native feature of SharePoint.
Thus, when you execute the *Enable-SPResponsiveUI* cmdlet,
that feature will be disabled on the root Site of the target Site Collection.
However, if you want to leverage the Responsive UI on all the sub-sites of the target
Site Collection, you will have to disable the "Mobile Browser View"
for those sub sites, as well.

<a name="disable"></a>
# Disable the Responsive UI #
If you want to disable the Responsive UI on a target Site Collection, you can execute the
*Disable-SPResponsiveUI* cmdlet, which is included in the
<a href="./Disable-SPResponsiveUI.ps1">Disable-SPResponsiveUI.ps1</a> script file of this solution.

The *Disable-SPResponsiveUI* cmdlet accepts the following two parameters:
* **TargetSiteUrl**: it is a mandatory parameter, which declares the URL of the Site Collection, where the Responsive UI will be disabled, if it is enabled. It has to be provided as a full URL, like for example: https://intranet.mydomain.com/sites/targetSite
* **Credentials**: it is an optional parameter, which defines the user credentials that will be used to authenticate against the target Site Collection. Should be the credentials of a user, who is Site Collection Administrator for the target Site Collection. If you don't provide this parameter, the script will directly prompt you for credentials.

Here you can see an example about how to invoke the *Disable-SPResponsiveUI* cmdlet:

###EXAMPLE 1
```PowerShell
PS C:\> .\Disable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite"
```

The example above disables the Responsive UI to the target Site Collection with URL https://intranet.mydomain.com/sites/targetSite . The user's credentials are not provided, so the cmdlet will directly prompt the user.

>Note: It is important to notice that even if you disable the Responsive UI for a target Site Collection, any JavaScript or CSS files will not be removed from that site or from the Infrastructural Site Collection.

<a name="overview"></a>
# Solution Overview #
The solution leverages some JavaScript embedding and CSS overriding to convert the out of 
the box UI of any SharePoint 2013/2016 Team Site (STS#0) into a Responsive UI.
The  Responsive UI supports three rendering models:
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
When you enable the Responsive UI, under the  cover the solution embeds - using a Site Collection level User Custom Action - a custom JavaScript file 
(<a href="./PnP-Responsive-UI.js">PnP-Responsive-UI.js</a>), which takes care of
loading jQuery, and embedding a custom CSS file (<a href="./pnp_responsive_ui.css">pnp_responsive_ui.css</a>) that overrides most of the native CSS styles of SharePoint 2013/2016, 
in order to make it responsive. Moreover, the embedded JS file also handles some inner logic, for example to replace TABLE/TR/TD with DIV elements in the Site Settings page, or to
replace the out of the box Global navigation bar and Current navigation bar with the common and well-known Bootstrap expansible menu. Overall the solution plays fairly with the content
of the pages, mainly overriding native CSS styles and using JavaScript and DOM (Document Object Model) rebuilding only when it is really needed.

If you define an infrastructural Site Collection, while enabling the Responsive UI, the PowerShell command will upload the custom JS and CSS files into the "Style Library" of that
infrastructural Site Collection, within a sub-folder that will be created on the fly with name "SP.Responsive.UI".

It is interesting to notice that the deployment phase of the solution leverages the PnP Remote Provisioning Engine. If you are interested in digging into the PnP Remote Provisioning Engine
you can read the document <a href="https://github.com/OfficeDev/PnP-Guidance/blob/master/articles/Introducing-the-PnP-Provisioning-Engine.md">"Introducing the PnP Provisioning Engine"</a> 
on GitHub, or you can watch the video
<a href="https://channel9.msdn.com/blogs/OfficeDevPnP/Getting-Started-with-PnP-Provisioning-Engine">"Getting Started with PnP Provisioning Engine"</a> on Channel 9.

