using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SearchQueryTool.Helpers
{
    public class XmlHelper
    {
        public static string PrintXml(String xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static string PrettyXml(string xml)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true, 
                Indent = true, 
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }
    }
}