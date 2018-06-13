namespace CamlBuilder.Internal.Values
{
    internal class AnyValue : Value
    {
        private readonly object anyValue;

        public AnyValue(ValueType type, bool? includeTimeValue, object anyValue)
            : base(type, includeTimeValue)
        {
            this.anyValue = anyValue;
        }

        protected override string GetCamlValue()
        {
            return anyValue.ToString();
        }
    }
}