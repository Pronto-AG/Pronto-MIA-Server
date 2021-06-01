using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pronto_MIA.BusinessLogic.API.Types;
using Pronto_MIA.Domain.Entities;

#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Xunit;

    public class TestDepartmentQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly DepartmentQuery departmentQuery;

        public TestDepartmentQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.departmentQuery = new DepartmentQuery();
        }

        [Fact]
        public async Task TestDepartments()
        {
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi");
            var userState = new ApiUserState(user);
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            this.departmentQuery.Departments(departmentManager, userState);

            departmentManager.Received().GetAll();

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }
    }
}