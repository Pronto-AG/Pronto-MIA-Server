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

    public class TestInternalNewsMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly InternalNewsMutation internalNewsMutation;

        public TestInternalNewsMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.internalNewsMutation = new InternalNewsMutation();
        }

        [Fact]
        public async void TestCreateInternalNews()
        {
            var internalNewsManager =
                Substitute.For<IInternalNewsManager>();

            await this.internalNewsMutation.CreateInternalNews(
                this.dbContext,
                internalNewsManager,
                "Test",
                string.Empty,
                DateTime.UtcNow,
                Substitute.For<IFile>());

            internalNewsManager.Received().SetDbContext(this.dbContext);
            await internalNewsManager.ReceivedWithAnyArgs()
                .Create(default!, default, default, default);
        }

        [Fact]
        public async void TestUpdateInternalNews()
        {
            var internalNewsManager =
                Substitute.For<IInternalNewsManager>();

            await this.internalNewsMutation.UpdateInternalNews(
                this.dbContext,
                internalNewsManager,
                1,
                "Test",
                string.Empty,
                DateTime.UtcNow,
                Substitute.For<IFile>());

            internalNewsManager.Received().SetDbContext(this.dbContext);
            await internalNewsManager.ReceivedWithAnyArgs()
                .Update(
                    default,
                    default,
                    default,
                    default,
                    default);
        }

        [Fact]
        public async void TestPublishInternalNews()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager = Substitute.For<IFirebaseTokenManager>();
            var internalNewsManager = Substitute.For<IInternalNewsManager>();
            internalNewsManager.Publish(default).ReturnsForAnyArgs(true);
            firebaseTokenManager.GetAllFcmToken()
                .ReturnsForAnyArgs(this.dbContext.FcmTokens);

            var res = await this.internalNewsMutation.PublishInternalNews(
                internalNewsManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.True(res);
            await internalNewsManager.ReceivedWithAnyArgs().Publish(default);
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
        public async void TestPublishInternalNewsAlreadyPublished()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var internalNewsManager =
                Substitute.For<IInternalNewsManager>();
            internalNewsManager.Publish(default).ReturnsForAnyArgs(false);

            var result = await this.internalNewsMutation
                .PublishInternalNews(
                internalNewsManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.False(result);
            await internalNewsManager.ReceivedWithAnyArgs().Publish(default);
            await firebaseMessagingManager.DidNotReceiveWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
        }

        [Fact]
        public async void TestHideInternalNews()
        {
            var internalNewsManager =
                Substitute.For<IInternalNewsManager>();

            await this.internalNewsMutation
                .HideInternalNews(internalNewsManager, 1);

            await internalNewsManager.Received().Hide(1);
        }

        [Fact]
        public async void TestRemoveInternalNews()
        {
            var internalNewsManager =
                Substitute.For<IInternalNewsManager>();

            await this.internalNewsMutation.RemoveInternalNews(
                internalNewsManager,
                1);

            await internalNewsManager.Received()
                .Remove(1);
        }
    }
}