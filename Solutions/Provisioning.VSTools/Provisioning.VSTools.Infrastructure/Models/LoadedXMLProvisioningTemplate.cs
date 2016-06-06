using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Models
{
    public class LoadedXMLProvisioningTemplate : ProvisioningTemplateLocationInfo
    {
        public XMLFileSystemTemplateProvider Provider { get; set; }
        public ProvisioningTemplate Template { get; set; }
    }
}
