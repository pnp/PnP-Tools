namespace CamlBuilder
{
    /// <summary>
    /// Defines a CAML statement. It can be a <see cref="LogicalJoin"/> or a <see cref="Operator"/>. 
    /// </summary>
    public abstract class Statement
    {
        public abstract string GetCaml();
    }
}
