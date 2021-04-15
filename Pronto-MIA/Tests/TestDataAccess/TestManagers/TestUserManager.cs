namespace Tests.TestDataAccess.TestManagers
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
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
    }
}