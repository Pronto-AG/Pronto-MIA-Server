namespace Pronto_MIA.Domain.EntityExtensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class containing helper methods for the
    /// <see cref="AccessControlList"/> entity.
    /// </summary>
    public static class AccessControlListExtension
    {
        /// <summary>
        /// Method that checks if an <see cref="AccessControlList"/>
        /// does contain the given <see cref="AccessControl"/>.
        /// </summary>
        /// <param name="acl">The <see cref="AccessControlList"/> to
        /// be checked.</param>
        /// <param name="control">The <see cref="AccessControl"/>
        /// which has to be present.</param>
        /// <returns>True if the given control is active within
        /// the given list.</returns>
        /// <exception cref="ArgumentException">If a control
        /// is used that has not yet been specified.</exception>
        [SuppressMessage(
            "Menees.Analyzers",
            "MEN003",
            Justification = "Trivial switch case.")]
        public static bool HasControl(
            this AccessControlList acl, AccessControl control)
        {
            switch (control)
            {
                case AccessControl.CanEditUsers:
                    return acl.CanEditUsers;
                case AccessControl.CanEditDepartmentUsers:
                    return acl.CanEditDepartmentUsers;
                case AccessControl.CanViewUsers:
                    return acl.CanViewUsers;
                case AccessControl.CanViewDepartmentUsers:
                    return acl.CanViewDepartmentUsers;
                case AccessControl.CanEditDepartments:
                    return acl.CanEditDepartments;
                case AccessControl.CanEditOwnDepartment:
                    return acl.CanEditOwnDepartment;
                case AccessControl.CanViewDepartments:
                    return acl.CanViewDepartments;
                case AccessControl.CanViewOwnDepartment:
                    return acl.CanViewOwnDepartment;
                case AccessControl.CanEditDeploymentPlans:
                    return acl.CanEditDeploymentPlans;
                case AccessControl.CanEditDepartmentDeploymentPlans:
                    return acl.CanEditDepartmentDeploymentPlans;
                case AccessControl.CanViewDeploymentPlans:
                    return acl.CanViewDeploymentPlans;
                case AccessControl.CanViewDepartmentDeploymentPlans:
                    return acl.CanViewDepartmentDeploymentPlans;
                case AccessControl.CanViewExternalNews:
                    return acl.CanViewExternalNews;
                case AccessControl.CanEditExternalNews:
                    return acl.CanEditExternalNews;
                default:
                    throw new ArgumentException(
                        $"Unknown AccessControl \"{control.ToString()}\"");
            }
        }
    }
}
