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
    /// Class responsible for the lifecycle of a internal news within the
    /// application.
    /// </summary>
    public class InternalNewsManager : IInternalNewsManager
    {
        private readonly ILogger logger;
        private readonly IFileManager fileManager;
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InternalNewsManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        /// <param name="fileManager">The file manager which handles the
        /// persistence of the file associated with the internal news object.
        /// </param>
        public InternalNewsManager(
            ProntoMiaDbContext dbContext,
            ILogger<InternalNewsManager> logger,
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
        public async Task<InternalNews>
            Create(
                string title,
                string description,
                DateTime availableFrom,
                IFile file)
        {
            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                IInternalNewsManager.FileDirectory, uuid.ToString(), file);

            var internalNews = new InternalNews(
                title,
                description,
                availableFrom,
                uuid,
                IFileManager.GetFileExtension(file));
            this.dbContext.InternalNews.Add(internalNews);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "internal news with id {Id} has been created",
                internalNews.Id);
            return internalNews;
        }

        /// <inheritdoc/>
        public async Task<InternalNews>
            Update(
                int id,
                string? title,
                string? description,
                DateTime? availableFrom,
                IFile? file)
        {
            var internalNews = await this.GetById(id);

            internalNews = this.UpdateTitle(
                internalNews, title);
            internalNews = this.UpdateDescription(
                internalNews, description);
            internalNews = this.UpdateAvailableFrom(
                internalNews, availableFrom);
            internalNews = await this.UpdateFile(internalNews, file);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "internal news with id {Id} has been updated",
                internalNews.Id);
            return internalNews;
        }

        /// <inheritdoc/>
        public async Task<bool> Publish(int id)
        {
            var internalNews = await this.GetById(id);

            if (internalNews.Published)
            {
                return false;
            }

            internalNews.Published = true;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> Hide(int id)
        {
            var internalNews = await this.GetById(id);

            if (!internalNews.Published)
            {
                return false;
            }

            internalNews.Published = false;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var internalNews = await this.GetById(id);

            this.fileManager.Remove(
                IInternalNewsManager.FileDirectory,
                internalNews.FileUuid.ToString(),
                internalNews.FileExtension);

            this.dbContext.Remove(internalNews);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <inheritdoc/>
        public IQueryable<InternalNews> GetAll()
        {
            return this.dbContext.InternalNews;
        }

        /// <inheritdoc/>
        public async Task<InternalNews> GetById(int id)
        {
            var internalNews = await this.dbContext.InternalNews
                .SingleOrDefaultAsync(eN => eN.Id == id);
            if (internalNews != default)
            {
                return internalNews;
            }

            this.logger.LogWarning(
                "Invalid internal news id {Id}", id);
            throw DataAccess.Error.InternalNewsNotFound
                .AsQueryException();
        }

        /// <summary>
        /// Updates the file of a given internal news.
        /// </summary>
        /// <param name="internalNews">The internal news to be updated.
        /// </param>
        /// <param name="file">The new file to be associated with the internal
        /// news.</param>
        private async Task<InternalNews> UpdateFile(
            InternalNews internalNews, IFile? file)
        {
            if (file == null)
            {
                return internalNews;
            }

            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                IInternalNewsManager.FileDirectory, uuid.ToString(), file);

            internalNews.FileUuid = uuid;
            internalNews.FileExtension =
                IFileManager.GetFileExtension(file);

            this.fileManager.Remove(
                IInternalNewsManager.FileDirectory,
                internalNews.FileUuid.ToString(),
                internalNews.FileExtension);

            return internalNews;
        }

        /// <summary>
        /// Updates the times within the given internal news to the new
        /// value.
        /// </summary>
        /// <param name="internalNews">The internal news to be updated.
        /// </param>
        /// <param name="availableFrom">The new available from datetime.</param>
        private InternalNews UpdateAvailableFrom(
            InternalNews internalNews,
            DateTime? availableFrom)
        {
            if (availableFrom.HasValue)
            {
                internalNews.AvailableFrom = (DateTime)availableFrom;
            }

            return internalNews;
        }

        private InternalNews UpdateDescription(
            InternalNews internalNews, string? description)
        {
            if (description != null)
            {
                internalNews.Description =
                    description == string.Empty ? null : description;
            }

            return internalNews;
        }

        private InternalNews UpdateTitle(
            InternalNews internalNews, string? title)
        {
            if (title != null)
            {
                internalNews.Title =
                    title == string.Empty ? null : title;
            }

            return internalNews;
        }
    }
}
