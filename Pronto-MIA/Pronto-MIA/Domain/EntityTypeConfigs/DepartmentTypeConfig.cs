namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="Department"/> entity.
    /// </summary>
    public class DepartmentTypeConfig : IEntityTypeConfiguration<Department>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="Department"/> entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.HasIndex(d => d.Name).IsUnique();
            builder.Property(d => d.Name).IsRequired();
            builder.HasMany<User>(d => d.Users)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasMany<DeploymentPlan>(d => d.DeploymentPlans)
                .WithOne(dp => dp.Department)
                .HasForeignKey(dp => dp.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
