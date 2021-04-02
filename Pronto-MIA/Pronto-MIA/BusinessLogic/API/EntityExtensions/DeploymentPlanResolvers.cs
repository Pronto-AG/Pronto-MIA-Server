namespace Pronto_MIA.BusinessLogic.API.EntityExtensions
{
    using System;
    using System.Threading;
    using DataAccess.Managers;
    using HotChocolate;
    using HotChocolate.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.Domain.Entities;

    [ExtendObjectType(Name = "DeploymentPlan")]
    public class DeploymentPlanResolvers
    {
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
                cfg.GetValue<string>("API:STATIC_FILE_ENDPOINT");
            var fileUrl = baseUrl + "/" + staticFileRoot + "/" +
                          DeploymentPlanManager.FileDirectory + "/" +
                          deploymentPlan.fileUUID + ".pdf";
            return new Uri(fileUrl);
        }
    }
}
