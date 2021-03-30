namespace Pronto_MIA.BusinessLogic.Security.Interfaces
{
    using System;

    /// <summary>
    /// Interface which defines common functionality of the hash generator
    /// options which is needed by the application in order to use them
    /// interchangeably.
    /// </summary>
    public interface IHashGeneratorOptions
    {
        /// <summary>
        /// Method to create a <see cref="IHashGeneratorOptions"/> object from
        /// its serialized Json-String.
        /// </summary>
        /// <param name="json">The serialized representation of the generator
        /// options which should be restored.</param>
        /// <returns>Deserialized <see cref="IHashGeneratorOptions"/> object.
        /// </returns>
        /// <exception cref="ApplicationException">If this method is called
        /// directly on the <see cref="IHashGeneratorOptions"/> interface.
        /// </exception>
        public static IHashGeneratorOptions FromJson(string json)
        {
            throw new ApplicationException("Method needs to be called" +
                                           " on an implementation of the" +
                                           " interface.");
        }

        /// <summary>
        /// Method to generate a JSON-String from the options object.
        /// </summary>
        /// <returns>Serialized options object.</returns>
        public string ToJson();

        /// <summary>
        /// Calls the respective <see cref="ToJson"/> method in order to
        /// retrieve the json representation of the object.
        /// </summary>
        /// <returns>Serialized object in json format.</returns>
        public string ToString() => this.ToJson();

        /// <summary>
        /// Method to determine if two option sets are the same.
        /// </summary>
        /// <remarks>It is important that equality is determined without taking
        /// the user specific options into account, since this method is used to
        /// check if the user is using the most recent version of hash
        /// generator by comparing different option objects.</remarks>
        /// <param name="other">The options which should be compared with this
        /// object.</param>
        /// <returns>True if the options are the same excluding user specific
        /// options like salt.</returns>
        public bool IsSame(IHashGeneratorOptions other);
    }
}
