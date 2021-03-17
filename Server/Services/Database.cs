using Server.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Services
{
    public static class DatabaseService
    {
        public static void AddDatabaseService(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<InformbobDbContext>(options =>
                options.UseNpgsql(cfg.GetConnectionString("InformbobDBContext")));
        }
    }
}