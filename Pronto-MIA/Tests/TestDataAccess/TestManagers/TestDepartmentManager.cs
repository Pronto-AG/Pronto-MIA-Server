namespace Tests.TestDataAccess.TestManagers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestDepartmentManager
    {
        private readonly DepartmentManager departmentManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestDepartmentManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);

            this.departmentManager = new DepartmentManager(
                this.dbContext,
                Substitute.For<ILogger<DepartmentManager>>());
        }

        [Fact]
        public async Task TestCreate()
        {
            var departmentName = "HR";

            var departmentResult = await this.departmentManager
                .Create(departmentName);

            var department = await this.dbContext.Departments
                .FirstOrDefaultAsync(
                    d => d.Name == departmentName);
            Assert.NotNull(department);
            Assert.NotEqual(0, departmentResult.Id);

            this.dbContext.Departments.Remove(departmentResult);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestCreateExistingName()
        {
            var departmentName = "Administration";
            var countBefore = await this.dbContext.Departments.CountAsync();

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.departmentManager
                    .Create(departmentName);
            });

            Assert.Equal(
                Error.DepartmentAlreadyExists.ToString(),
                error.Errors[0].Code);
            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task TestUpdate()
        {
            var departmentName = "HR";
            var newDepartmentName = "Production";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();

            await this.departmentManager.Update(
                department.Id, newDepartmentName);

            var departmentUpdated = await this.dbContext.Departments
                .FindAsync(department.Id);
            Assert.Equal(newDepartmentName, departmentUpdated.Name);

            this.dbContext.Departments.Remove(department);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateToExisting()
        {
            var departmentName = "HR";
            var newDepartmentName = "Administration";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();
            var countBefore = await this.dbContext.Departments.CountAsync();

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.departmentManager.Update(
                    department.Id, newDepartmentName);
            });

            Assert.Equal(
                Error.DepartmentAlreadyExists.ToString(),
                error.Errors[0].Code);
            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);

            this.dbContext.Departments.Remove(department);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateOwnName()
        {
            var departmentName = "HR";
            var newDepartmentName = "HR";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();
            var countBefore = await this.dbContext.Departments.CountAsync();

            await this.departmentManager.Update(
                    department.Id, newDepartmentName);

            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);

            this.dbContext.Departments.Remove(department);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateNullName()
        {
            var departmentName = "HR";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();
            var countBefore = await this.dbContext.Departments.CountAsync();

            await this.departmentManager.Update(
                department.Id, null);

            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);
            var departmentAfter = await this.dbContext.Departments
                .FindAsync(department.Id);
            Assert.Equal(departmentName, departmentAfter.Name);

            this.dbContext.Departments.Remove(department);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidId()
        {
            var newDepartmentName = "HR";
            var countBefore = await this.dbContext.Departments.CountAsync();

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.departmentManager.Update(
                    -5, newDepartmentName);
            });

            Assert.Equal(
                Error.DepartmentNotFound.ToString(),
                error.Errors[0].Code);
            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task TestRemove()
        {
            var departmentName = "HR";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();
            var countBefore = await this.dbContext.Departments.CountAsync();

            await this.departmentManager.Remove(
                department.Id);

            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore - 1, countAfter);
            var departmentAfter = await this.dbContext.Departments
                .FindAsync(department.Id);
            Assert.Null(departmentAfter);
        }

        [Fact]
        public async Task TestRemoveInvalidId()
        {
            var countBefore = await this.dbContext.Departments.CountAsync();

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.departmentManager.Remove(-5);
            });

            Assert.Equal(
                Error.DepartmentNotFound.ToString(),
                error.Errors[0].Code);
            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public async Task TestRemoveWithUser()
        {
            var departmentName = "HR";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();
            var user = await this.AddUserToDb();
            user.DepartmentId = department.Id;
            this.dbContext.Update(user);
            await this.dbContext.SaveChangesAsync();
            var countBefore = await this.dbContext.Departments.CountAsync();

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.departmentManager.Remove(department.Id);
            });

            Assert.Equal(
                Error.DepartmentInUse.ToString(),
                error.Errors[0].Code);
            var countAfter = await this.dbContext.Departments.CountAsync();
            Assert.Equal(countBefore, countAfter);

            await this.RemoveUserFromDb(user);
            this.dbContext.Departments.Remove(department);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllSingle()
        {
            var departmentName = "HR";
            var department = new Department(departmentName);
            this.dbContext.Departments.Add(department);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.departmentManager.GetAll();
            Assert.Equal(3, await deploymentPlans.CountAsync());

            this.dbContext.Remove(department);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllMultiple()
        {
            var departmentName1 = "HR";
            var department1 = new Department(departmentName1);
            var departmentName2 = "Production";
            var department2 = new Department(departmentName2);
            this.dbContext.Departments.Add(department1);
            this.dbContext.Departments.Add(department2);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.departmentManager.GetAll();
            Assert.Equal(4, await deploymentPlans.CountAsync());

            this.dbContext.Remove(department1);
            this.dbContext.Remove(department2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.Departments.RemoveRange(
                this.dbContext.Departments);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.departmentManager.GetAll();
            Assert.Empty(deploymentPlans);

            TestDataProvider.InsertTestData(this.dbContext);
        }

        private async Task<User> AddUserToDb()
        {
            var user = new User(
                "Bob2",
                new byte[5],
                string.Empty,
                string.Empty);
            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();
            return user;
        }

        private async Task RemoveUserFromDb(User user)
        {
            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }
    }
}