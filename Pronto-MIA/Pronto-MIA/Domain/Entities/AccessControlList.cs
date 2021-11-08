namespace Pronto_MIA.Domain.Entities
{
    using HotChocolate;

    /// <summary>
    /// Class containing the access control list of a specific user.
    /// </summary>
    public class AccessControlList
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessControlList"/> class.
        /// </summary>
        /// <param name="userId">The user associated with this acl.</param>
        public AccessControlList(int userId)
        {
            this.UserId = userId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessControlList"/>
        /// class.
        /// Used for entity framework construction.
        /// </summary>
        protected AccessControlList()
        {
        }

        /// <summary>
        /// Gets or sets the acl identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the user associated with this list.
        /// </summary>
        [GraphQLIgnore]
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
        /// view all users of his department.
        /// </summary>
        public bool CanViewDepartmentUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all users.
        /// </summary>
        public bool CanEditUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all users of his department.
        /// </summary>
        public bool CanEditDepartmentUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view all departments.
        /// </summary>
        public bool CanViewDepartments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view his own department.
        /// </summary>
        public bool CanViewOwnDepartment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all departments.
        /// </summary>
        public bool CanEditDepartments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit his own department.
        /// </summary>
        public bool CanEditOwnDepartment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view all deployment plans.
        /// </summary>
        public bool CanViewDeploymentPlans { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view all deployment plans of his department.
        /// </summary>
        public bool CanViewDepartmentDeploymentPlans { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all deployment plans.
        /// </summary>
        public bool CanEditDeploymentPlans { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all deployment plans of his department.
        /// </summary>
        public bool CanEditDepartmentDeploymentPlans { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// view all external news.
        /// </summary>
        public bool CanViewExternalNews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has permission to
        /// edit all external news.
        /// </summary>
        public bool CanEditExternalNews { get; set; }
    }
}
