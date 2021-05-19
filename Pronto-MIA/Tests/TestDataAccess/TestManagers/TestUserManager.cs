namespace Tests.TestDataAccess.TestManagers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestUserManager
    {
        private readonly UserManager userManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestUserManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);

            this.userManager = new UserManager(
                this.dbContext,
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
        public async Task TestAuthenticateUserOldHash()
        {
            var generator = new NullGenerator(null);
            var user = new User(
                "OldHash",
                generator.HashPassword("Password"),
                generator.GetIdentifier(),
                "{}");
            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();

            var result =
                await this.userManager.Authenticate("OldHash", "Password");

            var adjustedUser = await this.dbContext.Users.FindAsync(user.Id);
            Assert.NotEmpty(result);
            Assert.Equal(
                Pbkdf2Generator.Identifier, adjustedUser.HashGenerator);

            this.dbContext.Users.Remove(adjustedUser);
            await this.dbContext.SaveChangesAsync();
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

        [Fact]
        public async Task TestCreate()
        {
            var user = await this.userManager.Create(
                "Alice2", "H1-ello");

            var dbUser = await this.dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(user);

            this.dbContext.Users.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestCreateExisting()
        {
            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.Create(
                    "Alice", "H1-ello");
            });

            Assert.Equal(
                Error.UserAlreadyExists.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestCreateNonConformPassword()
        {
            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.Create(
                    "Alice2", "H-ello");
            });

            Assert.Equal(
                Error.PasswordTooWeak.ToString(),
                error.Errors[0].Code);
            var user = this.dbContext.Users.Where(u => u.UserName == "Alice2");
            Assert.Empty(user);
        }

        [Fact]
        public async Task TestRemove()
        {
            var user = new User(
                "Alice2", new byte[10], NullGenerator.Identifier, "{}");
            this.dbContext.Add(user);
            await this.dbContext.SaveChangesAsync();

            await this.userManager.Remove(user.Id);

            var users = this.dbContext.Users.Where(u => u.UserName == "Alice2");
            Assert.Empty(users);
        }

        [Fact]
        public async Task TestRemoveWrongId()
        {
            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.Remove(-5);
            });

            Assert.Equal(
                Error.UserNotFound.ToString(),
                error.Errors[0].Code);
        }
    }
}