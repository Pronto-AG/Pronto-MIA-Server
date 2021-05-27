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
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
            this.departmentQuery = new DepartmentQuery();
        }

        [Fact]
        public void TestDepartments()
        {
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            this.departmentQuery.Departments(departmentManager);

            departmentManager.Received().GetAll();
        }
    }
}