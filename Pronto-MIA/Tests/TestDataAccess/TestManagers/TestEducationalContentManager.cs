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
    public class TestEducationalContentManager
    {
        private readonly EducationalContentManager educationalContentManager;
        private readonly IFileManager fileManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestEducationalContentManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);

            this.fileManager = Substitute.For<IFileManager>();
            this.educationalContentManager = new EducationalContentManager(
                this.dbContext,
                Substitute.For<ILogger<EducationalContentManager>>(),
                this.fileManager);
        }

        [Fact]
        public async Task TestCreate()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.pdf");

            var educationalContent = await this.educationalContentManager
                .Create(
                    "Test",
                    "test",
                    file);

            await this.fileManager.Received().Create(
                IEducationalContentManager.FileDirectory,
                Arg.Any<string>(),
                file);
            educationalContent = await this.dbContext.EducationalContent
                .FirstOrDefaultAsync(
                    dP => dP.Id == educationalContent.Id);
            Assert.NotNull(educationalContent);
            Assert.Equal("Test", educationalContent.Title);
            Assert.Equal("test", educationalContent.Description);
            Assert.Equal(".pdf", educationalContent.FileExtension);

            this.dbContext.EducationalContent.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestCreateEmptyDescription()
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns("Important.pdf");

            var educationalContent = await this.educationalContentManager
                .Create(
                    "Test",
                    string.Empty,
                    file);

            await this.fileManager.Received().Create(
                IEducationalContentManager.FileDirectory,
                Arg.Any<string>(),
                file);
            educationalContent = await this.dbContext.EducationalContent
                .FirstOrDefaultAsync(
                    dP => dP.Id == educationalContent.Id);
            Assert.NotNull(educationalContent);
            Assert.Equal("Test", educationalContent.Title);
            Assert.Equal(".pdf", educationalContent.FileExtension);
            Assert.Equal(string.Empty, educationalContent.Description);

            this.dbContext.EducationalContent.Remove(educationalContent);
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
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();
            var oldFileUuid = educationalContent.FileUuid.ToString();
            var oldFileExtension = educationalContent.FileExtension;

            await this.educationalContentManager.Update(
                educationalContent.Id, null, null, file);

            await this.fileManager.Received().Create(
                IEducationalContentManager.FileDirectory,
                Arg.Any<string>(),
                file);
            this.fileManager.Received().Remove(
                IEducationalContentManager.FileDirectory,
                oldFileUuid.ToString(),
                oldFileExtension);
            Assert.NotEqual(
                oldFileUuid, educationalContent.FileUuid.ToString());
            Assert.Equal(
                this.GetSampleEducationalContent().Title,
                educationalContent.Title);
            Assert.Equal(
                this.GetSampleEducationalContent().Description,
                educationalContent.Description);

            this.dbContext.EducationalContent.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestUpdateTitle()
        {
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();
            var newTitle = "NewTitle";

            await this.educationalContentManager.Update(
                educationalContent.Id, newTitle, null, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newTitle, educationalContent.Title);
            Assert.Equal(
                this.GetSampleEducationalContent().Description,
                educationalContent.Description);
            Assert.Equal(
                this.GetSampleEducationalContent().FileUuid,
                educationalContent.FileUuid);
            Assert.Equal(
                this.GetSampleEducationalContent().FileExtension,
                educationalContent.FileExtension);

            this.dbContext.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescription()
        {
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();
            var newDescription = "NewDescription";

            await this.educationalContentManager.Update(
                educationalContent.Id, null, newDescription, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newDescription, educationalContent.Description);
            Assert.Equal(
                this.GetSampleEducationalContent().Title,
                educationalContent.Title);
            Assert.Equal(
                this.GetSampleEducationalContent().FileUuid,
                educationalContent.FileUuid);
            Assert.Equal(
                this.GetSampleEducationalContent().FileExtension,
                educationalContent.FileExtension);

            this.dbContext.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateTitleAndDescription()
        {
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();
            var newTitle = "newTitle";
            var newDescription = "newDescription";

            await this.educationalContentManager.Update(
                educationalContent.Id,
                newTitle,
                newDescription,
                null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(newTitle, educationalContent.Title);
            Assert.Equal(newDescription, educationalContent.Description);
            Assert.Equal(
                this.GetSampleEducationalContent().FileUuid,
                educationalContent.FileUuid);
            Assert.Equal(
                this.GetSampleEducationalContent().FileExtension,
                educationalContent.FileExtension);

            this.dbContext.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateDescriptionEmptyString()
        {
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();
            var newDescription = string.Empty;

            await this.educationalContentManager.Update(
                educationalContent.Id, null, newDescription, null);

            await this.fileManager.DidNotReceiveWithAnyArgs().Create(
                default, default, default);
            Assert.Equal(
                this.GetSampleEducationalContent().Title,
                educationalContent.Title);
            Assert.Equal(
                this.GetSampleEducationalContent().FileExtension,
                educationalContent.FileExtension);
            Assert.Equal(
                this.GetSampleEducationalContent().FileUuid,
                educationalContent.FileUuid);
            Assert.Null(educationalContent.Description);

            this.dbContext.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidEducationalContent()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.educationalContentManager.Update(
                        -5, null, null, null));
            Assert.Equal(
                Error.EducationalContentNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestPublish()
        {
            var educationalContent = this.GetSampleEducationalContent();
            educationalContent.Published = false;
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();

            await this.educationalContentManager.Publish(educationalContent.Id);

            Assert.True(educationalContent.Published);

            this.dbContext.EducationalContent.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDoublePublish()
        {
            var educationalContent = this.GetSampleEducationalContent();
            educationalContent.Published = true;
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();

            await this.educationalContentManager.Publish(educationalContent.Id);

            Assert.True(educationalContent.Published);

            this.dbContext.EducationalContent.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestHide()
        {
            var educationalContent = this.GetSampleEducationalContent();
            educationalContent.Published = true;
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();

            await this.educationalContentManager.Hide(educationalContent.Id);

            Assert.False(educationalContent.Published);

            this.dbContext.EducationalContent.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestDoubleHide()
        {
            var educationalContent = this.GetSampleEducationalContent();
            educationalContent.Published = false;
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();

            await this.educationalContentManager.Hide(educationalContent.Id);

            Assert.False(educationalContent.Published);

            this.dbContext.EducationalContent.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestRemove()
        {
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();

            await this.educationalContentManager.Remove(educationalContent.Id);

            this.fileManager.Received().Remove(
                IEducationalContentManager.FileDirectory,
                Guid.Empty.ToString(),
                string.Empty);
            Assert.Null(
                await this.dbContext.EducationalContent
                    .FirstOrDefaultAsync(dP => dP.Id == educationalContent.Id));

            this.fileManager.ClearReceivedCalls();
        }

        [Fact]
        public async Task TestRemoveInvalid()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.educationalContentManager.Remove(int.MaxValue));
            Assert.Equal(
                Error.EducationalContentNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestGetAllSingle()
        {
            var educationalContent = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();

            var educationalContents = this.educationalContentManager.GetAll();
            Assert.Equal(1, await educationalContents.CountAsync());

            this.dbContext.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllMultiple()
        {
            var educationalContent = this.GetSampleEducationalContent();
            var educationalContent2 = this.GetSampleEducationalContent();
            this.dbContext.EducationalContent.Add(educationalContent);
            this.dbContext.EducationalContent.Add(educationalContent2);
            await this.dbContext.SaveChangesAsync();

            var educationalContents = this.educationalContentManager.GetAll();
            Assert.Equal(2, await educationalContents.CountAsync());

            this.dbContext.Remove(educationalContent);
            this.dbContext.Remove(educationalContent2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.EducationalContent.RemoveRange(
                this.dbContext.EducationalContent);
            await this.dbContext.SaveChangesAsync();

            var educationalContents = this.educationalContentManager.GetAll();
            Assert.Empty(educationalContents);

            TestDataProvider.InsertTestData(this.dbContext);
        }

        private EducationalContent GetSampleEducationalContent()
        {
            return new EducationalContent(
                "Title",
                "Description",
                Guid.Empty,
                string.Empty);
        }
    }
}