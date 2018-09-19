namespace CamlBuilder.Internal.Values
{
    internal class NowValue : Value
    {
        public NowValue(bool? includeTimeValue) 
            : base(ValueType.DateTime, includeTimeValue)
        {
        }

        protected override string GetCamlValue()
        {
            return "<Now/>";
        }
    }
}