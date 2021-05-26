#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using NSubstitute;
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
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
            this.deploymentPlanQuery = new DeploymentPlanQuery();
        }

        [Fact]
        public void TestDeploymentPlans()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            this.deploymentPlanQuery.DeploymentPlans(deploymentPlanManager);

            deploymentPlanManager.Received().GetAll();
        }
    }
}