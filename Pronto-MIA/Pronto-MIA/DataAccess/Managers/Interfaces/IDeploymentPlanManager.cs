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
    /// Interface declaring the operations needed of a deployment plan manager
    /// service.
    /// </summary>
    public interface IDeploymentPlanManager
    {
        /// <summary>
        /// Gets the subdirectory into which files associated with deployment
        /// plans are persisted.
        /// </summary>
        public static string FileDirectory { get; } = "deployment_plans";

        /// <summary>
        /// Method which creates a new deployment plan object.
        /// </summary>
        /// <param name="file">The file to be associated with the new deployment
        /// plan.</param>
        /// <param name="availableFrom">Moment from which the deployment plan
        /// should be treated as active.</param>
        /// <param name="availableUntil">Moment until which the deployment plan
        /// is treated as active.</param>
        /// <returns>Queryable object of the deployment plan.</returns>
        public Task<IQueryable<DeploymentPlan>>
            Create(
                IFile file,
                DateTime availableFrom,
                DateTime availableUntil);

        /// <summary>
        /// Updates the deployment plan with the given id.
        /// </summary>
        /// <param name="id">The id of the deployment plan to be updated.
        /// </param>
        /// <param name="file">The new file associated with the deployment plan.
        /// </param>
        /// <param name="availableFrom">New moment from which the deployment
        /// plan should be treated as active.</param>
        /// <param name="availableUntil">New moment until which the deployment
        /// plan is treated as active.</param>
        /// <returns>Queryable object of the deployment plan.</returns>
        /// <exception cref="QueryException">If the deployment plan to be
        /// updated could not be found.</exception>
        public Task<IQueryable<DeploymentPlan>>
            Update(
                int id,
                IFile? file,
                DateTime? availableFrom,
                DateTime? availableUntil);

        /// <summary>
        /// Removes the deployment plan with the given id.
        /// </summary>
        /// <param name="id">Id of the deployment plan to be removed.</param>
        /// <returns>The id of the deployment plan that was removed.</returns>
        /// <exception cref="QueryException">If the deployment plan to remove
        /// could not be found.</exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get all available deployment plans.
        /// </summary>
        /// <returns>All available deployment plans.</returns>
        public IQueryable<DeploymentPlan?> GetAll();
    }
}
