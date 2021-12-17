#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestInternalNewsQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly InternalNewsQuery internalNewsQuery;

        public TestInternalNewsQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.internalNewsQuery = new InternalNewsQuery();
        }

        [Fact]
        public async Task TestInternalNews()
        {
            var internalNewsManager =
                Substitute.For<IInternalNewsManager>();
            internalNewsManager.GetAll().Returns(
                this.dbContext.InternalNews);
            var internalNewsCount =
                await this.dbContext.InternalNews.CountAsync();

            var result = this.internalNewsQuery.InternalNews(
               internalNewsManager);

            internalNewsManager.Received().GetAll();
            Assert.Equal(internalNewsCount, await result.CountAsync());

            await this.dbContext.SaveChangesAsync();
        }
    }
}