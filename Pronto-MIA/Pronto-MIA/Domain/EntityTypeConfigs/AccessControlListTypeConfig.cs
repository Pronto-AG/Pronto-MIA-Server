namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="AccessControlList"/> entity.
    /// </summary>
    public class AccessControlListTypeConfig :
        IEntityTypeConfiguration<AccessControlList>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="AccessControlList"/> 1entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<AccessControlList> builder)
        {
            builder.HasKey(acl => acl.Id);
            builder.Property(acl => acl.Id).ValueGeneratedOnAdd();
            builder.HasOne<User>(acl => acl.User)
                .WithOne(u => u.AccessControlList)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
