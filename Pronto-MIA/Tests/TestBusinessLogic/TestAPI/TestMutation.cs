#nullable enable
namespace Tests.TestBusinessLogic.TestAPI
{
    using System;
    using System.Linq;
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
                DateTime.UtcNow,
                string.Empty);

            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Create(default!, default, default, default);
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
                DateTime.UtcNow,
                string.Empty);

            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Update(default, default, default, default, default);
        }

        [Fact]
        public async void TestPublishDeploymentPlan()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            firebaseMessagingManager.GetAllFcmToken().Returns(
                this.dbContext.FcmTokens);

            await this.mutation.PublishDeploymentPlan(
                deploymentPlanManager,
                firebaseMessagingManager,
                1,
                "Hello World",
                "This is the notification body.");

            await deploymentPlanManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.ReceivedWithAnyArgs()
                .SendMulticastAsync(default, default, default);
        }

        [Fact]
        public async void TestPublishDeploymentPlanAlreadyPublished()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            deploymentPlanManager.Publish(default).ReturnsForAnyArgs(true);

            firebaseMessagingManager.GetAllFcmToken().Returns(
                this.dbContext.FcmTokens);

            await this.mutation.PublishDeploymentPlan(
                deploymentPlanManager,
                firebaseMessagingManager,
                1,
                "Hello World",
                "This is the notification body.");

            await deploymentPlanManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.DidNotReceiveWithAnyArgs()
                .SendMulticastAsync(default, default, default);
        }

        [Fact]
        public async void TestHideDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            await this.mutation
                .HideDeploymentPlan(deploymentPlanManager, 1);

            await deploymentPlanManager.Received().Hide(1);
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
            var userTask = this.dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName == "Bob");
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetByUserName("Bob").Returns(userTask);

            await this.mutation.RegisterFcmToken(
                firebaseMessagingManager,
                userManager,
                new ApiUserState("5", "Bob"),
                "Hello World");

            await userManager.Received().GetByUserName("Bob");
            await firebaseMessagingManager.Received()
                .RegisterFcmToken(await userTask, "Hello World");
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