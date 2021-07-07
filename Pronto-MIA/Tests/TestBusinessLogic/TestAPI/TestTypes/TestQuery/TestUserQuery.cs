#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestUserQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly UserQuery userQuery;

        public TestUserQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.userQuery = new UserQuery();
        }

        [Fact]
        public async void TestAuthentication()
        {
            var userManager = Substitute.For<IUserManager>();

            await this.userQuery.Authenticate(
                userManager, "Bob", "Hello World");

            await userManager.Received().Authenticate("Bob", "Hello World");
        }

        [Fact]
        public async void TestUser()
        {
            var user =
                await this.dbContext.Users
                    .SingleAsync(u => u.UserName == "Bob");
            var userManager = Substitute.For<IUserManager>();
            var userState = new ApiUserState(user.Id, user.UserName);

            await this.userQuery.User(
                userManager,
                userState);

            await userManager.Received().GetById(user.Id);
        }

        [Fact]
        public async Task TestUsersUnlimited()
        {
            var acl = new AccessControlList(-1)
                { CanViewUsers = true };
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi", acl);
            var userState = new ApiUserState(user.Id, user.UserName);
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetById(default).ReturnsForAnyArgs(user);
            userManager.GetAll().Returns(this.dbContext.Users);
            var userCount = await this.dbContext.Users.CountAsync();

            var result = await this.userQuery.Users(userManager, userState);

            userManager.Received().GetAll();
            Assert.Equal(userCount, await result.CountAsync());

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUsersLimited()
        {
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi");
            var userState = new ApiUserState(user.Id, user.UserName);
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetById(default).ReturnsForAnyArgs(user);
            userManager.GetAll().Returns(this.dbContext.Users);
            var userCount = await this.dbContext.Users.CountAsync();

            var result = await this.userQuery.Users(userManager, userState);

            userManager.Received().GetAll();
            Assert.Equal(1, await result.CountAsync());

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }
    }
}