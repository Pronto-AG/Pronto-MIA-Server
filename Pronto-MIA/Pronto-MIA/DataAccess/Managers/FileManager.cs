namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using IFile = HotChocolate.Types.IFile;

    /// <summary>
    /// Class responsible for the lifecycle of a user within the application.
    /// </summary>
    public class FileManager : IFileManager
    {
        private readonly IConfiguration cfg;
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this user manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        /// <param name="fileSystem">The file system to be used by the manager
        /// in order to persist files.</param>
        public FileManager(
            IConfiguration cfg,
            ILogger<FileManager> logger,
            IFileSystem fileSystem)
        {
            this.cfg = cfg;
            this.logger = logger;
            this.fileSystem = fileSystem;
        }

        private string RootDirectory => this.cfg.GetValue<string>(
            "StaticFiles:ROOT_DIRECTORY");

        /// <inheritdoc/>
        public async Task
            Create(string subDirectory, string fileName, IFile file)
        {
            var ext = IFileManager.GetFileExtension(file);

            try
            {
                this.fileSystem.Directory.CreateDirectory(
                    this.GetDirectoryPath(subDirectory));
                var readStream = file.OpenReadStream();
                var writeStream = this.fileSystem.File.Create(
                    this.GetFilePath(subDirectory, fileName, ext));
                await readStream.CopyToAsync(writeStream);
                readStream.Close();
                writeStream.Close();
                this.logger.LogDebug(
                    "File {File} has been created",
                    subDirectory + "/" + fileName + "." + ext);
            }
            catch (Exception error)
            {
                this.logger.LogError(
                    "File {File} could not be created. Error: \n {error}",
                    subDirectory + "/" + fileName + "." + ext,
                    error);
                throw DataAccess.Error.FileOperationError.AsQueryException();
            }
        }

        /// <inheritdoc/>
        public void Remove(
            string subDirectory,
            string fileName,
            string fileExtension)
        {
            try
            {
                this.fileSystem.File.Delete(
                    this.GetFilePath(subDirectory, fileName, fileExtension));
                this.logger.LogDebug(
                    "File {File} has been removed", fileName);
            }
            catch (Exception error)
            {
                this.logger.LogError(
                    "File {File} could not be removed. Error: \n {error}",
                    subDirectory + "/" + fileName + "." + fileExtension,
                    error);
                throw DataAccess.Error.FileOperationError.AsQueryException();
            }
        }

        /// <summary>
        /// Method that returns the complete file path to a file with given
        /// arguments inside the statically served directory.
        /// </summary>
        /// <param name="subDirectory">Subdirectory in which the file is
        /// located.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileExtension">The file extension of the file.</param>
        /// <returns>The absolute path to the file.</returns>
        private string GetFilePath(
            string subDirectory,
            string fileName,
            string fileExtension)
        {
            return Path.Combine(
                this.GetDirectoryPath(subDirectory),
                fileName) + fileExtension;
        }

        /// <summary>
        /// Method to get the path to the given directory within the statically
        /// served directory.
        /// </summary>
        /// <param name="subDirectory">The subdirectory for which the path
        /// shall be generated.</param>
        /// <returns>The absolute path to the subdirectory.</returns>
        private string GetDirectoryPath(string subDirectory)
        {
            return Path.Combine(
                this.RootDirectory,
                subDirectory);
        }
    }
}
