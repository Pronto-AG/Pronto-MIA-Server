using System.Linq;
using HotChocolate.Language;
using Microsoft.EntityFrameworkCore;

namespace Tests.TestBusinessLogic.TestSecurity.TestAuthorization
{
#nullable enable
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using NSubstitute;
    using NSubstitute.ReturnsExtensions;
    using Pronto_MIA.BusinessLogic.Security.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestDepartmentAccessAuthorizationHandler
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly DepartmentAccessAuthorizationHandler
            _authorizationHandler;

        public TestDepartmentAccessAuthorizationHandler()
        {
            this.dbContext = TestHelpers.InMemoryDbContext("AuthContext");
            TestDataProvider.InsertTestData(this.dbContext);
            this._authorizationHandler =
                new DepartmentAccessAuthorizationHandler(
                    TestHelpers.TestConfiguration);
            this._authorizationHandler.SetDbOptions(
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
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewDepartments,
                        AccessMode.Unrestricted
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

            Assert.True(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUnrestrictedError()
        {
            var user = await this.CreateUser();
            var resource = Substitute.For<IResolverContext>();
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewDepartments,
                        AccessMode.Unrestricted
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

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
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

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
            var method = this.CreateArgumentAttributeMethodInfo();
            resource.Field.ResolverMember.Returns(method);
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

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
            var resource = Substitute.For<IResolverContext>();
            var method = this.CreateArgumentAttributeMethodInfo("IGNORED");
            resource.Field.ResolverMember.Returns(method);
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

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
            var resource = Substitute.For<IResolverContext>();
            var method = this.CreateArgumentAttributeMethodInfo("id");
            resource.Field.ResolverMember.Returns(method);
            resource.Selection.SyntaxNode.Returns(
                new FieldNode(null, new NameNode(null, "Name"), null, new List<DirectiveNode>(),
                    new List<ArgumentNode>(), null));
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

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
            var resource = Substitute.For<IResolverContext>();
            var method = this.CreateArgumentAttributeMethodInfo("id");
            resource.Field.ResolverMember.Returns(method);
            resource.Selection.SyntaxNode.Returns(
                new FieldNode(null, new NameNode(null, "Name"), null, new List<DirectiveNode>(),
                    new List<ArgumentNode>()
                    {
                        new ArgumentNode("id", (await this.dbContext.Departments.SingleAsync(d => d.Name == "Finance")).Id.ToString()),
                    }, null));
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

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
            var resource = Substitute.For<IResolverContext>();
            var method = this.CreateArgumentAttributeMethodInfo("id");
            resource.Field.ResolverMember.Returns(method);
            resource.Selection.SyntaxNode.Returns(
                new FieldNode(null, new NameNode(null, "Name"), null, new List<DirectiveNode>(),
                    new List<ArgumentNode>()
                    {
                        new ArgumentNode("id", (await this.dbContext.Departments.SingleAsync(d => d.Name == "Administration")).Id.ToString()),
                    }, null));
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>()
                {
                    {
                        AccessControl.CanViewOwnDepartment,
                        AccessMode.Department
                    },
                });
            var context = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement>() { requirement },
                this.GetClaimsPrincipal(user),
                resource);

            await this._authorizationHandler.HandleAsync(context);

            Assert.True(context.HasSucceeded);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
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

        private ClaimsPrincipal GetClaimsPrincipal(User user)
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

        private MethodInfo CreateArgumentAttributeMethodInfo(
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
    }
}