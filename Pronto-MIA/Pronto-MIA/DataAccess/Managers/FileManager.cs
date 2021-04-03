namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HotChocolate.Types;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Class responsible for the lifecycle of a user within the application.
    /// </summary>
    public class FileManager
    {
        private readonly IConfiguration cfg;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this user manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        public FileManager(
            IConfiguration cfg,
            ILogger<FileManager> logger)
        {
            this.cfg = cfg;
            this.logger = logger;
        }

        private string RootDirectory => this.cfg.GetValue<string>(
            "StaticFiles:ROOT_DIRECTORY");

        /// <summary>
        /// Method to extract the file extension from a <see cref="IFile"/>
        /// object.
        /// </summary>
        /// <param name="file">The file from which the extension should be
        /// extracted.</param>
        /// <returns>The file extension.</returns>
        /// <exception cref="ArgumentException">If the file has no file
        /// extension or the filename contains invalid characters.</exception>
        public static string GetFileExtension(IFile file)
        {
            var ext = Path.GetExtension(file.Name);
            if (string.IsNullOrEmpty(ext))
            {
                throw new ArgumentException("No file extension found");
            }

            return ext!;
        }

        /// <summary>
        /// Method that creates a file inside the directory where files are
        /// statically served by ASP.NET.
        /// </summary>
        /// <param name="subDirectory">The subdirectory in which the new file
        /// should be placed.</param>
        /// <param name="fileName">The name the file should be saved as.</param>
        /// <param name="file">The file that will be saved.</param>
        /// <returns>Task that can be awaited.</returns>
        public async Task
            Create(string subDirectory, string fileName, IFile file)
        {
            var ext = FileManager.GetFileExtension(file);

            Directory.CreateDirectory(
                this.GetDirectoryPath(subDirectory));
            await using Stream stream = file.OpenReadStream();
            await using FileStream fs = File.Create(
                this.GetFilePath(subDirectory, fileName, ext));
            await stream.CopyToAsync(fs);
            stream.Close();
            fs.Close();
            this.logger.LogDebug(
                "File {File} has been created", fileName);
        }

        /// <summary>
        /// Method that removes a file from the directory statically served by
        /// ASP.NET.
        /// </summary>
        /// <param name="subDirectory">The subdirectory in which the file can
        /// be found.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileExtension">The file extension of the file.</param>
        public void Remove(
            string subDirectory,
            string fileName,
            string fileExtension)
        {
            File.Delete(
                this.GetFilePath(subDirectory, fileName, fileExtension));
            this.logger.LogDebug(
                "File {File} has been removed", fileName);
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
