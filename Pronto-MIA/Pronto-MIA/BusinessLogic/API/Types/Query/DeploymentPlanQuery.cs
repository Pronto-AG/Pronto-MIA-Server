namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
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
        /// Method which retrieves the available deployment plans. Depending
        /// on the requesting users access rights only a fraction of the
        /// available plans might be returned.
        /// </summary>
        /// <param name="deploymentPlanManager">The deployment plan manager
        /// responsible for managing deployment plans.</param>
        /// <param name="userState">Provides information about the user
        /// requesting this endpoint.</param>
        /// <returns>Queryable of all deployment plans available to the user.
        /// </returns>
        [Authorize(Policy = "ViewDeploymentPlan")]
        [AccessObjectIdArgument("IGNORED")]
        [UseFiltering]
        [UseSorting]
        public IQueryable<DeploymentPlan> DeploymentPlans(
            [Service] IDeploymentPlanManager deploymentPlanManager,
            [ApiUserGlobalState] ApiUserState userState)
        {
            if (userState.User.AccessControlList.CanViewDeploymentPlans)
            {
                return deploymentPlanManager.GetAll();
            }

            return deploymentPlanManager.GetAll().Where(
                dP => dP.DepartmentId == userState.User.DepartmentId);
        }
    }
}
