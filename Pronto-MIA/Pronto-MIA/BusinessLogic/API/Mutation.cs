#nullable enable
namespace Pronto_MIA.BusinessLogic.API
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
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// </summary>
    public class Mutation
    {
        /// <summary>
        /// Adds a deployment plan to the application.
        /// </summary>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="file">The file to be associated with the new deployment
        /// plan.</param>
        /// <param name="availableFrom">The moment from which the deployment
        /// plan will be treated as active.</param>
        /// <param name="availableUntil">The moment until which the deployment
        /// plan will be treated as active.</param>
        /// <param name="description">Short description to identify the
        /// deployment plan.</param>
        /// <returns>The newly generated deployment plan.</returns>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        [UseSingleOrDefault]
        public async Task<DeploymentPlan> CreateDeploymentPlan(
            [Service] IDeploymentPlanManager deploymentPlanManager,
            IFile file,
            DateTime availableFrom,
            DateTime availableUntil,
            string? description)
        {
            return await deploymentPlanManager.Create(
                file,
                availableFrom,
                availableUntil,
                description);
        }

        /// <summary>
        /// Method that updates the deployment plan with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="id">The id of the deployment plan to be adjusted.
        /// </param>
        /// <param name="file">The file to be associated with the new deployment
        /// plan.</param>
        /// <param name="availableFrom">The moment from which the deployment
        /// plan will be treated as active.</param>
        /// <param name="availableUntil">The moment until which the deployment
        /// plan will be treated as active.</param>
        /// <param name="description">Short description to identify the
        /// deployment plan.</param>
        /// <returns>The updated deployment plan.</returns>
        /// <exception cref="QueryException">Returns DeploymentPlanNotFound
        /// exception if the deployment plan with given id could not be found.
        /// </exception>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        public async Task<DeploymentPlan> UpdateDeploymentPlan(
            [Service] IDeploymentPlanManager deploymentPlanManager,
            int id,
            IFile? file,
            DateTime? availableFrom,
            DateTime? availableUntil,
            string? description)
        {
            return await deploymentPlanManager.Update(
                id, file, availableFrom, availableUntil, description);
        }

        /// <summary>
        /// Method that publishes the deployment plan with the given id. If
        /// the deployment plan is already published nothing will be done.
        /// </summary>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="firebaseMessagingManager">The manager used to inform
        /// affected users that a new deployment plan has been published.
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
        /// <returns>True if the deployment plan status was updated false if
        /// the deployment plan was already published.</returns>
        /// <exception cref="QueryException">If the deployment plan with the
        /// given id could not be found or the firebase manager encounters
        /// a sending error.</exception>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        [UseSingleOrDefault]
        public async Task<bool> PublishDeploymentPlan(
            [Service] IDeploymentPlanManager deploymentPlanManager,
            [Service] IFirebaseMessagingManager firebaseMessagingManager,
            [Service] IFirebaseTokenManager firebaseTokenManager,
            int id,
            string title,
            string body)
        {
            var statusChanged = await deploymentPlanManager.Publish(id);
            if (!statusChanged)
            {
                return false;
            }

            var tokens = await firebaseTokenManager
                .GetAllFcmToken().Select(token => token.Id).ToListAsync();
            var notification = new Notification { Title = title, Body = body };
            var data = new Dictionary<string, string>()
            {
                { "Action", "publish" }, { "TargetType", "deploymentPlan" },
                { "TargetId", id.ToString() },
            };

            var badTokens = await firebaseMessagingManager
                .SendMulticastAsync(tokens, notification, data);
            await firebaseTokenManager.UnregisterMultipleFcmToken(badTokens);
            return true;
        }

        /// <summary>
        /// Method that hides the deployment plan with the given id. If the
        /// deployment plan is already not in the published state nothing will
        /// be done.
        /// </summary>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="id">The id of the deployment plan to be hidden.
        /// </param>
        /// <returns>True if the deployment plan status was updated false if
        /// the deployment plan was already hidden.</returns>
        /// <exception cref="QueryException">If the deployment plan with the
        /// given id could not be found.</exception>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        [UseSingleOrDefault]
        public async Task<bool> HideDeploymentPlan(
            [Service] IDeploymentPlanManager deploymentPlanManager,
            int id)
        {
            return await deploymentPlanManager.Hide(id);
        }

        /// <summary>
        /// Method that removes the deployment plan with the given id.
        /// </summary>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="id">Id of the plan to be removed.</param>
        /// <returns>The id of the plan which was removed.</returns>
        /// <exception cref="QueryException">Returns DeploymentPlanNotFound
        /// exception if the deployment plan with the given id could not be
        /// found.
        /// </exception>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        public async Task<int> RemoveDeploymentPlan(
            [Service] IDeploymentPlanManager deploymentPlanManager,
            int id)
        {
            return await deploymentPlanManager.Remove(id);
        }

        /// <summary>
        /// Registers a fcm token for the authenticated user. If the token
        /// already exists it will be overwritten with the currently
        /// authenticated user.
        /// </summary>
        /// <param name="firebaseTokenManager">The manager responsible for
        /// firebase token operations.</param>
        /// <param name="userManager">The manager managing the users lifecycle.
        /// </param>
        /// <param name="userState">Information about the current user.</param>
        /// <param name="fcmToken">The token to be registered.</param>
        /// <returns>True if the token was saved successfully.</returns>
        [Authorize]
        [UseSingleOrDefault]
        [UseProjection]
        [Sensitive("fcmToken")]
        public async Task<IQueryable<FcmToken>> RegisterFcmToken(
            [Service] IFirebaseTokenManager firebaseTokenManager,
            [Service] IUserManager userManager,
            [ApiUserGlobalState] ApiUserState userState,
            string fcmToken)
        {
            var user = await userManager.GetByUserName(userState.UserName);
            if (user == default)
            {
                throw DataAccess.Error.UserNotFound.AsQueryException();
            }

            return
                await firebaseTokenManager.RegisterFcmToken(user, fcmToken);
        }

        /// <summary>
        /// Unregisters a given fcm token from the user it was assigned to.
        /// If the token cannot be found nothing will be done.
        /// </summary>
        /// <param name="firebaseTokenManager">The manager managing
        /// operations with firebase tokens.
        /// </param>
        /// <param name="fcmToken">The fcm token to be removed.</param>
        /// <returns>True if the token could be removed.</returns>
        public async Task<bool> UnregisterFcmToken(
            [Service] IFirebaseTokenManager firebaseTokenManager,
            string fcmToken)
        {
            return await firebaseTokenManager.UnregisterFcmToken(fcmToken);
        }
    }
}
