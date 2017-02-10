using EnvDTE;
using SharePointPnP.DeveloperTools.VisualStudio.Helpers;
using System;
using System.IO;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	public class ProvisioingTemplateExtenderProvider : IExtenderProvider
	{
		private const string ProvisioningTemplateItemType = "PnPTemplate";

		private IProvisioingTemplateExtender extender;
		public object GetExtender(string extenderCatid, string extenderName,
			 object extendeeObject, IExtenderSite extenderSite,
			int cookie)
		{
			return extender = CanExtend(extenderCatid, extenderName, extendeeObject) ?
				new ProvisioingTemplateExtender(extenderSite, cookie, extendeeObject) : null;
		}

		public bool CanExtend(string extenderCatid, string extenderName, object extendeeObject)
		{
			bool res = false;
			try
			{
				var itemType = extendeeObject.GetPropertyValue<string>("ItemType");
				if (itemType == ProvisioningTemplateItemType)
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
