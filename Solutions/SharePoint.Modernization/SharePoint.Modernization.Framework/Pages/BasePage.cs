using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// Get's the type of the web part by detecting if from the available properties
        /// </summary>
        /// <param name="properties">Web part properties to analyze</param>
        /// <returns>Type of the web part as fully qualified name</returns>
        public string GetTypeFromProperties(PropertyValues properties)
        {
            // Check for XSLTListView web part
            string[] xsltWebPart = new string[] { "ListUrl", "ListId", "Xsl", "JSLink", "ShowTimelineIfAvailable" };                        
            if (CheckWebPartProperties(xsltWebPart, properties))
            {
                return WebParts.XsltListView;
            }

            // Check for ListView web part
            string[] listWebPart = new string[] { "ListViewXml", "ListName", "ListId", "ViewContentTypeId", "PageType" };
            if (CheckWebPartProperties(listWebPart, properties))
            {
                return WebParts.ListView;
            }

            // check for Media web part
            string[] mediaWebPart = new string[] { "AutoPlay", "MediaSource", "Loop", "IsPreviewImageSourceOverridenForVideoSet", "PreviewImageSource" };
            if (CheckWebPartProperties(mediaWebPart, properties))
            {
                return WebParts.Media;
            }

            // check for SlideShow web part
            string[] slideShowWebPart = new string[] { "LibraryGuid", "Layout", "Speed", "ShowToolbar", "ViewGuid" };
            if (CheckWebPartProperties(slideShowWebPart, properties))
            {
                return WebParts.PictureLibrarySlideshow;
            }

            // check for Chart web part
            string[] chartWebPart = new string[] { "ConnectionPointEnabled", "ChartXml", "DataBindingsString", "DesignerChartTheme" };
            if (CheckWebPartProperties(chartWebPart, properties))
            {
                return WebParts.Chart;
            }

            // check for Site Members web part
            string[] membersWebPart = new string[] { "NumberLimit", "DisplayType", "MembershipGroupId", "Toolbar" };
            if (CheckWebPartProperties(membersWebPart, properties))
            {
                return WebParts.Members;
            }

            // check for Silverlight web part
            string[] silverlightWebPart = new string[] { "MinRuntimeVersion", "WindowlessMode", "CustomInitParameters", "Url", "ApplicationXml" };
            if (CheckWebPartProperties(silverlightWebPart, properties))
            {
                return WebParts.Silverlight;
            }

            // check for Add-in Part web part
            string[] addinPartWebPart = new string[] { "FeatureId", "ProductWebId", "ProductId" };
            if (CheckWebPartProperties(addinPartWebPart, properties))
            {
                return WebParts.Client;
            }

            // check for Script Editor web part
            string[] scriptEditorWebPart = new string[] { "Content"};
            if (CheckWebPartProperties(scriptEditorWebPart, properties))
            {
                return WebParts.ScriptEditor;
            }

            // This needs to be last, but we still pages with sandbox user code web parts on them
            string[] sandboxWebPart = new string[] { "CatalogIconImageUrl", "AllowEdit", "TitleIconImageUrl", "ExportMode" };
            if (CheckWebPartProperties(sandboxWebPart, properties))
            {
                return WebParts.SPUserCode;
            }

            return "NonExportable_Unidentified";
        }

        private bool CheckWebPartProperties(string[] propertiesToCheck, PropertyValues properties)
        {
            bool isWebPart = true;
            foreach (var wpProp in propertiesToCheck)
            {
                if (!properties.FieldValues.ContainsKey(wpProp))
                {
                    isWebPart = false;
                    break;
                }
            }

            return isWebPart;
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

            List<Property> propertiesToRetrieve = this.pageTransformation.BaseWebPart.Properties.ToList<Property>();
            var webPartProperties = this.pageTransformation.WebParts.Where(p => p.Type.Equals(webPartType, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (webPartProperties != null && webPartProperties.Properties != null)
            {
                propertiesToRetrieve.AddRange(webPartProperties.Properties.ToList<Property>());
            }

            if (string.IsNullOrEmpty(webPartXml))
            {
                if (webPartType == WebParts.Client)
                {
                    // Special case since we don't know upfront which properties are relevant here...so let's take them all
                    foreach(var prop in properties.FieldValues)
                    {
                        propertiesToKeep.Add(prop.Key, prop.Value != null ? prop.Value.ToString() : "");
                    }
                }
                else
                {
                    // Special case where we did not have export rights for the web part XML, assume this is a V3 web part
                    foreach (var property in propertiesToRetrieve)
                    {
                        if (!string.IsNullOrEmpty(property.Name) && properties.FieldValues.ContainsKey(property.Name))
                        {
                            propertiesToKeep.Add(property.Name, properties[property.Name] != null ? properties[property.Name].ToString() : "");
                        }
                    }
                }
            }
            else
            {
                var xml = XElement.Parse(webPartXml);
                var xmlns = xml.XPathSelectElement("*").GetDefaultNamespace();
                if (xmlns.NamespaceName.Equals("http://schemas.microsoft.com/WebPart/v3", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (webPartType == WebParts.Client)
                    {
                        // Special case since we don't know upfront which properties are relevant here...so let's take them all
                        foreach (var prop in properties.FieldValues)
                        {
                            propertiesToKeep.Add(prop.Key, prop.Value != null ? prop.Value.ToString() : "");
                        }
                    }
                    else
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

                                // Some properties do have their own namespace defined
                                if (webPartType == WebParts.SimpleForm && property.Name.Equals("Content", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    // Load using the http://schemas.microsoft.com/WebPart/v2/SimpleForm namespace
                                    XNamespace xmlcontentns = "http://schemas.microsoft.com/WebPart/v2/SimpleForm";
                                    v2Element = xml.Descendants(xmlcontentns + property.Name).FirstOrDefault();
                                    if (v2Element != null)
                                    {
                                        propertiesToKeep.Add(property.Name, v2Element.Value);
                                    }
                                }
                                else if (webPartType == WebParts.ContentEditor)
                                {
                                    if (property.Name.Equals("ContentLink", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("Content", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("PartStorage", StringComparison.InvariantCultureIgnoreCase) )
                                    {
                                        XNamespace xmlcontentns = "http://schemas.microsoft.com/WebPart/v2/ContentEditor";
                                        v2Element = xml.Descendants(xmlcontentns + property.Name).FirstOrDefault();
                                        if (v2Element != null)
                                        {
                                            propertiesToKeep.Add(property.Name, v2Element.Value);
                                        }
                                    }
                                }
                                else if (webPartType == WebParts.Xml)
                                {
                                    if (property.Name.Equals("XMLLink", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("XML", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("XSLLink", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("XSL", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("PartStorage", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        XNamespace xmlcontentns = "http://schemas.microsoft.com/WebPart/v2/Xml";
                                        v2Element = xml.Descendants(xmlcontentns + property.Name).FirstOrDefault();
                                        if (v2Element != null)
                                        {
                                            propertiesToKeep.Add(property.Name, v2Element.Value);
                                        }
                                    }
                                }
                                else if (webPartType == WebParts.SiteDocuments)
                                {
                                    if (property.Name.Equals("UserControlledNavigation", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("ShowMemberships", StringComparison.InvariantCultureIgnoreCase) ||
                                        property.Name.Equals("UserTabs", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        XNamespace xmlcontentns = "urn:schemas-microsoft-com:sharepoint:portal:sitedocumentswebpart";
                                        v2Element = xml.Descendants(xmlcontentns + property.Name).FirstOrDefault();
                                        if (v2Element != null)
                                        {
                                            propertiesToKeep.Add(property.Name, v2Element.Value);
                                        }
                                    }
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
