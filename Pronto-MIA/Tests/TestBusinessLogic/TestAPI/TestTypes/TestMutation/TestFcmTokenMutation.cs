#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Xunit;

    public class TestFcmTokenMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly FcmTokenMutation fcmTokenMutation;

        public TestFcmTokenMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.fcmTokenMutation = new FcmTokenMutation();
        }

        [Fact]
        public async void TestRegisterFcmToken()
        {
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var user = await this.dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName == "Bob");
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetById(default).ReturnsForAnyArgs(user);

            await this.fcmTokenMutation.RegisterFcmToken(
                this.dbContext,
                userManager,
                firebaseTokenManager,
                new ApiUserState(user.Id, user.UserName),
                "Hello World");

            userManager.Received().SetDbContext(this.dbContext);
            firebaseTokenManager.Received().SetDbContext(this.dbContext);
            await firebaseTokenManager.Received()
                .RegisterFcmToken(user, "Hello World");
        }

        [Fact]
        public async void TestUnregisterFcmToken()
        {
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();

            await this.fcmTokenMutation.UnregisterFcmToken(
                firebaseTokenManager,
                "Hello World");

            await firebaseTokenManager.Received()
                .UnregisterFcmToken("Hello World");
        }
    }
}