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
        /// Permission to view all users
        /// of the own department.
        /// </summary>
        CanViewDepartmentUsers,

        /// <summary>
        /// Permission to edit all users.
        /// </summary>
        CanEditUsers,

        /// <summary>
        /// Permission to edit all users
        /// of the own department.
        /// </summary>
        CanEditDepartmentUsers,

        /// <summary>
        /// Permission to view all departments.
        /// </summary>
        CanViewDepartments,

        /// <summary>
        /// Permission to view own department.
        /// </summary>
        CanViewOwnDepartment,

        /// <summary>
        /// Permission to edit all departments.
        /// </summary>
        CanEditDepartments,

        /// <summary>
        /// Permission to view own department.
        /// </summary>
        CanEditOwnDepartment,

        /// <summary>
        /// Permission to view all deployment plans.
        /// </summary>
        CanViewDeploymentPlans,

        /// <summary>
        /// Permission to view all deployment plans
        /// of the own department.
        /// </summary>
        CanViewDepartmentDeploymentPlans,

        /// <summary>
        /// Permission to edit all deployment plans.
        /// </summary>
        CanEditDeploymentPlans,

        /// <summary>
        /// Permission to edit all deployment plans
        /// of the own department.
        /// </summary>
        CanEditDepartmentDeploymentPlans,
    }
}
