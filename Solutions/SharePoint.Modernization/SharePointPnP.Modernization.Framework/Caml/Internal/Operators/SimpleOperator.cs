namespace CamlBuilder.Internal.Operators
{
    internal class SimpleOperator : Operator
    {
        internal SimpleOperator(
            OperatorType operatorType, 
            FieldReference fieldRef)
            : base(operatorType, fieldRef)
        {
        }

        public override string GetCaml() => $@"<{OperatorTypeString}>{FieldReference.GetCaml()}</{OperatorTypeString}>";
    }
}
