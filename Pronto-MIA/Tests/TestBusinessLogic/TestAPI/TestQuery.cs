#nullable enable
namespace Tests.TestBusinessLogic.TestAPI
{
    using System;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly Query query;

        public TestQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);
            this.query = new Query();
        }

        [Fact]
        public async void TestAuthentication()
        {
            var userManager = Substitute.For<IUserManager>();

            await this.query.Authenticate(userManager, "Bob", "Hello World");

            await userManager.Received().Authenticate("Bob", "Hello World");
        }

        [Fact]
        public void TestDeploymentPlans()
        {
            var deploymentPlanManager =
                Substitute.For<IDeploymentPlanManager>();

            this.query.DeploymentPlans(deploymentPlanManager);

            deploymentPlanManager.Received().GetAll();
        }
    }
}