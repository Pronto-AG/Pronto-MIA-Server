namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using System;
    using System.Collections.Generic;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Types;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestDeploymentPlanMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly DeploymentPlanMutation deploymentPlanMutation;

        public TestDeploymentPlanMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.deploymentPlanMutation = new DeploymentPlanMutation();
        }

        [Fact]
        public async void TestCreateDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.deploymentPlanMutation.CreateDeploymentPlan(
                this.dbContext,
                deploymentPlanManager,
                departmentManager,
                Substitute.For<IFile>(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                5,
                string.Empty);

            deploymentPlanManager.Received().SetDbContext(this.dbContext);
            departmentManager.Received().SetDbContext(this.dbContext);
            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Create(default!, default, default, default);
            await departmentManager.Received().AddDeploymentPlan(
                5, Arg.Any<DeploymentPlan>());
        }

        [Fact]
        public async void TestUpdateDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.deploymentPlanMutation.UpdateDeploymentPlan(
                this.dbContext,
                deploymentPlanManager,
                departmentManager,
                5,
                Substitute.For<IFile>(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                5,
                string.Empty);

            deploymentPlanManager.Received().SetDbContext(this.dbContext);
            departmentManager.Received().SetDbContext(this.dbContext);
            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Update(default, default, default, default, default);
            await departmentManager.Received().AddDeploymentPlan(
                5, Arg.Any<DeploymentPlan>());
        }

        [Fact]
        public async void TestUpdateDeploymentPlanEmptyDepartment()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.deploymentPlanMutation.UpdateDeploymentPlan(
                this.dbContext,
                deploymentPlanManager,
                departmentManager,
                5,
                Substitute.For<IFile>(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                string.Empty);

            deploymentPlanManager.Received().SetDbContext(this.dbContext);
            await deploymentPlanManager.ReceivedWithAnyArgs()
                .Update(default, default, default, default, default);
            await departmentManager.DidNotReceiveWithAnyArgs()
                .AddDeploymentPlan(Arg.Any<int>(), Arg.Any<DeploymentPlan>());
        }

        [Fact]
        public async void TestPublishDeploymentPlan()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager = Substitute.For<IFirebaseTokenManager>();
            var deploymentPlanManager = this.CreateManagerWithGetId(1);
            deploymentPlanManager.Publish(default).ReturnsForAnyArgs(true);
            firebaseTokenManager.GetDepartmentFcmToken(default)
                .ReturnsForAnyArgs(this.dbContext.FcmTokens);

            var res = await this.deploymentPlanMutation.PublishDeploymentPlan(
                deploymentPlanManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.True(res);
            await deploymentPlanManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.ReceivedWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
            firebaseTokenManager.Received().GetDepartmentFcmToken(1);
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

            var result = await this.deploymentPlanMutation
                .PublishDeploymentPlan(
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

            await this.deploymentPlanMutation
                .HideDeploymentPlan(deploymentPlanManager, 1);

            await deploymentPlanManager.Received().Hide(1);
        }

        [Fact]
        public async void TestRemoveDeploymentPlan()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            await this.deploymentPlanMutation.RemoveDeploymentPlan(
                deploymentPlanManager,
                5);

            await deploymentPlanManager.Received()
                .Remove(5);
        }

        private IDeploymentPlanManager CreateManagerWithGetId(int departmentId)
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            deploymentPlanManager.Publish(default).ReturnsForAnyArgs(true);
            deploymentPlanManager.GetById(default).ReturnsForAnyArgs(
                new DeploymentPlan(
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(2),
                    Guid.NewGuid(),
                    string.Empty)
                {
                    DepartmentId = departmentId,
                });

            return deploymentPlanManager;
        }
    }
}