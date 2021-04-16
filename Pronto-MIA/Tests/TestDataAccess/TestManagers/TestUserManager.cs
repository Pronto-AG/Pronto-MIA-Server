namespace Tests.TestDataAccess.TestManagers
{
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Xunit;

    public class TestUserManager
    {
        private readonly UserManager userManager;

        public TestUserManager()
        {
            var dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(dbContext);

            this.userManager = new UserManager(
                dbContext,
                TestHelpers.TestConfiguration,
                Substitute.For<ILogger<UserManager>>());
        }

        [Fact]
        public async Task TestAuthenticateUser()
        {
            var result =
                await this.userManager.Authenticate("Bob", "Hello Alice");

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task TestAuthenticateUserNotFound()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.userManager.Authenticate("_Bob", "Hello Alice"));
            Assert.Equal(Error.UserNotFound.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestAuthenticatePasswordIncorrect()
        {
            var error = await Assert.ThrowsAsync<QueryException>(async () =>
                await this.userManager.Authenticate("Bob", "Hello"));
            Assert.Equal(Error.WrongPassword.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestGetByUsername()
        {
            var user = await this.userManager.GetByUserName("Alice");

            Assert.NotNull(user);
            Assert.Equal("Alice", user.UserName);
        }

        [Fact]
        public async Task TestGetByInvalidUsername()
        {
            var user = await this.userManager.GetByUserName("_Alice");

            Assert.Null(user);
        }
    }
}