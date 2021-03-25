namespace Pronto_MIA.Data
{
    using System.Linq;
    using HotChocolate;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Class representing the query operation of graphql.
    /// </summary>
    public class Query
    {
        #pragma warning disable SA1642
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="cfg">The configuration used by the query class.</param>
        #pragma warning restore SA1642
        public Query(IConfiguration cfg)
        {
            this.Cfg = cfg;
        }

        private IConfiguration Cfg { get; }

        /// <summary>
        /// Method to get speakers from the database.
        /// </summary>
        /// <param name="context">The database context to get the speakers from.
        /// </param>
        /// <returns>An IQueryable containing the speakers.</returns>
        public IQueryable<Speaker> GetSpeakers(
            [Service] InformbobDbContext context) => context.Speakers;
    }
}
