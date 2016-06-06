using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Models
{
    public abstract class ProjectItemRequestBase
    {
        public ProjectItemRequestBase()
        {
            this.RequestedOn = System.DateTime.Now;
        }

        public ProvisioningTemplateLocationInfo TemplateInfo { get; set; }
        public string ItemPath { get; set; }
        public string ItemKind { get; set; }
        public ProjectItemRequestType RequestType { get; protected set; }
        public DateTime RequestedOn { get; private set; }
    }

    public class ProjectItemRequestAdd : ProjectItemRequestBase
    {
        public ProjectItemRequestAdd()
        {
            this.RequestType = ProjectItemRequestType.Add;
        }
    }

    public class ProjectItemRequestRemove : ProjectItemRequestBase
    {
        public ProjectItemRequestRemove()
        {
            this.RequestType = ProjectItemRequestType.Remove;
        }
    }

    public class ProjectItemRequestRename : ProjectItemRequestBase
    {
        public ProjectItemRequestRename()
        {
            this.RequestType = ProjectItemRequestType.Rename;
        }
        public string OldName { get; set; }
    }

    public enum ProjectItemRequestType
    {
        Add,
        Remove,
        Rename
    }
}
