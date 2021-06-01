namespace Pronto_MIA.Domain.Entities
{
    using System.Collections.Generic;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces;

    /// <summary>
    /// Class representing a department.
    /// </summary>
    public class Department : IDepartmentComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Department"/> class.
        /// </summary>
        /// <param name="name">The name of the department.</param>
        public Department(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the id of the department.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets list of <see cref="User"/> belonging to this
        /// department.
        /// </summary>
        public virtual ICollection<User> Users { get; set; }

        /// <summary>
        /// Gets or sets list of <see cref="DeploymentPlan"/> belonging to this
        /// department.
        /// </summary>
        public virtual ICollection<DeploymentPlan> DeploymentPlans { get; set; }

        public int? DepartmentId
        {
            get
            {
                return Id;
            }
        }
    }
}
