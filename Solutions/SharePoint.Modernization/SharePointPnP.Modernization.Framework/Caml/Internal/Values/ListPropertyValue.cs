namespace CamlBuilder.Internal.Values
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ListPropertyValue : Value
    {
        private readonly ListPropertyValueItem[] items;

        public ListPropertyValue(
            ValueType type, 
            bool? includeTimeValue,
            IEnumerable<ListPropertyValueItem> items)
            : base(type, includeTimeValue)
        {
            this.items = items.ToArray();
        }

        protected override string GetCamlValue()
        {
            return string.Join("\n", items.Select(GetItemElement));
        }

        private string GetItemElement(ListPropertyValueItem item)
        {
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    "Select",
                    item.Select)
            };
            
            if (item.AutoHyperLink.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "AutoHyperLink",
                    item.AutoHyperLink.Value.ToString().ToUpper()));
            }

            if (item.AutoHyperLinkNoEncoding.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "AutoHyperLinkNoEncoding",
                    item.AutoHyperLinkNoEncoding.Value.ToString().ToUpper()));
            }

            if (item.AutoHyperLink.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "AutoHyperLink",
                    item.AutoHyperLink.Value.ToString().ToUpper()));
            }

            if (item.AutoNewLine.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "AutoNewLine",
                    item.AutoNewLine.Value.ToString().ToUpper()));
            }

            if (!string.IsNullOrEmpty(item.Default))
            {
                values.Add(new KeyValuePair<string, string>(
                    "Default",
                    item.Default));
            }

            if (item.ExpandXml.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "ExpandXML",
                    item.ExpandXml.Value.ToString().ToUpper()));
            }

            if (item.HtmlEncode.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "HTMLEncode",
                    item.HtmlEncode.Value.ToString().ToUpper()));
            }

            if (item.StripWs.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "StripWS",
                    item.StripWs.Value.ToString().ToUpper()));
            }

            if (item.UrlEncode.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "URLEncode",
                    item.UrlEncode.Value.ToString().ToUpper()));
            }

            if (item.UrlEncodeAsUrl.HasValue)
            {
                values.Add(new KeyValuePair<string, string>(
                    "URLEncodeAsURL",
                    item.UrlEncodeAsUrl.Value.ToString().ToUpper()));
            }

            return $"<ListProperty {string.Join(" ", values.Select(kv => $"{kv.Key}='{kv.Value}'"))}></ListProperty>";
        } 
    }
}
