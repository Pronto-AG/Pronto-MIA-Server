namespace Tests.TestDataAccess.TestManagers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    [SuppressMessage(
        "Menees.Analyzers",
        "MEN005",
        Justification = "Tests for a single class.")]
    public class TestUserManager
    {
        private readonly UserManager userManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestUserManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
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
        public async Task TestPasswordChange()
        {
            var user = await this.dbContext.Users.SingleAsync(
                u => u.UserName == "Bob");
            var oldHash = user.PasswordHash;
            var newPassword = "H1-zkr.3";

            await this.userManager.ChangePassword(
                user.Id, "Hello Alice", newPassword);

            Assert.NotEqual(oldHash, user.PasswordHash);

            // Would throw if password where incorrect.
            await this.userManager.Authenticate("Bob", newPassword);

            this.dbContext.RemoveRange(this.dbContext.Users);
            await this.dbContext.SaveChangesAsync();
            TestDataProvider.InsertTestData(this.dbContext);
        }

        [Fact]
        public async Task TestPasswordChangeInvalidUser()
        {
            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.ChangePassword(
                    -5, "Hello Alice", "newPassword");
            });

            Assert.Equal(Error.UserNotFound.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestPasswordChangeWrongPassword()
        {
            var user = await this.dbContext.Users.SingleAsync(
                u => u.UserName == "Bob");

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.ChangePassword(
                    user.Id, "Hello Alice-", "newPassword");
            });

            Assert.Equal(Error.WrongPassword.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestPasswordChangePasswordToWeak()
        {
            var user = await this.dbContext.Users.SingleAsync(
                u => u.UserName == "Bob");

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.ChangePassword(
                    user.Id, "Hello Alice", "newPassword");
            });

            Assert.Equal(
                Error.PasswordTooWeak.ToString(), error.Errors[0].Code);
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
        public async Task TestGetAllMultiple()
        {
            var users = this.userManager.GetAll();

            Assert.Equal(2, await users.CountAsync());
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.Users.RemoveRange(
                this.dbContext.Users);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.userManager.GetAll();
            Assert.Empty(deploymentPlans);

            TestDataProvider.InsertTestData(this.dbContext);
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
        public async Task TestUpdate()
        {
            var user = new User(
                "Alice2", new byte[10], NullGenerator.Identifier, "{}");
            this.dbContext.Add(user);
            await this.dbContext.SaveChangesAsync();

            await this.userManager.Update(user.Id, "Alice3", "H1-ello");

            user = await this.dbContext.Users.FindAsync(user.Id);
            Assert.Equal("Alice3", user.UserName);
            Assert.Equal(
                "H1-ello", Encoding.ASCII.GetString(user.PasswordHash));

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateUsernameOnly()
        {
            var user = new User(
                "Alice2", new byte[10], NullGenerator.Identifier, "{}");
            this.dbContext.Add(user);
            await this.dbContext.SaveChangesAsync();

            await this.userManager.Update(user.Id, "Alice3", null);

            user = await this.dbContext.Users.FindAsync(user.Id);
            Assert.Equal("Alice3", user.UserName);
            Assert.Equal(
                new byte[10], user.PasswordHash);

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdatePasswordOnly()
        {
            var user = new User(
                "Alice2", new byte[10], NullGenerator.Identifier, "{}");
            this.dbContext.Add(user);
            await this.dbContext.SaveChangesAsync();

            await this.userManager.Update(user.Id, null, "H1-ello");

            user = await this.dbContext.Users.FindAsync(user.Id);
            Assert.Equal("Alice2", user.UserName);
            Assert.Equal(
                "H1-ello", Encoding.ASCII.GetString(user.PasswordHash));

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateEmpty()
        {
            var user = new User(
                "Alice2", new byte[10], NullGenerator.Identifier, "{}");
            this.dbContext.Add(user);
            await this.dbContext.SaveChangesAsync();

            await this.userManager.Update(user.Id, null, null);

            user = await this.dbContext.Users.FindAsync(user.Id);
            Assert.Equal("Alice2", user.UserName);
            Assert.Equal(
                new byte[10], user.PasswordHash);

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateToExistingUsername()
        {
            var user = new User(
                "Alice2", new byte[10], NullGenerator.Identifier, "{}");
            this.dbContext.Add(user);
            await this.dbContext.SaveChangesAsync();

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.userManager.Update(user.Id, "Alice", null);
            });

            Assert.Equal(
                Error.UserAlreadyExists.ToString(),
                error.Errors[0].Code);
            user = await this.dbContext.Users.FindAsync(user.Id);
            Assert.Equal("Alice2", user.UserName);
            Assert.Equal(
                new byte[10], user.PasswordHash);

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
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