namespace CamlBuilder.Internal.Operators
{
    internal class ComplexOperator : Operator
    {
        public Value Value { get; }
        
        internal ComplexOperator(
            OperatorType operatorType, 
            FieldReference fieldRef, 
            Value value)
            : base(operatorType, fieldRef)
        {
            Value = value;
        }

        public override string GetCaml()
        {
            return $@"<{OperatorTypeString}>{FieldReference.GetCaml()}{Value.GetCaml()}</{OperatorTypeString}>";
        }
    }
}
