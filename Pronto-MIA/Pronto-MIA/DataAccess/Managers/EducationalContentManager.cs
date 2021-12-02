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
    /// Class responsible for the lifecycle of a educational content within the
    /// application.
    /// </summary>
    public class EducationalContentManager : IEducationalContentManager
    {
        private readonly ILogger logger;
        private readonly IFileManager fileManager;
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EducationalContentManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        /// <param name="fileManager">The file manager which handles the
        /// persistence of the file associated with the educational content object.
        /// </param>
        public EducationalContentManager(
            ProntoMiaDbContext dbContext,
            ILogger<EducationalContentManager> logger,
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
        public async Task<EducationalContent>
            Create(
                string title,
                string description,
                IFile file)
        {
            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                IEducationalContentManager.FileDirectory, uuid.ToString(), file);

            var educationalContent = new EducationalContent(
                title,
                description,
                uuid,
                IFileManager.GetFileExtension(file));
            this.dbContext.EducationalContent.Add(educationalContent);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "educational content with id {Id} has been created",
                educationalContent.Id);
            return educationalContent;
        }

        /// <inheritdoc/>
        public async Task<EducationalContent>
            Update(
                int id,
                string? title,
                string? description,
                IFile? file)
        {
            var educationalContent = await this.GetById(id);

            educationalContent = this.UpdateTitle(
                educationalContent, title);
            educationalContent = this.UpdateDescription(
                educationalContent, description);
            educationalContent = await this.UpdateFile(educationalContent, file);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "educational content with id {Id} has been updated",
                educationalContent.Id);
            return educationalContent;
        }

        /// <inheritdoc/>
        public async Task<bool> Publish(int id)
        {
            var educationalContent = await this.GetById(id);

            if (educationalContent.Published)
            {
                return false;
            }

            educationalContent.Published = true;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> Hide(int id)
        {
            var educationalContent = await this.GetById(id);

            if (!educationalContent.Published)
            {
                return false;
            }

            educationalContent.Published = false;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var educationalContent = await this.GetById(id);

            this.fileManager.Remove(
                IEducationalContentManager.FileDirectory,
                educationalContent.FileUuid.ToString(),
                educationalContent.FileExtension);

            this.dbContext.Remove(educationalContent);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <inheritdoc/>
        public IQueryable<EducationalContent> GetAll()
        {
            return this.dbContext.EducationalContent;
        }

        /// <inheritdoc/>
        public async Task<EducationalContent> GetById(int id)
        {
            var educationalContent = await this.dbContext.EducationalContent
                .SingleOrDefaultAsync(eN => eN.Id == id);
            if (educationalContent != default)
            {
                return educationalContent;
            }

            this.logger.LogWarning(
                "Invalid educational content id {Id}", id);
            throw DataAccess.Error.EducationalContentNotFound
                .AsQueryException();
        }

        /// <summary>
        /// Updates the file of a given educational content.
        /// </summary>
        /// <param name="educationalContent">The educational content to be updated.
        /// </param>
        /// <param name="file">The new file to be associated with the educational
        /// news.</param>
        private async Task<EducationalContent> UpdateFile(
            EducationalContent educationalContent, IFile? file)
        {
            if (file == null)
            {
                return educationalContent;
            }

            var uuid = Guid.NewGuid();
            await this.fileManager.Create(
                IEducationalContentManager.FileDirectory, uuid.ToString(), file);

            educationalContent.FileUuid = uuid;
            educationalContent.FileExtension =
                IFileManager.GetFileExtension(file);

            this.fileManager.Remove(
                IEducationalContentManager.FileDirectory,
                educationalContent.FileUuid.ToString(),
                educationalContent.FileExtension);

            return educationalContent;
        }

        private EducationalContent UpdateDescription(
            EducationalContent educationalContent, string? description)
        {
            if (description != null)
            {
                educationalContent.Description =
                    description == string.Empty ? null : description;
            }

            return educationalContent;
        }

        private EducationalContent UpdateTitle(
            EducationalContent educationalContent, string? title)
        {
            if (title != null)
            {
                educationalContent.Title =
                    title == string.Empty ? null : title;
            }

            return educationalContent;
        }
    }
}
