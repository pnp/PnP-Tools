using SharePoint.Modernization.Framework.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Transform
{
    public interface ILayoutTransformator
    {
        void ApplyLayout(PageLayout layout);
    }
}
