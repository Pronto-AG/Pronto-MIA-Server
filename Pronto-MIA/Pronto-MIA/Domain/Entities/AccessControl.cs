namespace Pronto_MIA.Domain.Entities
{
    /// <summary>
    /// Enum representing all possible permissions
    /// a user can have.
    /// </summary>
    public enum AccessControl
    {
        /// <summary>
        /// Permission to view all users.
        /// </summary>
        CanViewUsers,

        /// <summary>
        /// Permission to edit all users.
        /// </summary>
        CanEditUsers,

        /// <summary>
        /// Permission to view all deployment plans.
        /// </summary>
        CanViewDeploymentPlans,

        /// <summary>
        /// Permission to edit all deployment plans.
        /// </summary>
        CanEditDeploymentPlans,
    }
}
