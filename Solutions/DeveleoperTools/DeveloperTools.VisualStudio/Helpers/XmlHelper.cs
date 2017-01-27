using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SharePointPnP.DeveloperTools.VisualStudio.Helpers
{
	class XmlHelper
	{
		private static XElement GetProvisioningTemplate(XDocument xml)
		{
			XElement res = null;
			if (xml.Root?.Name?.LocalName == "Provisioning" &&
				xml.Root.Name.NamespaceName != null &&
				xml.Root.Name.NamespaceName.StartsWith("http://schemas.dev.office.com/PnP/20", StringComparison.OrdinalIgnoreCase))
			{
				XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
				xnm.AddNamespace("pnp", xml.Root.Name.NamespaceName);
				res = xml.XPathSelectElement("/pnp:Provisioning/pnp:Templates/pnp:ProvisioningTemplate", xnm);
			}
			return res;
		}

		public static bool IsProvisioningTemplate(Stream stream)
		{
			bool res = false;
			try
			{
				XDocument xml = XDocument.Load(stream);
				res = GetProvisioningTemplate(xml) != null;
			}
			catch (Exception e)
			{
				//TODO trace
			}
			return res;
		}

		public static XDocument SetProvisioningTemplateAttribute(Stream stream, string attribute, string value)
		{
			XDocument xdoc = XDocument.Load(stream);
			var element = GetProvisioningTemplate(xdoc);
			if(element != null)
			{
				var xn = XName.Get(attribute, string.Empty);
				element.SetAttributeValue(xn, value);
			}
			return xdoc;
		}

		public static string GetProvisioningTemplateAttributeValue(Stream stream, string attribute)
		{
			string res = null;
			XDocument xdoc = XDocument.Load(stream);
			var element = GetProvisioningTemplate(xdoc);
			if (element != null)
			{
				var xn = XName.Get(attribute, string.Empty);
				var attr = element.Attribute(xn);
				if(attr != null)
				{
					res = attr.Value;
				}
			}
			return res;
		}

	}
}
