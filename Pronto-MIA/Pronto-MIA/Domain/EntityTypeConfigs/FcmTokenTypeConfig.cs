namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="FcmToken"/> entity.
    /// </summary>
    public class FcmTokenTypeConfig : IEntityTypeConfiguration<FcmToken>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="FcmToken"/> 1entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<FcmToken> builder)
        {
            builder.HasKey(token => token.Id);
            builder.HasOne<User>(token => token.Owner)
                .WithMany(u => u.FCMTokens)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
