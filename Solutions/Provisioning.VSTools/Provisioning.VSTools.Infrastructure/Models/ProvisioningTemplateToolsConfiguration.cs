using System.Xml.Serialization;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace Provisioning.VSTools.Models
{
    public class ProvisioningTemplateToolsConfiguration
    {
        public ProvisioningTemplateToolsConfiguration()
        {
            this.EnsureInitialState();
        }

        private bool _toolsEnabled = false;
        public bool ToolsEnabled
        {
            get
            {
                //only return enabled value if we have deployment credentials, otherwise enforce false.
                if (Deployment != null && Deployment.IsValid)
                {
                    return _toolsEnabled;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                _toolsEnabled = value;
            }
        }
        public Deployment Deployment { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public string FilePath { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public string ProjectPath { get; set; }

        [XmlArray("Templates")]
        [XmlArrayItem("Template")]
        public List<Template> Templates { get; set; }

        public void EnsureInitialState()
        {
            //ensure deployment is not null
            if (Deployment == null)
            {
                Deployment = new Deployment();
            }

            //ensure creds is not null
            if (Deployment.Credentials == null)
            {
                Deployment.Credentials = new ProvisioningCredentials();
            }

            //ensure templates is not null
            if (Templates == null)
            {
                this.Templates = new List<Template>();
            }
        }
    }

    public class Deployment
    {
        public string TargetSite { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public ProvisioningCredentials Credentials { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(TargetSite) && Credentials != null && !string.IsNullOrEmpty(Credentials.Username) && !string.IsNullOrEmpty(Credentials.SecurePassword);
            }
        }
    }

    public class Template
    {
        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "ResourcesFolder")]
        public string ResourcesFolder { get; set; }
    }
}
