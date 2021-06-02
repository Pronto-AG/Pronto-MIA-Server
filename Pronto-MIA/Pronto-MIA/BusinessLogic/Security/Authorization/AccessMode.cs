namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    /// <summary>
    /// Enum representing the different modes an access
    /// control may have.
    /// </summary>
    public enum AccessMode
    {
        /// <summary>
        /// The access control has to be checked
        /// against the users department in addition to
        /// the policy.
        /// </summary>
        Department,

        /// <summary>
        /// The access control is not bound to
        /// another object but simply to the policy.
        /// </summary>
        Unrestricted,
    }
}
