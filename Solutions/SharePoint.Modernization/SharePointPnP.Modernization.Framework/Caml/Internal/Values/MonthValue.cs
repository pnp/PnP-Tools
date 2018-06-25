namespace CamlBuilder.Internal.Values
{
    internal class MonthValue : Value
    {
        public MonthValue(bool? includeTimeValue)
            : base(ValueType.DateTime, includeTimeValue)
        {
        }

        protected override string GetCamlValue()
        {
            return "<Month/>";
        }
    }
}