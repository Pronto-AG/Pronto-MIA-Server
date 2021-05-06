namespace Tests.TestDataAccess.TestManagers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ClearExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Adapters.Interfaces;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    [SuppressMessage(
        "Menees.Analyzers",
        "MEN005",
        Justification = "Test file may be more than 300 lines.")]
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
                this.dbContext,
                TestHelpers.TestConfiguration,
                Substitute.For<ILogger<FirebaseMessagingManager>>(),
                this.firebaseMessagingAdapter);
        }

        [Fact]
        public async void TestFcmTokenCreateNew()
        {
            var bob =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Bob");

            await this.firebaseMessagingManager
                .RegisterFcmToken(bob, "Hello World");

            var token = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == "Hello World");
            Assert.Equal(token.Owner, bob);

            this.dbContext.FcmTokens.Remove(token);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void TestFcmTokenChangeOwner()
        {
            var bob =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Bob");
            await this.firebaseMessagingManager
                .RegisterFcmToken(bob, "Hello World");
            var alice =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Alice");

            await this.firebaseMessagingManager
                .RegisterFcmToken(alice, "Hello World");

            var token = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == "Hello World");
            Assert.Equal(token.Owner, alice);

            this.dbContext.FcmTokens.Remove(token);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void TestFcmTokenKeepOwner()
        {
            var bob =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Bob");
            await this.firebaseMessagingManager
                .RegisterFcmToken(bob, "Hello World");

            await this.firebaseMessagingManager
                .RegisterFcmToken(bob, "Hello World");

            var token = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == "Hello World");
            Assert.Equal(token.Owner, bob);

            this.dbContext.FcmTokens.Remove(token);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void TestFcmTokenRemove()
        {
            var bob =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Bob");
            await this.firebaseMessagingManager
                .RegisterFcmToken(bob, "Hello World");

            var result = await this.firebaseMessagingManager.UnregisterFcmToken(
                "Hello World");

            Assert.True(result);
            var token = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == "Hello World");
            Assert.Null(token);
        }

        [Fact]
        public async void TestFcmTokenRemoveNonExistent()
        {
            var result = await this.firebaseMessagingManager.UnregisterFcmToken(
                "Hello World");

            Assert.False(result);
            var token = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == "Hello World");
            Assert.Null(token);
        }

        [Fact]
        public async void TestGetAllTokens()
        {
            var testToken = new FcmToken(
                Guid.NewGuid().ToString(), this.dbContext.Users.First());
            this.dbContext.FcmTokens.Add(testToken);
            await this.dbContext.SaveChangesAsync();

            var result = await this.firebaseMessagingManager
                .GetAllFcmToken().ToListAsync();

            Assert.Equal(result.First(), testToken);
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

            var result = await this.firebaseMessagingManager
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

            var result = await this.firebaseMessagingManager
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

            var result = await this.firebaseMessagingManager
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
            this.dbContext.FcmTokens.Add(testToken);
            await this.dbContext.SaveChangesAsync();
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

            await this.firebaseMessagingManager
                    .SendMulticastAsync(tokens, notification, data);

            Assert.Empty(this.dbContext.FcmTokens.Where(token =>
                token.Id == testToken.Id));

            this.firebaseMessagingAdapter.ClearSubstitute();
        }

        [Fact]
        public async void TestSendAsync()
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Test",
                    Body = "This is a test.",
                },
                Data = new Dictionary<string, string>()
                    { { "score", "850" }, { "time", "2:45" }, },
                Token = "Hello World",
            };

            var result = await this.firebaseMessagingManager
                .SendAsync(message);

            await this.firebaseMessagingAdapter.Received().SendAsync(message);

            this.firebaseMessagingAdapter.ClearReceivedCalls();
        }

        [Fact]
        public async void TestSendAsyncError()
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Test",
                    Body = "This is a test.",
                },
                Data = new Dictionary<string, string>()
                    { { "score", "850" }, { "time", "2:45" }, },
                Token = "Hello World",
            };
            this.firebaseMessagingAdapter.SendAsync(default)
                .ReturnsForAnyArgs(
                    Task.FromException<string>(new IOException()));

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.firebaseMessagingManager.SendAsync(message);
            });

            Assert.Equal(
                Error.FirebaseOperationError.ToString(),
                error.Errors[0].Code);

            this.firebaseMessagingAdapter.ClearSubstitute();
        }
    }
}