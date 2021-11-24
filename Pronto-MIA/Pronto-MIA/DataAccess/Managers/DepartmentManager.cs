#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a department within the
    /// application.
    /// </summary>
    public class DepartmentManager : IDepartmentManager
    {
        private readonly ILogger logger;
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DepartmentManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        public DepartmentManager(
            ProntoMiaDbContext dbContext,
            ILogger<DepartmentManager> logger)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        public void SetDbContext(ProntoMiaDbContext context)
        {
            this.dbContext = context;
        }

        /// <inheritdoc/>
        public async Task<Department>
            Create(
                string name)
        {
            await this.CheckName(name);

            var department = new Department(name);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();
            return department;
        }

        /// <inheritdoc/>
        public async Task<Department>
            Update(
                int id,
                string? name)
        {
            var department = await this.GetById(id);

            if (name != null && name != department.Name)
            {
                await this.CheckName(name);
                department.Name = name;
                this.dbContext.Update(department);
                await this.dbContext.SaveChangesAsync();
            }

            return department;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var department = await this.GetById(id);

            // if (department.Users is { Count: > 0 })
            if (department.UserDepartments is { Count: > 0 })
                {
                throw Error.DepartmentInUse.AsQueryException();
            }

            this.dbContext.Remove(department);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <inheritdoc/>
        public IQueryable<Department> GetAll()
        {
            return this.dbContext.Departments;
        }

        /// <inheritdoc/>
        public async Task AddUser(int departmentId, User user)
        {
            // Check if id exists
            await this.GetById(departmentId);

            // user.DepartmentId = departmentId;
            user.Department.Id = departmentId;
            this.dbContext.Update(user);
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddDeploymentPlan(
            int departmentId, DeploymentPlan deploymentPlan)
        {
            // Check if id exists
            await this.GetById(departmentId);

            deploymentPlan.DepartmentId = departmentId;
            this.dbContext.Update(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<Department> GetById(int id)
        {
            var department = await this.dbContext.Departments
                .SingleOrDefaultAsync(dP => dP.Id == id);
            if (department != default)
            {
                return department;
            }

            this.logger.LogWarning(
                "Invalid department id {Id}", id);
            throw Error.DepartmentNotFound
                .AsQueryException();
        }

        private async Task CheckName(string name)
        {
            var departmentOld = await this.dbContext.Departments
                .SingleOrDefaultAsync(d => d.Name == name);

            if (departmentOld != default)
            {
                this.logger.LogWarning(
                    "The department with name \"{Name}\" already exists", name);
                throw Error.DepartmentAlreadyExists.AsQueryException();
            }
        }
    }
}
