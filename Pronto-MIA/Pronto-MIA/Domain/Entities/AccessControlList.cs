namespace Pronto_MIA.Domain.Entities
{
    /// <summary>
    /// Class containing the access control list of a specific user.
    /// </summary>
    public class AccessControlList
    {
        /// <summary>
        /// Gets or sets the acl identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the user associated with this list.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this list.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view all users.
        /// </summary>
        public bool CanViewUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all users.
        /// </summary>
        public bool CanEditUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view all deployment plans.
        /// </summary>
        public bool CanViewDeploymentPlans { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all deployment plans.
        /// </summary>
        public bool CanEditDeploymentPlans { get; set; }
    }
}
