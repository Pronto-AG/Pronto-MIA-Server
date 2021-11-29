namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="User"/> entity.
    /// </summary>
    public class UserTypeConfig : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures the database options required by the <see cref="User"/>
        /// entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.Property(u => u.UserName).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.HashGeneratorOptions)
                .IsRequired()
                .HasColumnType("jsonb");
            builder.HasMany<FcmToken>(u => u.FcmTokens)
                .WithOne(t => t.Owner);
            builder.HasOne<AccessControlList>(u => u.AccessControlList)
                .WithOne(a => a.User)
                .HasForeignKey<AccessControlList>(acl => acl.UserId);
            builder.HasMany<Department>(u => u.Departments)
                .WithMany(d => d.Users);
        }
    }
}
