namespace Pronto_MIA.Domain.EntityTypeConfigs
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Database configuration for the <see cref="Appointment"/> entity.
    /// </summary>
    public class AppointmentTypeConfig :
        IEntityTypeConfiguration<Appointment>
    {
        /// <summary>
        /// Configures the database options required by the
        /// <see cref="Appointment"/> entity.
        /// </summary>
        /// <param name="builder">The builder which will be used to create new
        /// entities of this type.</param>
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.Location);
            builder.Property(e => e.From).IsRequired();
            builder.Property(e => e.To).IsRequired();
            builder.Property(e => e.IsAllDay).IsRequired();
            builder.Property(e => e.IsYearly).IsRequired();
        }
    }
}
