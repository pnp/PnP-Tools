namespace CamlBuilder
{
    using System.Collections.Generic;
    using System.Text;
    using Internal.Values;

    /// <summary>
    /// Defines a CAML value. This class has no constructors available.To instanciate a
    /// new value use public static methods.
    /// </summary>
    public abstract class Value
    {
        /// <summary>
        /// Specifies the data type for the value contained by this element.
        /// </summary>
        public ValueType Type { get; }

        /// <summary>
        /// Specifies to build DateTime queries based on time as well as date. If you do not set
        /// this attribute, the time portion of queries that involve date and time are ignored.
        /// </summary>
        public bool? IncludeTimeValue { get; set; }

        protected internal Value(ValueType type)
            : this(type, null)
        {
        }

        protected internal Value(ValueType type, bool? includeTimeValue)
        {
            Type = type;
            IncludeTimeValue = includeTimeValue;
        }

        internal string GetCaml()
        {
            var sb = new StringBuilder();

            if (IncludeTimeValue.HasValue)
            {
                sb.Append($"<Value Type='{Type}' IncludeTimeValue='{(IncludeTimeValue.Value ? "TRUE" : "FALSE")}'>");
            }
            else
            {
                sb.Append($"<Value Type='{Type}'>");
            }

            sb.Append(GetCamlValue());
            sb.Append("</Value>");

            return sb.ToString();
        }

        protected abstract string GetCamlValue();

        /// <summary>
        /// Value representng the current date and time.
        /// </summary>
        /// <returns>Value representng the current date and time.</returns>
        public static Value Now()
        {
            return new NowValue(null);
        }

        /// <summary>
        /// Value representing the current date and time.
        /// </summary>
        /// <param name="includeTimeValue">True if is to be included the time part; otherwise, false</param>
        /// <returns>Value representng the current date and time.</returns>
        public static Value Now(bool includeTimeValue)
        {
            return new NowValue(includeTimeValue);
        }

        /// <summary>
        /// Value representing the current month.
        /// 
        /// Can be used in together with <see cref="Operator.DateRangesOverlap(FieldReference,ValueType,object)"/> operator
        /// to retrieve from a calendar all instances of a recurring event that occur within a month.
        /// </summary>
        /// <returns>Value representing the current month.</returns>
        public static Value Month()
        {
            return new MonthValue(null);
        }

        /// <summary>
        /// Can be used in together with <see cref="Operator.DateRangesOverlap(FieldReference,ValueType,object)"/> operator
        /// to retrieve from a calendar all instances of a recurring event that occur within a month.
        /// </summary>
        /// <param name="includeTimeValue">True if is to be included the time part; otherwise, false</param>
        /// <returns>Value representing the current month.</returns>
        public static Value Month(bool includeTimeValue)
        {
            return new MonthValue(includeTimeValue);
        }

        /// <summary>
        /// Value representing the current day.
        /// 
        /// Renders the current date in the format that is relative to the server's local time zone. For servers in
        /// the United States, the format is MM/DD/YYYY (for example, 1/21/2001).
        /// </summary>
        /// <returns>Value representing the current day.</returns>
        public static Value Today()
        {
            return new TodayValue(null, null);
        }

        /// <summary>
        /// Value representing the current day.
        /// 
        /// Renders the current date in the format that is relative to the server's local time zone. For servers in
        /// the United States, the format is MM/DD/YYYY (for example, 1/21/2001).
        /// </summary>
        /// <param name="offset">Adds or subtracts the number of days that are specified by the positive or negative integer value.</param>
        /// <returns>Value representing the current day.</returns>
        public static Value Today(int offset)
        {
            return new TodayValue(null, offset);
        }

        /// <summary>
        /// Value representing the current day.
        /// 
        /// Renders the current date in the format that is relative to the server's local time zone. For servers in
        /// the United States, the format is MM/DD/YYYY (for example, 1/21/2001).
        /// </summary>
        /// <param name="includeTimeValue">True if is to be included the time part; otherwise, false</param>
        /// <returns>Value representing the current day.</returns>
        public static Value Today(bool includeTimeValue)
        {
            return new TodayValue(includeTimeValue, null);
        }

        /// <summary>
        /// Value representing the current day.
        /// 
        /// Renders the current date in the format that is relative to the server's local time zone. For servers in
        /// the United States, the format is MM/DD/YYYY (for example, 1/21/2001).
        /// </summary>
        /// <param name="includeTimeValue">True if is to be included the time part; otherwise, false</param>
        /// <param name="offset">Adds or subtracts the number of days that are specified by the positive or negative integer value.</param>
        /// <returns>Value representing the current day.</returns>
        public static Value Today(bool includeTimeValue, int offset)
        {
            return new TodayValue(includeTimeValue, offset);
        }

        /// <summary>
        /// Can be used to represent any <paramref name="type"/> of value.
        /// </summary>
        /// <param name="type">Specifies the data type for the value contained by this element.</param>
        /// <param name="value">Value against which the value returned by the FieldRef element is compared</param>
        /// <returns>Value representing any object value.</returns>
        /// <remarks>
        /// CamlBuilder uses ToString() on top of <paramref name="value"/> to build the final CAML query.
        /// </remarks>
        public static Value ObjectValue(ValueType type, object value)
        {
            return new AnyValue(type, null, value);
        }

        /// <summary>
        /// Can be used to represent any <paramref name="type"/> of value.
        /// </summary>
        /// <param name="type">Specifies the data type for the value contained by this element.</param>
        /// <param name="includeTimeValue">
        /// Specifies to build DateTime queries based on time as well as date. If you set this to null
        /// the time portion of queries that involve date and time are ignored.
        /// </param>
        /// <param name="value">Value against which the value returned by the FieldRef element is compared</param>
        /// <returns>Value representing any object value.</returns>
        /// <remarks>
        /// CamlBuilder uses ToString() on top of <paramref name="value"/> to build the final CAML query.
        /// </remarks>
        public static Value ObjectValue(ValueType type, bool? includeTimeValue, object value)
        {
            return new AnyValue(type, includeTimeValue, value);
        }

        /// <summary>
        /// Contains the value if the unique ID number of the currently authenticated user of a site, as
        /// defined in the UserInfo table of the content database.
        /// </summary>
        /// <returns>Value representing the currently authenticated user unique ID number.</returns>
        public static Value UserId()
        {
            return new UserIdValue();
        }

        /// <summary>
        /// Value of a specified column in the List of Lists table.
        /// </summary>
        /// <param name="type">Specifies the data type for the value contained by this element.</param>
        /// <param name="listProperties">List of properties.</param>
        /// <returns>Value representing a list of lists table.</returns>
        public static Value ListProperties(
            ValueType type, 
            IEnumerable<ListPropertyValueItem> listProperties)
        {
            return new ListPropertyValue(type, null, listProperties);
        }

        /// <summary>
        /// Value of a specified column in the List of Lists table.
        /// </summary>
        /// <param name="type">Specifies the data type for the value contained by this element.</param>
        /// <param name="includeTimeValue">
        /// Specifies to build DateTime queries based on time as well as date. If you set this to null
        /// the time portion of queries that involve date and time are ignored.
        /// </param>
        /// <param name="listProperties">List of properties.</param>
        /// <returns>Value representing a list of lists table.</returns>
        public static Value ListProperties(
            ValueType type,
            bool? includeTimeValue,
            IEnumerable<ListPropertyValueItem> listProperties)
        {
            return new ListPropertyValue(type, includeTimeValue, listProperties);
        }
    }
}
