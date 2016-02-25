# SharePoint 2013/2016 Responsive UI #
This solution provides you the capability to enable Responsive UI for a 
Site Collection in Microsoft SharePoint 2013/2016 on-premises.
The solution leverages some JavaScript embedding and CSS overriding, 
in order to make responsive the out of the box UI of a classic Team Site (STS#0).

If you simply want to have an overview of this solution, you can read the
<a href="overview">following section</a>.

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
* **Credentials**: it is an optional parameter, which defines the user credentials that will be used to authenticate against both the target Site Collection and the infrastructure Site Collection, if any. Should be the credentials of a user, who is Site Collection Administrator for the target Site Collections. If you don't provide this parameter, the script will directly propmpt you for credentials.

Here you can see a couple of examples about how to invoke the *Enable-SPResponsiveUI* cmdlet:

###EXAMPLE 1
```PowerShell
PS C:\> .\Enable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite"
```

The example above enables the Responsive UI to the target Site Collection with URL https://intranet.mydomain.com/sites/targetSite and uses the same Site Collection for hosting the JavaScript and CSS files. The user's credentials are not provided, so the cmdlet will directly prompt the user.

###EXAMPLE 2
```PowerShell
PS C:\> $creds = Get-Credential
PS C:\> .\Enable-SPResponsiveUI.ps1 -TargetSiteUrl "https://intranet.mydomain.com/sites/targetSite" -InfrastructureSiteUrl "https://intranet.mydomain.com/sites/infrastructureSite" -Credentials $creds
```
 
The example above enables the Responsive UI to the target Site Collection with URL https://intranet.mydomain.com/sites/targetSite and uses the Site Collection with URL https://intranet.mydomain.com/sites/infrastructureSite for hosting the JavaScript and CSS files. The user's credentials are  provided through the $creds variable.

>**Important**: The Responsive UI can be experienced only by disabling the "Mobile Browser View" 
native feature of SharePoint. Thus, when you execute the *Enable-SPResponsiveUI* cmdlet,
that feature will be disabled on the root Site of the target Site Collection.
However, if you want to leverage the Responsive UI on all the sub sites of the target
Site Collection, you will have to disable the "Mobile Browser View"
for those sub sites, as well.


<a name="disable"></a>
# Disable the Responsive UI #
If you want to disable the Responsive UI on a target Site Collection, you can execute the
*Disable-SPResponsiveUI* cmdlet, which is included in the
<a href="./Disable-SPResponsiveUI.ps1">Disable-SPResponsiveUI.ps1</a> script file of this solution.

The *Disable-SPResponsiveUI* cmdlet accepts the following two parameters:
* **TargetSiteUrl**: it is a mandatory parameter, which declares the URL of the Site Collection, where the Responsive UI will be disabled, if it is enabled. It has to be provided as a full URL, like for example: https://intranet.mydomain.com/sites/targetSite
* **Credentials**: it is an optional parameter, which defines the user credentials that will be used to authenticate against the target Site Collection. Should be the credentials of a user, who is Site Collection Administrator for the target Site Collection. If you don't provide this parameter, the script will directly propmpt you for credentials.

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
the box UI of any SharePoint Team Site (STS#0) into a Responsive UI.
The  Responsive UI supports three rendering models:
* **Desktop**: screen width above 768px
* **Tablet**: screen width between 481px and 768px
* **SmartPhone**: screen width lower than or equal to 480px

In the following screenshots you can see a sample rendering of the Home Page of a 
Team Site, for the three supported rendering models.

>**HERE THERE WILL BE THREE SCREENSHOTS FOR DESKTOP, TABLET, AND MOBILE**

The Responsive UI is applied to the following pages of the root site of a Site Collection, as well as of any sub Site:
* Home Page
* Web Part Pages
* Wiki Pages
* Document Libraries
* Lists
* View All Site Contents
* Site Settings
