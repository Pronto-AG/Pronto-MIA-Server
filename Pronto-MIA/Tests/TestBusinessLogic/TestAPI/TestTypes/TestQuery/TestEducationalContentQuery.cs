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

    public class TestEducationalContentQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly EducationalContentQuery educationalContentQuery;

        public TestEducationalContentQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.educationalContentQuery = new EducationalContentQuery();
        }

        [Fact]
        public async Task TestEducationalContent()
        {
            var educationalContentManager =
                Substitute.For<IEducationalContentManager>();
            educationalContentManager.GetAll().Returns(
                this.dbContext.EducationalContent);
            var educationalContentCount =
                await this.dbContext.EducationalContent.CountAsync();

            var result = this.educationalContentQuery.EducationalContent(
               educationalContentManager);

            educationalContentManager.Received().GetAll();
            Assert.Equal(educationalContentCount, await result.CountAsync());

            await this.dbContext.SaveChangesAsync();
        }
    }
}