namespace Pronto_MIA.Domain.Entities
{
    using HotChocolate;

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
        /// <see cref="HashGeneratorOptions"/>
        /// <see cref="HashGeneratorOptions"/>.</param>
        public User(
            string userName,
            byte[] passwordHash,
            string hashGenerator,
            string hashGeneratorOptions = "{}")
        {
            this.UserName = userName;
            this.PasswordHash = passwordHash;
            this.HashGenerator = hashGenerator;
            this.HashGeneratorOptions = hashGeneratorOptions;
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
    }
}
