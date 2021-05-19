namespace Tests.TestBusinessLogic.TestSecurity
{
    using Pronto_MIA.BusinessLogic.Security.Abstract;
    using Pronto_MIA.BusinessLogic.Security.Interfaces;

    /// <summary>
    /// Empty implementation of the HashGenerator. Primarily used for testing.
    /// This class should never be used in a production environment.
    /// </summary>
    public class NullGenerator : HashGenerator
    {
        /// <summary>
        /// Identifier of this generator. Used to differentiate between
        /// implementations of the HashGenerator.
        /// </summary>
        public const string Identifier = "NullGenerator";

        private readonly IHashGeneratorOptions options;


        /// <summary>
        /// Initializes a new instance of the <see cref="NullGenerator"/> class.
        /// </summary>
        /// <param name="options">The options to be included.</param>
        public NullGenerator(IHashGeneratorOptions options)
            : base(options)
        {
            this.options = options;
        }

        /// <summary>
        /// Returns an empty byte array of size 10.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>Empty byte array of size 10.</returns>
        public override byte[] HashPassword(string password)
        {
            return new byte[10];
        }

        /// <summary>
        /// Null implementation that always returns true.
        /// </summary>
        /// <param name="password">The password to be validated.</param>
        /// <param name="hash">The hash to be checked against.</param>
        /// <returns>Always true.</returns>
        public override bool ValidatePassword(string password, byte[] hash)
        {
            return true;
        }

        /// <inheritdoc/>
        public override IHashGeneratorOptions GetOptions()
        {
            return options;
        }

        /// <inheritdoc/>
        public override string GetIdentifier()
        {
            return Identifier;
        }
    }
}