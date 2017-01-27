using EnvDTE;
using SharePointPnP.DeveloperTools.VisualStudio.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	[ComVisible(true)]
	public class SiteTemplatePropertiesExtender : ISiteTemplateExtender, IDisposable
	{
		private readonly IExtenderSite extenderSite;
		private readonly int cookie;
		private bool disposed;
		private string itemPath;

		[DisplayName("Display Name")]
		[Category("SharePoint PnP")]
		[Description("Specifies site template display name")]
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
		[Description("Specifies site template image preview URL")]
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

		[DisplayName("SharePoint Online")]
		[Category("SharePoint PnP Target Platform")]
		[Description("Specifies if site template supports SharePoint Online")]
		public bool SupportSPO { get; set; }

		[DisplayName("SharePoint 2016")]
		[Category("SharePoint PnP Target Platform")]
		[Description("Specifies if site template supports SharePoint 2016")]
		public bool SupportSP16 { get; set; }

		[DisplayName("SharePoint 2013")]
		[Category("SharePoint PnP Target Platform")]
		[Description("Specifies if site template supports SharePoint 2013")]
		public bool SupportSP13 { get; set; }

		public SiteTemplatePropertiesExtender(IExtenderSite extenderSite, int cookie, object extendee)
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
	}
}
