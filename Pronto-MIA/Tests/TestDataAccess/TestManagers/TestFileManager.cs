namespace Tests.TestDataAccess.TestManagers
{
    using System.IO;
    using System.IO.Abstractions;
    using HotChocolate.Execution;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ClearExtensions;
    using Pronto_MIA.DataAccess.Managers;
    using Xunit;
    using IFile = HotChocolate.Types.IFile;

    public class TestFileManager
    {
        private readonly FileManager fileManager;
        private readonly IFileSystem fileSystem;
        private readonly IFile testFile;

        public TestFileManager()
        {
            this.fileSystem = Substitute.For<IFileSystem>();

            this.fileManager = new FileManager(
                TestHelpers.TestConfiguration,
                Substitute.For<ILogger<FileManager>>(),
                this.fileSystem);

            this.testFile = Substitute.For<IFile>();
            this.testFile.Name.Returns("Test.pdf");

            this.testFile.OpenReadStream().Returns(new MemoryStream());
        }

        [Fact]
        public async void TestCreate()
        {
            this.fileSystem.File.Create(default)
                .ReturnsForAnyArgs(new MemoryStream());

            await this.fileManager.Create(
                "dir",
                "Test",
                this.testFile);

            this.fileSystem.Directory.Received().CreateDirectory("/empty/dir");
            this.fileSystem.File.Received().Create("/empty/dir/Test.pdf");

            this.fileSystem.ClearSubstitute();
            this.testFile.ClearReceivedCalls();
        }

        [Fact]
        public async void TestCreateError()
        {
            var error = await Assert.ThrowsAsync<QueryException>(async () =>
            {
                await this.fileManager.Create(
                    "dir",
                    "Test",
                    this.testFile);
            });

            Assert.Equal(
                Pronto_MIA.DataAccess.Error.FileOperationError.ToString(),
                error.Errors[0].Code);

            this.fileSystem.ClearReceivedCalls();
        }

        [Fact]
        public void TestRemove()
        {
            this.fileManager.Remove("dir", "Test", ".pdf");

            this.fileSystem.File.Received().Delete("/empty/dir/Test.pdf");

            this.fileSystem.ClearReceivedCalls();
        }

        [Fact]
        public void TestRemoveError()
        {
            this.fileSystem.File
                .When(x => x.Delete(Arg.Any<string>()))
                .Do(x => throw new IOException());

            var error = Assert.Throws<QueryException>(() =>
            {
                this.fileManager.Remove("dir", "Test", ".pdf");
            });

            Assert.Equal(
                Pronto_MIA.DataAccess.Error.FileOperationError.ToString(),
                error.Errors[0].Code);

            this.fileSystem.ClearSubstitute();
        }
    }
}