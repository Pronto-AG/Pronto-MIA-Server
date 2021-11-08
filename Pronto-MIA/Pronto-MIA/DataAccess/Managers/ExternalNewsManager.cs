#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a external news within the
    /// application.
    /// </summary>
    public class ExternalNewsManager : IExternalNewsManager
    {
        private readonly ILogger logger;
        private readonly IFileManager fileManager;
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ExternalNewsManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        /// <param name="fileManager">The file manager which handles the
        /// persistence of the file associated with the external news object.
        /// </param>
        public ExternalNewsManager(
            ProntoMiaDbContext dbContext,
            ILogger<ExternalNewsManager> logger,
            IFileManager fileManager)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.fileManager = fileManager;
        }

        /// <inheritdoc/>
        public void SetDbContext(ProntoMiaDbContext context)
        {
            this.dbContext = context;
        }

        /// <inheritdoc/>
        public async Task<ExternalNews>
            Create(
                string title,
                string description,
                DateTime availableFrom,
                IFile file)
        {

            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                IExternalNewsManager.FileDirectory, uuid.ToString(), file);

            var externalNews = new ExternalNews(
                title,
                description,
                availableFrom,
                uuid,
                IFileManager.GetFileExtension(file));
            this.dbContext.ExternalNews.Add(externalNews);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "External news with id {Id} has been created",
                externalNews.Id);
            return externalNews;
        }

        /// <inheritdoc/>
        public async Task<ExternalNews>
            Update(
                int id,
                string? title,
                string? description,
                DateTime? availableFrom,
                IFile? file)
        {
            var externalNews = await this.GetById(id);

            externalNews = this.UpdateTitle(
                externalNews, title);
            externalNews = this.UpdateDescription(
                externalNews, description);
            externalNews = this.UpdateAvailableFrom(
                externalNews, availableFrom);
            externalNews = await this.UpdateFile(externalNews, file);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "External news with id {Id} has been updated",
                externalNews.Id);
            return externalNews;
        }

        /// <inheritdoc/>
        public async Task<bool> Publish(int id)
        {
            var externalNews = await this.GetById(id);

            if (externalNews.Published)
            {
                return false;
            }

            externalNews.Published = true;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> Hide(int id)
        {
            var externalNews = await this.GetById(id);

            if (!externalNews.Published)
            {
                return false;
            }

            externalNews.Published = false;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var externalNews = await this.GetById(id);

            this.fileManager.Remove(
                IExternalNewsManager.FileDirectory,
                externalNews.FileUuid.ToString(),
                externalNews.FileExtension);

            this.dbContext.Remove(externalNews);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <inheritdoc/>
        public IQueryable<ExternalNews> GetAll()
        {
            return this.dbContext.ExternalNews;
        }

        /// <inheritdoc/>
        public async Task<ExternalNews> GetById(int id)
        {
            var externalNews = await this.dbContext.ExternalNews
                .SingleOrDefaultAsync(eN => eN.Id == id);
            if (externalNews != default)
            {
                return externalNews;
            }

            this.logger.LogWarning(
                "Invalid external news id {Id}", id);
            throw DataAccess.Error.ExternalNewsNotFound
                .AsQueryException();
        }

        /// <summary>
        /// Updates the file of a given external news.
        /// </summary>
        /// <param name="externalNews">The external news to be updated.
        /// </param>
        /// <param name="file">The new file to be associated with the external
        /// news.</param>
        private async Task<ExternalNews> UpdateFile(
            ExternalNews externalNews, IFile? file)
        {
            if (file == null)
            {
                return externalNews;
            }

            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                IExternalNewsManager.FileDirectory, uuid.ToString(), file);

            externalNews.FileUuid = uuid;
            externalNews.FileExtension =
                IFileManager.GetFileExtension(file);

            this.fileManager.Remove(
                IExternalNewsManager.FileDirectory,
                externalNews.FileUuid.ToString(),
                externalNews.FileExtension);

            return externalNews;
        }

        /// <summary>
        /// Updates the times within the given external news to the new
        /// value.
        /// </summary>
        /// <param name="externalNews">The external news to be updated.
        /// </param>
        /// <param name="availableFrom">The new available from datetime.</param>
        private ExternalNews UpdateAvailableFrom(
            ExternalNews externalNews,
            DateTime? availableFrom)
        {
            if (availableFrom.HasValue)
            {
                externalNews.AvailableFrom = (DateTime)availableFrom;
            }

            return externalNews;
        }

        private ExternalNews UpdateDescription(
            ExternalNews externalNews, string? description)
        {
            if (description != null)
            {
                externalNews.Description =
                    description == string.Empty ? null : description;
            }

            return externalNews;
        }

        private ExternalNews UpdateTitle(
            ExternalNews externalNews, string? title)
        {
            if (title != null)
            {
                externalNews.Title =
                    title == string.Empty ? null : title;
            }

            return externalNews;
        }
    }
}
