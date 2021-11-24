namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="UserDepartment"/> entity.
    /// </summary>
    public class UserDepartmentTypeConfig
        : IEntityTypeConfiguration<UserDepartment>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="UserDepartment"/>
        /// entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<UserDepartment> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.DepartmentId).IsRequired();
            builder.Property(u => u.UserId).IsRequired();
        }
    }
}
