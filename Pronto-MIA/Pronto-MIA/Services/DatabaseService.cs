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
            services.AddDbContext<ProntoMiaDbContext>((options) =>
            {
                DatabaseService.ConfigureOptions(cfg, options);
            });
        }

        /// <summary>
        /// Method to get the options defined on the DbContext.
        /// May be used to create DbContext instances manually.
        /// </summary>
        /// <param name="cfg">The configuration to be used
        /// with the options.</param>
        /// <returns>The DbContext options currently used by
        /// the database service.</returns>
        public static DbContextOptions<ProntoMiaDbContext>
            GetOptions(IConfiguration cfg)
        {
            var builder = new DbContextOptionsBuilder<ProntoMiaDbContext>();
            DatabaseService.ConfigureOptions(cfg, builder);
            return builder.Options;
        }

        private static void ConfigureOptions(
            IConfiguration cfg,
            DbContextOptionsBuilder options)
        {
            options.UseNpgsql(
                cfg.GetConnectionString("ProntoMIADbContext"));
            options.UseLazyLoadingProxies();
        }
    }
}
