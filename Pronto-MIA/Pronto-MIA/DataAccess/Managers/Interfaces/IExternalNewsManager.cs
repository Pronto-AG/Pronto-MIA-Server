#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of an external news manager
    /// service.
    /// </summary>
    public interface IExternalNewsManager
    {
        /// <summary>
        /// Gets the subdirectory into which files associated with external news
        /// are persisted.
        /// </summary>
        public static string FileDirectory { get; } = "external_news";

        /// <summary>
        /// Method to overwrite the <see cref="ProntoMiaDbContext"/>
        /// used by the manager. This can be used if transactions
        /// over multiple managers have to be implemented.
        /// </summary>
        /// <param name="dbContext">The db context to be used by the
        /// manager.</param>
        public void SetDbContext(ProntoMiaDbContext dbContext);

        /// <summary>
        /// Method which creates a new external news object.
        /// </summary>
        /// <param name="title">A short description to identify the
        /// deployment plan.</param>
        /// <param name="description">A description of the external news
        /// article.</param>
        /// <param name="availableFrom">Moment from which the deployment plan
        /// should be treated as active.</param>
        /// <returns>The new external news.</returns>
        public Task<ExternalNews>
            Create(
                string title,
                string description,
                DateTime availableFrom);

        /// <summary>
        /// Updates the external news identified by the given id.
        /// </summary>
        /// <param name="id">The id of the external news to be updated.
        /// </param>
        /// <param name="title">A short description to identify the
        /// deployment plan.</param>
        /// <param name="description">A description of the external news
        /// article.</param>
        /// <param name="availableFrom">Moment from which the deployment plan
        /// should be treated as active.</param>
        /// <returns>The updated external news.</returns>
        /// <exception cref="QueryException">If the external news to be
        /// updated could not be found.</exception>
        public Task<ExternalNews>
            Update(
                int id,
                string? title,
                string? description,
                DateTime? availableFrom);

        /// <summary>
        /// Sets the status of the external news identified by the given id to
        /// published.
        /// </summary>
        /// <param name="id">Id of the external news to be published.</param>
        /// <returns>True if the status was changed false if the external news
        /// was already published.</returns>
        /// <exception cref="QueryException">If the external news to publish
        /// could not be found.</exception>
        public Task<bool> Publish(int id);

        /// <summary>
        /// Sets the status of the external news identified by the given id to
        /// not published.
        /// </summary>
        /// <param name="id">Id of the external news.</param>
        /// <returns>True if the status was changed false if the external news
        /// was already hidden.</returns>
        /// <exception cref="QueryException">If the external news to change
        /// could not be found.</exception>
        public Task<bool> Hide(int id);

        /// <summary>
        /// Removes the external news identified by the given id.
        /// </summary>
        /// <param name="id">Id of the external news to be removed.</param>
        /// <returns>The id of the external news that was removed.</returns>
        /// <exception cref="QueryException">If the external news to remove
        /// could not be found.</exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get all available external news.
        /// </summary>
        /// <returns>All available external news.</returns>
        public IQueryable<ExternalNews> GetAll();

        /// <summary>
        /// Method to get a external news with the help of its id.
        /// </summary>
        /// <param name="id">The id of the external news.</param>
        /// <returns>The external news with the given id.</returns>
        /// <exception cref="QueryException">If the external news
        /// with the given id could not be found.</exception>
        public Task<ExternalNews> GetById(int id);
    }
}
