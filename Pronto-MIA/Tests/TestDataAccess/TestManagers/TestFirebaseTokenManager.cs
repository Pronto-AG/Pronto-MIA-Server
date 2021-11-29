namespace Tests.TestDataAccess.TestManagers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestFirebaseTokenManager
    {
        private readonly FirebaseTokenManager firebaseTokenManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestFirebaseTokenManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);

            this.dbContext.RemoveRange(this.dbContext.FcmTokens);
            this.dbContext.SaveChanges();

            this.firebaseTokenManager = new FirebaseTokenManager(
                this.dbContext,
                Substitute.For<ILogger<FirebaseTokenManager>>());
        }

        [Fact]
        public async void TestFcmTokenCreateNew()
        {
            var bob =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Bob");

            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, "Hello World");

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
            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, "Hello World");
            var alice =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Alice");

            await this.firebaseTokenManager
                .RegisterFcmToken(alice.Id, "Hello World");

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
            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, "Hello World");

            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, "Hello World");

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
            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, "Hello World");

            var result = await this.firebaseTokenManager.UnregisterFcmToken(
                "Hello World");

            Assert.True(result);
            var token = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == "Hello World");
            Assert.Null(token);
        }

        [Fact]
        public async void TestFcmTokenRemoveMultiple()
        {
            var tokenString1 = "HelloWorld1";
            var tokenString2 = "HelloWorld2";
            var bob =
                await this.dbContext.Users.SingleOrDefaultAsync(u =>
                    u.UserName == "Bob");
            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, tokenString1);
            await this.firebaseTokenManager
                .RegisterFcmToken(bob.Id, tokenString2);

            await this.firebaseTokenManager
                .UnregisterMultipleFcmToken(
                    new HashSet<string>() { tokenString1, tokenString2 });

            var tokenResult1 = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == tokenString1);
            Assert.Null(tokenResult1);
            var tokenResult2 = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == tokenString2);
            Assert.Null(tokenResult2);
        }

        [Fact]
        public async void TestFcmTokenRemoveNonExistent()
        {
            var result = await this.firebaseTokenManager.UnregisterFcmToken(
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

            var result = await this.firebaseTokenManager
                .GetAllFcmToken().ToListAsync();

            Assert.Equal(result.First(), testToken);

            this.dbContext.Remove(testToken);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void TestGetDepartmentTokensNone()
        {
            const int virtualDepartmentId = 0;
            var testToken = new FcmToken(
                Guid.NewGuid().ToString(), this.dbContext.Users.First());
            this.dbContext.FcmTokens.Add(testToken);
            await this.dbContext.SaveChangesAsync();

            var result = await this.firebaseTokenManager
                .GetDepartmentFcmToken(virtualDepartmentId).ToListAsync();

            Assert.Empty(result);

            this.dbContext.Remove(testToken);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void TestGetDepartmentTokensOne()
        {
            var department = await this.dbContext.Departments.FirstAsync();
            var user = await this.dbContext.Users.FirstAsync();
            var list = new List<Department>();
            list.Add(department);
            user.Departments = list;
            this.dbContext.Users.Update(user);
            var testToken = new FcmToken(
                Guid.NewGuid().ToString(), user);
            this.dbContext.FcmTokens.Add(testToken);
            await this.dbContext.SaveChangesAsync();

            var result = await this.firebaseTokenManager
                .GetDepartmentFcmToken(department.Id).ToListAsync();

            Assert.Equal(testToken, result.FirstOrDefault());

            user.Departments = null;
            this.dbContext.Update(user);
            this.dbContext.Remove(testToken);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void TestGetDepartmentTokensMultiple()
        {
            var department = await this.dbContext.Departments.FirstAsync();
            var user = await this.dbContext.Users.FindAsync(1);
            var user2 = await this.dbContext.Users.FindAsync(2);
            var list = new List<Department>();
            list.Add(department);
            user.Departments = list;
            user2.Departments = list;
            this.dbContext.Users.UpdateRange(user, user2);
            var testToken = new FcmToken(
                Guid.NewGuid().ToString(), user);
            var testToken2 = new FcmToken(
                Guid.NewGuid().ToString(), user2);
            var controlToken = new FcmToken(
                Guid.NewGuid().ToString(), null);
            this.dbContext.FcmTokens.AddRange(
                testToken, testToken2, controlToken);
            await this.dbContext.SaveChangesAsync();

            var result = await this.firebaseTokenManager
                .GetDepartmentFcmToken(department.Id).ToListAsync();

            Assert.Equal(2, result.Count());

            this.dbContext.Users.RemoveRange(user, user2);
            this.dbContext.FcmTokens.RemoveRange(this.dbContext.FcmTokens);
            await this.dbContext.SaveChangesAsync();
        }
    }
}