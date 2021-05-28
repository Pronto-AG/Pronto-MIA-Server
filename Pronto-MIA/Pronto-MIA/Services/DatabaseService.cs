namespace Pronto_MIA.Services
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pronto_MIA.DataAccess;

    /// <summary>
    /// Class containing all logic regarding the database service.
    /// </summary>
    public static class DatabaseService
    {
        /// <summary>
        /// Method to add the database service to the service collection.
        /// </summary>
        /// <param name="services">The service collection used in this
        /// application.</param>
        /// <param name="cfg">The configuration which contains the database
        /// connection string.</param>
        public static void AddDatabaseService(
            this IServiceCollection services,
            IConfiguration cfg)
        {
            Action<DbContextOptionsBuilder> optionsAction = (options) =>
            {
                options.UseNpgsql(
                    cfg.GetConnectionString("ProntoMIADbContext"));
                options.UseLazyLoadingProxies();
            };
            services.AddDbContext<ProntoMiaDbContext>(optionsAction);
        }
    }
}
