namespace Pronto_MIA.DataAccess.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Configuration;
    using FirebaseAdmin.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class managing operations regarding fcm tokens.
    /// </summary>
    public class FirebaseTokenManager : IFirebaseTokenManager
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseTokenManager"/>
        /// class.
        /// </summary>
        /// <param name="dbContext">The db context to be used in order to
        /// persist fcm tokens.</param>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        public FirebaseTokenManager(
            ProntoMiaDbContext dbContext,
            ILogger<FirebaseTokenManager> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IQueryable<FcmToken>> RegisterFcmToken(
            User user, string fcmToken)
        {
            await this.MoveOrCreateFcmToken(user, fcmToken);

            return this.dbContext.FcmTokens.Where(
                fcmTokenDb => fcmTokenDb.Id == fcmToken);
        }

        /// <inheritdoc/>
        public async Task<bool> UnregisterFcmToken(string fcmToken)
        {
            var tokenObject = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == fcmToken);
            if (tokenObject != default)
            {
                this.logger.LogDebug(
                    "FCMToken {FcmToken} was removed from User {UserName}",
                    fcmToken,
                    tokenObject.Owner.UserName);
                this.dbContext.Remove(tokenObject);
                await this.dbContext.SaveChangesAsync();

                return true;
            }

            this.logger.LogDebug(
                "FCMToken {FcmToken} did not exist. Nothing to remove",
                fcmToken);
            return false;
        }

        /// <inheritdoc/>
        public async Task UnregisterMultipleFcmToken(
            HashSet<string> tokenIds)
        {
            this.dbContext.RemoveRange(
                this.dbContext.FcmTokens.Where(fcmToken =>
                    tokenIds.Contains(fcmToken.Id)));
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public IQueryable<FcmToken> GetAllFcmToken()
        {
            return this.dbContext.FcmTokens;
        }

        /// <inheritdoc/>
        public IQueryable<FcmToken> GetDepartmentFcmToken(int departmentId)
        {
            var users =
                this.dbContext.Users.Where(u => u.DepartmentId == departmentId);
            var tokens = this.dbContext.FcmTokens
                .Where(fcmToken => users.Contains(fcmToken.Owner));
            return tokens;
        }

        /// <summary>
        /// If a fcm token already exists it will be moved else a new token will
        /// be created.
        /// </summary>
        private async Task MoveOrCreateFcmToken(User user, string fcmToken)
        {
            var fcmTokenObject = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == fcmToken);
            user = await this.dbContext.Users.FindAsync(user.Id);

            if (fcmTokenObject == default)
            {
                fcmTokenObject = new FcmToken(fcmToken, user);
                this.dbContext.FcmTokens.Add(fcmTokenObject);
                await this.dbContext.SaveChangesAsync();

                this.logger.LogDebug(
                    "FCMToken {FcmToken} was created for user {UserName}",
                    fcmToken,
                    user.UserName);
            }
            else if (fcmTokenObject.Owner != user)
            {
                await this.MoveFcmToken(fcmTokenObject, user);
            }
            else
            {
                this.logger.LogDebug(
                    "FCMToken {FcmToken} already registered for " +
                    "user {UserName}",
                    fcmToken,
                    user.UserName);
            }
        }

        /// <summary>
        /// Method to move the given token to a new owner.
        /// </summary>
        /// <param name="fcmTokenObject">The token to be moved.</param>
        /// <param name="newOwner">The new owner which will be set.</param>
        private async Task MoveFcmToken(
            FcmToken fcmTokenObject,
            User newOwner)
        {
            var oldOwner = fcmTokenObject.Owner;
            fcmTokenObject.Owner = newOwner;
            await this.dbContext.SaveChangesAsync();

            this.logger.LogDebug(
                "FCMToken {Token} was moved from user {Old} to user {New}",
                fcmTokenObject.Id,
                oldOwner.UserName,
                newOwner.UserName);
        }
    }
}
