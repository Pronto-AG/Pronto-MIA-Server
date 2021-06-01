namespace Pronto_MIA.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pronto_MIA.BusinessLogic.Security.Authorization;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing an authorization service which can be used to
    /// authorize incoming users depending on policies.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AuthorizationService
    {
        /// <summary>
        /// Method to add the authorization service to the service collection.
        /// In this method the different available authorization policies are
        /// configured.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="cfg">Application configuration.</param>
        public static void AddAuthorizationService(
            this IServiceCollection services, IConfiguration cfg)
        {
            services.AddAuthorization(options =>
            {
                AddDepartmentPolicies(options);
                AddUserPolicies(options);
                AddDeploymentPlanPolicies(options);
            });
            services.AddScoped<IAuthorizationHandler,
                DepartmentAccessAuthorizationHandler>();
        }

        private static void AddDepartmentPolicies(AuthorizationOptions options)
        {
            var editDepartmentControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    { AccessControl.CanEditDepartments, AccessMode.Unrestricted },
                    { AccessControl.CanEditOwnDepartment, AccessMode.Department },
                };
            options.AddPolicy(
                "EditDepartment",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(Department),
                            editDepartmentControls));
                });

            var viewDepartmentControls = new Dictionary<AccessControl, AccessMode>()
            {
                { AccessControl.CanViewDepartments, AccessMode.Unrestricted },
                { AccessControl.CanViewOwnDepartment, AccessMode.Department },
            };
            options.AddPolicy(
                "ViewDepartment",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(Department),
                            viewDepartmentControls));
                });
        }

        private static void AddUserPolicies(AuthorizationOptions options)
        {
            var editUserControls = new Dictionary<AccessControl, AccessMode>()
            {
                { AccessControl.CanEditUsers, AccessMode.Unrestricted },
                { AccessControl.CanEditDepartmentUsers, AccessMode.Department },
            };
            options.AddPolicy(
                "EditUser",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(User),
                            editUserControls));
                });

            var viewUsersControls = new Dictionary<AccessControl, AccessMode>()
            {
                { AccessControl.CanViewUsers, AccessMode.Unrestricted },
                { AccessControl.CanViewDepartmentUsers, AccessMode.Department },
            };
            options.AddPolicy(
                "ViewUser",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(User),
                            viewUsersControls));
                });
        }

        private static void AddDeploymentPlanPolicies(
            AuthorizationOptions options)
        {
            var editDeploymentPlanControls = new Dictionary<AccessControl, AccessMode>()
                {
                    { AccessControl.CanEditDeploymentPlans, AccessMode.Unrestricted },
                    { AccessControl.CanEditDepartmentDeploymentPlans, AccessMode.Department },
                };
            options.AddPolicy(
                "EditDeploymentPlan",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(DeploymentPlan),
                            editDeploymentPlanControls));
                });

            var viewDeploymentPlanControls = new Dictionary<AccessControl, AccessMode>()
            {
                { AccessControl.CanViewDeploymentPlans, AccessMode.Unrestricted },
                { AccessControl.CanViewDepartmentDeploymentPlans, AccessMode.Department },
            };
            options.AddPolicy(
                "ViewDeploymentPlan",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(DeploymentPlan),
                            viewDeploymentPlanControls));
                });
        }
    }
}
