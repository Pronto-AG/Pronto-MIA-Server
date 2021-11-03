namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="ExternalNews"/> entity.
    /// </summary>
    public class ExternalNewsTypeConfig :
        IEntityTypeConfiguration<ExternalNews>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="ExternalNews"/> entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<ExternalNews> builder)
        {
            builder.HasKey(eN => eN.Id);
            builder.Property(eN => eN.Id).ValueGeneratedOnAdd();
            builder.Property(eN => eN.Title).IsRequired();
            builder.Property(eN => eN.Description).IsRequired();
            builder.Property(eN => eN.AvailableFrom).IsRequired();
            builder.Property(eN => eN.Published).IsRequired();
        }
    }
}
