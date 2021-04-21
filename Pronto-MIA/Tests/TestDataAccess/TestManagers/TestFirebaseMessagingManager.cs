namespace Tests.TestDataAccess.TestManagers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ClearExtensions;
    using NSubstitute.ExceptionExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Adapters.Interfaces;
    using Pronto_MIA.DataAccess.Managers;
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

            var result = await this.firebaseMessagingManager
                .SendAsync(message);

            Assert.False(result);

            this.firebaseMessagingAdapter.ClearSubstitute();
        }
    }
}