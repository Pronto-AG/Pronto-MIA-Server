#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin;
    using FirebaseAdmin.Messaging;
    using Google.Apis.Auth.OAuth2;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Adapters;
    using Pronto_MIA.DataAccess.Adapters.Interfaces;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for managing firebase messaging.
    /// </summary>
    public class FirebaseMessagingManager : IFirebaseMessagingManager
    {
        /// <summary>
        /// A multicast firebase message can contain up to 500 fcm tokens.
        /// </summary>
        private static int maxTokensPerMessage = 500;

        private readonly ProntoMiaDbContext dbContext;
        private readonly IConfiguration cfg;
        private readonly ILogger logger;
        private readonly IFirebaseMessagingAdapter instance;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="FirebaseMessagingManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context where users are
        /// persisted.</param>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        /// <param name="instance">The firebase messaging instance used
        /// in order to send messages.</param>
        public FirebaseMessagingManager(
            ProntoMiaDbContext dbContext,
            IConfiguration cfg,
            ILogger<FirebaseMessagingManager> logger,
            IFirebaseMessagingAdapter? instance = null)
        {
            this.dbContext = dbContext;
            this.cfg = cfg;
            this.logger = logger;
            this.instance = instance ?? this.GetInstance();
        }

        /// <inheritdoc/>
        public async Task<bool> SendMulticastAsync(
            List<string> tokens,
            Notification notification,
            Dictionary<string, string> data)
        {
            if (tokens.Count == 0)
            {
                this.logger.LogInformation("No firebase tokens available.");
                return true;
            }

            if (tokens.Count <= maxTokensPerMessage)
            {
                await this.SendMulticastAsyncBatch(
                    tokens, notification, data);
            }
            else
            {
                var tokenBatches = SplitTokensToBatches(tokens);

                foreach (var batch in tokenBatches)
                {
                    await this.SendMulticastAsyncBatch(
                        batch, notification, data);
                }
            }

            return true;
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
                throw Error.FirebaseOperationError.AsQueryException();
            }

            return true;
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
        public IQueryable<FcmToken> GetAllFcmToken()
        {
            return this.dbContext.FcmTokens;
        }

        /// <summary>
        /// Method to split the given list of tokens into a list of lists
        /// each containing a maximum of <see cref="maxTokensPerMessage"/>
        /// items.
        /// </summary>
        /// <param name="tokens">The list of tokens to be split.</param>
        /// <returns>A list containing multiple lists of tokens each containing
        /// a maximum of <see cref="maxTokensPerMessage"/> items.</returns>
        private static List<List<string>> SplitTokensToBatches(
            List<string> tokens)
        {
            List<List<string>> tokenBatches = new ();
            for (int i = 0; i < tokens.Count; i += maxTokensPerMessage)
            {
                tokenBatches.Add(
                    tokens.GetRange(
                        i,
                        Math.Min(maxTokensPerMessage, tokens.Count - i)));
            }

            return tokenBatches;
        }

        /// <summary>
        /// Method to check if the response of the firebase messaging api
        /// indicates that the token used is not valid.
        /// </summary>
        /// <param name="response">The response that was received for the token
        /// in question by the firebase messaging api.</param>
        /// <returns>True if no error indicating an invalid token could be found
        /// false otherwise.</returns>
        private static bool FcmTokenValid(SendResponse response)
        {
            if (response.IsSuccess)
            {
                return true;
            }

            var errorCode = response.Exception.MessagingErrorCode;
            switch (errorCode)
            {
                case MessagingErrorCode.Internal:
                case MessagingErrorCode.QuotaExceeded:
                case MessagingErrorCode.ThirdPartyAuthError:
                    return true;
                case MessagingErrorCode.InvalidArgument:
                case MessagingErrorCode.SenderIdMismatch:
                case MessagingErrorCode.Unavailable:
                case MessagingErrorCode.Unregistered:
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to send a message to a batch of devices. The token count has
        /// to be below or equal to <see cref="maxTokensPerMessage"/>.
        /// </summary>
        /// <param name="tokens">The device tokens the message should be sent
        /// to. No more than <see cref="maxTokensPerMessage"/> tokens are
        /// allowed by this method.
        /// </param>
        /// <param name="notification">The notification to be included
        /// in the message.</param>
        /// <param name="data">The data to be included in the message.</param>
        /// <returns>True if sending was successful.</returns>
        /// <exception cref="QueryException">If a global error occured with
        /// the firebase operation. If some devices could not be reached no
        /// error is thrown.</exception>
        /// <exception cref="ArgumentException">If to many tokens are passed
        /// to this method.</exception>
        private async Task SendMulticastAsyncBatch (
            IReadOnlyList<string> tokens,
            Notification notification,
            IReadOnlyDictionary<string, string> data)
        {
            if (tokens.Count > maxTokensPerMessage)
            {
                throw new ArgumentException("To many firebase tokens.");
            }

            var message = new MulticastMessage()
            {
                Notification = notification, Data = data, Tokens = tokens,
            };
            try
            {
                var response = await this.instance.SendMulticastAsync(message);
                this.logger.LogTrace(
                    "Successfully sent message {Message}", response);
                if (response.FailureCount > 0)
                {
                    await this.CleanTokenDb(message.Tokens, response.Responses);
                }
            }
            catch (Exception error)
            {
                this.logger.LogWarning("Firebase error: {Error}", error);
                throw Error.FirebaseOperationError.AsQueryException();
            }
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
        private IFirebaseMessagingAdapter GetInstance()
        {
            if (FirebaseMessaging.DefaultInstance != null)
            {
                return new FirebaseMessagingAdapter(
                    FirebaseMessaging.DefaultInstance);
            }

            var credentialFile =
                this.cfg.GetValue<string>("Firebase:CREDENTIAL_FILE");
            FirebaseApp.Create(new AppOptions()
            {
                Credential =
                    GoogleCredential.FromFile(credentialFile),
            });
            this.logger.LogDebug("Created firebase app instance");

            return new FirebaseMessagingAdapter(
                FirebaseMessaging.DefaultInstance);
        }

        /// <summary>
        /// Method to remove all fcmTokens which are invalid according
        /// to the api responses from the database.
        /// </summary>
        /// <param name="tokens">The tokens which have been used in the api
        /// request. It is important, that the tokens are still in the same
        /// order as they have been sent to the api.</param>
        /// <param name="responses">The Responses sent by the api. It is
        /// important, that the responses are still in the same order as they
        /// were received from the api.</param>
        /// <returns>An empty task which does not have to be awaited since this
        /// is only a cleanup operation.</returns>
        private Task CleanTokenDb(
            IReadOnlyList<string> tokens, IReadOnlyList<SendResponse> responses)
        {
            return Task.Run(async () =>
            {
                HashSet<string> toDelete = new ();
                for (var i = 0; i < tokens.Count; i++)
                {
                    var response = responses[i];
                    var token = tokens[i];
                    if (FcmTokenValid(response))
                    {
                        continue;
                    }

                    var errorCode = response.Exception.MessagingErrorCode;
                    toDelete.Add(token);
                    this.logger.LogDebug(
                        "Token: \"{Token}\" will be removed." +
                        " Reason: {ErrorCode}",
                        token,
                        errorCode.ToString());
                }

                this.dbContext.RemoveRange(
                    this.dbContext.FcmTokens.Where(fcmToken =>
                        toDelete.Contains(fcmToken.Id)));
                await this.dbContext.SaveChangesAsync();
            });
        }
    }
}
