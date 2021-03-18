using Microsoft.EntityFrameworkCore;

namespace Pronto_MIA.Data
{
    public class InformbobDbContext : DbContext
    {
        public InformbobDbContext(DbContextOptions<InformbobDbContext> options)
            : base(options)
        {
        }
        
       /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=informbob;Username=informbob-db-admin;Password=super-secure-babula");*/

        public DbSet<Speaker> Speakers { get; set; }
    }
}