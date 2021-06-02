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
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="DeploymentPlan"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class DeploymentPlanMutation
    {
        /// <summary>
        /// Adds a deployment plan to the application.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="departmentManager">The manager responsible for managing
        /// the departments linked to the deployment plans.</param>
        /// <param name="file">The file to be associated with the new deployment
        /// plan.</param>
        /// <param name="availableFrom">The moment from which the deployment
        /// plan will be treated as active.</param>
        /// <param name="availableUntil">The moment until which the deployment
        /// plan will be treated as active.</param>
        /// <param name="departmentId">The id of the department the new
        /// deployment plan should be linked to.</param>
        /// <param name="description">Short description to identify the
        /// deployment plan.</param>
        /// <returns>The newly generated deployment plan.</returns>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        [UseSingleOrDefault]
        public async Task<DeploymentPlan> CreateDeploymentPlan(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IDeploymentPlanManager deploymentPlanManager,
            [Service] IDepartmentManager departmentManager,
            IFile file,
            DateTime availableFrom,
            DateTime availableUntil,
            int departmentId,
            string? description)
        {
            deploymentPlanManager.SetDbContext(dbContext);
            departmentManager.SetDbContext(dbContext);

            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var deploymentPlan = await deploymentPlanManager.Create(
                    file,
                    availableFrom,
                    availableUntil,
                    description);
                await departmentManager.AddDeploymentPlan(
                    departmentId, deploymentPlan);

                await dbContextTransaction.CommitAsync();
                return deploymentPlan;
            }
        }

        /// <summary>
        /// Method that updates the deployment plan with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="deploymentPlanManager">The manager responsible for
        /// managing deployment plans.</param>
        /// <param name="departmentManager">The manager responsible for managing
        /// the departments linked to the deployment plans.</param>
        /// <param name="id">The id of the deployment plan to be adjusted.
        /// </param>
        /// <param name="file">The file to be associated with the new deployment
        /// plan.</param>
        /// <param name="availableFrom">The moment from which the deployment
        /// plan will be treated as active.</param>
        /// <param name="availableUntil">The moment until which the deployment
        /// plan will be treated as active.</param>
        /// <param name="departmentId">The id of the department the
        /// deployment plan should be linked to.</param>
        /// <param name="description">Short description to identify the
        /// deployment plan.</param>
        /// <returns>The updated deployment plan.</returns>
        /// <exception cref="QueryException">Returns DeploymentPlanNotFound
        /// exception if the deployment plan with given id could not be found.
        /// </exception>
        [Authorize(Policy = "CanEditDeploymentPlans")]
        public async Task<DeploymentPlan> UpdateDeploymentPlan(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IDeploymentPlanManager deploymentPlanManager,
            [Service] IDepartmentManager departmentManager,
            int id,
            IFile? file,
            DateTime? availableFrom,
            DateTime? availableUntil,
            int? departmentId,
            string? description)
        {
            deploymentPlanManager.SetDbContext(dbContext);
            departmentManager.SetDbContext(dbContext);

            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var deploymentPlan = await deploymentPlanManager.Update(
                    id,
                    file,
                    availableFrom,
                    availableUntil,
                    description);
                await this.UpdateDeploymentPlanDepartment(
                    departmentManager, departmentId, deploymentPlan);
                await dbContextTransaction.CommitAsync();
                return deploymentPlan;
            }
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

            var deploymentPlan = await deploymentPlanManager.GetById(id);
            var tokens = await firebaseTokenManager
                .GetDepartmentFcmToken(deploymentPlan.DepartmentId!.Value)
                .Select(token => token.Id).ToListAsync();
            var notification = new Notification { Title = title, Body = body };
            var data = this.CreateNotificationData(id);

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

        private async Task UpdateDeploymentPlanDepartment(
            IDepartmentManager departmentManager,
            int? departmentId,
            DeploymentPlan deploymentPlan)
        {
            if (departmentId != null)
            {
                await departmentManager.AddDeploymentPlan(
                    departmentId.Value, deploymentPlan);
            }
        }

        private Dictionary<string, string> CreateNotificationData(
            int deploymentPlanId)
        {
            return new ()
            {
                { "Action", "publish" }, { "TargetType", "deploymentPlan" },
                { "TargetId", deploymentPlanId.ToString() },
            };
        }
    }
}
