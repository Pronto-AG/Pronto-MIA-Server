#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a deployment plan within the
    /// application.
    /// </summary>
    public class DeploymentPlanManager
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly ILogger logger;
        private readonly FileManager fileManager;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DeploymentPlanManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        /// <param name="fileManager">The file manager which handles the
        /// persistence of the file associated with the deployment plan object.
        /// </param>
        public DeploymentPlanManager(
            ILogger<DeploymentPlanManager> logger,
            ProntoMiaDbContext dbContext,
            FileManager fileManager)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.fileManager = fileManager;
        }

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
        public async Task<IQueryable<DeploymentPlan>>
            Create(
                IFile file,
                DateTime availableFrom,
                DateTime availableUntil)
        {
            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                    FileDirectory, uuid.ToString(), file);

            var deploymentPlan = new DeploymentPlan(
                availableFrom,
                availableUntil,
                uuid,
                FileManager.GetFileExtension(file));
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "Deployment plan with id {Id} has been created",
                deploymentPlan.Id);
            return this.GetQueryableById(deploymentPlan.Id);
        }

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
        public async Task<IQueryable<DeploymentPlan>>
            Update(
                int id,
                IFile? file,
                DateTime? availableFrom,
                DateTime? availableUntil)
        {
            var deploymentPlan = await this.GetById(id);
            if (deploymentPlan == default)
            {
                this.logger.LogWarning(
                    "Invalid deployment plan id {Id}", id);
                throw DataAccess.Error.DeploymentPlanNotFound
                    .AsQueryException();
            }

            deploymentPlan = await this.UpdateFile(deploymentPlan, file);
            deploymentPlan = this.UpdateTimes(
                deploymentPlan, availableFrom, availableUntil);

            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "Deployment plan with id {Id} has been updated",
                deploymentPlan.Id);
            return this.GetQueryableById(id);
        }

        /// <summary>
        /// Removes the deployment plan with the given id.
        /// </summary>
        /// <param name="id">Id of the deployment plan to be removed.</param>
        /// <returns>The id of the deployment plan that was removed.</returns>
        /// <exception cref="QueryException">If the deployment plan to remove
        /// could not be found.</exception>
        public async Task<int>
            Remove(int id)
        {
            var deploymentPlan = await this.GetById(id);
            if (deploymentPlan == default)
            {
                this.logger.LogWarning(
                    "Invalid deployment plan id {Id}", id);
                throw DataAccess.Error.DeploymentPlanNotFound
                    .AsQueryException();
            }

            this.fileManager.Remove(
                FileDirectory,
                deploymentPlan.FileUuid.ToString(),
                deploymentPlan.FileExtension);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <summary>
        /// Method to get all available deployment plans.
        /// </summary>
        /// <returns>All available deployment plans.</returns>
        public IQueryable<DeploymentPlan?> GetAll()
        {
            return this.dbContext.DeploymentPlans;
        }

        /// <summary>
        /// Method to get a deployment plan queryable with the help of the
        /// deployment plan id.
        /// </summary>
        /// <param name="id">The id of the deployment plan.</param>
        /// <returns>IQueryable witch might contain the Deployment plan
        /// with said id.</returns>
        private IQueryable<DeploymentPlan> GetQueryableById(int id)
        {
            return this.dbContext.DeploymentPlans
                .Where(dP => dP.Id == id);
        }

        /// <summary>
        /// Method to get a deployment plan with the help of its id.
        /// </summary>
        /// <param name="id">The id of the deployment plan.</param>
        /// <returns>The deployment plan with the given id.</returns>
        private async Task<DeploymentPlan?> GetById(int id)
        {
            return await this.dbContext.DeploymentPlans
                .SingleOrDefaultAsync(dP => dP.Id == id);
        }

        /// <summary>
        /// Updates the file of a given deployment plan.
        /// </summary>
        /// <param name="deploymentPlan">The deployment plan to be updated.
        /// </param>
        /// <param name="file">The new file to be associated with the deployment
        /// plan.</param>
        private async Task<DeploymentPlan> UpdateFile(
            DeploymentPlan deploymentPlan, IFile? file)
        {
            if (file == null)
            {
                return deploymentPlan;
            }

            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                FileDirectory, uuid.ToString(), file);

            deploymentPlan.FileUuid = uuid;
            deploymentPlan.FileExtension =
                FileManager.GetFileExtension(file);

            this.fileManager.Remove(
                FileDirectory,
                deploymentPlan.FileUuid.ToString(),
                deploymentPlan.FileExtension);

            return deploymentPlan;
        }

        /// <summary>
        /// Updates the times within the given deployment plan to the new
        /// values.
        /// </summary>
        /// <param name="deploymentPlan">The deployment plan to be updated.
        /// </param>
        /// <param name="availableFrom">The new available from datetime.</param>
        /// <param name="availableUntil">The new available until datetime.
        /// </param>
        private DeploymentPlan UpdateTimes(
            DeploymentPlan deploymentPlan,
            DateTime? availableFrom,
            DateTime? availableUntil)
        {
            if (availableFrom.HasValue)
            {
                deploymentPlan.AvailableFrom = (DateTime)availableFrom;
            }

            if (availableUntil.HasValue)
            {
                deploymentPlan.AvailableUntil = (DateTime)availableUntil;
            }

            return deploymentPlan;
        }
    }
}
