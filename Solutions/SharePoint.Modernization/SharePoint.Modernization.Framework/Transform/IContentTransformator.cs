using SharePoint.Modernization.Framework.Entities;
using System.Collections.Generic;

namespace SharePoint.Modernization.Framework.Transform
{
    public interface IContentTransformator
    {
        void Transform(List<WebPartEntity> webParts);
    }
}
