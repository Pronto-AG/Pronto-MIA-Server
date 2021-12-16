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

    public class TestExternalNewsQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly ExternalNewsQuery externalNewsQuery;

        public TestExternalNewsQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.externalNewsQuery = new ExternalNewsQuery();
        }

        [Fact]
        public async Task TestExternalNews()
        {
            var externalNewsManager =
                Substitute.For<IExternalNewsManager>();
            externalNewsManager.GetAll().Returns(
                this.dbContext.ExternalNews);
            var externalNewsCount =
                await this.dbContext.ExternalNews.CountAsync();

            var result = this.externalNewsQuery.ExternalNews(
               externalNewsManager);

            externalNewsManager.Received().GetAll();
            Assert.Equal(externalNewsCount, await result.CountAsync());

            await this.dbContext.SaveChangesAsync();
        }
    }
}