namespace Pronto_MIA.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Domain.EntityTypeConfigs;

    /// <summary>
    /// Class representing the database context of the application.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class ProntoMIADbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProntoMIADbContext"/>
        /// class. Which can then be used to access the database.
        /// </summary>
        /// <param name="options">Options used to initialize the context.
        /// </param>
        public ProntoMIADbContext(DbContextOptions<ProntoMIADbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DBSet containing speakers.
        /// </summary>
        public DbSet<Speaker> Speakers { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing deployment plans.
        /// </summary>
        public DbSet<DeploymentPlan> DeploymentPlans { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);

           modelBuilder.ApplyConfiguration(new UserTypeConfig());
           modelBuilder.ApplyConfiguration(new DeploymentPlanTypeConfig());

           var generatorOptions = new Pbkdf2GeneratorOptions(
               1500);
           var generator = new Pbkdf2Generator(generatorOptions);
           var hash = generator.HashPassword("HelloWorld");
           var franz = new User(
               "Franz",
               hash,
               generator.GetType().Name,
               generator.GetOptions().ToJson())
           {
               Id = -1,
           };

           modelBuilder.Entity<User>().HasData(franz);
       }
    }
}
