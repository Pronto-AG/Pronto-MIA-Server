using Pronto_MIA.BusinessLogic.API.EntityExtensions;

namespace Tests.TestBusinessLogic.TestAPI.TestEntityExtensions
{
    using System;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Domain.EntityExtensions;
    using Xunit;

    public class TestErrorExtension
    {
        private readonly ProntoMiaDbContext dbContext;

        public TestErrorExtension()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
        }

        [Fact]
        public void TestHasMessage()
        {
            foreach (
                Error error in
                (Error[])Enum.GetValues(typeof(Error)))
            {
                Assert.NotNull(error.Message());
            }
        }
    }
}