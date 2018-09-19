using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.Modernization.Framework.Entities
{
    /// <summary>
    /// Holds information about the defined audiences
    /// </summary>
    public class AudienceEntity
    {
        public AudienceEntity()
        {
            this.GlobalAudiences = new List<string>();
            this.SecurityGroups = new List<string>();
            this.SharePointGroups = new List<string>();
        }

        public List<string> GlobalAudiences { get; set; }
        public List<string> SecurityGroups { get; set; }
        public List<string> SharePointGroups { get; set; }
    }
}
