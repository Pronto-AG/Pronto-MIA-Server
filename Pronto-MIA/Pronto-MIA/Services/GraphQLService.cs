namespace Pronto_MIA.Services
{
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Pronto_MIA.BusinessLogic.API.Types;

    /// <summary>
    /// Service which initializes and contains all information regarding the
    /// GraphQL-API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class GraphQLService
    {
        /// <summary>
        /// Method to add the configured GraphQL-Service to the service
        /// collection.
        /// </summary>
        /// <param name="services">The service collection of the application.
        /// </param>
        // ReSharper disable once InconsistentNaming
        public static void AddGraphQLService(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .AddAuthorization()
                .AddHttpRequestInterceptor(AddUserState)
                .AddType<UploadType>()
                .AddType<DeploymentPlanResolvers>()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddErrorFilter<ErrorFilter>()
                .AddDiagnosticEventListener(sp =>
                    new QueryLogger(
                        sp.GetApplicationService<ILogger<QueryLogger>>()));
        }

        /// <summary>
        /// Adds user information to the global state cache so that it may be
        /// accessed in a query.
        /// </summary>
        private static ValueTask AddUserState(
            HttpContext context,
            IRequestExecutor executor,
            IQueryRequestBuilder builder,
            CancellationToken token)
        {
            ApiUserState apiUserState = default;
            var identity = context.User.Identity;
            if (identity != null && identity.IsAuthenticated)
            {
                var userId = context
                    .User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = context
                    .User.FindFirstValue(ClaimTypes.Name);
                apiUserState = new ApiUserState(userId, userName);
            }

            builder.SetProperty(
                ApiUserGlobalState.ApiUserStateName,
                apiUserState);

            return ValueTask.CompletedTask;
        }
    }
}
