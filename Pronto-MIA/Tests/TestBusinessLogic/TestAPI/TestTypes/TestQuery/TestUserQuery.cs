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
        public async Task TestUsers()
        {
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi");
            var userState = new ApiUserState(user);
            var userManager =
                Substitute.For<IUserManager>();

            this.userQuery.Users(userManager, userState);

            userManager.Received().GetAll();
        }
    }
}