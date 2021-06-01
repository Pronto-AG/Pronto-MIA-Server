#nullable enable
namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using System.Collections.Generic;
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

    public class DepartmentAccessAuthorizationHandler
        : AuthorizationHandler<AccessObjectRequirement, IResolverContext>
    {
        private DbContextOptions<ProntoMiaDbContext> options;

        public DepartmentAccessAuthorizationHandler(
            IConfiguration cfg)
        {
            this.options = DatabaseService.GetOptions(cfg);
        }

        public void SetDbOptions(DbContextOptions<ProntoMiaDbContext> options)
        {
            this.options = options;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessObjectRequirement requirement,
            IResolverContext resource)
        {
            var userId = int.Parse(
                context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            AccessControlList? accessControlList;

            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                accessControlList = dbContext
                    .AccessControlLists.SingleOrDefault(
                        acl => acl.UserId == userId);
            }

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
                this.GetAccessObjectIdArgument(resource);
            if (accessObjectIdArgument != null)
            {
                if (accessObjectIdArgument.AccessObjectIdArgumentName
                    == "IGNORED")
                {
                    context.Succeed(requirement);
                    return true;
                }
                else
                {
                    var accessObjectId = this.GetAccessObjectId(
                        resource,
                        accessObjectIdArgument.AccessObjectIdArgumentName);
                    if (accessObjectIdArgument.IsDepartmentId)
                    {
                        return await this.CheckDepartmentWithUser(
                            context, requirement, userId, accessObjectId);
                    }

                    return await this.CheckWithUser(
                        context, requirement, userId, accessObjectId);
                }
            }

            return false;
        }

        private AccessObjectIdArgumentAttribute? GetAccessObjectIdArgument(
            IResolverContext resource)
        {
            var method = resource.Field.ResolverMember;
            if (method == null)
            {
                return null;
            }

            var accessObjectIdArgument =
                (AccessObjectIdArgumentAttribute?)method.GetCustomAttribute(
                    typeof(AccessObjectIdArgumentAttribute), false);

            return accessObjectIdArgument;
        }

        private int? GetAccessObjectId(
            IResolverContext resource,
            string attributeName)
        {
            var idArgumentNode = resource.Selection.SyntaxNode.Arguments
                .SingleOrDefault(
                    a => a.Name.Value == attributeName);

            if (idArgumentNode == null)
            {
                return null;
            }

            return int.Parse((string)idArgumentNode.Value.Value!);
        }

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

            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                var accessObject = dbContext.Find(
                    requirement.ObjectType, accessObjectId);
                var user = dbContext.Users.Find(userId);

                if (user == null || accessObject == null)
                {
                    return false;
                }

                if (user.DepartmentId ==
                    ((IDepartmentComparable)accessObject).DepartmentId)
                {
                    context.Succeed(requirement);
                    return true;
                }
            }

            return false;
        }

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

            await using (var dbContext = new ProntoMiaDbContext(this.options))
            {
                var user = dbContext.Users.Find(userId);

                if (user == null)
                {
                    return false;
                }

                if (user.DepartmentId == departmentId)
                {
                    context.Succeed(requirement);
                    return true;
                }
            }

            return false;
        }
    }
}
