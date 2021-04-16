#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin;
    using FirebaseAdmin.Messaging;
    using Google.Apis.Auth.OAuth2;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for managing firebase messaging.
    /// </summary>
    public class FirebaseMessagingManager : IFirebaseMessagingManager
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly IConfiguration cfg;
        private readonly ILogger logger;
        private readonly FirebaseMessaging instance;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="FirebaseMessagingManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context where users are
        /// persisted.</param>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        public FirebaseMessagingManager(
            ProntoMiaDbContext dbContext,
            IConfiguration cfg,
            ILogger<FirebaseMessagingManager> logger)
        {
            this.dbContext = dbContext;
            this.cfg = cfg;
            this.logger = logger;
            this.instance = this.GetInstance();
        }

        /// <inheritdoc/>
        public async Task<bool> SendAsync(Message message)
        {
            try
            {
                string response = await this.instance.SendAsync(message);
                this.logger.LogTrace(
                    "Successfully sent message {Message}", response);
            }
            catch (Exception error)
            {
                this.logger.LogWarning("Firebase error: {Error}", error);
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<IQueryable<FcmToken>> RegisterFcmToken(
            User user, string fcmToken)
        {
            await this.MoveOrCreateFcmToken(user, fcmToken);

            var result = this.dbContext.FcmTokens.Where(
                fcmTokenDb => fcmTokenDb.Id == fcmToken);

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

        /// <summary>
        /// If a fcm token already exists it will be moved else a new token will
        /// be created.
        /// </summary>
        private async Task MoveOrCreateFcmToken(User user, string fcmToken)
        {
            var fcmTokenObject = await this.dbContext.FcmTokens
                .SingleOrDefaultAsync(t => t.Id == fcmToken);

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

        /// <summary>
        /// Method to get a singleton instance of the firebase app.
        /// </summary>
        private FirebaseMessaging GetInstance()
        {
            if (FirebaseMessaging.DefaultInstance != null)
            {
                return FirebaseMessaging.DefaultInstance;
            }

            var credentialFile =
                this.cfg.GetValue<string>("Firebase:CREDENTIAL_FILE");
            FirebaseApp.Create(new AppOptions()
            {
                Credential =
                    GoogleCredential.FromFile(credentialFile),
            });
            this.logger.LogDebug("Created firebase app instance");

            return FirebaseMessaging.DefaultInstance!;
        }
    }
}
