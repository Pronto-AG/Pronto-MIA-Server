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
    public class TestExternalNewsManager
    {
        private readonly ExternalNewsManager externalNewsManager;
        private readonly IFileManager fileManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestExternalNewsManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);

            this.fileManager = Substitute.For<IFileManager>();
            this.externalNewsManager = new ExternalNewsManager(
                this.dbContext,
                Substitute.For<ILogger<ExternalNewsManager>>(),
                this.fileManager);
        }

        [Fact]
        public async Task TestCreate()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.png");

            var externalNews = await this.externalNewsManager.Create(
                "Title",
                "Description",
                DateTime.MinValue,
                file);

            await this.fileManager.Received().Create(
                IExternalNewsManager.FileDirectory,
                Arg.Any<string>(),
                file);
            externalNews = await this.dbContext.ExternalNews
                .FirstOrDefaultAsync(
                    dP => dP.Id == externalNews.Id);
            Assert.NotNull(externalNews);
            Assert.Equal(DateTime.MinValue, externalNews.AvailableFrom);
            Assert.Equal("Title", externalNews.Title);
            Assert.Equal("Description", externalNews.Description);

            this.dbContext.ExternalNews.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestCreateEmptyDescription()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.png");

            var externalNews = await this.externalNewsManager.Create(
                "Title",
                string.Empty,
                DateTime.MinValue,
                file);

            await this.fileManager.Received().Create(
                IExternalNewsManager.FileDirectory,
                Arg.Any<string>(),
                file);
            externalNews = await this.dbContext.ExternalNews
                .FirstOrDefaultAsync(
                    dP => dP.Id == externalNews.Id);
            Assert.NotNull(externalNews);
            Assert.Equal(DateTime.MinValue, externalNews.AvailableFrom);
            Assert.Equal("Title", externalNews.Title);
            Assert.Equal(string.Empty, externalNews.Description);

            this.dbContext.ExternalNews.Remove(externalNews);
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
            file.Name.Returns("Important.png");
            var externalNews = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();
            var oldFileUuid = externalNews.FileUuid.ToString();
            var oldFileExtension = externalNews.FileExtension;

            await this.externalNewsManager.Update(
                externalNews.Id, null, null, null, file);

            await this.fileManager.Received().Create(
                IExternalNewsManager.FileDirectory, Arg.Any<string>(), file);
            this.fileManager.Received().Remove(
                IExternalNewsManager.FileDirectory,
                oldFileUuid.ToString(),
                oldFileExtension);
            Assert.NotEqual(
                oldFileUuid, externalNews.FileUuid.ToString());
            Assert.Equal(
                this.GetSampleExternalNews().AvailableFrom,
                externalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleExternalNews().Title,
                externalNews.Title);
            Assert.Equal(
                this.GetSampleExternalNews().Description,
                externalNews.Description);

            this.dbContext.ExternalNews.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestUpdateAvailableFrom()
        {
            var externalNews = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();
            var newAvailableFrom = DateTime.UtcNow;

            await this.externalNewsManager.Update(
                externalNews.Id, null, null, newAvailableFrom, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newAvailableFrom, externalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleExternalNews().Title,
                externalNews.Title);
            Assert.Equal(
                this.GetSampleExternalNews().FileUuid,
                externalNews.FileUuid);
            Assert.Equal(
                this.GetSampleExternalNews().Description,
                externalNews.Description);

            this.dbContext.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescription()
        {
            var externalNews = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();
            var newDescription = "New";

            await this.externalNewsManager.Update(
                externalNews.Id, null, newDescription, null, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleExternalNews().AvailableFrom,
                externalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleExternalNews().Title,
                externalNews.Title);
            Assert.Equal(
                this.GetSampleExternalNews().FileUuid,
                externalNews.FileUuid);
            Assert.Equal(newDescription, externalNews.Description);

            this.dbContext.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescriptionEmptyString()
        {
            var externalNews = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();
            var newDescription = string.Empty;

            await this.externalNewsManager.Update(
                externalNews.Id, null, newDescription, null, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleExternalNews().AvailableFrom,
                externalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleExternalNews().Title,
                externalNews.Title);
            Assert.Equal(
                this.GetSampleExternalNews().FileUuid,
                externalNews.FileUuid);
            Assert.Null(externalNews.Description);

            this.dbContext.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidExternalNews()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.externalNewsManager.Update(
                        -5, null, null, DateTime.UtcNow, null));
            Assert.Equal(
                Error.ExternalNewsNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestPublish()
        {
            var externalNews = this.GetSampleExternalNews();
            externalNews.Published = false;
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();

            await this.externalNewsManager.Publish(externalNews.Id);

            Assert.True(externalNews.Published);

            this.dbContext.ExternalNews.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDoublePublish()
        {
            var externalNews = this.GetSampleExternalNews();
            externalNews.Published = true;
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();

            await this.externalNewsManager.Publish(externalNews.Id);

            Assert.True(externalNews.Published);

            this.dbContext.ExternalNews.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestHide()
        {
            var externalNews = this.GetSampleExternalNews();
            externalNews.Published = true;
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();

            await this.externalNewsManager.Hide(externalNews.Id);

            Assert.False(externalNews.Published);

            this.dbContext.ExternalNews.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDoubleHide()
        {
            var externalNews = this.GetSampleExternalNews();
            externalNews.Published = false;
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();

            await this.externalNewsManager.Hide(externalNews.Id);

            Assert.False(externalNews.Published);

            this.dbContext.ExternalNews.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRemove()
        {
            var externalNews = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();

            await this.externalNewsManager.Remove(externalNews.Id);

            this.fileManager.Received().Remove(
                IExternalNewsManager.FileDirectory,
                Guid.Empty.ToString(),
                string.Empty);
            Assert.Null(
                await this.dbContext.ExternalNews
                    .FirstOrDefaultAsync(dP => dP.Id == externalNews.Id));

            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestRemoveInvalid()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.externalNewsManager.Remove(int.MaxValue));
            Assert.Equal(
                Error.ExternalNewsNotFound.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestGetAllSingle()
        {
            var externalNews = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();

            var externalNewss = this.externalNewsManager.GetAll();
            Assert.Equal(1, await externalNewss.CountAsync());

            this.dbContext.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllMultiple()
        {
            var externalNews = this.GetSampleExternalNews();
            var externalNews2 = this.GetSampleExternalNews();
            this.dbContext.ExternalNews.Add(externalNews);
            this.dbContext.ExternalNews.Add(externalNews2);
            await this.dbContext.SaveChangesAsync();

            var externalNewss = this.externalNewsManager.GetAll();
            Assert.Equal(2, await externalNewss.CountAsync());

            this.dbContext.Remove(externalNews);
            this.dbContext.Remove(externalNews2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.ExternalNews.RemoveRange(
                this.dbContext.ExternalNews);
            await this.dbContext.SaveChangesAsync();

            var externalNewss = this.externalNewsManager.GetAll();
            Assert.Empty(externalNewss);

            TestDataProvider.InsertTestData(this.dbContext);
        }

        private ExternalNews GetSampleExternalNews()
        {
            return new ExternalNews(
                "Title",
                "Description",
                DateTime.MinValue,
                Guid.Empty,
                string.Empty);
        }
    }
}