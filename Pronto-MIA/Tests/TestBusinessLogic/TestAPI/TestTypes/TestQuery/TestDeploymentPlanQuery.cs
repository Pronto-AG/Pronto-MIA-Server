#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
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
        public async Task TestDeploymentPlans()
        {
            var user = await QueryTestHelpers
                .CreateUserWithAcl(this.dbContext, "Fredi");
            var userState = new ApiUserState(user);
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            this.deploymentPlanQuery.DeploymentPlans(
               deploymentPlanManager, userState);

            deploymentPlanManager.Received().GetAll();
        }
    }
}