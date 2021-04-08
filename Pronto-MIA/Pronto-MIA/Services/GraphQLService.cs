namespace Pronto_MIA.Services
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Npgsql;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess;

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
                .AddSorting();
            AddErrorHandling(services);
        }

        /// <summary>
        /// Adds a global error handler which catches unknown errors as well
        /// as problems with the database.
        /// </summary>
        /// <param name="services">The service collection of the application.
        /// </param>
        private static void AddErrorHandling(IServiceCollection services)
        {
            services.AddErrorFilter(error =>
            {
                switch (error.Exception)
                {
                    case null:
                        return error;
                    case InvalidOperationException _
                        when error.Exception.InnerException is NpgsqlException:
                    case NpgsqlException:
                        return error
                            .WithCode(Error.DatabaseOperationError
                                .ToString())
                            .WithMessage(
                                Error.DatabaseOperationError.Message());
                    default:
                        Console.WriteLine(error.Exception.Message);
                        return error
                            .WithCode(Error.UnknownError
                                .ToString())
                            .WithMessage(
                                Error.UnknownError.Message());
                }
            });
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
