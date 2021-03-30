namespace Pronto_MIA.BusinessLogic.Security
{
    using System;
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Pronto_MIA.BusinessLogic.Security.Abstract;
    using Pronto_MIA.BusinessLogic.Security.Interfaces;

    /// <summary>
    /// Class representing a hash generator which uses the Pbkdf2 algorithm to
    /// generate hashes. The Pbkdf2 hash algorithm is the only one which is
    /// cryptographically proven and directly implemented into C# therefore this
    /// is the standard hash generator.
    /// </summary>
    public class Pbkdf2Generator : HashGenerator
    {
        private Pbkdf2GeneratorOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pbkdf2Generator"/>
        /// class.
        /// </summary>
        /// <param name="options">The options necessary for creating a new
        /// password hash with the Pbkdf2 algorithm.</param>
        public Pbkdf2Generator(Pbkdf2GeneratorOptions options)
            : base(options)
        {
            this.options = options;
        }

        /// <summary>
        /// Hashes a password with Pbkdf2 with given salt and round count as
        /// well as HMACSHA512.
        /// </summary>
        /// <param name="password">Password to be hashed with given options.
        /// </param>
        /// <returns>Hash encoded in base64.</returns>
        public override byte[] HashPassword(
            string password)
        {
            if (this.options.Salt == null)
            {
                var salt = new byte[this.options.SaltSize];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
                this.options = new Pbkdf2GeneratorOptions(
                    this.options.HashIterations,
                    this.options.HashSize,
                    this.options.SaltSize,
                    salt);
            }
            else
            {
                if (this.options.Salt.Length != this.options.SaltSize)
                {
                    throw new ArgumentException("Invalid salt");
                }
            }

            var hash = KeyDerivation.Pbkdf2(
                password,
                this.options.Salt!,
                KeyDerivationPrf.HMACSHA512,
                this.options.HashIterations,
                this.options.HashSize);
            return hash;
        }

        /// <summary>
        /// Validates a password.
        /// </summary>
        /// <param name="password">Submitted password.</param>
        /// <param name="hash">Hash of the actual password.</param>
        /// <returns>Whether the password is correct or not.</returns>
        public override bool ValidatePassword(
            string password,
            byte[] hash)
        {
            var computedHash = this.HashPassword(password);
            return Convert.ToBase64String(computedHash) ==
                   Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Method to retrieve the options currently configured on this hash
        /// generator instance.
        /// </summary>
        /// <returns>The options used by this hash generator instance in order
        /// to create password hashes.</returns>
        public override IHashGeneratorOptions GetOptions()
        {
            return this.options;
        }
    }
}
