namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Xunit;

    public class TestDepartmentMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly DepartmentMutation departmentMutation;

        public TestDepartmentMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.departmentMutation = new DepartmentMutation();
        }

        [Fact]
        public async void TestCreateDepartment()
        {
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.departmentMutation.CreateDepartment(
                departmentManager,
                "HR");

            await departmentManager.Received()
                .Create("HR");
        }

        [Fact]
        public async void TestUpdateDepartment()
        {
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.departmentMutation.UpdateDepartment(
                departmentManager,
                1,
                "HR");

            await departmentManager.Received()
                .Update(1, "HR");
        }

        [Fact]
        public async void TestRemoveDepartment()
        {
            var departmentManager =
                Substitute.For<IDepartmentManager>();

            await this.departmentMutation.RemoveDepartment(
                departmentManager,
                5);

            await departmentManager.Received()
                .Remove(5);
        }
    }
}