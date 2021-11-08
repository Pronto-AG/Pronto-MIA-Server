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
    /// Contains all mutations that concern <see cref="ExternalNews"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class ExternalNewsMutation
    {
        /// <summary>
        /// Adds an external news to the application.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="externalNewsManager">The manager responsible for
        /// managing external news.</param>
        /// <param name="title">Short description to identify the
        /// external news.</param>
        /// <param name="description">A description of the external news
        /// article.</param>
        /// <param name="availableFrom">The moment from which the external
        /// news will be treated as active.</param>
        /// <param name="file">The file to be associated with the new external
        /// news.</param>
        /// <returns>The newly generated external news.</returns>
        [Authorize(Policy = "EditExternalNews")]
        [UseSingleOrDefault]
        public async Task<ExternalNews> CreateExternalNews(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IExternalNewsManager externalNewsManager,
            string title,
            string description,
            DateTime availableFrom,
            IFile file)
        {
            externalNewsManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var externalNews = await externalNewsManager.Create(
                    title,
                    description,
                    availableFrom,
                    file);

                await dbContextTransaction.CommitAsync();
                return externalNews;
            }
        }

        /// <summary>
        /// Method that updates the external news with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="externalNewsManager">The manager responsible for
        /// managing external news.</param>
        /// <param name="id">The id of the deployment plan to be adjusted.
        /// </param>
        /// <param name="title">Short description to identify the
        /// external news.</param>
        /// <param name="description">A description of the external news
        /// article.</param>
        /// <param name="availableFrom">The moment from which the external
        /// news will be treated as active.</param>
        /// <param name="file">The file to be associated with the new external
        /// news.</param>
        /// <returns>The updated external news.</returns>
        /// <exception cref="QueryException">Returns ExternalNewsNotFound
        /// exception if the external news with given id could not be found.
        /// </exception>
        [Authorize(Policy = "EditExternalNews")]
        [AccessObjectIdArgument("id")]
        public async Task<ExternalNews> UpdateExternalNews(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IExternalNewsManager externalNewsManager,
            int id,
            string? title,
            string? description,
            DateTime? availableFrom,
            IFile? file)
        {
            externalNewsManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var externalNews = await externalNewsManager.Update(
                    id,
                    title,
                    description,
                    availableFrom,
                    file);
                await dbContextTransaction.CommitAsync();
                return externalNews;
            }
        }

        /// <summary>
        /// Method that publishes the external news with the given id. If
        /// the external news is already published nothing will be done.
        /// </summary>
        /// <param name="externalNewsManager">The manager responsible for
        /// managing external news.</param>
        /// <param name="firebaseMessagingManager">The manager used to inform
        /// affected users that a new external news has been published.
        /// </param>
        /// <param name="firebaseTokenManager">The manager responsible for the
        /// fcm tokens used by this operation.
        /// </param>
        /// <param name="id">The id of the deployment plan to be published.
        /// </param>
        /// <param name="title">The title of the info notification to be sent.
        /// </param>
        /// <param name="body">The body of the info notification to be sent.
        /// </param>
        /// <returns>True if the external news status was updated false if
        /// the external news was already published.</returns>
        /// <exception cref="QueryException">If the external news with the
        /// given id could not be found or the firebase manager encounters
        /// a sending error.</exception>
        [Authorize(Policy = "EditExternalNews")]
        [AccessObjectIdArgument("id")]
        [UseSingleOrDefault]
        public async Task<bool> PublishExternalNews(
            [Service] IExternalNewsManager externalNewsManager,
            [Service] IFirebaseMessagingManager firebaseMessagingManager,
            [Service] IFirebaseTokenManager firebaseTokenManager,
            int id,
            string title,
            string body)
        {
            var statusChanged = await externalNewsManager.Publish(id);
            if (!statusChanged)
            {
                return false;
            }

            var externalNews = await externalNewsManager.GetById(id);
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
        /// Method that hides the external news with the given id. If the
        /// external news is already not in the published state nothing will
        /// be done.
        /// </summary>
        /// <param name="externalNewsManager">The manager responsible for
        /// managing external news.</param>
        /// <param name="id">The id of the external news to be hidden.
        /// </param>
        /// <returns>True if the external news status was updated false if
        /// the external news was already hidden.</returns>
        /// <exception cref="QueryException">If the external news with the
        /// given id could not be found.</exception>
        [Authorize(Policy = "EditExternalNews")]
        [AccessObjectIdArgument("id")]
        [UseSingleOrDefault]
        public async Task<bool> HideExternalNews(
            [Service] IExternalNewsManager externalNewsManager,
            int id)
        {
            return await externalNewsManager.Hide(id);
        }

        /// <summary>
        /// Method that removes the external news with the given id.
        /// </summary>
        /// <param name="externalNewsManager">The manager responsible for
        /// managing external news.</param>
        /// <param name="id">Id of the external news to be removed.</param>
        /// <returns>The id of the external news which was removed.</returns>
        /// <exception cref="QueryException">Returns ExternalNewsNotFound
        /// exception if the external news with the given id could not be
        /// found.
        /// </exception>
        [Authorize(Policy = "EditExternalNews")]
        [AccessObjectIdArgument("id")]
        public async Task<int> RemoveExternalNews(
            [Service] IExternalNewsManager externalNewsManager,
            int id)
        {
            return await externalNewsManager.Remove(id);
        }

        private Dictionary<string, string> CreateNotificationData(
            int externalNewsId)
        {
            return new ()
            {
                { "Action", "publish" }, { "TargetType", "externalNews" },
                { "TargetId", externalNewsId.ToString() },
            };
        }
    }
}
