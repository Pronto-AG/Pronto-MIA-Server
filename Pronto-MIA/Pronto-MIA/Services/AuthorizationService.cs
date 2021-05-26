namespace Pronto_MIA.Services
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pronto_MIA.BusinessLogic.Security.Authorization;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing an authorization service which can be used to
    /// authorize incoming users depending on policies.
    /// </summary>
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
                foreach (
                    AccessControl accessControl in
                    (AccessControl[])Enum.GetValues(typeof(AccessControl)))
                {
                    options.AddPolicy(
                        accessControl.ToString(),
                        policy => policy.Requirements.Add(
                            new AccessControlListRequirement(accessControl)));
                }
            });
            services.AddScoped<IAuthorizationHandler,
                AccessControlListAuthorizationHandler>();
        }
    }
}