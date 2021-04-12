#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Types;
    using LanguageExt;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a deployment plan within the
    /// application.
    /// </summary>
    public class DeploymentPlanManager
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly IConfiguration cfg;
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
        /// <param name="cfg">The configuration of this application.</param>
        public DeploymentPlanManager(
            ILogger<DeploymentPlanManager> logger,
            ProntoMiaDbContext dbContext,
            FileManager fileManager,
            IConfiguration cfg)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.fileManager = fileManager;
            this.cfg = cfg;
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
        /// <returns>`Either` object which contains either an error
        /// if one occured or a queryable object of the newly generated
        /// deployment plan.
        /// </returns>
        public async Task<Either<DataAccess.Error, IQueryable<DeploymentPlan>>>
            Create(
                IFile file,
                DateTime availableFrom,
                DateTime availableUntil)
        {
            var uuid = Guid.NewGuid();
            try
            {
                await this.fileManager.Create(
                    FileDirectory, uuid.ToString(), file);
            }
            catch (Exception error)
            {
                this.logger.LogWarning(
                    "DeploymentPlan could no be saved: {Error}", error);
                return Prelude.Left(DataAccess.Error.FileOperationError);
            }

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
            return Prelude.Right(this.GetQueryableById(deploymentPlan.Id));
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
        /// <returns>`Either` object which contains either an error
        /// if one occured or a queryable object of the updated deployment plan.
        /// </returns>
        public async Task<Either<DataAccess.Error, IQueryable<DeploymentPlan>>>
            Update(
                int id,
                IFile? file,
                DateTime? availableFrom,
                DateTime? availableUntil)
        {
            var deploymentPlan = await this.GetById(id);
            if (deploymentPlan == default)
            {
                return Prelude.Left(DataAccess.Error.DeploymentPlanNotFound);
            }

            if (file != null)
            {
                var uuid = Guid.NewGuid();
                try
                {
                    await this.fileManager.Create(
                        FileDirectory, uuid.ToString(), file);
                    this.fileManager.Remove(
                        FileDirectory,
                        deploymentPlan.FileUuid.ToString(),
                        deploymentPlan.FileExtension);

                    deploymentPlan.FileUuid = uuid;
                    deploymentPlan.FileExtension =
                        FileManager.GetFileExtension(file);
                }
                catch (Exception error)
                {
                    this.logger.LogWarning(
                        "DeploymentPlan could no be updated: {Error}", error);
                    return Prelude.Left(DataAccess.Error.FileOperationError);
                }
            }

            if (availableFrom.HasValue)
            {
                deploymentPlan.AvailableFrom = (DateTime)availableFrom;
            }

            if (availableUntil.HasValue)
            {
                deploymentPlan.AvailableUntil = (DateTime)availableUntil;
            }

            await this.dbContext.SaveChangesAsync();

            return Prelude.Right(this.GetQueryableById(id));
        }

        /// <summary>
        /// Removes the deployment plan with the given id.
        /// </summary>
        /// <param name="id">Id of the deployment plan to be removed.</param>
        /// <returns>The id of the deployment plan that was removed.</returns>
        public async Task<Either<DataAccess.Error, int>>
            Remove(int id)
        {
            var deploymentPlan = await this.GetById(id);
            if (deploymentPlan == default)
            {
                return Prelude.Left(DataAccess.Error.DeploymentPlanNotFound);
            }

            try
            {
                this.fileManager.Remove(
                    FileDirectory,
                    deploymentPlan.FileUuid.ToString(),
                    deploymentPlan.FileExtension);
            }
            catch (Exception error)
            {
                this.logger.LogWarning(
                    "DeploymentPlan could no be Removed: {Error}", error);
                return Prelude.Left(DataAccess.Error.FileOperationError);
            }

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();

            return Prelude.Right(id);
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
    }
}
