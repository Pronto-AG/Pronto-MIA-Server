namespace Tests.TestDataAccess.TestManagers
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestAccessControlListManager
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly AccessControlListManager accessControlListManager;

        public TestAccessControlListManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);

            this.accessControlListManager = new AccessControlListManager(
                this.dbContext);
        }

        [Fact]
        public async Task TestLinkAclNew()
        {
            var user = new User(
                "Bob2",
                new byte[5],
                string.Empty,
                string.Empty);
            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();

            await
                this.accessControlListManager.
                    LinkAccessControlList(
                        user.Id, new AccessControlList(user.Id));

            var accessControlList =
                await this.dbContext.AccessControlLists
                    .SingleOrDefaultAsync(acl => acl.UserId == user.Id);
            Assert.NotNull(accessControlList);
        }

        [Fact]
        public async Task TestLinkAclUpdate()
        {
            var user = new User(
                "Bob2",
                new byte[5],
                string.Empty,
                string.Empty);
            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();
            var previousAcl = new AccessControlList(user.Id);
            this.dbContext.AccessControlLists.Add(previousAcl);
            await this.dbContext.SaveChangesAsync();

            await
                this.accessControlListManager.
                    LinkAccessControlList(
                        user.Id, new AccessControlList(user.Id)
                        {
                            CanEditUsers = true,
                        });

            var accessControlList =
                await this.dbContext.AccessControlLists
                    .SingleOrDefaultAsync(acl => acl.UserId == user.Id);
            Assert.True(accessControlList.CanEditUsers);
            Assert.Equal(previousAcl.Id, accessControlList.Id);
        }
    }
}
