using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Provisioning.VSTools.Models
{
    public class ProvisioningCredentials
    {
        public string Username { get; set; }

        public void SetSecurePassword(string password)
        {
            var securePw = Provisioning.VSTools.Helpers.SecureStringHelper.ToSecureString(password);
            this.SecurePassword = Provisioning.VSTools.Helpers.SecureStringHelper.EncryptString(securePw);
        }

        public System.Security.SecureString GetSecurePassword()
        {
            return Provisioning.VSTools.Helpers.SecureStringHelper.DecryptString(this.SecurePassword);
        }

        public string SecurePassword { get; set; }

        public string AuthType { get { return "Office365"; } }
        
        [System.Xml.Serialization.XmlIgnore]
        public string FilePath { get; set; }
    }
}
