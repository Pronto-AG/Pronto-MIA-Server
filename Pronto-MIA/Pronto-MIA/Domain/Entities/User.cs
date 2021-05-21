namespace Pronto_MIA.Domain.Entities
{
    using System.Collections.Generic;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;

    /// <summary>
    /// Class representing a user of the application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="userName"><see cref="UserName"/>.</param>
        /// <param name="passwordHash"><see cref="PasswordHash"/>.</param>
        /// <param name="hashGenerator"><see cref="HashGenerator"/>.</param>
        /// <param name="hashGeneratorOptions">
        /// <see cref="HashGeneratorOptions"/>.</param>
        /// <param name="departmentId">
        /// <see cref="DepartmentId"/>.</param>
        public User(
            string userName,
            byte[] passwordHash,
            string hashGenerator,
            string hashGeneratorOptions)
        {
            this.UserName = userName;
            this.PasswordHash = passwordHash;
            this.HashGenerator = hashGenerator;
            this.HashGeneratorOptions = hashGeneratorOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected User()
        {
            this.UserName = default;
            this.PasswordHash = default;
            this.HashGenerator = default;
            this.HashGeneratorOptions = default;
        }

        /// <summary>
        /// Gets or sets the id used as primary key by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name the user uses in order to authenticate
        /// himself.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the access control list associated with this
        /// user.
        /// </summary>
        public virtual AccessControlList AccessControlList { get; set; }

        /// <summary>
        /// Gets or sets the id of the department associated with this user.
        /// </summary>
        [GraphQLIgnore]
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the access control list associated with this
        /// user.
        /// </summary>
        [Authorize(Policy = "CanViewDepartments")]
        public virtual Department Department { get; set; }

        /// <summary>
        /// Gets or sets the hash of the password the user would like to use in
        /// order to authenticate.
        /// </summary>
        [GraphQLIgnore]
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the name of the HashGenerator used to create the users
        /// hash.
        /// </summary>
        [GraphQLIgnore]
        public string HashGenerator { get; set; }

        /// <summary>
        /// Gets or sets the JSON representation of the options used by the hash
        /// generator for creating this users hash.
        /// </summary>
        [GraphQLIgnore]
        public string HashGeneratorOptions { get; set; }

        /// <summary>
        /// Gets or sets list of <see cref="FcmToken"/> belonging to this user.
        /// </summary>
        [GraphQLIgnore]
        public virtual ICollection<FcmToken> FcmTokens { get; set; }
    }
}
