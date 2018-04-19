namespace SharePoint.Modernization.Framework.Transform
{
    /// <summary>
    /// Interface implemented by all html transformators
    /// </summary>
    public interface IHtmlTransformator
    {

        /// <summary>
        /// Transforms the passed html to be usable by the client side text part
        /// </summary>
        /// <param name="text">Html to be transformed</param>
        /// <returns>Html that can be used and edited via the client side text part</returns>
        string Transform(string text);
    }
}
