namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;

    /// <summary>
    /// Class which represents a deployment plan object.
    /// </summary>
    public class DeploymentPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentPlan"/>
        /// class.
        /// </summary>
        /// <param name="availableFrom">The
        /// <see cref="AvailableFrom"/> property.</param>
        /// <param name="availableUntil">The
        /// <see cref="AvailableUntil"/> property.</param>
        /// <param name="fileUUID">The
        /// <see cref="FileUUID"/> property.</param>
        /// <param name="fileExtension">The
        /// <see cref="FileExtension"/> property.</param>
        public DeploymentPlan(
            DateTime availableFrom,
            DateTime availableUntil,
            Guid fileUUID,
            string fileExtension)
        {
            this.AvailableFrom = availableFrom;
            this.AvailableUntil = availableUntil;
            this.FileUUID = fileUUID;
            this.FileExtension = fileExtension;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentPlan"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected DeploymentPlan()
        {
            this.AvailableFrom = default;
            this.AvailableUntil = default;
            this.FileUUID = Guid.Empty;
            this.FileExtension = string.Empty;
        }

        /// <summary>
        /// Gets or sets the id used as primary key by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the moment from which this deployment plan should be treated as
        /// active.
        /// </summary>
        public DateTime AvailableFrom { get; }

        /// <summary>
        /// Gets the moment until which the deployment plan will be treated as
        /// active.
        /// </summary>
        public DateTime AvailableUntil { get; }

        /// <summary>
        /// Gets the uuid used as file name for the file associated with this
        /// deployment plan.
        /// </summary>
        [GraphQLIgnore]
        public Guid FileUUID { get; }

        /// <summary>
        /// Gets the file extension of the file associated with this deployment
        /// plan.
        /// </summary>
        [GraphQLIgnore]
        public string FileExtension { get; }
    }
}
