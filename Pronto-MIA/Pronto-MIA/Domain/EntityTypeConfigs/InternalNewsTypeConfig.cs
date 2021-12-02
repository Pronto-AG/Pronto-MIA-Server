namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="InternalNews"/> entity.
    /// </summary>
    public class InternalNewsTypeConfig :
        IEntityTypeConfiguration<InternalNews>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="InternalNews"/> entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<InternalNews> builder)
        {
            builder.HasKey(eN => eN.Id);
            builder.Property(eN => eN.Id).ValueGeneratedOnAdd();
            builder.Property(eN => eN.Title).IsRequired();
            builder.Property(eN => eN.Description).IsRequired();
            builder.Property(eN => eN.AvailableFrom).IsRequired();
            builder.Property(eN => eN.Published).IsRequired();
            builder.Property(eN => eN.FileUuid).IsRequired();
            builder.HasIndex(eN => eN.FileUuid).IsUnique();
            builder.Property(eN => eN.FileUuid)
                .HasConversion(
                    uuid => uuid.ToString(),
                    uuid => Guid.Parse(uuid));
            builder.Property(eN => eN.FileExtension).IsRequired();
        }
    }
}
