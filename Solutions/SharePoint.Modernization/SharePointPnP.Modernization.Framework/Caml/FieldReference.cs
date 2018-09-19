namespace CamlBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a reference to a field within a query.
    /// </summary>
    public class FieldReference
    {
        /// <summary>
        /// Field alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// This specifies the sort order on a FieldRef.
        /// </summary>
        /// <remarks>
        /// Query defaults this to true when no value is specified.
        /// </remarks>
        public bool? Ascending { get; set; }

        /// <summary>
        /// Specifies the URL for the .aspx file that is used to create a Meeting Workspace site.
        /// </summary>
        public string CreateUrl { get; set; }

        /// <summary>
        /// This attribute provides the display name of the field that is referenced.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// This attribute is only supported within the ViewFields element. True if the field is 
        /// explicitly declared in the view definition and is not returned in a Fields enumeration inside a view.
        /// </summary>
        public bool? Explicit { get; set; }

        /// <summary>
        /// Field format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Specifies the GUID that identifies the field.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// If set to Primary, specifies that the field is the primary key for its table and thus
        /// uniquely identifies each record in the table.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Specifies the parent foreign list when the FieldRef element is a child of an Eq element
        /// in Join element. The value is an alias for the list that is defined by the ListAlias
        /// attribute of the Join element
        /// </summary>
        public string List { get; set; }

        /// <summary>
        /// When the field is a Lookup type, specifies that queries should look for the item by its
        /// unique item ID rather than the field value. This can be useful, for example, when multiple
        /// items have identical values in the field and you want to query for a specific item.
        /// </summary>
        /// <remarks>
        /// Query default this to false when no value is specified
        /// </remarks>
        public bool? LookupId { get; set; }

        /// <summary>
        /// This attribute provides the internal name of the field that is referenced.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies the type of reference for a field in an events list.
        /// </summary>
        public string RefType { get; set; }

        /// <summary>
        /// The ShowField attribute can be set to the field name to display. By default, a hyperlinked text
        /// from the Title field of the record in the external list is displayed. But the ShowField attribute
        /// can be used to override that and display another field from the external list.
        /// </summary>
        /// <remarks>
        /// The following data types are allowed as targets of a ShowField attribute: Text, Choice, and Counter.
        /// </remarks>
        public string ShowField { get; set; }

        /// <summary>
        /// Specifies that the field contains only text values.
        /// </summary>
        public bool? TextOnly { get; set; }

        /// <summary>
        /// Specifies the function that is applied to a totals column or a calculated column.
        /// </summary>
        public FieldReferenceFunctionType? Type { get; set; }

        /// <summary>
        /// Creates an instance of FieldReference with all it's properties set to default.
        /// </summary>
        public FieldReference()
        {
        }

        /// <summary>
        /// Creates an instance of FieldReference with the initial specified <param name="name"></param>. 
        /// </summary>
        /// <param name="name">Internal name of the field.</param>
        public FieldReference(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Uses <paramref name="fieldName"/> as a field reference internal name and returns
        /// a new <see cref="FieldReference"/>.
        /// </summary>
        /// <param name="fieldName">String to be used as FieldReference internal name.</param>
        public static implicit operator FieldReference(string fieldName)
        {
            return new FieldReference(fieldName);
        } 

        internal string GetCaml()
        {
            var values = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrEmpty(Alias))
            {
                values.Add(new KeyValuePair<string, string>("Alias", Alias));
            }

            if (!string.IsNullOrEmpty(CreateUrl))
            {
                values.Add(new KeyValuePair<string, string>("CreateURL", CreateUrl));
            }

            if (!string.IsNullOrEmpty(DisplayName))
            {
                values.Add(new KeyValuePair<string, string>("DisplayName", DisplayName));
            }

            if (!string.IsNullOrEmpty(Format))
            {
                values.Add(new KeyValuePair<string, string>("Format", Format));
            }

            if (!string.IsNullOrEmpty(Id))
            {
                values.Add(new KeyValuePair<string, string>("ID", Id));
            }

            if (!string.IsNullOrEmpty(Key))
            {
                values.Add(new KeyValuePair<string, string>("Key", Key));
            }

            if (!string.IsNullOrEmpty(List))
            {
                values.Add(new KeyValuePair<string, string>("List", List));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                values.Add(new KeyValuePair<string, string>("Name", Name));
            }

            if (!string.IsNullOrEmpty(RefType))
            {
                values.Add(new KeyValuePair<string, string>("RefType", RefType));
            }

            if (!string.IsNullOrEmpty(ShowField))
            {
                values.Add(new KeyValuePair<string, string>("ShowField", ShowField));
            }

            if (Type.HasValue)
            {
                values.Add(new KeyValuePair<string, string>("Type", GetTypeString(Type.Value)));
            }

            if (Ascending.HasValue)
            {
                values.Add(new KeyValuePair<string, string>("Ascending", Ascending.Value.ToString().ToUpperInvariant()));
            }

            if (Explicit.HasValue)
            {
                values.Add(new KeyValuePair<string, string>("Explicit", Explicit.ToString().ToUpperInvariant()));
            }

            if (LookupId.HasValue)
            {
                values.Add(new KeyValuePair<string, string>("LookupId", LookupId.Value.ToString().ToUpperInvariant()));
            }

            if (TextOnly.HasValue)
            {
                values.Add(new KeyValuePair<string, string>("TextOnly", TextOnly.Value.ToString().ToUpperInvariant()));
            }

            return $"<FieldRef {string.Join("", values.Select(kv => $"{kv.Key}='{kv.Value}'"))}/>";
        }

        private string GetTypeString(FieldReferenceFunctionType type)
        {
            switch (type)
            {
                case FieldReferenceFunctionType.Average:
                    return "AVG";
                case FieldReferenceFunctionType.Count:
                    return "COUNT";
                case FieldReferenceFunctionType.Maximum:
                    return "MAX";
                case FieldReferenceFunctionType.Minimum:
                    return "MIN";
                case FieldReferenceFunctionType.Sum:
                    return "SUM";
                case FieldReferenceFunctionType.StandardDeviation:
                    return "STDEV";
                case FieldReferenceFunctionType.Variance:
                    return "VAR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), type, null);
            }
        }
    }
}
