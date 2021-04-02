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
        private readonly ProntoMIADbContext dbContext;
        private readonly IConfiguration cfg;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this user manager.</param>
        /// <param name="dbContext">The database context where users are
        /// persisted.</param>
        /// <param name="cfg">The configuration of this application.</param>
        public FileManager(
            ILogger<FileManager> logger,
            ProntoMIADbContext dbContext,
            IConfiguration cfg)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.cfg = cfg;
        }

        public async Task
            CreateFileWithUUID(string directory, Guid uuid, IFile file)
        {
            Directory.CreateDirectory(
                this.GetDirectoryPath(directory));
            await using Stream stream = file.OpenReadStream();
            await using FileStream fs = File.Create(
                this.GetFilePath(directory, uuid) + ".pdf");
            await stream.CopyToAsync(fs);
            stream.Close();
            fs.Close();
        }

        public void RemoveFileByUUID(string directory, Guid uuid)
        {
            File.Delete(this.GetFilePath(directory, uuid));
        }

        private string GetFilePath(string directory, Guid uuid)
        {
            return Path.Combine(
                this.GetDirectoryPath(directory),
                uuid.ToString());
        }

        private string GetDirectoryPath(string directory)
        {
            return Path.Combine(
                this.cfg.GetValue<string>(
                    "FileManager:ROOT_DIRECTORY"),
                directory);
        }
    }
}
