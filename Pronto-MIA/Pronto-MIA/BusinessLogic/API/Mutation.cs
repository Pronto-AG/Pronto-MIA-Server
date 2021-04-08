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
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess.Managers;
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
        /// <returns>The newly generated deployment plan.</returns>
        [Authorize]
        [UseFirstOrDefault]
        public async Task<IQueryable<DeploymentPlan?>> AddDeploymentPlan(
            [Service] DeploymentPlanManager deploymentPlanManager,
            IFile file,
            DateTime availableFrom,
            DateTime availableUntil)
        {
            var result = await deploymentPlanManager.Create(
                file,
                availableFrom,
                availableUntil);
            return result.Match(
                deploymentPlan => deploymentPlan,
                error => throw error.AsQueryException());
        }

        /// <summary>
        /// Sends a testing push message to the device with the given device
        /// token.
        /// </summary>
        /// <param name="firebaseManager">The manager responsible for managing
        /// firebase messaging.</param>
        /// <param name="deviceToken">The firebase registration token of the
        /// device the message should be sent to.</param>
        /// <returns>True if the message could be sent false otherwise.
        /// </returns>
        [Authorize]
        public async Task<bool> SendPushTo(
            [Service] FirebaseManager firebaseManager,
            string deviceToken)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Test",
                    Body = "This is a test.4",
                },
                Data = new Dictionary<string, string>()
                { { "score", "850" }, { "time", "2:45" }, },
                Token = deviceToken,
            };
            return await firebaseManager.SendAsync(message);
        }

        /// <summary>
        /// Registers a fcm token for the authenticated user. If the token
        /// already exists it will be overwritten with the currently
        /// authenticated user.
        /// </summary>
        /// <param name="userManager">The manager managing the users lifecycle.
        /// </param>
        /// <param name="userState">Information about the current user.</param>
        /// <param name="token">The token to be registered.</param>
        /// <returns>True if the token was saved successfully.</returns>
        [Authorize]
        public async Task<bool> RegisterFcmToken(
            [Service] UserManager userManager,
            [ApiUserGlobalState] ApiUserState userState,
            string token)
        {
            var result = await
                userManager.RegisterFcmToken(userState.UserName, token);
            return result.Match(
                deploymentPlan => deploymentPlan,
                error => throw error.AsQueryException());
        }

        /// <summary>
        /// Unregisters a given fcm token from the user it was assigned to.
        /// If the token cannot be found nothing will be done.
        /// </summary>
        /// <param name="userManager">The manager managing the users lifecycle.
        /// </param>
        /// <param name="token">The fcm token to be removed.</param>
        /// <returns>True if the token could be removed.</returns>
        public async Task<bool> UnregisterFcmToken(
            [Service] UserManager userManager,
            string token)
        {
            return await userManager.UnregisterFcmToken(token);
        }
    }
}
