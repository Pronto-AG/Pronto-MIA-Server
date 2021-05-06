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

    [SuppressMessage(
        "Menees.Analyzers",
        "MEN005",
        Justification = "Test file may be more than 300 lines.")]
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
                DateTime.MaxValue,
                "Hello World");

            await this.fileManager.Received().Create(
                IDeploymentPlanManager.FileDirectory,
                Arg.Any<string>(),
                file);
            var deploymentPlan = await this.dbContext.DeploymentPlans
                .FirstOrDefaultAsync(
                    dP => dP.AvailableUntil == DateTime.MaxValue);
            Assert.NotNull(deploymentPlan);
            Assert.Equal(DateTime.MinValue, deploymentPlan.AvailableFrom);
            Assert.Equal("Hello World", deploymentPlan.Description);

            this.dbContext.DeploymentPlans.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestCreateEmptyDescription()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.pdf");

            await this.deploymentPlanManager.Create(
                file,
                DateTime.MinValue,
                DateTime.MaxValue,
                string.Empty);

            await this.fileManager.Received().Create(
                IDeploymentPlanManager.FileDirectory,
                Arg.Any<string>(),
                file);
            var deploymentPlan = await this.dbContext.DeploymentPlans
                .FirstOrDefaultAsync(
                    dP => dP.AvailableUntil == DateTime.MaxValue);
            Assert.NotNull(deploymentPlan);
            Assert.Equal(DateTime.MinValue, deploymentPlan.AvailableFrom);
            Assert.Null(deploymentPlan.Description);

            this.dbContext.DeploymentPlans.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestCreateTimeError()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.pdf");

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.deploymentPlanManager.Create(
                    file,
                    DateTime.MaxValue,
                    DateTime.MinValue,
                    null);
            });

            Assert.Equal(
                Error.DeploymentPlanImpossibleTime.ToString(),
                error.Errors[0].Code);
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
                deploymentPlan.Id, file, null, null, null);

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
            Assert.Equal(
                this.GetSampleDeploymentPlan().Description,
                deploymentPlan.Description);

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
                deploymentPlan.Id, null, newAvailableFrom, null, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newAvailableFrom, deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableUntil,
                deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);
            Assert.Equal(
                this.GetSampleDeploymentPlan().Description,
                deploymentPlan.Description);

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
                deploymentPlan.Id, null, null, newAvailableUntil, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newAvailableUntil, deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableFrom,
                deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);
            Assert.Equal(
                this.GetSampleDeploymentPlan().Description,
                deploymentPlan.Description);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateAvailableBoth()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newAvailableFrom = DateTime.UtcNow;
            var newAvailableUntil = DateTime.UtcNow.AddHours(2);

            await this.deploymentPlanManager.Update(
                deploymentPlan.Id,
                null,
                newAvailableFrom,
                newAvailableUntil,
                null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newAvailableFrom, deploymentPlan.AvailableFrom);
            Assert.Equal(newAvailableUntil, deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);
            Assert.Equal(
                this.GetSampleDeploymentPlan().Description,
                deploymentPlan.Description);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescription()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newDescription = "New";

            await this.deploymentPlanManager.Update(
                deploymentPlan.Id, null, null, null, newDescription);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableFrom,
                deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableUntil,
                deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);
            Assert.Equal(newDescription, deploymentPlan.Description);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescriptionEmptyString()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newDescription = string.Empty;

            await this.deploymentPlanManager.Update(
                deploymentPlan.Id, null, null, null, newDescription);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableFrom,
                deploymentPlan.AvailableFrom);
            Assert.Equal(
                this.GetSampleDeploymentPlan().AvailableUntil,
                deploymentPlan.AvailableUntil);
            Assert.Equal(
                this.GetSampleDeploymentPlan().FileUuid,
                deploymentPlan.FileUuid);
            Assert.Null(deploymentPlan.Description);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidTimeBoth()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newAvailableFrom = DateTime.MaxValue;
            var newAvailableUntil = DateTime.MinValue;

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.deploymentPlanManager.Update(
                    deploymentPlan.Id,
                    null,
                    newAvailableFrom,
                    newAvailableUntil,
                    null);
            });

            Assert.Equal(
                Error.DeploymentPlanImpossibleTime.ToString(),
                error.Errors[0].Code);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidTimeFrom()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newAvailableFrom = DateTime.MaxValue;

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.deploymentPlanManager.Update(
                    deploymentPlan.Id,
                    null,
                    newAvailableFrom,
                    null,
                    null);
            });

            Assert.Equal(
                Error.DeploymentPlanImpossibleTime.ToString(),
                error.Errors[0].Code);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidTimeUntil()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
            var newAvailableUntil = DateTime.MinValue;

            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.deploymentPlanManager.Update(
                    deploymentPlan.Id,
                    null,
                    null,
                    newAvailableUntil,
                    null);
            });

            Assert.Equal(
                Error.DeploymentPlanImpossibleTime.ToString(),
                error.Errors[0].Code);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidPlan()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.deploymentPlanManager.Update(
                        -5, null, null, DateTime.UtcNow, null));
            Assert.Equal(
                Error.DeploymentPlanNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestHide()
        {
            var deploymentPlan = this.GetSampleDeploymentPlan();
            deploymentPlan.Published = true;
            this.dbContext.DeploymentPlans.Add(deploymentPlan);
            await this.dbContext.SaveChangesAsync();

            await this.deploymentPlanManager.Hide(deploymentPlan.Id);

            Assert.False(deploymentPlan.Published);
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
            Assert.Equal(3, await deploymentPlans.CountAsync());

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
            Assert.Equal(4, await deploymentPlans.CountAsync());

            this.dbContext.Remove(deploymentPlan);
            this.dbContext.Remove(deploymentPlan2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.DeploymentPlans.RemoveRange(
                this.dbContext.DeploymentPlans);
            await this.dbContext.SaveChangesAsync();

            var deploymentPlans = this.deploymentPlanManager.GetAll();
            Assert.Empty(deploymentPlans);

            TestDataProvider.InsertTestData(this.dbContext);
        }

        private DeploymentPlan GetSampleDeploymentPlan()
        {
            return new DeploymentPlan(
                DateTime.MinValue,
                DateTime.MaxValue,
                Guid.Empty,
                string.Empty,
                "Hello World");
        }
    }
}