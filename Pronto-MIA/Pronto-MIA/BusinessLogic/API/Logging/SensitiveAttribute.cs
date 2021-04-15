namespace Pronto_MIA.BusinessLogic.API.Logging
{
    using System;

    /// <summary>
    /// Attribute which may be used to mark request parameters in graphql as
    /// sensitive.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SensitiveAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensitiveAttribute"/>
        /// class.
        /// </summary>
        /// <param name="queryPropertyName">The name of the property which$
        /// should be hidden inside the log.</param>
        public SensitiveAttribute(string queryPropertyName)
        {
            this.QueryPropertyName = queryPropertyName;
        }

        /// <summary>
        /// Gets the name of the property which should be hidden inside the log.
        /// </summary>
        public string QueryPropertyName { get; }
    }
}
