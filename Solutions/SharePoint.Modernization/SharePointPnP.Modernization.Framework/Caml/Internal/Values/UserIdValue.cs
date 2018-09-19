namespace CamlBuilder.Internal.Values
{
    internal class UserIdValue : Value
    {
        public UserIdValue()
            : base(ValueType.Integer)
        {
        }

        protected override string GetCamlValue()
        {
            return "<UserID/>";
        }
    }
}