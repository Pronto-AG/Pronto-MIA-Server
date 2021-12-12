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
    /// Class that contains graphQL extensions for the internal news object.
    /// </summary>
    [ExtendObjectType(nameof(InternalNews))]
    public class InternalNewsResolvers
    {
        /// <summary>
        /// Gets the link to the file associated with this internal news.
        /// </summary>
        /// <param name="internalNews">The internal news in question.
        /// </param>
        /// <param name="cfg">The application configuration object.</param>
        /// <param name="httpContext">The http context used to generated
        /// the link to the file.</param>
        /// <returns>The link to where the file associated with this internal
        /// news is statically served.</returns>
        public Uri Link(
            [Parent] InternalNews internalNews,
            [Service] IConfiguration cfg,
            [Service] IHttpContextAccessor httpContext)
        {
            if (httpContext.HttpContext == null)
            {
                return null;
            }

            var request = httpContext.HttpContext.Request;
            var baseUrl = "https://" +
                          $"{request.Host.Value}" +
                          $"{request.PathBase.Value}";
            var staticFileRoot =
                cfg.GetValue<string>("StaticFiles:ENDPOINT");
            var fileUrl = baseUrl + "/" + staticFileRoot + "/" +
                          IInternalNewsManager.FileDirectory + "/" +
                          internalNews.FileUuid +
                          internalNews.FileExtension;
            return new Uri(fileUrl);
        }
    }
}
