namespace Tests.TestDomain.TestEntityExtensions
{
    using System;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Domain.EntityExtensions;
    using Xunit;

    public class TestAccessControlListExtension
    {
        private readonly ProntoMiaDbContext dbContext;

        public TestAccessControlListExtension()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
        }

        [Fact]
        public void TestHasControl()
        {
            var acl = new AccessControlList(-1);
            foreach (
                AccessControl accessControl in
                (AccessControl[])Enum.GetValues(typeof(AccessControl)))
            {
                Assert.False(acl.HasControl(accessControl));
            }
        }
    }
}