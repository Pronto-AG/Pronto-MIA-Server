namespace Pronto_MIA.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;

    /// <summary>
    /// Class representing a user department of the application.
    /// </summary>
    public class UserDepartment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDepartment"/> class.
        /// </summary>
        /// <param name="departmentId"><see cref="DepartmentId"/>.</param>
        /// <param name="userId"><see cref="UserId"/>.</param>
        public UserDepartment(
            int departmentId,
            int userId)
        {
            this.DepartmentId = departmentId;
            this.UserId = userId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDepartment"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected UserDepartment()
        {
            this.DepartmentId = default;
            this.UserId = default;
        }

        /// <summary>
        /// Gets or sets the id used as primary key by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the department associated with this user.
        /// </summary>
        [GraphQLIgnore]
        public int DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the id of the user associated with this department.
        /// </summary>
        [GraphQLIgnore]
        public int UserId { get; set; }
    }
}
