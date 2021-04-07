namespace Pronto_MIA.BusinessLogic.API.EntityExtensions
{
    using System;
    using HotChocolate;
    using HotChocolate.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class that contains graphQL extensions for the deployment plan object.
    /// </summary>
    [ExtendObjectType(nameof(DeploymentPlan))]
    public class DeploymentPlanResolvers
    {
        /// <summary>
        /// Gets the link to the file associated with this deployment plan.
        /// </summary>
        /// <param name="deploymentPlan">The deployment plan in question.
        /// </param>
        /// <param name="cfg">The application configuration object.</param>
        /// <param name="httpContext">The http context used to generated
        /// the link to the file.</param>
        /// <returns>The link to where the file associated with this deployment
        /// plan is statically served.</returns>
        public Uri Link(
            [Parent] DeploymentPlan deploymentPlan,
            [Service]IConfiguration cfg,
            [Service]IHttpContextAccessor httpContext)
        {
            var request = httpContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://" +
                          $"{request.Host.Value}" +
                          $"{request.PathBase.Value}";
            var staticFileRoot =
                cfg.GetValue<string>("StaticFiles:ENDPOINT");
            var fileUrl = baseUrl + "/" + staticFileRoot + "/" +
                          DeploymentPlanManager.FileDirectory + "/" +
                          deploymentPlan.FileUUID +
                          deploymentPlan.FileExtension;
            return new Uri(fileUrl);
        }
    }
}
