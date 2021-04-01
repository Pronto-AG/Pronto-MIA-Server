/*using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.FileProviders;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Extensions;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provides file download extension to the
    /// <see cref="IEndpointConventionBuilder"/>.
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        public static GraphQLEndpointConventionBuilder MapFileDownload(
            this IEndpointRouteBuilder endpointRouteBuilder,
            PathString path)
        {
            if (endpointRouteBuilder is null)
            {
                throw new ArgumentNullException(nameof(endpointRouteBuilder));
            }

            path = path.ToString().TrimEnd('/');

            RoutePattern pattern = 
                RoutePatternFactory.Parse(path + "/{**slug}");
            IApplicationBuilder requestPipeline =
                endpointRouteBuilder.CreateApplicationBuilder();

            requestPipeline
                .UseMiddleware<WebSocketSubscriptionMiddleware>(schemaNameOrDefault)
                .UseMiddleware<HttpPostMiddleware>(schemaNameOrDefault)
                .UseMiddleware<HttpMultipartMiddleware>(schemaNameOrDefault)
                .UseMiddleware<HttpGetSchemaMiddleware>(schemaNameOrDefault)
                .UseMiddleware<ToolDefaultFileMiddleware>(fileProvider, path)
                .UseMiddleware<ToolOptionsFileMiddleware>(schemaNameOrDefault, path)
                .UseMiddleware<ToolStaticFileMiddleware>(fileProvider, path)
                .UseMiddleware<HttpGetMiddleware>(schemaNameOrDefault)
                .Use(next => context =>
                {
                    context.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });

            return new GraphQLEndpointConventionBuilder(
                endpointRouteBuilder
                    .Map(pattern, requestPipeline.Build())
                    .WithDisplayName("Hot Chocolate GraphQL Pipeline"));
        }
    }
}*/
