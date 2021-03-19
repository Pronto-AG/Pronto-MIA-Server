namespace Pronto_MIA.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pronto_MIA.Data;

    public static class DatabaseService
    {
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