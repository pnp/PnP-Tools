using EnvDTE;
using SharePointPnP.DeveloperTools.VisualStudio.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj80;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	public class SiteTemplateExtenderProvider : IExtenderProvider
	{
		private const string SiteTemplateItemType = "PnPSiteTemplate";

		private ISiteTemplateExtender extender;
		public object GetExtender(string extenderCatid, string extenderName,
			 object extendeeObject, IExtenderSite extenderSite,
			int cookie)
		{
			return extender = CanExtend(extenderCatid, extenderName, extendeeObject) ?
				new SiteTemplatePropertiesExtender(extenderSite, cookie, extendeeObject) : null;
		}

		public bool CanExtend(string extenderCatid, string extenderName, object extendeeObject)
		{
			bool res = false;
			try
			{
				var itemType = extendeeObject.GetPropertyValue<string>("ItemType");
				if (itemType == SiteTemplateItemType)
				{
					var fullPath = extendeeObject.GetPropertyValue<string>("FullPath");
					if (!string.IsNullOrEmpty(fullPath))
					{
						using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
						{
							res = XmlHelper.IsProvisioningTemplate(stream);
						}
					}
				}
			}
			catch (Exception e)
			{
				//TODO trace
			}
			return res;
		}
	}
}
