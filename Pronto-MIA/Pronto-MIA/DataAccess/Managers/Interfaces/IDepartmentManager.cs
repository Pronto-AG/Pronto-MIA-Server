#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of a department manager
    /// service.
    /// </summary>
    public interface IDepartmentManager
    {
        /// <summary>
        /// Method to overwrite the <see cref="ProntoMiaDbContext"/>
        /// used by the manager. This can be used if transactions
        /// over multiple managers have to be implemented.
        /// </summary>
        /// <param name="dbContext">The db context to be used by the
        /// manager.</param>
        public void SetDbContext(ProntoMiaDbContext dbContext);

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
        /// Method to get Department by id.
        /// </summary>
        /// <returns>Department by id.</returns>
        /// /// <param name="id">Id of the department to be found.</param>
        /// <returns>The id of the department that was found.</returns>
        public Task<Department> GetById(int id);

        /// <summary>
        /// Adds a given user to a department. The current department of the
        /// user will be overwritten.
        /// </summary>
        /// <param name="departmentId"> The id of the <see cref="Department"/>
        /// to which the user will be added.</param>
        /// <param name="user">The user to add to the <see cref="Department"/>.
        /// </param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="QueryException">If the department to add the
        /// user to could not be found.</exception>
        public Task AddUser(int departmentId, User user);

        /// <summary>
        /// Adds a given deployment plan to a department. The current department
        /// of the deployment plan will be overwritten.
        /// </summary>
        /// <param name="departmentId"> The id of the <see cref="Department"/>
        /// to which the deployment plan will be added.</param>
        /// <param name="deploymentPlan">The deployment plan to add to
        /// the <see cref="Department"/>.</param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="QueryException">If the department to add the
        /// deployment plan to could not be found.</exception>
        public Task AddDeploymentPlan(
            int departmentId, DeploymentPlan deploymentPlan);
    }
}
