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
    public class TestInternalNewsManager
    {
        private readonly InternalNewsManager internalNewsManager;
        private readonly IFileManager fileManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestInternalNewsManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);

            this.fileManager = Substitute.For<IFileManager>();
            this.internalNewsManager = new InternalNewsManager(
                this.dbContext,
                Substitute.For<ILogger<InternalNewsManager>>(),
                this.fileManager);
        }

        [Fact]
        public async Task TestCreate()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.png");

            var internalNews = await this.internalNewsManager.Create(
                "Title",
                "Description",
                DateTime.MinValue,
                file);

            await this.fileManager.Received().Create(
                IInternalNewsManager.FileDirectory,
                Arg.Any<string>(),
                file);
            internalNews = await this.dbContext.InternalNews
                .FirstOrDefaultAsync(
                    dP => dP.Id == internalNews.Id);
            Assert.NotNull(internalNews);
            Assert.Equal(DateTime.MinValue, internalNews.AvailableFrom);
            Assert.Equal("Title", internalNews.Title);
            Assert.Equal("Description", internalNews.Description);

            this.dbContext.InternalNews.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestCreateEmptyDescription()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.png");

            var internalNews = await this.internalNewsManager.Create(
                "Title",
                string.Empty,
                DateTime.MinValue,
                file);

            await this.fileManager.Received().Create(
                IInternalNewsManager.FileDirectory,
                Arg.Any<string>(),
                file);
            internalNews = await this.dbContext.InternalNews
                .FirstOrDefaultAsync(
                    dP => dP.Id == internalNews.Id);
            Assert.NotNull(internalNews);
            Assert.Equal(DateTime.MinValue, internalNews.AvailableFrom);
            Assert.Equal("Title", internalNews.Title);
            Assert.Equal(string.Empty, internalNews.Description);

            this.dbContext.InternalNews.Remove(internalNews);
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
            var internalNews = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();
            var oldFileUuid = internalNews.FileUuid.ToString();
            var oldFileExtension = internalNews.FileExtension;

            await this.internalNewsManager.Update(
                internalNews.Id, null, null, null, file);

            await this.fileManager.Received().Create(
                IInternalNewsManager.FileDirectory, Arg.Any<string>(), file);
            this.fileManager.Received().Remove(
                IInternalNewsManager.FileDirectory,
                oldFileUuid.ToString(),
                oldFileExtension);
            Assert.NotEqual(
                oldFileUuid, internalNews.FileUuid.ToString());
            Assert.Equal(
                this.GetSampleInternalNews().AvailableFrom,
                internalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleInternalNews().Title,
                internalNews.Title);
            Assert.Equal(
                this.GetSampleInternalNews().Description,
                internalNews.Description);

            this.dbContext.InternalNews.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestUpdateAvailableFrom()
        {
            var internalNews = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();
            var newAvailableFrom = DateTime.UtcNow;

            await this.internalNewsManager.Update(
                internalNews.Id, null, null, newAvailableFrom, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newAvailableFrom, internalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleInternalNews().Title,
                internalNews.Title);
            Assert.Equal(
                this.GetSampleInternalNews().FileUuid,
                internalNews.FileUuid);
            Assert.Equal(
                this.GetSampleInternalNews().Description,
                internalNews.Description);

            this.dbContext.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescription()
        {
            var internalNews = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();
            var newDescription = "New";

            await this.internalNewsManager.Update(
                internalNews.Id, null, newDescription, null, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleInternalNews().AvailableFrom,
                internalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleInternalNews().Title,
                internalNews.Title);
            Assert.Equal(
                this.GetSampleInternalNews().FileUuid,
                internalNews.FileUuid);
            Assert.Equal(newDescription, internalNews.Description);

            this.dbContext.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescriptionEmptyString()
        {
            var internalNews = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();
            var newDescription = string.Empty;

            await this.internalNewsManager.Update(
                internalNews.Id, null, newDescription, null, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleInternalNews().AvailableFrom,
                internalNews.AvailableFrom);
            Assert.Equal(
                this.GetSampleInternalNews().Title,
                internalNews.Title);
            Assert.Equal(
                this.GetSampleInternalNews().FileUuid,
                internalNews.FileUuid);
            Assert.Null(internalNews.Description);

            this.dbContext.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidInternalNews()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.internalNewsManager.Update(
                        -5, null, null, DateTime.UtcNow, null));
            Assert.Equal(
                Error.InternalNewsNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestPublish()
        {
            var internalNews = this.GetSampleInternalNews();
            internalNews.Published = false;
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();

            await this.internalNewsManager.Publish(internalNews.Id);

            Assert.True(internalNews.Published);

            this.dbContext.InternalNews.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDoublePublish()
        {
            var internalNews = this.GetSampleInternalNews();
            internalNews.Published = true;
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();

            await this.internalNewsManager.Publish(internalNews.Id);

            Assert.True(internalNews.Published);

            this.dbContext.InternalNews.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestHide()
        {
            var internalNews = this.GetSampleInternalNews();
            internalNews.Published = true;
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();

            await this.internalNewsManager.Hide(internalNews.Id);

            Assert.False(internalNews.Published);

            this.dbContext.InternalNews.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDoubleHide()
        {
            var internalNews = this.GetSampleInternalNews();
            internalNews.Published = false;
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();

            await this.internalNewsManager.Hide(internalNews.Id);

            Assert.False(internalNews.Published);

            this.dbContext.InternalNews.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRemove()
        {
            var internalNews = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();

            await this.internalNewsManager.Remove(internalNews.Id);

            this.fileManager.Received().Remove(
                IInternalNewsManager.FileDirectory,
                Guid.Empty.ToString(),
                string.Empty);
            Assert.Null(
                await this.dbContext.InternalNews
                    .FirstOrDefaultAsync(dP => dP.Id == internalNews.Id));

            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestRemoveInvalid()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.internalNewsManager.Remove(int.MaxValue));
            Assert.Equal(
                Error.InternalNewsNotFound.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestGetAllSingle()
        {
            var internalNews = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();

            var internalNewss = this.internalNewsManager.GetAll();
            Assert.Equal(1, await internalNewss.CountAsync());

            this.dbContext.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllMultiple()
        {
            var internalNews = this.GetSampleInternalNews();
            var internalNews2 = this.GetSampleInternalNews();
            this.dbContext.InternalNews.Add(internalNews);
            this.dbContext.InternalNews.Add(internalNews2);
            await this.dbContext.SaveChangesAsync();

            var internalNewss = this.internalNewsManager.GetAll();
            Assert.Equal(2, await internalNewss.CountAsync());

            this.dbContext.Remove(internalNews);
            this.dbContext.Remove(internalNews2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.InternalNews.RemoveRange(
                this.dbContext.InternalNews);
            await this.dbContext.SaveChangesAsync();

            var internalNewss = this.internalNewsManager.GetAll();
            Assert.Empty(internalNewss);

            TestDataProvider.InsertTestData(this.dbContext);
        }

        private InternalNews GetSampleInternalNews()
        {
            return new InternalNews(
                "Title",
                "Description",
                DateTime.MinValue,
                Guid.Empty,
                string.Empty);
        }
    }
}