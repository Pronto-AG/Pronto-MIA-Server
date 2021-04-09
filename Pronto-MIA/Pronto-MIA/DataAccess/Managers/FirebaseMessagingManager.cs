#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin;
    using FirebaseAdmin.Messaging;
    using Google.Apis.Auth.OAuth2;
    using HotChocolate;
    using LanguageExt;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for managing firebase messaging.
    /// </summary>
    public class FirebaseMessagingManager
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly IConfiguration cfg;
        private readonly ILogger logger;
        private FirebaseMessaging instance;

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
            ILogger<FirebaseMessagingManager> logger,
            IConfiguration cfg)
        {
            this.dbContext = dbContext;
            this.cfg = cfg;
            this.logger = logger;
            this.instance = this.GetInstance();
        }

        /// <summary>
        /// Sends the given message with firebase messaging to the devices
        /// specified within the message.
        /// </summary>
        /// <param name="message">Firebase messaging message object which
        /// contains the information required to send a message.</param>
        /// <returns>True if sending was successful false otherwise.</returns>
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

        /// <summary>
        /// Method to register a new fcm token for a user. If the token is
        /// already registered the owner will be adjusted.
        /// </summary>
        /// <param name="user"> The owner of the fcm token.
        /// </param>
        /// <param name="fcmToken">The token to be added.</param>
        /// <returns>The created fcm token.</returns>
        public async Task<IQueryable<FcmToken>> RegisterFcmToken(
            User user, string fcmToken)
        {
            await this.MoveOrCreateFcmToken(user, fcmToken);

            var result = this.dbContext.FcmTokens.Where(
                fcmTokenDb => fcmTokenDb.Id == fcmToken);
            Console.WriteLine(result.First().Id);
            return this.dbContext.FcmTokens.Where(
                fcmTokenDb => fcmTokenDb.Id == fcmToken);
        }

        /// <summary>
        /// Method to remove a fcm token from the database.
        /// </summary>
        /// <param name="fcmToken">The token to be removed.</param>
        /// <returns>True if the token could be removed false if the token did
        /// not exist.</returns>
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
                await this.dbContext.FcmTokens.AddAsync(fcmTokenObject);
                await this.dbContext.SaveChangesAsync();

                this.logger.LogDebug(
                    "FCMToken {FcmToken} was created for user {UserName}",
                    fcmToken,
                    user.UserName);
            }
            else if (fcmTokenObject.Owner != user)
            {
                var oldUsername = fcmTokenObject.Owner.UserName;
                fcmTokenObject.Owner = user;
                await this.dbContext.SaveChangesAsync();

                this.logger.LogDebug(
                    "FCMToken {Token} was moved from user {Old} to user {New}",
                    fcmToken,
                    oldUsername,
                    user.UserName);
            }
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
