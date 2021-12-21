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
    /// Contains all mutations that concern <see cref="InternalNews"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class InternalNewsMutation
    {
        /// <summary>
        /// Adds an internal news to the application.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="internalNewsManager">The manager responsible for
        /// managing internal news.</param>
        /// <param name="title">Short description to identify the
        /// internal news.</param>
        /// <param name="description">A description of the internal news
        /// article.</param>
        /// <param name="availableFrom">The moment from which the internal
        /// news will be treated as active.</param>
        /// <param name="file">The file to be associated with the new internal
        /// news.</param>
        /// <returns>The newly generated internal news.</returns>
        [Authorize(Policy = "EditInternalNews")]
        [UseSingleOrDefault]
        public async Task<InternalNews> CreateInternalNews(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IInternalNewsManager internalNewsManager,
            string title,
            string description,
            DateTime availableFrom,
            IFile file)
        {
            internalNewsManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var internalNews = await internalNewsManager.Create(
                    title,
                    description,
                    availableFrom,
                    file);

                await dbContextTransaction.CommitAsync();
                return internalNews;
            }
        }

        /// <summary>
        /// Method that updates the internal news with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="internalNewsManager">The manager responsible for
        /// managing internal news.</param>
        /// <param name="id">The id of the internal news to be adjusted.
        /// </param>
        /// <param name="title">Short description to identify the
        /// internal news.</param>
        /// <param name="description">A description of the internal news
        /// article.</param>
        /// <param name="availableFrom">The moment from which the internal
        /// news will be treated as active.</param>
        /// <param name="file">The file to be associated with the new internal
        /// news.</param>
        /// <returns>The updated internal news.</returns>
        /// <exception cref="QueryException">Returns InternalNewsNotFound
        /// exception if the internal news with given id could not be found.
        /// </exception>
        [Authorize(Policy = "EditInternalNews")]
        [AccessObjectIdArgument("id")]
        public async Task<InternalNews> UpdateInternalNews(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IInternalNewsManager internalNewsManager,
            int id,
            string? title,
            string? description,
            DateTime? availableFrom,
            IFile? file)
        {
            internalNewsManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var internalNews = await internalNewsManager.Update(
                    id,
                    title,
                    description,
                    availableFrom,
                    file);
                await dbContextTransaction.CommitAsync();
                return internalNews;
            }
        }

        /// <summary>
        /// Method that publishes the internal news with the given id. If
        /// the internal news is already published nothing will be done.
        /// </summary>
        /// <param name="internalNewsManager">The manager responsible for
        /// managing internal news.</param>
        /// <param name="firebaseMessagingManager">The manager used to inform
        /// affected users that a new internal news has been published.
        /// </param>
        /// <param name="firebaseTokenManager">The manager responsible for the
        /// fcm tokens used by this operation.
        /// </param>
        /// <param name="id">The id of the internal news to be published.
        /// </param>
        /// <param name="title">The title of the info notification to be sent.
        /// </param>
        /// <param name="body">The body of the info notification to be sent.
        /// </param>
        /// <returns>True if the internal news status was updated false if
        /// the internal news was already published.</returns>
        /// <exception cref="QueryException">If the internal news with the
        /// given id could not be found or the firebase manager encounters
        /// a sending error.</exception>
        [Authorize(Policy = "EditInternalNews")]
        [AccessObjectIdArgument("id")]
        [UseSingleOrDefault]
        public async Task<bool> PublishInternalNews(
            [Service] IInternalNewsManager internalNewsManager,
            [Service] IFirebaseMessagingManager firebaseMessagingManager,
            [Service] IFirebaseTokenManager firebaseTokenManager,
            int id,
            string title,
            string body)
        {
            var statusChanged = await internalNewsManager.Publish(id);
            if (!statusChanged)
            {
                return false;
            }

            var internalNews = await internalNewsManager.GetById(id);
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
        /// Method that hides the internal news with the given id. If the
        /// internal news is already not in the published state nothing will
        /// be done.
        /// </summary>
        /// <param name="internalNewsManager">The manager responsible for
        /// managing internal news.</param>
        /// <param name="id">The id of the internal news to be hidden.
        /// </param>
        /// <returns>True if the internal news status was updated false if
        /// the internal news was already hidden.</returns>
        /// <exception cref="QueryException">If the internal news with the
        /// given id could not be found.</exception>
        [Authorize(Policy = "EditInternalNews")]
        [AccessObjectIdArgument("id")]
        [UseSingleOrDefault]
        public async Task<bool> HideInternalNews(
            [Service] IInternalNewsManager internalNewsManager,
            int id)
        {
            return await internalNewsManager.Hide(id);
        }

        /// <summary>
        /// Method that removes the internal news with the given id.
        /// </summary>
        /// <param name="internalNewsManager">The manager responsible for
        /// managing internal news.</param>
        /// <param name="id">Id of the internal news to be removed.</param>
        /// <returns>The id of the internal news which was removed.</returns>
        /// <exception cref="QueryException">Returns InternalNewsNotFound
        /// exception if the internal news with the given id could not be
        /// found.
        /// </exception>
        [Authorize(Policy = "EditInternalNews")]
        [AccessObjectIdArgument("id")]
        public async Task<int> RemoveInternalNews(
            [Service] IInternalNewsManager internalNewsManager,
            int id)
        {
            return await internalNewsManager.Remove(id);
        }

        private Dictionary<string, string> CreateNotificationData(
            int internalNewsId)
        {
            return new ()
            {
                { "Action", "publish" },
                { "TargetType", "internalNews" },
                { "TargetId", internalNewsId.ToString() },
            };
        }
    }
}
