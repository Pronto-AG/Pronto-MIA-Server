namespace Pronto_MIA.DataAccess
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Class containing helper methods needed for db migrations.
    /// </summary>
    public static class DbMigrationHelper
    {
        /// <summary>
        /// Method which runs all available migrations for the application on
        /// the connected database.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static void Migrate(IApplicationBuilder builder)
        {
            using var scope = builder
                .ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var ctx = scope
                .ServiceProvider
                .GetRequiredService<ProntoMiaDbContext>();

            ctx.Database.Migrate();
        }
    }
}
