namespace Tests.TestDataAccess.TestManagers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Execution;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ClearExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Adapters.Interfaces;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestFirebaseMessagingManager
    {
        private readonly FirebaseMessagingManager firebaseMessagingManager;
        private readonly ProntoMiaDbContext dbContext;
        private readonly IFirebaseMessagingAdapter firebaseMessagingAdapter;

        public TestFirebaseMessagingManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
            this.firebaseMessagingAdapter =
                Substitute.For<IFirebaseMessagingAdapter>();

            this.firebaseMessagingManager = new FirebaseMessagingManager(
                TestHelpers.TestConfiguration,
                Substitute.For<ILogger<FirebaseMessagingManager>>(),
                this.firebaseMessagingAdapter);
        }

        [Fact]
        public async void TestSendMulticastAsyncNoTokens()
        {
            var notification = new Notification
            {
                Title = "Test",
                Body = "This is a test.",
            };
            var data = new Dictionary<string, string>()
                { { "score", "850" }, { "time", "2:45" }, };
            var tokens = new List<string>();

            await this.firebaseMessagingManager
                .SendMulticastAsync(tokens, notification, data);

            await this.firebaseMessagingAdapter.DidNotReceiveWithAnyArgs()
                .SendMulticastAsync(default);

            this.firebaseMessagingAdapter.ClearReceivedCalls();
        }

        [Fact]
        public async void TestSendMulticastAsyncFewTokens()
        {
            var notification = new Notification
            {
                Title = "Test",
                Body = "This is a test.",
            };
            var data = new Dictionary<string, string>()
                { { "score", "850" }, { "time", "2:45" }, };
            var tokens = TestHelpersFirebaseMessagingManager.CreateTokens(8);
            this.firebaseMessagingAdapter.SendMulticastAsync(default)
                .ReturnsForAnyArgs(Task.FromResult(
                    TestHelpersFirebaseMessagingManager
                        .CreateBatchResponse(8)));

            await this.firebaseMessagingManager
                .SendMulticastAsync(tokens, notification, data);

            var receivedMessage = (MulticastMessage)this.
                firebaseMessagingAdapter.ReceivedCalls().First()
                .GetArguments()[0];

            Assert.Equal(tokens, receivedMessage.Tokens);

            this.firebaseMessagingAdapter.ClearSubstitute();
        }

        [Fact]
        public async void TestSendMulticastAsyncManyTokens()
        {
            var notification = new Notification
            { Title = "Test", Body = "This is a test." };
            var data = new Dictionary<string, string>() { { "score", "850" } };
            var tokens = TestHelpersFirebaseMessagingManager.CreateTokens(520);
            this.firebaseMessagingAdapter.SendMulticastAsync(default)
                .ReturnsForAnyArgs(Task.FromResult(
                    TestHelpersFirebaseMessagingManager
                        .CreateBatchResponse(8)));

            await this.firebaseMessagingManager
                .SendMulticastAsync(tokens, notification, data);

            await this.firebaseMessagingAdapter.ReceivedWithAnyArgs(2)
                .SendMulticastAsync(default);

            var receivedMessage1 = (MulticastMessage)this.
                firebaseMessagingAdapter.ReceivedCalls().First()
                .GetArguments()[0];
            var receivedMessage2 = (MulticastMessage)this.
                firebaseMessagingAdapter.ReceivedCalls().Skip(1).First()
                .GetArguments()[0];

            Assert.Equal(500, receivedMessage1.Tokens.Count);
            Assert.Equal(20, receivedMessage2.Tokens.Count);

            this.firebaseMessagingAdapter.ClearSubstitute();
        }

        [Fact]
        public async void TestSendMulticastAsyncWithGlobalError()
        {
            var notification = new Notification
                { Title = "Test", Body = "This is a test." };
            var data = new Dictionary<string, string>() { { "score", "850" } };
            var tokens = TestHelpersFirebaseMessagingManager.CreateTokens(8);

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.firebaseMessagingManager
                    .SendMulticastAsync(tokens, notification, data);
            });

            Assert.Equal(
                Error.FirebaseOperationError.ToString(),
                error.Errors[0].Code);

            this.firebaseMessagingAdapter.ClearReceivedCalls();
        }

        [Fact]
        public async void TestSendMulticastAsyncWithResponseErrors()
        {
            var testToken = new FcmToken(
                Guid.NewGuid().ToString(), this.dbContext.Users.First());
            var notification = new Notification
                { Title = "Test", Body = "This is a test." };
            var data = new Dictionary<string, string>() { { "score", "850" } };
            var tokens = new List<string>() { testToken.Id };
            tokens.AddRange(
                TestHelpersFirebaseMessagingManager.CreateTokens(7));

            this.firebaseMessagingAdapter.SendMulticastAsync(default)
                .ReturnsForAnyArgs(Task.FromResult(
                    TestHelpersFirebaseMessagingManager
                        .CreateBatchResponse(8, 4)));

            var invalidTokens = await this.firebaseMessagingManager
                    .SendMulticastAsync(tokens, notification, data);

            Assert.Contains(testToken.Id, invalidTokens);
            Assert.Equal(4, invalidTokens.Count);

            this.firebaseMessagingAdapter.ClearSubstitute();
        }
    }
}