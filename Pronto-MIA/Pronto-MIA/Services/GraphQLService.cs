using HotChocolate.Types;

namespace Pronto_MIA.Services
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Npgsql;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
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
                .AddType<UploadType>()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>();
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
                            .WithCode(Error.DatabaseUnavailable
                                .ToString())
                            .WithMessage(
                                Error.DatabaseUnavailable.Message());
                    default:
                        return error
                            .WithCode(Error.UnknownError.ToString())
                            .WithMessage(
                                Error.UnknownError.Message());
                }
            });
        }
    }
}
