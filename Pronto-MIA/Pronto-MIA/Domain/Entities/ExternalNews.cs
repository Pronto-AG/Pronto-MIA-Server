namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;

    /// <summary>
    /// Class which represents an external news object.
    /// </summary>
    public class ExternalNews
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalNews"/>
        /// class.
        /// </summary>
        /// <param name="availableFrom">The
        /// <see cref="AvailableFrom"/> property.</param>
        /// <param name="title">The
        /// <see cref="Title"/> property.</param>
        /// <param name="description">The
        /// <see cref="Description"/> property.</param>
        public ExternalNews(
            string title,
            string description,
            DateTime availableFrom)
        {
            this.AvailableFrom = availableFrom;
            this.Title = title;
            this.Description = description;
            this.Published = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalNews"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected ExternalNews()
        {
            this.AvailableFrom = default;
            this.Title = null;
            this.Description = null;
            this.Published = false;
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
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the moment from which this external news should be
        /// treated as active.
        /// </summary>
        public DateTime AvailableFrom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the external news is in
        /// published status. Unpublished external news should not be shown
        /// to users without administrative permissions.
        /// </summary>
        public bool Published { get; set; }
    }
}
