namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;

    /// <summary>
    /// Class which represents an internal news object.
    /// </summary>
    public class InternalNews
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalNews"/>
        /// class.
        /// </summary>
        /// <param name="availableFrom">The
        /// <see cref="AvailableFrom"/> property.</param>
        /// <param name="title">The
        /// <see cref="Title"/> property.</param>
        /// <param name="description">The
        /// <see cref="Description"/> property.</param>
        /// <param name="fileUuid">The
        /// <see cref="FileUuid"/> property.</param>
        /// <param name="fileExtension">The
        /// <see cref="FileExtension"/> property.</param>
        public InternalNews(
            string title,
            string description,
            DateTime availableFrom,
            Guid fileUuid,
            string fileExtension)
        {
            this.AvailableFrom = availableFrom;
            this.Title = title;
            this.Description = description;
            this.FileUuid = fileUuid;
            this.FileExtension = fileExtension;
            this.Published = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalNews"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected InternalNews()
        {
            this.AvailableFrom = default;
            this.Title = null;
            this.Description = null;
            this.FileUuid = Guid.Empty;
            this.FileExtension = string.Empty;
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
        /// Gets or sets the moment from which this internal news should be
        /// treated as active.
        /// </summary>
        public DateTime AvailableFrom { get; set; }

        /// <summary>
        /// Gets or sets the uuid used as file name for the file associated with
        /// this deployment plan.
        /// </summary>
        [GraphQLIgnore]
        public Guid FileUuid { get; set; }

        /// <summary>
        /// Gets or sets the file extension of the file associated with this
        /// deployment plan.
        /// </summary>
        [GraphQLIgnore]
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the internal news is in
        /// published status. Unpublished internal news should not be shown
        /// to users without administrative permissions.
        /// </summary>
        public bool Published { get; set; }
    }
}
