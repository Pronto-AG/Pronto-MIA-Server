namespace Pronto_MIA.Domain.Entities
{
    using System.Collections.Generic;
    using HotChocolate;

    public class Department
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Department"/> class.
        /// </summary>
        /// <param name="name">The name of the department.</param>
        public Department(string name)
        {
            this.Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Gets or sets list of <see cref="User"/> belonging to this
        /// department.
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
    }
}
