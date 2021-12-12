namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;

    /// <summary>
    /// Class which represents an appointment object.
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Appointment"/>
        /// class.
        /// </summary>
        /// <param name="title">The
        /// <see cref="Title"/> property.</param>
        /// <param name="location">The
        /// <see cref="Location"/> property.</param>
        /// <param name="from">The
        /// <see cref="From"/> property.</param>
        /// <param name="to">The
        /// <see cref="To"/> property.</param>
        /// <param name="isAllDay">The
        /// <see cref="IsAllDay"/> property.</param>
        /// <param name="isYearly">The
        /// <see cref="IsYearly"/> property.</param>
        public Appointment(
            string title,
            string location,
            DateTime from,
            DateTime to,
            bool isAllDay,
            bool isYearly)
        {
            this.Title = title;
            this.Location = location;
            this.From = from;
            this.To = to;
            this.IsAllDay = isAllDay;
            this.IsYearly = isYearly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Appointment"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected Appointment()
        {
            this.Title = null;
            this.Location = null;
            this.From = default;
            this.To = default;
            this.IsAllDay = false;
            this.IsYearly = false;
        }

        /// <summary>
        /// Gets or sets the id used as primary key by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title which is needed to identify the object.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the start datetime.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets the end datetime.
        /// </summary>
        public DateTime To { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment
        /// is an all day event or not.
        /// </summary>
        public bool IsAllDay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appointment
        /// is a yearly event or not.
        /// </summary>
        public bool IsYearly { get; set; }
    }
}
