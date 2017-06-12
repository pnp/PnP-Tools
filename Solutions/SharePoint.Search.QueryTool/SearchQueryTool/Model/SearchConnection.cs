using System;
using System.IO;
using System.Xml.Linq;

namespace SearchQueryTool.Model
{
    public class SearchConnection
    {
        public string SpSiteUrl { get; set; }
        public string Timeout { get; set; }
        public string Accept { get; set; }
        public string HttpMethod { get; set; }
        public int AuthTypeIndex { get; set; }
        public int AuthMethodIndex { get; set; }
        public string Username { get; set; }
        public bool EnableExperimentalFeatures { get; set; }

        public XElement GetXml()
        {
            var connectionPropsElm = new XElement("Connection-Props");
            connectionPropsElm.Add(new XElement("spsiteurl", SpSiteUrl));
            connectionPropsElm.Add(new XElement("timeout", Timeout));
            connectionPropsElm.Add(new XElement("accept", Accept));
            connectionPropsElm.Add(new XElement("httpmethod", HttpMethod));
            if (!String.IsNullOrWhiteSpace(Username))
            {
                connectionPropsElm.Add(new XElement("username", Username));
            }
            connectionPropsElm.Add(new XElement("authtype", AuthTypeIndex));
            connectionPropsElm.Add(new XElement("authmethod", AuthMethodIndex));
            connectionPropsElm.Add(new XElement("experimental", EnableExperimentalFeatures));

            return connectionPropsElm;
        }

        public void Load(string path)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var connectionPropFilePath = Path.Combine(Environment.CurrentDirectory, path);
                    if (File.Exists(connectionPropFilePath))
                    {
                        var connectionPropsElm = XElement.Load(connectionPropFilePath);
                        if (connectionPropsElm.HasElements)
                        {
                            SpSiteUrl = (string)connectionPropsElm.Element("spsiteurl");
                            Timeout = (string)connectionPropsElm.Element("timeout");
                            Accept = (string)connectionPropsElm.Element("accept");
                            HttpMethod = (string)connectionPropsElm.Element("httpmethod");
                            AuthTypeIndex = (int)connectionPropsElm.Element("authtype");
                            AuthMethodIndex = (int)connectionPropsElm.Element("authmethod");
                            Username = (string)connectionPropsElm.Element("username");
                            EnableExperimentalFeatures = (bool)connectionPropsElm.Element("experimental");
                        }
                    } 
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Failed to load search connection from path {0}, error: {1}", path, ex.Message));
                }               
            }
        }

        public void SaveXml(string outputPath)
        {
            try
            {
                var xml = GetXml();
                using (var fs = new FileStream(outputPath, FileMode.Create))
                {
                    xml.Save(fs);
                }
            }
            catch (Exception ex)
            {       
                throw new Exception(String.Format("Failed to save XML, error: {0}", ex.Message));
            } 
        }

    }
}
