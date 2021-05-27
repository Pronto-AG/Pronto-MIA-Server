namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="DeploymentPlan"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class DeploymentPlanQuery
    {
        /// <summary>
        /// Method which retrieves the available deployment plans.
        /// </summary>
        /// <param name="deploymentPlanManager">The deployment plan manager
        /// responsible for managing deployment plans.</param>
        /// <returns>Queryable of all available deployment plans.</returns>
        [Authorize(Policy = "CanViewDeploymentPlans")]
        [UseFiltering]
        [UseSorting]
        public IQueryable<DeploymentPlan> DeploymentPlans(
            [Service] IDeploymentPlanManager deploymentPlanManager)
        {
            return deploymentPlanManager.GetAll();
        }
    }
}
