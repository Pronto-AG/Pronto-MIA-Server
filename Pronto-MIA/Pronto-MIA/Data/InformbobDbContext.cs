namespace Pronto_MIA.Data
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class representing the database context of the application.
    /// </summary>
    public class InformbobDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformbobDbContext"/>
        /// class. Which can then be used to access the database.
        /// </summary>
        /// <param name="options">Options used to initialize the context.
        /// </param>
        public InformbobDbContext(DbContextOptions<InformbobDbContext> options)
            : base(options)
        {
        }

       /*protected override void OnConfiguring(
           DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=127.0.0.1;" +
                                        "Database=informbob;" +
                                        "Username=informbob-db-admin;" +
                                        "Password=super-secure-babula");*/

       /// <summary>
       /// Gets or sets the contents of a DBSet containing speakers.
       /// </summary>
        public DbSet<Speaker> Speakers { get; set; }
    }
}
