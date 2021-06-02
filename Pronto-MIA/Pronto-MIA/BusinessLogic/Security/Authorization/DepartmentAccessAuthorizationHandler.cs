#nullable enable
namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Domain.EntityExtensions;
    using Pronto_MIA.Services;

    /// <summary>
    /// Authorization handler for department based authorization.
    /// Will first check if an unrestricted access is present for the
    /// user and if not will check if department based authorization
    /// with the help of the <see cref="AccessObjectIdArgumentAttribute"/>
    /// is possible.
    /// </summary>
    [SuppressMessage(
    "Menees.Analyzers",
    "MEN005",
    Justification = "Mostly comments.")]
    public class DepartmentAccessAuthorizationHandler
        : AuthorizationHandler<AccessObjectRequirement, IResolverContext>
    {
        private DbContextOptions<ProntoMiaDbContext> options;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DepartmentAccessAuthorizationHandler"/> class.
        /// </summary>
        /// <param name="cfg">The application configuration. Used for
        /// the creation of new database contexts.</param>
        public DepartmentAccessAuthorizationHandler(
            IConfiguration cfg)
        {
            this.options = DatabaseService.GetOptions(cfg);
        }

        /// <summary>
        /// Method to overwrite the database options used by this authorization
        /// handler. Primarily used for testing.
        /// </summary>
        /// <param name="options">The database context options to be used
        /// in order to create new db contexts.</param>
        public void SetDbOptions(DbContextOptions<ProntoMiaDbContext> options)
        {
            this.options = options;
        }

        /// <inheritdoc/>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessObjectRequirement requirement,
            IResolverContext resource)
        {
            var userId = int.Parse(
                context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            AccessControlList? accessControlList =
                await this.GetAccessControlList(userId);

            if (accessControlList == default)
            {
                return;
            }

            foreach (var control in requirement.Controls)
            {
                if (accessControlList.HasControl(control.Key))
                {
                    var result = await this.HandleControl(
                        context, requirement, resource, control, userId);
                    if (result)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Method that extracts the
        /// <see cref="AccessObjectIdArgumentAttribute"/> from the given
        /// resource.
        /// </summary>
        /// <param name="resource">The resource to be used.</param>
        /// <returns>The attribute or `null` if none was found.</returns>
        private static AccessObjectIdArgumentAttribute?
            GetAccessObjectIdArgument(IResolverContext resource)
        {
            var method = resource.Field.ResolverMember;
            if (method == null)
            {
                return null;
            }

            var attributes = method.GetCustomAttributes(
                typeof(AccessObjectIdArgumentAttribute),
                false);

            if (attributes.Length == 0 || attributes.Length > 1)
            {
                return null;
            }

            var accessObjectIdArgument = 
                (AccessObjectIdArgumentAttribute?)attributes[0];

            return accessObjectIdArgument;
        }

        /// <summary>
        /// Method that extracts the access object id from a given
        /// resource with the help of its argument name.
        /// </summary>
        /// <param name="resource">The resource to be used.</param>
        /// <param name="argumentName">The name of the argument that
        /// contains the access object id.</param>
        /// <returns>The id of the access object or null if the id
        /// could not be extracted.</returns>
        private static int? GetAccessObjectId(
            IResolverContext resource,
            string argumentName)
        {
            var idArgumentNode = resource.Selection.SyntaxNode.Arguments
                .SingleOrDefault(
                    a => a.Name.Value == argumentName);

            if (idArgumentNode == null)
            {
                return null;
            }

            return int.Parse((string)idArgumentNode.Value.Value!);
        }

        /// <summary>
        /// Method to handle a single control contained inside
        /// a <see cref="AccessObjectRequirement"/>.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <param name="resource">The resource to evaluate.</param>
        /// <param name="control">The control to check.</param>
        /// <param name="userId">The id of the user making the
        /// authorization request.</param>
        /// <returns>True if the authorization was successful.</returns>
        private async Task<bool> HandleControl(
            AuthorizationHandlerContext context,
            AccessObjectRequirement requirement,
            IResolverContext resource,
            KeyValuePair<AccessControl, AccessMode> control,
            int userId)
        {
            if (control.Value == AccessMode.Unrestricted)
            {
                context.Succeed(requirement);
                return true;
            }

            var accessObjectIdArgument =
                GetAccessObjectIdArgument(resource);
            if (accessObjectIdArgument != null)
            {
                return await this.HandleObjectIdArgumentAttribute(
                    context,
                    requirement,
                    resource,
                    accessObjectIdArgument,
                    userId);
            }

            return false;
        }

        /// <summary>
        /// Method to handle a <see cref="AccessObjectIdArgumentAttribute"/>.
        /// The method will analyse the attribute and determine if a department
        /// based authorization can succeed or not.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <param name="resource">The resource to evaluate.</param>
        /// <param name="argumentAttribute">The attribute to check.</param>
        /// <param name="userId">The id of the user making the
        /// authorization request.</param>
        /// <returns>True if authorization succeeded.</returns>
        private async Task<bool> HandleObjectIdArgumentAttribute(
            AuthorizationHandlerContext context,
            AccessObjectRequirement requirement,
            IResolverContext resource,
            AccessObjectIdArgumentAttribute argumentAttribute,
            int userId)
        {
            if (argumentAttribute.AccessObjectIdArgumentName
                == "IGNORED")
            {
                context.Succeed(requirement);
                return true;
            }
            else
            {
                var accessObjectId = GetAccessObjectId(
                    resource,
                    argumentAttribute.AccessObjectIdArgumentName);
                if (argumentAttribute.IsDepartmentId)
                {
                    return await this.CheckDepartmentWithUser(
                        context, requirement, userId, accessObjectId);
                }

                return await this.CheckWithUser(
                    context, requirement, userId, accessObjectId);
            }
        }

        /// <summary>
        /// Method that authorizes the request if the department of the user
        /// does correspond to the department defined within the access object.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <param name="userId">The id of the user making the
        /// authorization request.</param>
        /// <param name="accessObjectId">The id of the access object to be
        /// taken for comparison.</param>
        /// <returns>True if the authorization was successful.</returns>
        private async Task<bool> CheckWithUser(
            AuthorizationHandlerContext context,
            AccessObjectRequirement requirement,
            int userId,
            int? accessObjectId)
        {
            if (accessObjectId == null)
            {
                return false;
            }

            var userDepartmentId = await this.GetUserDepartmentId(userId);
            var accessObjectDepartmentId = await this
                .GetAccessObjectDepartmentId(
                    requirement.ObjectType, accessObjectId.Value);

            if (userDepartmentId == null
                || accessObjectDepartmentId == null
                || userDepartmentId != accessObjectDepartmentId)
            {
                return false;
            }

            context.Succeed(requirement);
            return true;
        }

        /// <summary>
        /// Method that authorizes the request if the department of the user
        /// does correspond to the departmentId provided.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <param name="userId">The id of the user making the
        /// authorization request.</param>
        /// <param name="departmentId">The id of the department to be
        /// taken for comparison.</param>
        /// <returns>True if the authorization was successful.</returns>
        private async Task<bool> CheckDepartmentWithUser(
            AuthorizationHandlerContext context,
            AccessObjectRequirement requirement,
            int userId,
            int? departmentId)
        {
            if (departmentId == null)
            {
                return false;
            }

            var userDepartmentId = await this.GetUserDepartmentId(userId);
            if (userDepartmentId == null || userDepartmentId != departmentId)
            {
                return false;
            }

            context.Succeed(requirement);
            return true;
        }

        /// <summary>
        /// Method to get the <see cref="AccessControlList"/> of a
        /// user.
        /// </summary>
        /// <param name="userId">The id of the user from which the
        /// list should be retrieved.</param>
        /// <returns>The acl of the user or null if no acl was found.</returns>
        private async Task<AccessControlList?> GetAccessControlList(int userId)
        {
            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                var accessControlList = dbContext
                    .AccessControlLists.SingleOrDefault(
                        acl => acl.UserId == userId);

                return accessControlList;
            }
        }

        /// <summary>
        /// Method to extract the id of the department
        /// associated with the given user.
        /// </summary>
        /// <param name="userId">Id of the user the
        /// department should be extracted from.</param>
        /// <returns>The department id or null if no
        /// department could be found.</returns>
        private async Task<int?> GetUserDepartmentId(int userId)
        {
            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                var user = dbContext.Users.Find(userId);

                if (user == null)
                {
                    return null;
                }

                return user.DepartmentId;
            }
        }

        /// <summary>
        /// Method to extract the department id from
        /// the given access object.
        /// </summary>
        /// <param name="objectType">The type of the
        /// access object. Must implement
        /// <see cref="IDepartmentComparable"/>.</param>
        /// <param name="accessObjectId">The id of the access object.</param>
        /// <returns>The department id of the access object or null if no
        /// department could be found.</returns>
        private async Task<int?> GetAccessObjectDepartmentId(
            Type objectType,
            int accessObjectId)
        {
            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                var accessObject = dbContext.Find(objectType, accessObjectId);

                if (accessObject == null)
                {
                    return null;
                }

                return ((IDepartmentComparable)accessObject).DepartmentId;
            }
        }
    }
}
