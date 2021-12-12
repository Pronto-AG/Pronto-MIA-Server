namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;

    /// <summary>
    /// Class which represents an educational content object.
    /// </summary>
    public class EducationalContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EducationalContent"/>
        /// class.
        /// </summary>
        /// <param name="title">The
        /// <see cref="Title"/> property.</param>
        /// <param name="description">The
        /// <see cref="Description"/> property.</param>
        /// <param name="fileUuid">The
        /// <see cref="FileUuid"/> property.</param>
        /// <param name="fileExtension">The
        /// <see cref="FileExtension"/> property.</param>
        public EducationalContent(
            string title,
            string description,
            Guid fileUuid,
            string fileExtension)
        {
            this.Title = title;
            this.Description = description;
            this.FileUuid = fileUuid;
            this.FileExtension = fileExtension;
            this.Published = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EducationalContent"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected EducationalContent()
        {
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
        /// Gets or sets the uuid used as file name for the file associated with
        /// this educational content.
        /// </summary>
        [GraphQLIgnore]
        public Guid FileUuid { get; set; }

        /// <summary>
        /// Gets or sets the file extension of the file associated with this
        /// educational content.
        /// </summary>
        [GraphQLIgnore]
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the educational content is
        /// in published status. Unpublished external news should not be shown
        /// to users without administrative permissions.
        /// </summary>
        public bool Published { get; set; }
    }
}
