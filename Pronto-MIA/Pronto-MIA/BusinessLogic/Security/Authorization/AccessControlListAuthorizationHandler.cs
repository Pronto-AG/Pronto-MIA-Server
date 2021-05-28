namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Domain.EntityExtensions;
    using Pronto_MIA.Services;

    /// <summary>
    /// Class responsible for handling user authorization
    /// based on AccessControl-Policies.
    /// </summary>
    public class AccessControlListAuthorizationHandler
        : AuthorizationHandler<AccessControlListRequirement, IResolverContext>
    {
        private DbContextOptions<ProntoMiaDbContext> options;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessControlListAuthorizationHandler"/> class.
        /// </summary>
        /// <param name="cfg">The configuration of the current
        /// application.</param>
        public AccessControlListAuthorizationHandler(
            IConfiguration cfg)
        {
            this.options = DatabaseService.GetOptions(cfg);
        }

        /// <summary>
        /// Method to overwrite the configured db context options.
        /// This method can be used for testing or in general if other
        /// options should be necessary.
        /// </summary>
        /// <param name="options">The options to be used within the
        /// db context.</param>
        public void SetDbOptions(DbContextOptions<ProntoMiaDbContext> options)
        {
            this.options = options;
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
            AccessControlList accessControlList;

            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                accessControlList = dbContext
                    .AccessControlLists.SingleOrDefault(
                        acl => acl.UserId == userId);
            }

            if (
                accessControlList != default &&
                accessControlList.HasControl(requirement.Control))
            {
                context.Succeed(requirement);
            }
        }
    }
}
