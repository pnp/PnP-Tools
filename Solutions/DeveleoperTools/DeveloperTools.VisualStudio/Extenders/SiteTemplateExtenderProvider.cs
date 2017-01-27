using EnvDTE;
using SharePointPnP.DeveloperTools.VisualStudio.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj80;

namespace SharePointPnP.DeveloperTools.VisualStudio.Extenders
{
	public class SiteTemplateExtenderProvider : IExtenderProvider
	{
		private const string SiteTemplateItemType = "PnPSiteTemplate";

		private ISiteTemplateExtender _extender;
		public object GetExtender(string extenderCatid, string extenderName,
			 object extendeeObject, IExtenderSite extenderSite,
			int cookie)
		{
			return _extender = CanExtend(extenderCatid, extenderName, extendeeObject) ?
				new SiteTemplatePropertiesExtender(extenderSite, cookie) : null;
		}

		public bool CanExtend(string extenderCatid, string extenderName, object extendeeObject)
		{
			var itemType = extendeeObject.GetPropertyValue<string>("ItemType");
			return itemType == SiteTemplateItemType;
		}
	}
}
