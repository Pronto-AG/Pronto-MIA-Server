namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="EducationalContent"/> entity.
    /// </summary>
    public class EducationalContentTypeConfig :
        IEntityTypeConfiguration<EducationalContent>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="EducationalContent"/> entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<EducationalContent> builder)
        {
            builder.HasKey(eC => eC.Id);
            builder.Property(eC => eC.Id).ValueGeneratedOnAdd();
            builder.Property(eC => eC.Title).IsRequired();
            builder.Property(eC => eC.Description).IsRequired();
            builder.Property(eC => eC.Published).IsRequired();
            builder.Property(eC => eC.FileUuid).IsRequired();
            builder.HasIndex(eC => eC.FileUuid).IsUnique();
            builder.Property(eC => eC.FileUuid)
                .HasConversion(
                    uuid => uuid.ToString(),
                    uuid => Guid.Parse(uuid));
            builder.Property(eC => eC.FileExtension).IsRequired();
        }
    }
}
