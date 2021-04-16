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

    public class TestDeploymentPlanManager
    {
        private readonly DeploymentPlanManager deploymentPlanManager;
        private readonly IFileManager fileManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestDeploymentPlanManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
            TestDataProvider.InsertTestData(this.dbContext);

            this.fileManager = Substitute.For<IFileManager>();
            this.deploymentPlanManager = new DeploymentPlanManager(
                this.dbContext,
                Substitute.For<ILogger<DeploymentPlanManager>>(),
                this.fileManager);
        }

        [Fact]
        public async Task TestCreate()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.pdf");

            await this.deploymentPlanManager.Create(
                file,
                DateTime.MinValue,
                DateTime.MaxValue);

            await this.fileManager.Received().Create(
                IDeploymentPlanManager.FileDirectory,
                Arg.Any<string>(),
                file);
            var deploymentPlan = await this.dbContext.DeploymentPlans
                .FirstOrDefaultAsync(
                    dP => dP.AvailableUntil == DateTime.MaxValue);
            Assert.NotNull(deploymentPlan);
            Assert.Equal(DateTime.MinValue, deploymentPlan.AvailableFrom);

            this.dbContext.DeploymentPlans.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [SuppressMessage(
            "Menees.Analyzers",
            "MEN003",
            Justification = "Update has much to arrange.")]
        [Fact]
        public async Task TestUpdateFile()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.pdf");
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var oldFileUuid = deploymentPlan.FileUuid.ToString();

            await this.deploymentPlanManager.Update(
                deploymentPlan.Id, file, null, null);

            await this.fileManager.Received().Create(
                IDeploymentPlanManager.FileDirectory, Arg.Any<string>(), file);
            this.fileManager.Received().Remove(
                IDeploymentPlanManager.FileDirectory,
                deploymentPlan.FileUuid.ToString(),
                deploymentPlan.FileExtension);
            Assert.NotEqual(
                oldFileUuid, deploymentPlan.FileUuid.ToString());
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableFrom,
                deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableUntil,
                deploymentPlan.AvailableUntil);

            this.dbContext.DeploymentPlans.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestUpdateAvailableFrom()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newAvailableFrom = DateTime.UtcNow;

            await this.deploymentPlanManager.Update(
                deploymentPlan.Id, null, newAvailableFrom, null);

            await this.fileManager.DidNotReceive().Create(
                default, default, default);
            Assert.Equal(newAvailableFrom, deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableUntil,
                deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateAvailableUntil()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newAvailableUntil = DateTime.UtcNow;

            await this.deploymentPlanManager.Update(
                deploymentPlan.Id, null, null, newAvailableUntil);

            await this.fileManager.DidNotReceive().Create(
                default, default, default);
            Assert.Equal(newAvailableUntil, deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableFrom,
                deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidPlan()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.deploymentPlanManager.Update(
                        -5, null, null, DateTime.UtcNow));
            Assert.Equal(
                Error.DeploymentPlanNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestRemove()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();

            await this.deploymentPlanManager.Remove(deploymentPlan.Id);

            this.fileManager.Received().Remove(
                IDeploymentPlanManager.FileDirectory,
                Guid.Empty.ToString(),
                string.Empty);
            Assert.Null(
                await this.dbContext.DeploymentPlans
                    .FirstOrDefaultAsync(dP => dP.Id == deploymentPlan.Id));

            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestRemoveInvalid()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.deploymentPlanManager.Remove(int.MaxValue));
            Assert.Equal(
                Error.DeploymentPlanNotFound.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestGetAllSingle()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.deploymentPlanManager.GetAll();
            Assert.Equal(1, await deploymentPlans.CountAsync());

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllMultiple()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            var deploymentPlan2 = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            this.dbContext.DeploymentPlans.Add(deploymentPlan2);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.deploymentPlanManager.GetAll();
            Assert.Equal(2, await deploymentPlans.CountAsync());

            this.dbContext.Remove(deploymentPlan);
            this.dbContext.Remove(deploymentPlan2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public void TestGetAllEmpty()
        {
            var deploymentPlans = this.deploymentPlanManager.GetAll();
            Assert.Empty(deploymentPlans);
        }

        private DeploymentPlan GetSampleDeploymentPlan()
        {
            return new DeploymentPlan(
                DateTime.MinValue,
                DateTime.MaxValue,
                Guid.Empty,
                string.Empty);
        }
    }
}