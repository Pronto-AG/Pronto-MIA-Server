namespace Pronto_MIA.Services.AuthorizationHandlers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.Domain.EntityExtensions;

    /// <summary>
    /// Class responsible for handling user authorization
    /// based on AccessControl-Policies.
    /// </summary>
    public class AccessControlListAuthorizationHandler
        : AuthorizationHandler<AccessControlListRequirement, IResolverContext>
    {
        /// <summary>
        /// Method that checks if the user has the access control
        /// required by the requirement.
        /// </summary>
        /// <param name="context">The authorization handler context.</param>
        /// <param name="requirement">The requirement to be fulfilled.</param>
        /// <param name="resource">The resolver context.</param>
        /// <returns>A completed task. Also signals the context if
        /// the requirement is fulfilled.</returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessControlListRequirement requirement,
            IResolverContext resource)
        {
            var id = int.Parse(
                context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = requirement.DbContext.Users.Find(id);
            if (
                user.AccessControlList != default &&
                user.AccessControlList.HasControl(requirement.Control))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
