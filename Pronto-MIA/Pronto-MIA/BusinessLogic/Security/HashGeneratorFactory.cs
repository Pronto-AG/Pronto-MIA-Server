namespace Pronto_MIA.BusinessLogic.Security
{
    using System;
    using Pronto_MIA.BusinessLogic.Security.Abstract;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for creating hash generator instances.
    /// </summary>
    public static class HashGeneratorFactory
    {
        /// <summary>
        /// Method which creates the concrete hash generator implementation
        /// needed by the given user.
        /// </summary>
        /// <param name="user">The user from which to get the required
        /// information.</param>
        /// <returns>Implementation of the <see cref="HashGenerator"/> class
        /// which is needed by the user.</returns>
        /// <exception cref="ArgumentException">If the hash generator could not
        /// be deducted from the user.</exception>
        public static HashGenerator GetGeneratorForUser(User user)
        {
            switch (user.HashGenerator)
            {
                case "Pbkdf2Generator":
                    var options = Pbkdf2GeneratorOptions.FromJson(
                        user.HashGeneratorOptions);
                    return new Pbkdf2Generator(
                        (Pbkdf2GeneratorOptions)options);
                default:
                    throw new ArgumentException(
                        "Unknown hash generator");
            }
        }
    }
}
