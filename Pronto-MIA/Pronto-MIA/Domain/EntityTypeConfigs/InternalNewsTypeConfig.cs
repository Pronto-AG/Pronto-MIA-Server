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
            builder.HasKey(iN => iN.Id);
            builder.Property(iN => iN.Id).ValueGeneratedOnAdd();
            builder.Property(iN => iN.Title).IsRequired();
            builder.Property(iN => iN.Description).IsRequired();
            builder.Property(iN => iN.AvailableFrom).IsRequired();
            builder.Property(iN => iN.Published).IsRequired();
            builder.Property(iN => iN.FileUuid).IsRequired();
            builder.HasIndex(iN => iN.FileUuid).IsUnique();
            builder.Property(iN => iN.FileUuid)
                .HasConversion(
                    uuid => uuid.ToString(),
                    uuid => Guid.Parse(uuid));
            builder.Property(iN => iN.FileExtension).IsRequired();
        }
    }
}
