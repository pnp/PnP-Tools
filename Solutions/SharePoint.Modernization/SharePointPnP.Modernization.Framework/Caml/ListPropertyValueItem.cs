namespace CamlBuilder
{
    using System;

    /// <summary>
    /// Represents an item to be used by ListProperty value.
    /// </summary>
    public class ListPropertyValueItem
    {
        /// <summary>
        /// True to surround text with anchor tags if the text appears like a
        /// hyperlink, for example, www.microsoft.com.
        /// </summary>
        public bool? AutoHyperLink { get; set; }

        /// <summary>
        /// True to surround text with anchor tags if the text appears like a
        /// hyperlink (for example, www.microsoft.com) but without HTML encoding.
        /// </summary>
        public bool? AutoHyperLinkNoEncoding { get; set; }

        /// <summary>
        /// True to insert break-line tags into the text stream and to
        /// replace multiple spaces with a nonbreaking space (&nbsp;).
        /// </summary>
        public bool? AutoNewLine { get; set; }

        /// <summary>
        /// Sets the default ProgID for the application that created the list.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// True to re-pass the rendered content through the Collaborative Application
        /// Markup Language (CAML) interpreter, which allows CAML to render CAML.
        /// </summary>
        public bool? ExpandXml { get; set; }

        /// <summary>
        /// True to convert embedded characters so that they are displayed as text in the
        /// browser. In other words, characters that could be confused with HTML tags are 
        /// converted to entities.
        /// </summary>
        public bool? HtmlEncode { get; set; }

        /// <summary>
        /// Specifies a field in the List of Lists table.
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// True to remove white space from the beginning and end of the value returned by the element.
        /// </summary>
        public bool? StripWs { get; set; }

        /// <summary>
        /// True to convert special characters, such as spaces, to quoted UTF-8 format,
        /// for example, %c3%ab for character ë.
        /// </summary>
        public bool? UrlEncode { get; set; }

        /// <summary>
        /// Like URLEncode, but true to specify that the string to encode is a path component of a
        /// URL and not to encode the forward slash (/).
        /// </summary>
        public bool? UrlEncodeAsUrl { get; set; }

        /// <summary>
        /// Creates an instance of ListPropertyValueItem with initial specified <paramref name="select"/>
        /// </summary>
        /// <param name="select">Specifies a field in the List of Lists table.</param>
        public ListPropertyValueItem(string select)
        {
            if (string.IsNullOrEmpty(select))
            {
                throw new ArgumentNullException(nameof(select));
            }

            Select = select;
        }
    }
}