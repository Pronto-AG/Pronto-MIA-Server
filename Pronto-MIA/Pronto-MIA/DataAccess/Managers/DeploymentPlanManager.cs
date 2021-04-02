#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Entities;
    using HotChocolate.Types;
    using LanguageExt;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class DeploymentPlanManager
    {
        public static string FileDirectory { get; } = "deployment_plans";

        private readonly ProntoMIADbContext dbContext;
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
        /// <param name="cfg">The configuration of this application.</param>
        public DeploymentPlanManager(
            ILogger<DeploymentPlanManager> logger,
            ProntoMIADbContext dbContext,
            FileManager fileManager,
            IConfiguration cfg)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.fileManager = fileManager;
            this.cfg = cfg;
        }

        public async Task<Either<DataAccess.Error, IQueryable<DeploymentPlan>>>
            Create(
                IFile file,
                DateTime availableFrom,
                DateTime availableUntil)
        {
            var deploymentPlan = new DeploymentPlan(
                availableFrom, availableUntil, Guid.NewGuid());
            try
            {
                await this.fileManager.CreateFileWithUUID(
                    FileDirectory, deploymentPlan.fileUUID, file);
                await this.dbContext.DeploymentPlans.AddAsync(deploymentPlan);
                await this.dbContext.SaveChangesAsync();
                return Prelude.Right(this.dbContext.DeploymentPlans
                    .Where(dP => dP.Id == deploymentPlan.Id));
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    "DeploymentPlan could no be saved");
                return Prelude.Left(DataAccess.Error.FileOperationError);
            }
        }

        public IQueryable<DeploymentPlan?> GetAll()
        {
            return this.dbContext.DeploymentPlans;
        }
    }
}
