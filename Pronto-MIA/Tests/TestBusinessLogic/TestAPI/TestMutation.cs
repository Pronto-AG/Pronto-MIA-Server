#nullable enable
namespace Tests.TestBusinessLogic.TestAPI
{
    using System;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly Mutation mutation;

        public TestMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
            this.mutation = new Mutation();
        }

        [Fact]
        public async void TestAddDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            await this.mutation.AddDeploymentPlan(
                deploymentPlanManager,
                Substitute.For<IFile>(),
                DateTime.UtcNow,
                DateTime.UtcNow);

            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Create(default!, default, default);
        }

        [Fact]
        public async void TestUpdateDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            await this.mutation.UpdateDeploymentPlan(
                deploymentPlanManager,
                5,
                Substitute.For<IFile>(),
                DateTime.UtcNow,
                DateTime.UtcNow);

            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Update(default, default, default, default);
        }

        [Fact]
        public async void TestRemoveDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            await this.mutation.RemoveDeploymentPlan(
                deploymentPlanManager,
                5);

            await deploymentPlanManager.Received()
                .Remove(5);
        }

        [Fact]
        public async void TestSendPushTo()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();

            await this.mutation.SendPushTo(
                firebaseMessagingManager,
                "token123");

            await firebaseMessagingManager.ReceivedWithAnyArgs()
                .SendAsync(default);
        }

        [Fact]
        public async void TestRegisterFcmToken()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var user = await this.dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName == "Bob");
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetByUserName("Bob").Returns(
                Task.FromResult(user));

            await this.mutation.RegisterFcmToken(
                firebaseMessagingManager,
                userManager,
                new ApiUserState("5", "Bob"),
                "Hello World");

            await userManager.Received().GetByUserName("Bob");
            await firebaseMessagingManager.Received()
                .RegisterFcmToken(user, "Hello World");
        }

        [Fact]
        public async void TestRegisterFcmTokenError()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetByUserName("Bob").Returns(
                Task.FromResult<User?>(default));

            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                {
                    await this.mutation.RegisterFcmToken(
                        firebaseMessagingManager,
                        userManager,
                        new ApiUserState("5", "Bob"),
                        "Hello World");
                });

            Assert.Equal(
                Error.UserNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async void TestUnregisterFcmToken()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();

            await this.mutation.UnregisterFcmToken(
                firebaseMessagingManager,
                "Hello World");

            await firebaseMessagingManager.Received()
                .UnregisterFcmToken("Hello World");
        }
    }
}