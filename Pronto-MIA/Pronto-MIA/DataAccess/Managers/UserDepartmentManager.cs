#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.BusinessLogic.Security.Abstract;
    using Pronto_MIA.BusinessLogic.Security.Interfaces;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a user within the application.
    /// </summary>
    [SuppressMessage(
        "Menees.Analyzers",
        "MEN005",
        Justification = "Many comments.")]
    public class UserDepartmentManager : IUserDepartmentManager
    {
        private readonly IConfiguration cfg;
        private readonly ILogger logger;
        private readonly ILogger loggerDepartment;
        private readonly ILogger loggerUser;
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UserDepartmentManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context where userDepartments
        /// are persisted.</param>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this userDepartment manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        /// <param name="loggerDepartment">The logger to be used in order to
        /// document events regarding this department manager.</param>
        /// <param name="loggerUser">The logger to be used in order to
        /// document events regarding this user manager.</param>
        public UserDepartmentManager(
            ProntoMiaDbContext dbContext,
            IConfiguration cfg,
            ILogger<UserDepartmentManager> logger,
            ILogger<DepartmentManager> loggerDepartment,
            ILogger<UserManager> loggerUser)
        {
            this.dbContext = dbContext;
            this.cfg = cfg;
            this.logger = logger;
            this.loggerDepartment = loggerDepartment;
            this.loggerUser = loggerUser;
        }

        /// <inheritdoc/>
        public void SetDbContext(ProntoMiaDbContext context)
        {
            this.dbContext = context;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var user = await this.GetById(id);

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <inheritdoc/>
        public IQueryable<UserDepartment> GetAll()
        {
            return this.dbContext.UserDepartments;
        }

        /// <inheritdoc/>
        public async Task<UserDepartment> Create(
            int departmentId, int userId)
        {
            var checkDepartmentAndUserId =
                await this.GetByDepartmentAndUserId(departmentId, userId);

            if (checkDepartmentAndUserId != default)
            {
                throw DataAccess.Error.UserDepartmentNotFound
                    .AsQueryException();
            }

            var userDepartment = new UserDepartment(
                departmentId,
                userId);
            this.dbContext.UserDepartments.Add(userDepartment);
            await this.dbContext.SaveChangesAsync();
            return userDepartment;
        }

        /// <inheritdoc/>
        public async Task<UserDepartment> Update(
            int id, int? departmentId, int? userId)
        {
            var userDepartment = await this.GetById(id);
            if (departmentId == null && userId == null)
            {
                return userDepartment;
            }

            if (departmentId != null)
            {
                await this.UpdateDepartmentId(
                    departmentId.Value, userDepartment);
            }

            if (userId != null)
            {
                this.UpdateUserId(userId, userDepartment);
            }

            this.dbContext.UserDepartments.Update(userDepartment);
            await this.dbContext.SaveChangesAsync();
            return userDepartment;
        }

        /// <inheritdoc/>
        public async Task<UserDepartment> GetById(int id)
        {
            var userDepartment = await this.dbContext.UserDepartments
                .SingleOrDefaultAsync(u => u.Id == id);
            if (userDepartment != default)
            {
                return userDepartment;
            }

            this.logger.LogWarning(
                "Invalid userDepartment id {Id}", id);
            throw DataAccess.Error.UserDepartmentNotFound
                .AsQueryException();
        }

        /// <inheritdoc/>
        public async Task<UserDepartment?> GetByDepartmentId(int departmentId)
        {
            var userDepartment = await this.dbContext.UserDepartments
                .SingleOrDefaultAsync(d => d.Id == departmentId);
            if (userDepartment != default)
            {
                return userDepartment;
            }

            this.logger.LogWarning(
                "Invalid department id {Id}", departmentId);
            throw DataAccess.Error.DepartmentNotFound
                .AsQueryException();
        }

        /// <inheritdoc/>
        public async Task<UserDepartment?> GetByUserId(int userId)
        {
            var userDepartment = await this.dbContext.UserDepartments
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (userDepartment != default)
            {
                return userDepartment;
            }

            this.logger.LogWarning(
                "Invalid user id {Id}", userId);
            throw DataAccess.Error.UserNotFound
                .AsQueryException();
        }

        /// <inheritdoc/>
        public async Task<UserDepartment?> GetByDepartmentAndUserId(
            int departmentId, int userId)
        {
            var userDepartmentUser = await this.dbContext.UserDepartments
                .SingleOrDefaultAsync(u => u.Id == userId);
            var userDepartmentDepartment = await this.dbContext.UserDepartments
                .SingleOrDefaultAsync(d => d.Id == departmentId);
            if (userDepartmentUser != default &&
                userDepartmentDepartment != default &&
                userDepartmentUser.Equals(userDepartmentDepartment))
            {
                return userDepartmentUser;
            }

            this.logger.LogWarning(
                "Invalid user id {Id}", userId);
            throw DataAccess.Error.UserDepartmentNotFound
                .AsQueryException();
        }

        private async Task UpdateDepartmentId(
            int departmentId, UserDepartment userDepartment)
        {
            var departmentManager = new DepartmentManager(
                this.dbContext,
                (ILogger<DepartmentManager>)this.loggerDepartment);
            var departmentIdCheck =
                await departmentManager.GetById(departmentId);
            if (departmentIdCheck != default)
            {
                throw DataAccess.Error.DepartmentIdNotFound.AsQueryException();
            }

            userDepartment.DepartmentId = departmentId;
        }

        private async Task UpdateUserId(
            int userId, UserDepartment userDepartment)
        {
            var userManager = new UserManager(
                this.dbContext,
                this.cfg,
                (ILogger<UserManager>)this.loggerUser);
            var userIdCheck =
                await userManager.GetById(userId);
            if (userIdCheck != default)
            {
                throw DataAccess.Error.UserIdNotFound.AsQueryException();
            }

            userDepartment.UserId = userId;
        }
    }
}
