using SharePoint.Modernization.Framework.Pages;

namespace SharePoint.Modernization.Framework.Transform
{
    /// <summary>
    /// Interface implemented by all layout transformators
    /// </summary>
    public interface ILayoutTransformator
    {
        /// <summary>
        /// Transforms a classic wiki/webpart page layout into a modern client side page layout
        /// </summary>
        /// <param name="layout">Source wiki/webpart page layout</param>
        void Transform(PageLayout layout);
    }
}
