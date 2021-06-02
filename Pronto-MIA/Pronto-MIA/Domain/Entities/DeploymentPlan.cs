namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;

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
        /// <param name="fileUuid">The
        /// <see cref="FileUuid"/> property.</param>
        /// <param name="fileExtension">The
        /// <see cref="FileExtension"/> property.</param>
        /// <param name="description">The
        /// <see cref="Description"/> property.</param>
        public DeploymentPlan(
            DateTime availableFrom,
            DateTime availableUntil,
            Guid fileUuid,
            string fileExtension,
            string description = null)
        {
            this.AvailableFrom = availableFrom;
            this.AvailableUntil = availableUntil;
            this.FileUuid = fileUuid;
            this.FileExtension = fileExtension;
            this.Description = description;
            this.Published = false;
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
            this.FileUuid = Guid.Empty;
            this.FileExtension = string.Empty;
            this.Description = null;
            this.Published = false;
        }

        /// <summary>
        /// Gets or sets the id used as primary key by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the description which may be used to identify the
        /// deployment plan.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the moment from which this deployment plan should be
        /// treated as active.
        /// </summary>
        public DateTime AvailableFrom { get; set; }

        /// <summary>
        /// Gets or sets the moment until which the deployment plan will be
        /// treated as active.
        /// </summary>
        public DateTime AvailableUntil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the deployment plan is in
        /// published status. Unpublished deployment plans should not be shown
        /// to users without administrative permissions.
        /// </summary>
        public bool Published { get; set; }

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
        /// Gets or sets the id of the department associated with this
        /// deployment plan.
        /// </summary>
        [GraphQLIgnore]
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the department associated with this deployment
        /// plan.
        /// </summary>
        [Authorize(Policy = "CanViewDepartments")]
        public virtual Department Department { get; set; }
    }
}
