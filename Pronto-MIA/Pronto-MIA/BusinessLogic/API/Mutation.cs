#nullable enable
namespace Pronto_MIA.BusinessLogic.API
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;

    public class Mutation
    {
        /// <summary>
        /// Method to upload a pdf to the server.
        /// </summary>
        /// <returns>If successful.</returns>
        [UseFirstOrDefault]
        // [UseProjection] -> Breaks everything
        [Authorize]
        public async Task<IQueryable<DeploymentPlan?>> AddDeploymentPlan(
            [Service] DeploymentPlanManager deploymentPlanManager,
            IFile file,
            DateTime availableFrom,
            DateTime availableUntil)
        {
            var result = await deploymentPlanManager.Create(
                file,
                availableFrom,
                availableUntil);
            return result.Match(
                deploymentPlan => deploymentPlan,
                error => throw error.AsQueryException());
        }
    }
}
