using System.Linq;

namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.EntityExtensions;

    /// <summary>
    /// Class responsible for handling user authorization
    /// based on AccessControl-Policies.
    /// </summary>
    public class AccessControlListAuthorizationHandler
        : AuthorizationHandler<AccessControlListRequirement, IResolverContext>
    {
        private readonly ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessControlListAuthorizationHandler"/> class.
        /// </summary>
        /// <param name="dbContext">The database context used to retrieve
        /// up to date permission information.</param>
        public AccessControlListAuthorizationHandler(
            ProntoMiaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Method that checks if the user has the access control
        /// required by the requirement.
        /// </summary>
        /// <param name="context">The authorization handler context.</param>
        /// <param name="requirement">The requirement to be fulfilled.</param>
        /// <param name="resource">The resolver context.</param>
        /// <returns>A completed task. Also signals the context if
        /// the requirement is fulfilled.</returns>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessControlListRequirement requirement,
            IResolverContext resource)
        {
            var userId = int.Parse(
                context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            lock (this)
            {
                var accessControlList = this.dbContext
                    .AccessControlLists.SingleOrDefault(
                        acl => acl.UserId == userId);

                if (
                    accessControlList != default &&
                    accessControlList.HasControl(requirement.Control))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
