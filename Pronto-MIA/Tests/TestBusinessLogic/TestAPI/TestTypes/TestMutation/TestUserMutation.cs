#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using System.Threading.Tasks;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestUserMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly UserMutation userMutation;

        public TestUserMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.userMutation = new UserMutation();
        }

        [Fact]
        public async Task TestPasswordChange()
        {
            var userManager = this.CreateUserManager();
            var user = new User("ChangeTest", new byte[5], "{}", "{}")
            {
                Id = -5,
            };
            var userState = new ApiUserState(user.Id, user.UserName);

            await this.userMutation.ChangePassword(
                userManager,
                userState,
                "old",
                "new");

            await userManager.Received()
                .ChangePassword(-5, "old", "new");
        }

        [Fact]
        public async void TestCreateUser()
        {
            var userManager = this.CreateUserManager();
            var aclManager =
                Substitute.For<IAccessControlListManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();
            var acl = new AccessControlList(1);

            var user = await this.userMutation.CreateUser(
                this.dbContext,
                userManager,
                aclManager,
                departmentManager,
                "Ruedi",
                "HelloWorld1-",
                acl,
                5);

            userManager.Received().SetDbContext(this.dbContext);
            aclManager.Received().SetDbContext(this.dbContext);
            departmentManager.Received().SetDbContext(this.dbContext);
            await userManager.Received()
                .Create("Ruedi", "HelloWorld1-");
            await aclManager.Received()
                .LinkAccessControlList(user.Id, acl);
            await departmentManager.Received()
                .AddUser(5, user);
        }

        [Fact]
        public async void TestUpdateUser()
        {
            var userManager = this.CreateUserManager();
            var aclManager =
                Substitute.For<IAccessControlListManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();
            var acl = new AccessControlList(1);

            await this.userMutation.UpdateUser(
                this.dbContext,
                userManager,
                aclManager,
                departmentManager,
                1,
                "Ruedi",
                "HelloWorld1-",
                acl,
                5);

            userManager.Received().SetDbContext(this.dbContext);
            aclManager.Received().SetDbContext(this.dbContext);
            departmentManager.Received().SetDbContext(this.dbContext);
            await userManager.Received()
                .Update(1, "Ruedi", "HelloWorld1-");
            await aclManager.Received().LinkAccessControlList(1, acl);
            await departmentManager.Received().AddUser(5, Arg.Any<User>());
        }

        [Fact]
        public async void TestUpdateUserEmptyAcl()
        {
            var userManager =
                Substitute.For<IUserManager>();
            var aclManager =
                Substitute.For<IAccessControlListManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.userMutation.UpdateUser(
                this.dbContext,
                userManager,
                aclManager,
                departmentManager,
                1,
                "Ruedi",
                "HelloWorld1-",
                null,
                5);

            userManager.Received().SetDbContext(this.dbContext);
            departmentManager.Received().SetDbContext(this.dbContext);
            await userManager.Received()
                .Update(1, "Ruedi", "HelloWorld1-");
            await aclManager.DidNotReceiveWithAnyArgs()
                .LinkAccessControlList(Arg.Any<int>(), default);
            await departmentManager.Received().AddUser(5, Arg.Any<User>());
        }

        [Fact]
        public async void TestUpdateUserEmptyDepartment()
        {
            var userManager = this.CreateUserManager();
            var aclManager =
                Substitute.For<IAccessControlListManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.userMutation.UpdateUser(
                this.dbContext,
                userManager,
                aclManager,
                departmentManager,
                1,
                "Ruedi",
                "HelloWorld1-",
                new AccessControlList(0),
                null);

            userManager.Received().SetDbContext(this.dbContext);
            aclManager.Received().SetDbContext(this.dbContext);
            await userManager.Received()
                .Update(1, "Ruedi", "HelloWorld1-");
            await aclManager.Received()
                .LinkAccessControlList(
                    Arg.Any<int>(), Arg.Any<AccessControlList>());
            await departmentManager.DidNotReceiveWithAnyArgs()
                .AddUser(Arg.Any<int>(), Arg.Any<User>());
        }

        [Fact]
        public async void TestRemoveUser()
        {
            var userManager =
                Substitute.For<IUserManager>();

            await this.userMutation.RemoveUser(userManager, 1);

            await userManager.Received()
                .Remove(1);
        }

        private IUserManager CreateUserManager()
        {
            var userManager =
                Substitute.For<IUserManager>();
            userManager.Update(
                    Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(
                    new User("Ruedi", new byte[5], string.Empty, string.Empty)
                    {
                        Id = 1,
                    });
            userManager.Create(Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(
                    new User("Ruedi", new byte[5], string.Empty, string.Empty)
                    {
                        Id = 1,
                    });
            return userManager;
        }
    }
}