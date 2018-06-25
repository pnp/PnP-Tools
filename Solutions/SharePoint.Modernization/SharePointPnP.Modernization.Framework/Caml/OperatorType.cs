namespace CamlBuilder
{
    /// <summary>
    /// Specifies operator types.
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// Indicates an Equal operator
        /// </summary>
        Equal,

        /// <summary>
        /// Indicates a NotEqual operator
        /// </summary>
        NotEqual,

        /// <summary>
        /// Indicates a GreaterThan operator
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Indicates a GreaterThanOrEqualTo operator
        /// </summary>
        GreaterThanOrEqualTo,

        /// <summary>
        /// Indicates a LowerThan operator
        /// </summary>
        LowerThan,

        /// <summary>
        /// Indicates a LowerThanOrEqualTo operator
        /// </summary>
        LowerThanOrEqualTo,

        /// <summary>
        /// Indicates an IsNull operator
        /// </summary>
        IsNull,

        /// <summary>
        /// Indicates an IsNotNull operator
        /// </summary>
        IsNotNull,

        /// <summary>
        /// Indicates a BeginsWith operator
        /// </summary>
        BeginsWith,

        /// <summary>
        /// Indicates a Contains operator
        /// </summary>
        Contains,

        /// <summary>
        /// Indicates a DateRangesOverlap operator
        /// </summary>
        DateRangesOverlap,

        /// <summary>
        /// Indicates an Includes operator
        /// </summary>
        Includes,

        /// <summary>
        /// Indicates an NotIncludes operator
        /// </summary>
        NotIncludes,

        /// <summary>
        /// Indicates an In operator
        /// </summary>
        In,

        /// <summary>
        /// Indicates a Membership operator
        /// </summary>
        Membership
    }
}
