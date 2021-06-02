namespace Tests.TestBusinessLogic.TestSecurity.TestAuthorization
{
#nullable enable
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate.Language;
    using HotChocolate.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.Security.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    [SuppressMessage(
        "Menees.Analyzers",
        "MEN005",
        Justification = "Tests of same class.")]
    public class TestDepartmentAccessAuthorizationHandler
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly DepartmentAccessAuthorizationHandler
            authorizationHandler;

        public TestDepartmentAccessAuthorizationHandler()
        {
            this.dbContext = TestHelpers.InMemoryDbContext("AuthContext");
            TestDataProvider.InsertTestData(this.dbContext);
            this.authorizationHandler =
                new DepartmentAccessAuthorizationHandler(
                    TestHelpers.TestConfiguration);
            this.authorizationHandler.SetDbOptions(
                TestHelpers.GetInMemoryDbOptions("AuthContext"));
        }

        [Fact]
        public async Task TestUnrestricted()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewDepartments = true,
            });
            var resource = Substitute.For<IResolverContext>();
            var requirement = CreateRequirement(
                AccessControl.CanViewDepartments, AccessMode.Unrestricted);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.True(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUnrestrictedError()
        {
            var user = await this.CreateUser();
            var resource = Substitute.For<IResolverContext>();
            var requirement = CreateRequirement(
                AccessControl.CanViewDepartments, AccessMode.Unrestricted);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.False(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRestrictedNoMethod()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewOwnDepartment = true,
            });
            var resource = Substitute.For<IResolverContext>();
            var requirement = CreateRequirement(
                AccessControl.CanViewOwnDepartment, AccessMode.Department);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.False(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRestrictedNoAttribute()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewOwnDepartment = true,
            });
            var resource = Substitute.For<IResolverContext>();
            var method = CreateArgumentAttributeMethodInfo();
            resource.Field.ResolverMember.Returns(method);
            var requirement = CreateRequirement(
                AccessControl.CanViewOwnDepartment, AccessMode.Department);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.False(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRestrictedAttributeIgnored()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewOwnDepartment = true,
            });
            var resource = CreateResource(
                "IGNORED", false, null);
            var requirement = CreateRequirement(
                AccessControl.CanViewOwnDepartment, AccessMode.Department);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.True(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRestrictedInvalidArgument()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewOwnDepartment = true,
            });
            var resource = CreateResource(
                null, false, null);
            var requirement = CreateRequirement(
                AccessControl.CanViewOwnDepartment, AccessMode.Department);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.False(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRestrictedWrongDepartmentId()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewOwnDepartment = true,
            });
            var departmentId = (await this.dbContext.Departments.SingleAsync(
                d => d.Name == "Finance")).Id.ToString();
            var resource = CreateResource(
                "id", false, departmentId);
            var requirement = CreateRequirement(
                AccessControl.CanViewOwnDepartment, AccessMode.Department);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.False(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRestricted()
        {
            var user = await this.CreateUser(new AccessControlList(-1)
            {
                CanViewOwnDepartment = true,
            });
            var departmentId = (await this.dbContext.Departments.SingleAsync(
                d => d.Name == "Administration")).Id.ToString();
            var resource = CreateResource(
                "id", false, departmentId);
            var requirement = CreateRequirement(
                AccessControl.CanViewOwnDepartment, AccessMode.Department);
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                CreateClaimsPrincipal(user),
                resource);

            await this.authorizationHandler.HandleAsync(context);

            Assert.True(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            var identity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(identity);
        }

        private static MethodInfo CreateArgumentAttributeMethodInfo(
            string? attributeValue = null, bool isDepartment = false)
        {
            var method = Substitute.For<MethodInfo>();
            if (attributeValue == null)
            {
                method.GetCustomAttributes(
                        typeof(AccessObjectIdArgumentAttribute), false)
                    .Returns(new object[] { });
            }
            else
            {
                method.GetCustomAttributes(
                        typeof(AccessObjectIdArgumentAttribute), false)
                    .Returns(new object[]
                    {
                        new AccessObjectIdArgumentAttribute(
                            attributeValue, isDepartment),
                    });
            }

            return method;
        }

        private static IResolverContext CreateResource(
            string? argumentName, bool isDepartmentId, string? argumentValue)
        {
            var resource = Substitute.For<IResolverContext>();
            var method = CreateArgumentAttributeMethodInfo(
                argumentName, isDepartmentId);
            resource.Field.ResolverMember.Returns(method);

            var argumentList = new List<ArgumentNode>();
            if (argumentName != null && argumentValue != null)
            {
                argumentList.Add(
                    new ArgumentNode(argumentName, argumentValue));
            }

            resource.Selection.SyntaxNode.Returns(
                new FieldNode(
                    null,
                    new NameNode(null, "Name"),
                    null,
                    new List<DirectiveNode>(),
                    argumentList,
                    null));

            return resource;
        }

        private static AccessObjectRequirement CreateRequirement(
            AccessControl control, AccessMode mode)
        {
            return new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        control,
                        mode
                    },
                });
        }

        private async Task<User> CreateUser(AccessControlList? acl = null)
        {
            var user = new User("AuthTest", new byte[5], "{}", "{}");
            {
                user.DepartmentId = (await this.dbContext.Departments
                    .SingleAsync(d => d.Name == "Administration")).Id;
            }

            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();

            if (acl == null)
            {
                acl = new AccessControlList(user.Id);
            }
            else
            {
                acl.UserId = user.Id;
            }

            this.dbContext.AccessControlLists.Add(acl);
            await this.dbContext.SaveChangesAsync();
            return user;
        }
    }
}