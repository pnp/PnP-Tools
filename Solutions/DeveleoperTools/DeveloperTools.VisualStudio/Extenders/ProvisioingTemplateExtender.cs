using EnvDTE;
using SharePointPnP.DeveloperTools.VisualStudio.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	[ComVisible(true)]
	public class ProvisioingTemplateExtender : IProvisioingTemplateExtender, IDisposable
	{
		private readonly IExtenderSite extenderSite;
		private readonly int cookie;
		private bool disposed;
		private string itemPath;

		#region IProvisioingTemplateExtender
		[DisplayName("Display Name")]
		[Category("SharePoint PnP")]
		[Description("Specifies provisioning template display name")]
		public string DisplayName
		{
			get
			{
				return GetProvisioningTemplateAttributeValue("DisplayName");
			}
			set
			{
				SetProvisioningTemplateAttributeValue("DisplayName", value);
			}
		}

		[DisplayName("Image Preview URL")]
		[Category("SharePoint PnP")]
		[Description("Specifies provisioning template image preview URL")]
		public string ImagePreviewUrl
		{
			get
			{
				return GetProvisioningTemplateAttributeValue("ImagePreviewUrl");
			}
			set
			{
				SetProvisioningTemplateAttributeValue("ImagePreviewUrl", value);
			}
		}

		[DisplayName("Version")]
		[Category("SharePoint PnP")]
		[Description("Specifies provisioning template version")]
		public string Version
		{
			get
			{
				return GetProvisioningTemplateAttributeValue("Version");
			}
			set
			{
				SetProvisioningTemplateAttributeValue("Version", value);
			}
		}

		[DisplayName("SharePoint Online")]
		[Category("SharePoint PnP")]
		[Description("Specifies if provisioning template supports SharePoint Online")]
		public bool SupportSPO
		{
			get
			{
				return GetProvisioningTemplatePropertyBool("PnP_Supports_SPO_Platform");
			}
			set
			{
				SetProvisioningTemplatePropertyValue("PnP_Supports_SPO_Platform", value.ToString());
			}
		}

		[DisplayName("SharePoint 2016")]
		[Category("SharePoint PnP")]
		[Description("Specifies if provisioning template supports SharePoint 2016")]
		public bool SupportSP16
		{
			get
			{
				return GetProvisioningTemplatePropertyBool("PnP_Supports_SP2016_Platform");
			}
			set
			{
				SetProvisioningTemplatePropertyValue("PnP_Supports_SP2016_Platform", value.ToString());
			}
		}


		[DisplayName("SharePoint 2013")]
		[Category("SharePoint PnP")]
		[Description("Specifies if provisioning template supports SharePoint 2013")]
		public bool SupportSP13
		{
			get
			{
				return GetProvisioningTemplatePropertyBool("PnP_Supports_SP2013_Platform");
			}
			set
			{
				SetProvisioningTemplatePropertyValue("PnP_Supports_SP2013_Platform", value.ToString());
			}
		}

		#endregion

		public ProvisioingTemplateExtender(IExtenderSite extenderSite, int cookie, object extendee)
		{
			this.extenderSite = extenderSite;
			this.cookie = cookie;
			itemPath = extendee.GetPropertyValue<string>("FullPath");
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && cookie != 0)
				{
					extenderSite.NotifyDelete(cookie);
				}
				disposed = true;
			}
		}

		private void SetProvisioningTemplateAttributeValue(string name, string value)
		{
			if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(itemPath))
			{
				try
				{
					XDocument xml = null;
					using (var stream = new FileStream(itemPath, FileMode.Open, FileAccess.Read))
					{
						xml = XmlHelper.SetProvisioningTemplateAttribute(stream, name, value);
					}
					xml?.Save(itemPath);
				}
				catch (Exception e)
				{
					//TODO trace
				}
			}
		}

		private string GetProvisioningTemplateAttributeValue(string name)
		{
			string res = null;
			if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(itemPath))
			{
				try
				{
					using (var stream = new FileStream(itemPath, FileMode.Open, FileAccess.Read))
					{
						res = XmlHelper.GetProvisioningTemplateAttributeValue(stream, name);
					}
				}
				catch (Exception e)
				{
					//TODO trace
				}
			}
			return res;
		}

		private bool GetProvisioningTemplatePropertyBool(string name)
		{
			bool res = false;
			if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(itemPath))
			{
				try
				{
					using (var stream = new FileStream(itemPath, FileMode.Open, FileAccess.Read))
					{
						res = XmlHelper.GetProvisioningTemplatePropertyBool(stream, name);
					}
				}
				catch (Exception e)
				{
					//TODO trace
				}
			}
			return res;
		}


		private void SetProvisioningTemplatePropertyValue(string name, string value)
		{
			if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(itemPath))
			{
				try
				{
					XDocument xml = null;
					using (var stream = new FileStream(itemPath, FileMode.Open, FileAccess.Read))
					{
						xml = XmlHelper.SetProvisioningTemplatePropertyValue(stream, name, value);
					}
					xml?.Save(itemPath);
				}
				catch (Exception e)
				{
					//TODO trace
				}
			}
		}

	}
}
