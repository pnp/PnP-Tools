using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Modernization.Framework.Transform
{
    interface IPageTransformator
    {
        void Transform(ListItem sourcePage, string targetPageName);

    }
}
