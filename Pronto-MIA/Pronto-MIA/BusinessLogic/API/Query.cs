#nullable enable
namespace Pronto_MIA.BusinessLogic.API
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the query operation of graphql.
    /// </summary>
    public class Query
    {
        #pragma warning disable SA1642
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="cfg">The configuration used by the query class.</param>
        #pragma warning restore SA1642
        public Query(IConfiguration cfg)
        {
            this.Cfg = cfg;
        }

        private IConfiguration Cfg { get; }

        /// <summary>
        /// Method which allows the user to retrieve a token which may then be
        /// used for authentication in further requests.
        /// </summary>
        /// <param name="userManager">The manager responsible for user
        /// operations.</param>
        /// <param name="userName">The name of the user wanting to authenticate.
        /// </param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A JWT-Bearer-Token which can be used within the
        /// authentication header in order to authenticate the user.</returns>
        /// <exception cref="QueryException">If a problem occured during
        /// authentication.</exception>
        public string? Authenticate(
            [Service] UserManager userManager,
            string userName,
            string password)
        {
            return userManager.Authenticate(userName, password).Match(
                token => token,
                error => throw error.AsQueryException());
        }

        /// <summary>
        /// Method which retrieves the available deployment plans.
        /// </summary>
        /// <param name="deploymentPlanManager">The deployment plan manager
        /// responsible for managing deployment plans.</param>
        /// <returns>Queryable of all available deployment plans.</returns>
        // [UseProjection]
        [UseFiltering]
        [UseSorting]
        [Authorize]
        public IQueryable<DeploymentPlan?> DeploymentPlans(
            [Service] DeploymentPlanManager deploymentPlanManager)
        {
            return deploymentPlanManager.GetAll();
        }
    }
}
