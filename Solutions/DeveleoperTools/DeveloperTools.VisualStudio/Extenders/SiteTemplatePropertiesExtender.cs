using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	[ComVisible(true)]
	public class SiteTemplatePropertiesExtender : ISiteTemplateExtender, IDisposable
	{
		// These attibutes supply the property with some information
		// on how to display and which UITypeEditor to use.
		[DisplayName("Display Name")]
		[Category("SharePoint PnP")]
		[Description("Specifies site template display name")]
		//[Editor(typeof(CustomUiTypeEditor), typeof(UITypeEditor))]
		public string DisplayName { get; set; }

		private readonly IExtenderSite extenderSite;
		private readonly int cookie;
		private bool disposed;

		public SiteTemplatePropertiesExtender(IExtenderSite extenderSite, int cookie)
		{
			this.extenderSite = extenderSite;
			this.cookie = cookie;
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
	}
}
