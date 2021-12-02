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
    /// Interface declaring the operations needed of an educational content
    /// manager service.
    /// </summary>
    public interface IEducationalContentManager
    {
        /// <summary>
        /// Gets the subdirectory into which files associated with
        /// educational content are persisted.
        /// </summary>
        public static string FileDirectory { get; } = "educational_content";

        /// <summary>
        /// Method to overwrite the <see cref="ProntoMiaDbContext"/>
        /// used by the manager. This can be used if transactions
        /// over multiple managers have to be implemented.
        /// </summary>
        /// <param name="dbContext">The db context to be used by the
        /// manager.</param>
        public void SetDbContext(ProntoMiaDbContext dbContext);

        /// <summary>
        /// Method which creates a new educational content object.
        /// </summary>
        /// <param name="title">A short description to identify the
        /// educational content.</param>
        /// <param name="description">A description of the educational content
        /// article.</param>
        /// <param name="file">The file to be associated with the educational
        /// content.</param>
        /// <returns>The new educational content.</returns>
        public Task<EducationalContent>
            Create(
                string title,
                string description,
                IFile file);

        /// <summary>
        /// Updates the educational content identified by the given id.
        /// </summary>
        /// <param name="id">The id of the educational content to be updated.
        /// </param>
        /// <param name="title">A short description to identify the
        /// educational content.</param>
        /// <param name="description">A description of the
        /// educational content.</param>
        /// <param name="file">The new file associated with the
        /// educational content.</param>
        /// <returns>The updated educational content.</returns>
        /// <exception cref="QueryException">If the educational content
        /// to be updated could not be found.</exception>
        public Task<EducationalContent>
            Update(
                int id,
                string? title,
                string? description,
                IFile? file);

        /// <summary>
        /// Sets the status of the educational content identified by the given
        /// id to published.
        /// </summary>
        /// <param name="id">Id of the educational content to be
        /// published.</param>
        /// <returns>True if the status was changed false if the
        /// educational content was already published.</returns>
        /// <exception cref="QueryException">If the educational content to
        /// publish could not be found.</exception>
        public Task<bool> Publish(int id);

        /// <summary>
        /// Sets the status of the educational content identified by the given
        /// id to not published.
        /// </summary>
        /// <param name="id">Id of the educational content.</param>
        /// <returns>True if the status was changed false if the
        /// educational content was already hidden.</returns>
        /// <exception cref="QueryException">If the educational content to
        /// change could not be found.</exception>
        public Task<bool> Hide(int id);

        /// <summary>
        /// Removes the educational content identified by the given id.
        /// </summary>
        /// <param name="id">Id of the educational
        ///  content to be removed.</param>
        /// <returns>The id of the educational
        /// content that was removed.</returns>
        /// <exception cref="QueryException">If the educational content to
        /// remove could not be found.</exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get all available educational contents.
        /// </summary>
        /// <returns>All available educational contents.</returns>
        public IQueryable<EducationalContent> GetAll();

        /// <summary>
        /// Method to get a educational content with the help of its id.
        /// </summary>
        /// <param name="id">The id of the educational content.</param>
        /// <returns>The educational content with the given id.</returns>
        /// <exception cref="QueryException">If the educational content
        /// with the given id could not be found.</exception>
        public Task<EducationalContent> GetById(int id);
    }
}
