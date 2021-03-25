namespace Pronto_MIA.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pronto_MIA.Data;

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
            services.AddDbContext<InformbobDbContext>(options =>
                options.UseNpgsql(
                    cfg.GetConnectionString("InformbobDBContext")));
        }
    }
}
