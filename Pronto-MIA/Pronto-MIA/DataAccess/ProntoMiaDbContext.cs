namespace Pronto_MIA.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Domain.EntityTypeConfigs;

    /// <summary>
    /// Class representing the database context of the application.
    /// </summary>
    public class ProntoMiaDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProntoMiaDbContext"/>
        /// class. Which can then be used to access the database.
        /// </summary>
        /// <param name="options">Options used to initialize the context.
        /// </param>
        public ProntoMiaDbContext(DbContextOptions<ProntoMiaDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DBSet containing users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing deployment plans.
        /// </summary>
        public DbSet<DeploymentPlan> DeploymentPlans { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing fcm tokens.
        /// </summary>
        public DbSet<FcmToken> FcmTokens { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);

           modelBuilder.ApplyConfiguration(new UserTypeConfig());
           modelBuilder.ApplyConfiguration(new DeploymentPlanTypeConfig());
           modelBuilder.ApplyConfiguration(new FcmTokenTypeConfig());

           var generatorOptions = new Pbkdf2GeneratorOptions(
               1500);
           var generator = new Pbkdf2Generator(generatorOptions);
           var hash = generator.HashPassword("HelloWorld");
           var franz = new User(
               "Franz",
               hash,
               Pbkdf2Generator.Identifier,
               generator.GetOptions().ToJson())
           {
               Id = -1,
           };

           modelBuilder.Entity<User>().HasData(franz);
       }
    }
}
