#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Pronto_MIA.Domain.Entities;

    public interface IDepartmentManager
    {
        /// <summary>
        /// Method which creates a new department object.
        /// </summary>
        /// <param name="name">The name for the department.</param>
        /// <returns>The department.</returns>
        public Task<Department>
            Create(string name);

        /// <summary>
        /// Updates the department identified by the given id.
        /// </summary>
        /// <param name="id">The id of the department to be updated.
        /// </param>
        /// <param name="name">The new name of the department.
        /// </param>
        /// <returns>The updated department.</returns>
        /// <exception cref="QueryException">If the department to be
        /// updated could not be found.</exception>
        public Task<Department>
            Update(
                int id,
                string? name);

        /// <summary>
        /// Removes the department identified by the given id.
        /// </summary>
        /// <param name="id">Id of the department to be removed.</param>
        /// <returns>The id of the deployment plan that was removed.</returns>
        /// <exception cref="QueryException">If the department to remove
        /// could not be found or users are still linked to the specified
        /// department.</exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get all available departments.
        /// </summary>
        /// <returns>All available departments.</returns>
        public IQueryable<Department> GetAll();

        /// <summary>
        /// Adds a given user to a department. The current department of the
        /// user will be overwritten.
        /// </summary>
        /// <param name="departmentId"> The id of the <see cref="Department"/>
        /// to which the user will be added.</param>
        /// <param name="user">The id of the user to add to
        /// the <see cref="Department"/>.</param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="QueryException">If the department to add the
        /// user to could not be found.</exception>
        public Task AddUser(int departmentId, User user);
    }
}
