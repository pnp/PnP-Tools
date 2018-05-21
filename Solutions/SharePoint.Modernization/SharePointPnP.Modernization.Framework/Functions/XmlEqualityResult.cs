// Eli Algranti Copyright ©  2013
// Code taken from http://xmlspecificationcompare.codeplex.com/
using System.Xml.Linq;

namespace SharePointPnP.Modernization.Framework.Functions
{
    /// <summary>
    /// The result of an equiality comparison with <see cref="XmlComparer"/>
    /// </summary>
    public class XmlEqualityResult
    {
        private const string DefaultError = "Can't find match for subtree.";
        private const string SuccessMessage = "Success";

        /// <summary>
        /// Gets whether the match was successful
        /// </summary>
        public bool Success
        {
            get { return FailObject == null; }
        }

        /// <summary>
        /// Gets or sets the object that failed the match
        /// </summary>
        public XObject FailObject { get; set; }


        private string _errorMessage;

        /// <summary>
        /// Gets or sets a descriptive error message if the match failed.
        /// </summary>
        /// <remarks>
        /// If set to null or not set the default Error Message is returned.
        /// </remarks>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage ?? (Success ? SuccessMessage : DefaultError);
            }

            set
            {
                _errorMessage = value;
            }
        }
    }
}
