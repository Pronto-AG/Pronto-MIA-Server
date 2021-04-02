namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;

    public class DeploymentPlan
    {
        public DeploymentPlan(
            DateTime availableFrom,
            DateTime availableUntil)
            : this(availableFrom, availableUntil, Guid.NewGuid())
        {
        }

        public DeploymentPlan(
            DateTime availableFrom,
            DateTime availableUntil,
            Guid fileUUID)
        {
            this.AvailableFrom = availableFrom;
            this.AvailableUntil = availableUntil;
            this.fileUUID = fileUUID;
        }

        /// <summary>
        /// Gets or sets the id used as primary key by the database.
        /// </summary>
        public int Id { get; set; }

        public DateTime AvailableFrom { get; }

        public DateTime AvailableUntil { get; }

        [GraphQLIgnore]
        public Guid fileUUID { get; }
    }
}
