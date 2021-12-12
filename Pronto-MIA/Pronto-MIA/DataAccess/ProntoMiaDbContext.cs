namespace Pronto_MIA.DataAccess
{
    using System;
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

        /// <summary>
        /// Gets or sets the DBSet containing AccessControlLists.
        /// </summary>
        public DbSet<AccessControlList> AccessControlLists { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing departments.
        /// </summary>
        public DbSet<Department> Departments { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing external news.
        /// </summary>
        public DbSet<ExternalNews> ExternalNews { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing internal news.
        /// </summary>
        public DbSet<InternalNews> InternalNews { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing educational content.
        /// </summary>
        public DbSet<EducationalContent> EducationalContent { get; set; }

        /// <summary>
        /// Gets or sets the DBSet containing appointments.
        /// </summary>
        public DbSet<Appointment> Appointments { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserTypeConfig());
            modelBuilder.ApplyConfiguration(new DeploymentPlanTypeConfig());
            modelBuilder.ApplyConfiguration(new FcmTokenTypeConfig());
            modelBuilder.ApplyConfiguration(new AccessControlListTypeConfig());
            modelBuilder.ApplyConfiguration(new DepartmentTypeConfig());
            modelBuilder.ApplyConfiguration(new ExternalNewsTypeConfig());
            modelBuilder.ApplyConfiguration(new InternalNewsTypeConfig());
            modelBuilder.ApplyConfiguration(new EducationalContentTypeConfig());
            modelBuilder.ApplyConfiguration(new AppointmentTypeConfig());

            this.AddAdminUser(modelBuilder);
        }

        private void AddAdminUser(ModelBuilder modelBuilder)
        {
            var acl = this.CreateAdminAcl();

            var generatorOptions = new Pbkdf2GeneratorOptions(
                1500, salt: Convert.FromBase64String(
                    "A+16bv/SvaC7ZJgS7u+CB8nN32PBUAbJuT09NigsCzQx6/CxS1I/5l" +
                    "aUaFoJNZ3QhTm4TqFnWYzokdrvrUxbOEN0MN3ZhINcblSLF9LwbZeiT0" +
                    "nYOnQgTBEPL0KoszXdm8x2mYXHAJFYQ9KOsIZregzuiBQfSqsFfR2uDn" +
                    "FHm9o="));
            var generator = new Pbkdf2Generator(generatorOptions);
            var hash = generator.HashPassword("ProntoMIA.");
            var admin = new User(
                "Admin",
                hash,
                Pbkdf2Generator.Identifier,
                generator.GetOptions().ToJson())
            {
                Id = -1,
            };

            modelBuilder.Entity<AccessControlList>().HasData(acl);
            modelBuilder.Entity<User>().HasData(admin);
        }

        private AccessControlList CreateAdminAcl()
        {
            return new AccessControlList(-1)
            {
                Id = -1,
                CanEditUsers = true,
                CanViewUsers = true,
                CanViewDepartments = true,
                CanEditDepartments = true,
                CanEditDeploymentPlans = true,
                CanViewDeploymentPlans = true,
                CanViewExternalNews = true,
                CanEditExternalNews = true,
                CanViewInternalNews = true,
                CanEditInternalNews = true,
                CanViewEducationalContent = true,
                CanEditEducationalContent = true,
                CanViewAppointment = true,
                CanEditAppointment = true,
            };
        }
    }
}
