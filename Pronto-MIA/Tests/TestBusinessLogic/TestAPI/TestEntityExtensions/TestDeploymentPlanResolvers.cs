namespace Tests.TestBusinessLogic.TestAPI.TestEntityExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestDeploymentPlanResolvers
    {
        private readonly ProntoMiaDbContext dbContext;

        private readonly DeploymentPlanResolvers
            deploymentPlanResolvers;

        public TestDeploymentPlanResolvers()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.deploymentPlanResolvers =
                new DeploymentPlanResolvers();
        }

        [Fact]
        public void TestCreatesLink()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            var httpContextAccessor =
                Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext
                .Request.Host = new HostString("localhost");
            httpContextAccessor.HttpContext
                .Request.PathBase = new PathString("/path");
            var myConfiguration = new Dictionary<string, string>
            {
                { "StaticFiles:ENDPOINT", "static" },
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration).Build();
            var uri = this.deploymentPlanResolvers
                .Link(deploymentPlan, configuration, httpContextAccessor);
            var expectedPath =
                "https://localhost/path/static/deployment_plans/"
                    + deploymentPlan.FileUuid
                    + deploymentPlan.FileExtension;
            Assert.Equal(expectedPath, uri.AbsoluteUri);
        }

        private DeploymentPlan GetSampleDeploymentPlan()
        {
            return new DeploymentPlan(
                DateTime.MinValue,
                DateTime.MaxValue,
                Guid.NewGuid(),
                ".pdf");
        }
    }
}