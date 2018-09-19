namespace CamlBuilder
{
    /// <summary>
    /// Defines membership types used by Membership operator.
    /// </summary>
    /// <seealso cref="Operator.Membership"/>
    public enum MembershipType
    {
        /// <summary>
        /// Indicates SPWeb.AllUsers membership.
        /// </summary>
        SpWebAllUsers,

        /// <summary>
        /// Indicates SPGroup membership.
        /// </summary>
        SpGroup,

        /// <summary>
        /// Indicates SPWebGroups membership.
        /// </summary>
        SpWebGroups,

        /// <summary>
        /// Indicates CurrentUserGroups membership.
        /// </summary>
        CurrentUserGroups,

        /// <summary>
        /// Indicates SPWebUsers membership.
        /// </summary>
        SpWebUsers
    }
}
