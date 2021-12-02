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
        /// configured and added to the authorization workflow.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="cfg">Application configuration.</param>
        public static void AddAuthorizationService(
            this IServiceCollection services, IConfiguration cfg)
        {
            services.AddAuthorization(options =>
            {
                AddEditDepartment(options);
                AddViewDepartment(options);
                AddEditUser(options);
                AddViewUser(options);
                AddEditDeploymentPlan(options);
                AddViewDeploymentPlan(options);
                AddEditExternalNews(options);
                AddViewExternalNews();
                AddEditInternalNews(options);
                AddViewInternalNews(options);
                AddEditEducationalContent(options);
                AddViewEducationalContent(options);
            });
            services.AddScoped<IAuthorizationHandler,
                DepartmentAccessAuthorizationHandler>();
        }

        private static void AddEditDepartment(AuthorizationOptions options)
        {
            var editDepartmentControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanEditDepartments,
                        AccessMode.Unrestricted
                    },
                    {
                        AccessControl.CanEditOwnDepartment,
                        AccessMode.Department
                    },
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
        }

        private static void AddViewDepartment(AuthorizationOptions options)
        {
            var viewDepartmentControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewDepartments,
                        AccessMode.Unrestricted
                    },
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
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

        private static void AddEditUser(AuthorizationOptions options)
        {
            var editUserControls = new Dictionary<AccessControl, AccessMode>()
            {
                {
                    AccessControl.CanEditUsers,
                    AccessMode.Unrestricted
                },
                {
                    AccessControl.CanEditDepartmentUsers,
                    AccessMode.Department
                },
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
        }

        private static void AddViewUser(AuthorizationOptions options)
        {
            var viewUsersControls = new Dictionary<AccessControl, AccessMode>()
            {
                {
                    AccessControl.CanViewUsers,
                    AccessMode.Unrestricted
                },
                {
                    AccessControl.CanViewDepartmentUsers,
                    AccessMode.Department
                },
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

        private static void AddEditDeploymentPlan(AuthorizationOptions options)
        {
            var editDeploymentPlanControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanEditDeploymentPlans,
                        AccessMode.Unrestricted
                    },
                    {
                        AccessControl.CanEditDepartmentDeploymentPlans,
                        AccessMode.Department
                    },
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
        }

        private static void AddViewDeploymentPlan(AuthorizationOptions options)
        {
            var viewDeploymentPlanControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewDeploymentPlans,
                        AccessMode.Unrestricted
                    },
                    {
                        AccessControl.CanViewDepartmentDeploymentPlans,
                        AccessMode.Department
                    },
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

        private static void AddEditExternalNews(AuthorizationOptions options)
        {
            var editExternalNewsControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanEditExternalNews,
                        AccessMode.Unrestricted
                    },
                };
            options.AddPolicy(
                "EditExternalNews",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
        }

        private static void AddViewExternalNews()
        {
            var viewExternalNewsControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewExternalNews,
                        AccessMode.Unrestricted
                    },
                };
        }

        private static void AddEditInternalNews(AuthorizationOptions options)
        {
            var editInternalNewsControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanEditInternalNews,
                        AccessMode.Unrestricted
                    },
                };
            options.AddPolicy(
                "EditInternalNews",
                policy => {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(InternalNews),
                            editInternalNewsControls));
                });;
        }

        private static void AddViewInternalNews(AuthorizationOptions options)
        {
            var viewInternalNewsControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewInternalNews,
                        AccessMode.Unrestricted
                    },
                };
            options.AddPolicy(
                "ViewInternalNews",
                policy => policy.RequireAuthenticatedUser());
        }

        private static void AddEditEducationalContent(
            AuthorizationOptions options)
        {
            var editEducationalContentControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanEditEducationalContent,
                        AccessMode.Unrestricted
                    },
                };
            options.AddPolicy(
                "EditEducationalContent",
                policy => {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new AccessObjectRequirement(
                            typeof(EducationalContent),
                            editEducationalContentControls));
                });
        }

        private static void AddViewEducationalContent(
            AuthorizationOptions options)
        {
            var viewEducationalContentControls =
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewEducationalContent,
                        AccessMode.Unrestricted
                    },
                };
            options.AddPolicy(
                "ViewEducationalContent",
                policy => policy.RequireAuthenticatedUser());
        }
    }
}
