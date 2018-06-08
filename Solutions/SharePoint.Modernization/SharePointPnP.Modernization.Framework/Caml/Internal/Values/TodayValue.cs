namespace CamlBuilder.Internal.Values
{
    internal class TodayValue : Value
    {
        private readonly int? offset;

        public TodayValue(bool? includeTimeValue, int? offset)
            : base(ValueType.DateTime, includeTimeValue)
        {
            this.offset = offset;
        }

        public TodayValue(bool? includeTimeValue)
            : this(includeTimeValue, null)
        {
        }

        protected override string GetCamlValue()
        {
            return offset.HasValue
                ? $"<Today Offset='{offset.Value}'/>"
                : "<Today/>";
        }
    }
}