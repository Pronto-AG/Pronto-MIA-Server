namespace Pronto_MIA.BusinessLogic.Security.Abstract
{
    using Pronto_MIA.BusinessLogic.Security.Interfaces;

    /// <summary>
    /// Abstract class for hash generators used in order to secure user
    /// passwords.
    /// </summary>
    public abstract class HashGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashGenerator"/> class.
        /// </summary>
        /// <param name="options">The specific options needed by the concrete
        /// implementation of the hash generator.</param>
        protected HashGenerator(IHashGeneratorOptions options)
        {
        }

        /// <summary>
        /// Hashes the given password. Options like salt etc will be given to
        /// the generator by using the <see cref="IHashGeneratorOptions"/>
        /// object.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>A tuple containing the Password (first) and the salt
        /// (second).</returns>
        public abstract byte[] HashPassword(
            string password);

        /// <summary>
        /// Method to validate if the given password matches the given hash
        /// combined with the given salt.
        /// </summary>
        /// <param name="password">Password to be checked.</param>
        /// <param name="hash">Hash to check the password against.</param>
        /// <returns>True if the password matches else false.</returns>
        public abstract bool ValidatePassword(
            string password,
            byte[] hash);

        /// <summary>
        /// Returns the options currently configured on this HashGenerator
        /// instance.
        /// </summary>
        /// <returns>The <see cref="IHashGeneratorOptions"/> object
        /// for this instance.
        /// </returns>
        public abstract IHashGeneratorOptions GetOptions();

        /// <summary>
        /// Method which returns the identifier of the concrete implementation
        /// of this abstract class.
        /// </summary>
        /// <returns>The identifier of the abstract class implementation.
        /// </returns>
        public abstract string GetIdentifier();
    }
}
