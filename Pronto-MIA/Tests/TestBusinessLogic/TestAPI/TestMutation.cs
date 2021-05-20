#nullable enable
namespace Tests.TestBusinessLogic.TestAPI
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
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
        public async void TestCreateUser()
        {
            var userManager =
                Substitute.For<IUserManager>();

            await this.mutation.CreateUser(
                userManager,
                "Ruedi",
                "HelloWorld1-");

            await userManager.Received()
                .Create("Ruedi", "HelloWorld1-");
        }

        [Fact]
        public async void TestUpdateUser()
        {
            var userManager =
                Substitute.For<IUserManager>();

            await this.mutation.UpdateUser(
                userManager,
                1,
                "Ruedi",
                "HelloWorld1-");

            await userManager.Received()
                .Update(1, "Ruedi", "HelloWorld1-");
        }

        [Fact]
        public async void TestRemoveUser()
        {
            var userManager =
                Substitute.For<IUserManager>();

            await this.mutation.RemoveUser(userManager, 1);

            await userManager.Received()
                .Remove(1);
        }

        [Fact]
        public async void TestCreateDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            await this.mutation.CreateDeploymentPlan(
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
            var firebaseTokenManager = Substitute.For<IFirebaseTokenManager>();
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            deploymentPlanManager.Publish(default).ReturnsForAnyArgs(true);
            firebaseTokenManager.GetAllFcmToken().Returns(
                this.dbContext.FcmTokens);

            var result = await this.mutation.PublishDeploymentPlan(
                deploymentPlanManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.True(result);
            await deploymentPlanManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.ReceivedWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
            await firebaseTokenManager.ReceivedWithAnyArgs()
                .UnregisterMultipleFcmToken(Arg.Any<HashSet<string>>());
        }

        [Fact]
        public async void TestPublishDeploymentPlanAlreadyPublished()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            deploymentPlanManager.Publish(default).ReturnsForAnyArgs(false);
            firebaseTokenManager.GetAllFcmToken().Returns(
                this.dbContext.FcmTokens);

            var result = await this.mutation.PublishDeploymentPlan(
                deploymentPlanManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.False(result);
            await deploymentPlanManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.DidNotReceiveWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
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
        public async void TestRegisterFcmToken()
        {
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var userTask = this.dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName == "Bob");
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetByUserName("Bob").Returns(userTask);

            await this.mutation.RegisterFcmToken(
                firebaseTokenManager,
                userManager,
                new ApiUserState("5", "Bob"),
                "Hello World");

            await userManager.Received().GetByUserName("Bob");
            await firebaseTokenManager.Received()
                .RegisterFcmToken(await userTask, "Hello World");
        }

        [Fact]
        public async void TestRegisterFcmTokenError()
        {
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var userManager =
                Substitute.For<IUserManager>();
            userManager.GetByUserName("Bob").Returns(
                Task.FromResult<User?>(default));

            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                {
                    await this.mutation.RegisterFcmToken(
                        firebaseTokenManager,
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
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();

            await this.mutation.UnregisterFcmToken(
                firebaseTokenManager,
                "Hello World");

            await firebaseTokenManager.Received()
                .UnregisterFcmToken("Hello World");
        }
    }
}