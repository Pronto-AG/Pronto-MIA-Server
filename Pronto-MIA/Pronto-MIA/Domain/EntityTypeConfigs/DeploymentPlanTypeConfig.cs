namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="DeploymentPlan"/> entity.
    /// </summary>
    public class DeploymentPlanTypeConfig :
        IEntityTypeConfiguration<DeploymentPlan>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="DeploymentPlan"/> entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<DeploymentPlan> builder)
        {
            builder.HasKey(dP => dP.Id);
            builder.Property(dP => dP.Id).ValueGeneratedOnAdd();
            builder.Property(dP => dP.AvailableFrom).IsRequired();
            builder.Property(dP => dP.AvailableUntil).IsRequired();
            builder.Property(dP => dP.FileUUID).IsRequired();
            builder.HasIndex(dP => dP.FileUUID).IsUnique();
            builder.Property(dP => dP.FileUUID)
                .HasConversion(
                    uuid => uuid.ToString(),
                    uuid => Guid.Parse(uuid));
            builder.Property(dP => dP.FileExtension).IsRequired();
        }
    }
}
