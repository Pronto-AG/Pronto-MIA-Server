namespace Pronto_MIA.BusinessLogic.API.EntityExtensions
{
    using System;
    using HotChocolate;
    using HotChocolate.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class that contains graphQL extensions for the external news object.
    /// </summary>
    [ExtendObjectType(nameof(ExternalNews))]
    public class ExternalNewsResolvers
    {
        /// <summary>
        /// Gets the link to the file associated with this external news.
        /// </summary>
        /// <param name="externalNews">The external news in question.
        /// </param>
        /// <param name="cfg">The application configuration object.</param>
        /// <param name="httpContext">The http context used to generated
        /// the link to the file.</param>
        /// <returns>The link to where the file associated with this external
        /// news is statically served.</returns>
        public Uri Link(
            [Parent] ExternalNews externalNews,
            [Service]IConfiguration cfg,
            [Service]IHttpContextAccessor httpContext)
        {
            if (httpContext.HttpContext == null)
            {
                return null;
            }

            var request = httpContext.HttpContext.Request;
            var baseUrl = "http://" + // TODO
                          $"{request.Host.Value}" +
                          $"{request.PathBase.Value}";
            var staticFileRoot =
                cfg.GetValue<string>("StaticFiles:ENDPOINT");
            var fileUrl = baseUrl + "/" + staticFileRoot + "/" +
                          IExternalNewsManager.FileDirectory + "/" +
                          externalNews.FileUuid +
                          externalNews.FileExtension;
            return new Uri(fileUrl);
        }
    }
}
