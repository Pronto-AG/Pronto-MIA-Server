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

    public class TestDeploymentPlanQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly DeploymentPlanQuery deploymentPlanQuery;

        public TestDeploymentPlanQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.deploymentPlanQuery = new DeploymentPlanQuery();
        }

        [Fact]
        public async Task TestDeploymentPlansUnlimited()
        {
            var acl = new AccessControlList(-1)
            { CanViewDeploymentPlans = true };
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi", acl);
            var userState = new ApiUserState(user);
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            deploymentPlanManager.GetAll().Returns(
                this.dbContext.DeploymentPlans);
            var deploymentPlanCount =
                await this.dbContext.DeploymentPlans.CountAsync();

            var result = this.deploymentPlanQuery.DeploymentPlans(
               deploymentPlanManager, userState);

            deploymentPlanManager.Received().GetAll();
            Assert.Equal(deploymentPlanCount, await result.CountAsync());

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDeploymentPlansLimited()
        {
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi");
            var list = new List<Department>();
            list.Add(await this.dbContext.Departments
                    .SingleAsync(d => d.Name == "Finance"));
            user.Departments = list;
            var userState = new ApiUserState(user);
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();
            deploymentPlanManager.GetAll().Returns(
                this.dbContext.DeploymentPlans);
            var deploymentPlanCount =
                await this.dbContext.DeploymentPlans.CountAsync();

            var result = this.deploymentPlanQuery.DeploymentPlans(
                deploymentPlanManager, userState);

            deploymentPlanManager.Received().GetAll();
            Assert.Equal(2, await result.CountAsync());

            this.dbContext.Remove(user);
            await this.dbContext.SaveChangesAsync();
        }
    }
}