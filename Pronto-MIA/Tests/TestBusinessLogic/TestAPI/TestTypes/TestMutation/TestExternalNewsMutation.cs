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

    public class TestExternalNewsMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly ExternalNewsMutation externalNewsMutation;

        public TestExternalNewsMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.externalNewsMutation = new ExternalNewsMutation();
        }

        [Fact]
        public async void TestCreateExternalNews()
        {
            var externalNewsManager =
                Substitute.For<IExternalNewsManager>();

            await this.externalNewsMutation.CreateExternalNews(
                this.dbContext,
                externalNewsManager,
                "Test",
                string.Empty,
                DateTime.UtcNow,
                Substitute.For<IFile>());

            externalNewsManager.Received().SetDbContext(this.dbContext);
            await externalNewsManager.ReceivedWithAnyArgs()
                .Create(default!, default, default, default);
        }

        [Fact]
        public async void TestUpdateExternalNews()
        {
            var externalNewsManager =
                Substitute.For<IExternalNewsManager>();

            await this.externalNewsMutation.UpdateExternalNews(
                this.dbContext,
                externalNewsManager,
                1,
                "Test",
                string.Empty,
                DateTime.UtcNow,
                Substitute.For<IFile>());

            externalNewsManager.Received().SetDbContext(this.dbContext);
            await externalNewsManager.ReceivedWithAnyArgs()
                .Update(
                    default,
                    default,
                    default,
                    default,
                    default);
        }

        [Fact]
        public async void TestPublishExternalNews()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager = Substitute.For<IFirebaseTokenManager>();
            var externalNewsManager = Substitute.For<IExternalNewsManager>();
            externalNewsManager.Publish(default).ReturnsForAnyArgs(true);
            firebaseTokenManager.GetAllFcmToken()
                .ReturnsForAnyArgs(this.dbContext.FcmTokens);

            var res = await this.externalNewsMutation.PublishExternalNews(
                externalNewsManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.True(res);
            await externalNewsManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.ReceivedWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
            firebaseTokenManager.Received().GetAllFcmToken();
            await firebaseTokenManager.ReceivedWithAnyArgs()
                .UnregisterMultipleFcmToken(Arg.Any<HashSet<string>>());
        }

        [Fact]
        public async void TestPublishExternalNewsAlreadyPublished()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var externalNewsManager =
                Substitute.For<IExternalNewsManager>();
            externalNewsManager.Publish(default).ReturnsForAnyArgs(false);

            var result = await this.externalNewsMutation
                .PublishExternalNews(
                externalNewsManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.False(result);
            await externalNewsManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.DidNotReceiveWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
        }

        [Fact]
        public async void TestHideExternalNews()
        {
            var externalNewsManager =
                Substitute.For<IExternalNewsManager>();

            await this.externalNewsMutation
                .HideExternalNews(externalNewsManager, 1);

            await externalNewsManager.Received().Hide(1);
        }

        [Fact]
        public async void TestRemoveExternalNews()
        {
            var externalNewsManager =
                Substitute.For<IExternalNewsManager>();

            await this.externalNewsMutation.RemoveExternalNews(
                externalNewsManager,
                1);

            await externalNewsManager.Received()
                .Remove(1);
        }
    }
}