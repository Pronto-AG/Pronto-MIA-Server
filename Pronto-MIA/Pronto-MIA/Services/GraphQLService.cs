namespace Pronto_MIA.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;

    /// <summary>
    /// Service which initializes and contains all information regarding the
    /// GraphQL-API.
    /// </summary>
    [ExcludeFromCodeCoverage]

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
            var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetService<IConfiguration>();

            services.AddGraphQLServer()
                .AddAuthorization()
                .AddHttpRequestInterceptor(
                    (context, executor, builder, token) =>
                        AddUserState(config, context, executor, builder, token))
                .AddType<UploadType>()
                .AddType<DeploymentPlanResolvers>()
                .AddType<ExternalNewsResolvers>()
                .AddType<AccessControlListInputType>()
                .AddQueryType<Query>()
                .AddTypeExtension<DeploymentPlanQuery>()
                .AddTypeExtension<UserQuery>()
                .AddTypeExtension<DepartmentQuery>()
                .AddTypeExtension<ExternalNewsQuery>()
                .AddTypeExtension<InternalNewsQuery>()
                .AddTypeExtension<EducationalContentQuery>()
                .AddMutationType<Mutation>()
                .AddTypeExtension<DeploymentPlanMutation>()
                .AddTypeExtension<FcmTokenMutation>()
                .AddTypeExtension<UserMutation>()
                .AddTypeExtension<DepartmentMutation>()
                .AddTypeExtension<ExternalNewsMutation>()
                .AddTypeExtension<InternalNewsMutation>()
                .AddTypeExtension<EducationalContentMutation>()
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
            IConfiguration cfg,
            HttpContext context,
            IRequestExecutor executor,
            IQueryRequestBuilder builder,
            CancellationToken token)
        {
            var dbContext = new ProntoMiaDbContext(
                DatabaseService.GetOptions(cfg));

            ApiUserState apiUserState = default;
            var identity = context.User.Identity;
            if (identity is { IsAuthenticated: true })
            {
                var userId = int.Parse(context
                    .User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userName = context
                    .User.FindFirstValue(ClaimTypes.Name);
                var user = dbContext.Users.Find(userId);

                apiUserState = new ApiUserState(user);
            }

            builder.SetProperty(
                ApiUserGlobalState.ApiUserStateName,
                apiUserState);

            return ValueTask.CompletedTask;
        }
    }
}
