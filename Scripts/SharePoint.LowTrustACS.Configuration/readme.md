# PowerShell to enable low trust authentication model at on-premises #

### Summary ###
This document and scripts explain how to setup low trust authentication between your SharePoint 2013/2016 on-premises farm and an Office 365 tenant.
 
### Applies to ###
-  SharePoint 2013 on-premises
-  SharePoint 2016 on-premises

### Prerequisites ###
An Office 365 tenant which you'll use for the low trust association.

### Solution ###
Solution | Author(s)
---------|----------
Enable low trust in on-premises | Vesa Juvonen, Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 8th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction
This document and scripts explain how to setup low trust authentication between your SharePoint 2013/2016 on-premises farm and an Office 365 tenant. Setting up low trust for your on-premises farms will allow you to use Azure ACS as OAuth broker for your on-premises environment and as such run your apps and client side applications in the same manner as you would do for SharePoint Online.

The [detailed steps for this task are documented in MSDN](https://msdn.microsoft.com/en-us/library/office/dn155905.aspx), you can consider this document as a shortened and easier set of instructions.

Below chapters describe the individual steps needed to make this happen

## Step 1: Obtain a new signing certificate for the on-premises SharePoint Token Service (STS)
You’ll need to replace the default security token service (STS) certificate of your on-premises installation of SharePoint 2013/2016 with your own certificate. This section describes how to obtain this certificate by making use of [makecert.exe which is available as part of the Windows SDK](https://msdn.microsoft.com/library/windows/desktop/aa386968.aspx). If you do not want to use makecert.exe you can also use IIS Manager to create a certificate as described [here](https://msdn.microsoft.com/en-us/library/office/dn155905.aspx).

Using `makecert.exe` allows you to choose a certificate with a longer lifetime which means less maintenance on your low trust setup. In below sample we create our own trusted root authority and then individual certs that have our trusted root authority as issuer. See [Mike O'Brien's blog](http://www.mikeobrien.net/blog/creating-self-signed-wildcard/) for more details.

Import detail for the token signing certificate is that it **requires a key length of at least 2048** which is in below command provided via the **len** parameter.

Navigate to folder **C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin** and execute the following commands:

```Cmd
#creates the trusted root authority
./makecert.exe -n "CN=PnP Development Root CA,O=PnP,OU=Development,L=Bree,S=Limburg,C=Belgium" -pe -ss Root -sr LocalMachine -sky exchange -m 120 -a sha256 -len 2048 -r

#creates the token signing cert
./makecert.exe -n "CN=STSTokenSigning" -pe -ss My -sr LocalMachine -sky exchange -m 120 -in "PnP Development Root CA" -is Root -ir LocalMachine -len 2048 -a sha256 -eku 1.3.6.1.5.5.7.3.1
```

**Note:**
You can create these certificates an any machine that has `makecert.exe` available, no need to deploy the SDK to your SharePoint servers.

Once you've created the token signing certificate you'll need to export it to a PFX file via the following steps:
- Start `mmc.exe`
- Load the **Certificates** snap-in for the **Computer Account**
- Locate the created token signing certificate in the **Personal --> Certificates** node, right-click and choose the **Export** task
- Choose to export the **private key**
- The PFX option will be pre-selected, chech the "Include all certificates in the certification path if possible" and deselect the other options
- Select the **Password** check box and provide a password
- Provide a name and path for the PFX file (use .pfx extension)
- Press **Finish**
- Copy the created pfx file to your SharePoint 2013/2016 server or a share that you can reach from that server


## Step 2: Install the binary dependencies and download the custom PowerShell module
Before you can load and execute the needed module in step 3 you'll need to first install the following two dependencies on the SharePoint 2013/2016 server you're executing these steps on:
- [The 64-bit edition of Microsoft Online Services Sign-In Assistant](https://www.microsoft.com/en-us/download/details.aspx?id=41950)
- [Microsoft Online Services Module for Windows Powershell (64-bit)](http://go.microsoft.com/fwlink/p/?linkid=236297)

Once that's done you'll need to copy the [Connect-SPFarmToAAD function script](https://msdn.microsoft.com/en-us/library/office/dn155905.aspx) and save it to the filesystem with the name `Connect-SPFarmToAAD.psm1`.

## Step 3: Apply the new signing certificate and setup the low trust association
Start this step by running the `LowTrustConfigurationSession.ps1` script and provide as input:
- **PFXFile**: the location to the PFX file created in step 1
- **PFXPassword**: the password of the PFX file created in step 1
- **ConnectSPFarmToAADModulePath**: the path to the module named `Connect-SPFarmToAAD.psm1` downloaded in step 2

This script will perform the following tasks:
- Set the new STS signing certificate
- Reset the SharePoint Timer service
- Import the `Connect-SPFarmToAAD.psm1` module

```PowerShell
.\LowTrustConfigurationSession.ps1 -PFXFile "\\dc1\admin\certificates\STSTokenSigning.pfx" -PFXPassword "****" -ConnectSPFarmToAADModulePath "\\dc1\admin\lowtrust"
```

Once the above is done you've setup a session that's ready for the final step, performing the actual low trust configuration. You can do this by issuing the `Connect-SPFarmToAAD` cmdlet like shown in below samples:

```PowerShell
Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com

Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com –SharePointWeb https://fabrikam.com

Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com –SharePointWeb http://northwind.com -AllowOverHttp

Connect-SPFarmToAAD –AADDomain 'MyO365Domain.onmicrosoft.com' –SharePointOnlineUrl https://MyO365Domain.sharepoint.com –SharePointWeb http://northwind.com –AllowOverHttp –RemoveExistingACS –RemoveExistingSTS –RemoveExistingSPOProxy –RemoveExistingAADCredentials
```

The final steps are the doing an IISReset all the farm servers + perform a SharePoint Timer Server restart on all the servers of the farm except the one where you did run the `Connect-SPFarmToAAD` cmdlet as that was already done.



