namespace Pronto_MIA.BusinessLogic.Security
{
    using System;
    using Newtonsoft.Json;
    using Pronto_MIA.BusinessLogic.Security.Interfaces;

    /// <summary>
    /// Class containing the options necessary in order to create a hash by
    /// using the Pbkdf2 hash algorithm.
    /// </summary>
    public class Pbkdf2GeneratorOptions : IHashGeneratorOptions
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Pbkdf2GeneratorOptions"/> class.
        /// </summary>
        /// <param name="hashIterations"><see cref="HashIterations"/>.</param>
        /// <param name="saltSize"><see cref="SaltSize"/>.</param>
        /// <param name="hashSize"><see cref="HashSize"/>.</param>
        /// <param name="salt">The salt to be used by the generator to enrich
        /// the password with.</param>
        public Pbkdf2GeneratorOptions(
            int hashIterations,
            int hashSize = 512,
            int saltSize = 128,
            byte[] salt = null)
        {
            this.HashIterations = hashIterations;
            this.HashSize = hashSize;
            this.SaltSize = saltSize;
            this.Salt = salt;
        }

        /// <summary>
        /// Gets size of the salt to be used to enrich the password with.
        /// </summary>
        public int SaltSize { get; }

        /// <summary>
        /// Gets the count of iterations the Pbkdf2 hash
        /// algorithm should use in order to generate the hash.
        /// </summary>
        public int HashIterations { get; }

        /// <summary>
        /// Gets the size of the resulting hash generated by
        /// the hash generator.
        /// </summary>
        public int HashSize { get; }

        /// <summary>
        /// Gets the salt which is used to add more entropy to the users
        /// password.
        /// </summary>
        public byte[] Salt { get; }

        /// <summary>
        /// Method to create a <see cref="Pbkdf2GeneratorOptions"/> object from
        /// a json string.
        /// </summary>
        /// <param name="json">The string representation of the serialized
        /// <see cref="Pbkdf2GeneratorOptions"/> object.</param>
        /// <returns>The deserialized <see cref="Pbkdf2GeneratorOptions"/>
        /// object.</returns>
        /// <exception cref="ArgumentException">If the string provided cannot
        /// be deserialized into a <see cref="Pbkdf2GeneratorOptions"/> object.
        /// </exception>
        public static IHashGeneratorOptions FromJson(string json)
        {
            try
            {
                var options =
                    JsonConvert.DeserializeObject<Pbkdf2GeneratorOptions>(
                        json);
                return options;
            }
            catch
            {
                throw new ArgumentException(
                    "Argument is no Pbkdf2GeneratorOptions object");
            }
        }

        /// <inheritdoc/>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <inheritdoc/>
        public bool IsSame(IHashGeneratorOptions other)
        {
            try
            {
                var otherTemp = (Pbkdf2GeneratorOptions)other;
                if (this.SaltSize == otherTemp.SaltSize &&
                    this.HashIterations == otherTemp.HashIterations &&
                    this.HashSize == otherTemp.HashSize)
                {
                    return true;
                }

                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }
    }
}
