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
    /// Interface declaring the operations needed of an internal news manager
    /// service.
    /// </summary>
    public interface IInternalNewsManager
    {
        /// <summary>
        /// Gets the subdirectory into which files associated with internal news
        /// are persisted.
        /// </summary>
        public static string FileDirectory { get; } = "internal_news";

        /// <summary>
        /// Method to overwrite the <see cref="ProntoMiaDbContext"/>
        /// used by the manager. This can be used if transactions
        /// over multiple managers have to be implemented.
        /// </summary>
        /// <param name="dbContext">The db context to be used by the
        /// manager.</param>
        public void SetDbContext(ProntoMiaDbContext dbContext);

        /// <summary>
        /// Method which creates a new internal news object.
        /// </summary>
        /// <param name="title">A short description to identify the
        /// internal news.</param>
        /// <param name="description">A description of the internal news
        /// article.</param>
        /// <param name="availableFrom">Moment from which the internal news
        /// should be treated as active.</param>
        /// <param name="file">The file to be associated with the new internal
        /// news.</param>
        /// <returns>The new internal news.</returns>
        public Task<InternalNews>
            Create(
                string title,
                string description,
                DateTime availableFrom,
                IFile file);

        /// <summary>
        /// Updates the internal news identified by the given id.
        /// </summary>
        /// <param name="id">The id of the internal news to be updated.
        /// </param>
        /// <param name="title">A short description to identify the
        /// internal news.</param>
        /// <param name="description">A description of the internal news
        /// article.</param>
        /// <param name="availableFrom">Moment from which the internal news
        /// should be treated as active.</param>
        /// <param name="file">The new file associated with the internal news.
        /// </param>
        /// <returns>The updated internal news.</returns>
        /// <exception cref="QueryException">If the internal news to be
        /// updated could not be found.</exception>
        public Task<InternalNews>
            Update(
                int id,
                string? title,
                string? description,
                DateTime? availableFrom,
                IFile? file);

        /// <summary>
        /// Sets the status of the internal news identified by the given id to
        /// published.
        /// </summary>
        /// <param name="id">Id of the internal news to be published.</param>
        /// <returns>True if the status was changed false if the internal news
        /// was already published.</returns>
        /// <exception cref="QueryException">If the internal news to publish
        /// could not be found.</exception>
        public Task<bool> Publish(int id);

        /// <summary>
        /// Sets the status of the internal news identified by the given id to
        /// not published.
        /// </summary>
        /// <param name="id">Id of the internal news.</param>
        /// <returns>True if the status was changed false if the internal news
        /// was already hidden.</returns>
        /// <exception cref="QueryException">If the internal news to change
        /// could not be found.</exception>
        public Task<bool> Hide(int id);

        /// <summary>
        /// Removes the internal news identified by the given id.
        /// </summary>
        /// <param name="id">Id of the internal news to be removed.</param>
        /// <returns>The id of the internal news that was removed.</returns>
        /// <exception cref="QueryException">If the internal news to remove
        /// could not be found.</exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get all available internal news.
        /// </summary>
        /// <returns>All available internal news.</returns>
        public IQueryable<InternalNews> GetAll();

        /// <summary>
        /// Method to get a internal news with the help of its id.
        /// </summary>
        /// <param name="id">The id of the internal news.</param>
        /// <returns>The internal news with the given id.</returns>
        /// <exception cref="QueryException">If the internal news
        /// with the given id could not be found.</exception>
        public Task<InternalNews> GetById(int id);
    }
}
