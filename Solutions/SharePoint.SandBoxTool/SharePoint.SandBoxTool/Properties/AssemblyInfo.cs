using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

#if !ONPREMISES
[assembly: AssemblyTitle("SharePoint PnP Sandbox Tool for SharePoint Online")]
[assembly: AssemblyProduct("SharePoint PnP Sandbox Tool for SharePoint Online")]
#elif SP2013
[assembly: AssemblyTitle("SharePoint PnP Sandbox Tool for SharePoint 2013")]
[assembly: AssemblyProduct("SharePoint PnP Sandbox Tool for SharePoint 2013")]
#else
[assembly: AssemblyTitle("SharePoint PnP Sandbox Tool for SharePoint 2016")]
[assembly: AssemblyProduct("SharePoint PnP Sandbox Tool for SharePoint 2016")]
#endif
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SharePoint PnP")]
[assembly: AssemblyCopyright("Copyright © 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("95f45f04-1e1d-47fe-a862-9f8fd84d4b5a")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.3.0")]
[assembly: AssemblyFileVersion("1.0.3.0")]
