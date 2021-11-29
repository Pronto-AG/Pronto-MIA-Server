#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
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
        public async Task TestDepartmentsUnlimited()
        {
            var acl = new AccessControlList(-1) { CanViewDepartments = true };
            var user = await QueryTestHelpers
                .CreateUserWithAcl(
                    this.dbContext, "Eric", acl);
            var userState = new ApiUserState(user);
            var departmentManager =
                Substitute.For<IDepartmentManager>();
            departmentManager.GetAll().Returns(
                this.dbContext.Departments);
            var departmentCount = await this.dbContext.Departments.CountAsync();

            var result =
                this.departmentQuery.Departments(departmentManager, userState);

            departmentManager.Received().GetAll();
            Assert.Equal(departmentCount, await result.CountAsync());

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDepartmentsLimited()
        {
            var user = await QueryTestHelpers
                .CreateUserWithAcl(
                    this.dbContext, "Eric");
            var list = new List<Department>();
            list.Add(await this.dbContext.Departments.FirstAsync());
            user.Departments = list;
            var userState = new ApiUserState(user);
            var departmentManager =
                Substitute.For<IDepartmentManager>();
            departmentManager.GetAll().Returns(
                this.dbContext.Departments);

            var result =
                this.departmentQuery.Departments(departmentManager, userState);

            departmentManager.Received().GetAll();
            Assert.Equal(1, result.Count());

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }
    }
}