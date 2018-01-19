using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SharePoint.Modernization.Framework.Pages
{

    /// <summary>
    /// Base class for the page analyzers
    /// </summary>
    public abstract class BasePage
    {
        public ListItem page;
        public ClientContext cc;
        public PageTransformation pageTransformation;
        public bool forceCheckout;

        #region construction
        /// <summary>
        /// Constructs the base page class instance
        /// </summary>
        /// <param name="page">page ListItem</param>
        /// <param name="pageTransformation">page transformation model to use for extraction or transformation</param>
        public BasePage(ListItem page, PageTransformation pageTransformation)
        {
            this.page = page;
            this.cc = (page.Context as ClientContext);
            this.cc.RequestTimeout = Timeout.Infinite;

            this.pageTransformation = pageTransformation;

            page.ParentList.EnsureProperty(l => l.ForceCheckout);
            this.forceCheckout = page.ParentList.ForceCheckout;
        }
        #endregion

        /// <summary>
        /// Get's the type of the web part
        /// </summary>
        /// <param name="webPartXml">Web part xml to analyze</param>
        /// <returns>Type of the web part as fully qualified name</returns>
        public string GetType(string webPartXml)
        {
            string type = "Unknown";

            if (!string.IsNullOrEmpty(webPartXml))
            {
                var xml = XElement.Parse(webPartXml);
                var xmlns = xml.XPathSelectElement("*").GetDefaultNamespace();
                if (xmlns.NamespaceName.Equals("http://schemas.microsoft.com/WebPart/v3", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = xml.Descendants(xmlns + "type").FirstOrDefault().Attribute("name").Value;
                }
                else if (xmlns.NamespaceName.Equals("http://schemas.microsoft.com/WebPart/v2", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = $"{xml.Descendants(xmlns + "TypeName").FirstOrDefault().Value}, {xml.Descendants(xmlns + "Assembly").FirstOrDefault().Value}";
                }
            }

            return type;
        }

        /// <summary>
        /// Checks the PageTransformation XML data to know which properties need to be kept for the given web part and collects their values
        /// </summary>
        /// <param name="properties">Properties collection retrieved when we loaded the web part</param>
        /// <param name="webPartType">Type of the web part</param>
        /// <param name="webPartXml">Web part XML</param>
        /// <returns>Collection of the requested property/value pairs</returns>
        public Dictionary<string, string> Properties(PropertyValues properties, string webPartType, string webPartXml)
        {
            Dictionary<string, string> propertiesToKeep = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(webPartXml))
            {
                List<Property> propertiesToRetrieve = this.pageTransformation.BaseWebPart.Properties.ToList<Property>();
                var webPartProperties = this.pageTransformation.WebParts.Where(p => p.Type.Equals(webPartType, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (webPartProperties != null && webPartProperties.Properties != null)
                {
                    propertiesToRetrieve.AddRange(webPartProperties.Properties.ToList<Property>());
                }

                var xml = XElement.Parse(webPartXml);
                var xmlns = xml.XPathSelectElement("*").GetDefaultNamespace();
                if (xmlns.NamespaceName.Equals("http://schemas.microsoft.com/WebPart/v3", StringComparison.InvariantCultureIgnoreCase))
                {
                    // the retrieved properties are sufficient
                    foreach (var property in propertiesToRetrieve)
                    {
                        if (!string.IsNullOrEmpty(property.Name) && properties.FieldValues.ContainsKey(property.Name))
                        {
                            propertiesToKeep.Add(property.Name, properties[property.Name] != null ? properties[property.Name].ToString() : "");
                        }
                    }
                }
                else if (xmlns.NamespaceName.Equals("http://schemas.microsoft.com/WebPart/v2", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var property in propertiesToRetrieve)
                    {
                        if (!string.IsNullOrEmpty(property.Name))
                        {
                            if (properties.FieldValues.ContainsKey(property.Name))
                            {
                                propertiesToKeep.Add(property.Name, properties[property.Name] != null ? properties[property.Name].ToString() : "");
                            }
                            else
                            {
                                // check XMl for property
                                var v2Element = xml.Descendants(xmlns + property.Name).FirstOrDefault();
                                if (v2Element != null)
                                {
                                    propertiesToKeep.Add(property.Name, v2Element.Value);
                                }
                            }
                        }
                    }
                }
            }

            return propertiesToKeep;
        }


    }
}
