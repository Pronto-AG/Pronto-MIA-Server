#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="EducationalContent"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class EducationalContentMutation
    {
        /// <summary>
        /// Adds an educational content to the application.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="educationalContentManager">The manager responsible for
        /// managing educational content.</param>
        /// <param name="title">Short description to identify the
        /// educational content.</param>
        /// <param name="description">A description of the educational content
        /// article.</param>
        /// <param name="file">The file to be associated with the new
        /// educational content.</param>
        /// <returns>The newly generated educational content.</returns>
        [Authorize(Policy = "EditEducationalContent")]
        [UseSingleOrDefault]
        public async Task<EducationalContent> CreateEducationalContent(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IEducationalContentManager educationalContentManager,
            string title,
            string description,
            IFile file)
        {
            educationalContentManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var educationalContent = await educationalContentManager.Create(
                    title,
                    description,
                    file);

                await dbContextTransaction.CommitAsync();
                return educationalContent;
            }
        }

        /// <summary>
        /// Method that updates the educational content with the given id
        /// according to the provided information.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="educationalContentManager">The manager responsible
        /// for managing educational content.</param>
        /// <param name="id">The id of the educational content to be adjusted.
        /// </param>
        /// <param name="title">Short description to identify the
        /// educational content.</param>
        /// <param name="description">A description of the educational content
        /// article.</param>
        /// <param name="file">The file to be associated with the new
        /// educational content.</param>
        /// <returns>The updated educational content.</returns>
        /// <exception cref="QueryException">Returns EducationalContentNotFound
        /// exception if the educational content with given id could not be
        /// found.</exception>
        [Authorize(Policy = "EditEducationalContent")]
        [AccessObjectIdArgument("id")]
        public async Task<EducationalContent> UpdateEducationalContent(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IEducationalContentManager educationalContentManager,
            int id,
            string? title,
            string? description,
            IFile? file)
        {
            educationalContentManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var educationalContent = await educationalContentManager.Update(
                    id,
                    title,
                    description,
                    file);
                await dbContextTransaction.CommitAsync();
                return educationalContent;
            }
        }

        /// <summary>
        /// Method that publishes the educational content with the given id. If
        /// the educational content is already published nothing will be done.
        /// </summary>
        /// <param name="educationalContentManager">The manager responsible for
        /// managing educational content.</param>
        /// <param name="firebaseMessagingManager">The manager used to inform
        /// affected users that a new educational content has been published.
        /// </param>
        /// <param name="firebaseTokenManager">The manager responsible for the
        /// fcm tokens used by this operation.
        /// </param>
        /// <param name="id">The id of the educational content to be published.
        /// </param>
        /// <param name="title">The title of the info notification to be sent.
        /// </param>
        /// <param name="body">The body of the info notification to be sent.
        /// </param>
        /// <returns>True if the educational content status was updated false if
        /// the educational content was already published.</returns>
        /// <exception cref="QueryException">If the educational content with the
        /// given id could not be found or the firebase manager encounters
        /// a sending error.</exception>
        [Authorize(Policy = "EditEducationalContent")]
        [AccessObjectIdArgument("id")]
        [UseSingleOrDefault]
        public async Task<bool> PublishEducationalContent(
            [Service] IEducationalContentManager educationalContentManager,
            [Service] IFirebaseMessagingManager firebaseMessagingManager,
            [Service] IFirebaseTokenManager firebaseTokenManager,
            int id,
            string title,
            string body)
        {
            var statusChanged = await educationalContentManager.Publish(id);
            if (!statusChanged)
            {
                return false;
            }

            var educationalContent = await educationalContentManager
                .GetById(id);
            var tokens = await firebaseTokenManager
                .GetAllFcmToken()
                .Select(token => token.Id).ToListAsync();
            var notification = new Notification { Title = title, Body = body };
            var data = this.CreateNotificationData(id);

            var badTokens = await firebaseMessagingManager
                .SendMulticastAsync(tokens, notification, data);
            await firebaseTokenManager.UnregisterMultipleFcmToken(badTokens);
            return true;
        }

        /// <summary>
        /// Method that hides the educational content with the given id. If the
        /// educational content is already not in the published state nothing
        /// will be done.
        /// </summary>
        /// <param name="educationalContentManager">The manager responsible for
        /// managing educational content.</param>
        /// <param name="id">The id of the educational content to be hidden.
        /// </param>
        /// <returns>True if the educational content status was updated false if
        /// the educational content was already hidden.</returns>
        /// <exception cref="QueryException">If the educational content with the
        /// given id could not be found.</exception>
        [Authorize(Policy = "EditEducationalContent")]
        [AccessObjectIdArgument("id")]
        [UseSingleOrDefault]
        public async Task<bool> HideEducationalContent(
            [Service] IEducationalContentManager educationalContentManager,
            int id)
        {
            return await educationalContentManager.Hide(id);
        }

        /// <summary>
        /// Method that removes the educational content with the given id.
        /// </summary>
        /// <param name="educationalContentManager">The manager responsible for
        /// managing educational content.</param>
        /// <param name="id">Id of the educational content to be removed.
        /// </param>
        /// <returns>The id of the educational content which was removed.
        /// </returns>
        /// <exception cref="QueryException">Returns EducationalContentNotFound
        /// exception if the educational content with the given id could not be
        /// found.
        /// </exception>
        [Authorize(Policy = "EditEducationalContent")]
        [AccessObjectIdArgument("id")]
        public async Task<int> RemoveEducationalContent(
            [Service] IEducationalContentManager educationalContentManager,
            int id)
        {
            return await educationalContentManager.Remove(id);
        }

        private Dictionary<string, string> CreateNotificationData(
            int educationalContentId)
        {
            return new ()
            {
                { "Action", "publish" },
                { "TargetType", "educationalContent" },
                { "TargetId", educationalContentId.ToString() },
            };
        }
    }
}
