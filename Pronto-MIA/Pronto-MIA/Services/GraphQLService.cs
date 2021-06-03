namespace Pronto_MIA.Services
{
    using System.Diagnostics.CodeAnalysis;
    using HotChocolate.Types;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Interceptors;
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
                        UserStateRequestInterceptor.AddUserState(
                            config, context, builder))
                .AddType<UploadType>()
                .AddType<DeploymentPlanResolvers>()
                .AddType<AccessControlListInputType>()
                .AddQueryType<Query>()
                .AddTypeExtension<DeploymentPlanQuery>()
                .AddTypeExtension<UserQuery>()
                .AddTypeExtension<DepartmentQuery>()
                .AddMutationType<Mutation>()
                .AddTypeExtension<DeploymentPlanMutation>()
                .AddTypeExtension<FcmTokenMutation>()
                .AddTypeExtension<UserMutation>()
                .AddTypeExtension<DepartmentMutation>()
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddErrorFilter<ErrorFilter>()
                .AddDiagnosticEventListener(sp =>
                    new QueryLogger(
                        sp.GetApplicationService<ILogger<QueryLogger>>()));
        }
    }
}
