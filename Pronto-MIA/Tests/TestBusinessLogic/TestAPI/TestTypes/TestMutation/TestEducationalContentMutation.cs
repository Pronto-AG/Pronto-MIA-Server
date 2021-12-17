namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Types;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestEducationalContentMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly EducationalContentMutation educationalContentMutation;

        public TestEducationalContentMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.educationalContentMutation = new EducationalContentMutation();
        }

        [Fact]
        public async void TestCreateEducationalContent()
        {
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();

            await this.educationalContentMutation.CreateEducationalContent(
                this.dbContext,
                educationalContentManager,
                "Test",
                string.Empty,
                Substitute.For<IFile>());

            educationalContentManager.Received().SetDbContext(this.dbContext);
            await educationalContentManager.ReceivedWithAnyArgs()
                .Create(default!, default, default);
        }

        [Fact]
        public async void TestUpdateEducationalContent()
        {
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();

            await this.educationalContentMutation.UpdateEducationalContent(
                this.dbContext,
                educationalContentManager,
                1,
                "Test",
                string.Empty,
                Substitute.For<IFile>());

            educationalContentManager.Received().SetDbContext(this.dbContext);
            await educationalContentManager.ReceivedWithAnyArgs()
                .Update(
                    default,
                    default,
                    default,
                    default);
        }

        [Fact]
        [SuppressMessage(
        "Menees.Analyzers",
        "MEN003",
        Justification = "Test may be more than 30 lines.")]
        public async void TestPublishEducationalContent()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();
            educationalContentManager.Publish(default).ReturnsForAnyArgs(true);
            firebaseTokenManager.GetAllFcmToken()
                .ReturnsForAnyArgs(this.dbContext.FcmTokens);

            var res = await this.educationalContentMutation
                .PublishEducationalContent(
                    educationalContentManager,
                    firebaseMessagingManager,
                    firebaseTokenManager,
                    1,
                    "Hello World",
                    "This is the notification body.");

            Assert.True(res);
            await educationalContentManager.ReceivedWithAnyArgs()
                .Publish(default);
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
        public async void TestPublishEducationalContentAlreadyPublished()
        {
            var firebaseMessagingManager =
                Substitute.For<IFirebaseMessagingManager>();
            var firebaseTokenManager =
                Substitute.For<IFirebaseTokenManager>();
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();
            educationalContentManager.Publish(default).ReturnsForAnyArgs(false);

            var result = await this.educationalContentMutation
                .PublishEducationalContent(
                educationalContentManager,
                firebaseMessagingManager,
                firebaseTokenManager,
                1,
                "Hello World",
                "This is the notification body.");

            Assert.False(result);
            await educationalContentManager.ReceivedWithAnyArgs()
                .Publish(default);
            await firebaseMessagingManager.DidNotReceiveWithAnyArgs()
                .SendMulticastAsync(
                    Arg.Any<List<string>>(),
                    Arg.Any<Notification>(),
                    Arg.Any<Dictionary<string, string>>());
        }

        [Fact]
        public async void TestHideEducationalContent()
        {
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();

            await this.educationalContentMutation
                .HideEducationalContent(educationalContentManager, 1);

            await educationalContentManager.Received().Hide(1);
        }

        [Fact]
        public async void TestRemoveEducationalContent()
        {
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();

            await this.educationalContentMutation.RemoveEducationalContent(
                educationalContentManager,
                1);

            await educationalContentManager.Received()
                .Remove(1);
        }
    }
}