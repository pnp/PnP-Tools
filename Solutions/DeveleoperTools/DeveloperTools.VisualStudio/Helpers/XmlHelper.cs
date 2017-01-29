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

		public static bool GetProvisioningTemplatePropertyBool(Stream stream, string key)
		{
			bool res = false;
			XDocument xdoc = XDocument.Load(stream);
			var element = GetProvisioningTemplateProperty(xdoc, key);
			if (element != null)
			{
				var xn = XName.Get("Value", string.Empty);
				var attr = element.Attribute(xn);
				if (attr != null)
				{
					if(!bool.TryParse(attr.Value, out res))
					{
						res = false;
					}
				}
			}
			return res;
		}

		public static XDocument SetProvisioningTemplatePropertyValue(Stream stream, string key, string value)
		{
			XDocument xdoc = XDocument.Load(stream);
			var element = EnsureProvisioningTemplateProperty(xdoc, key);
			if (element != null)
			{
				var xn = XName.Get("Value", string.Empty);
				element.SetAttributeValue(xn, value);
			}
			return xdoc;
		}

		private static XElement EnsureProvisioningTemplateProperty(XDocument xml, string key)
		{
			XElement res = null;
			if (xml.Root?.Name?.LocalName == "Provisioning" &&
				xml.Root.Name.NamespaceName != null &&
				xml.Root.Name.NamespaceName.StartsWith("http://schemas.dev.office.com/PnP/20", StringComparison.OrdinalIgnoreCase))
			{
				XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
				var pnpNs = xml.Root.Name.NamespaceName;
				xnm.AddNamespace("pnp", pnpNs);
				var template = xml.XPathSelectElement("/pnp:Provisioning/pnp:Templates/pnp:ProvisioningTemplate", xnm);
				if (template != null)
				{
					var props = template.XPathSelectElement("pnp:Properties", xnm);
					if (props == null)
					{
						props = new XElement(XName.Get("Properties", pnpNs), "");
						template.Add(props);
						res = new XElement(XName.Get("Property", pnpNs), new XAttribute(XName.Get("Key", ""), key));
						props.Add(res);
					}
					else
					{
						res = props.XPathSelectElement($"pnp:Property[@Key='{key}']", xnm);
						if (res == null)
						{
							res = new XElement(XName.Get("Property", pnpNs), new XAttribute(XName.Get("Key", ""), key));
							props.Add(res);
						}
					}
				}
			}
			return res;
		}

		private static XElement GetProvisioningTemplateProperty(XDocument xml, string key)
		{
			XElement res = null;
			if (xml.Root?.Name?.LocalName == "Provisioning" &&
				xml.Root.Name.NamespaceName != null &&
				xml.Root.Name.NamespaceName.StartsWith("http://schemas.dev.office.com/PnP/20", StringComparison.OrdinalIgnoreCase))
			{
				XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
				xnm.AddNamespace("pnp", xml.Root.Name.NamespaceName);
				res = xml.XPathSelectElement($"/pnp:Provisioning/pnp:Templates/pnp:ProvisioningTemplate/pnp:Properties/pnp:Property[@Key='{key}']", xnm);
			}
			return res;
		}
	}
}
