#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Threading.Tasks;
    using FirebaseAdmin;
    using FirebaseAdmin.Messaging;
    using Google.Apis.Auth.OAuth2;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Class responsible for managing firebase messaging.
    /// </summary>
    public class FirebaseManager
    {
        private readonly IConfiguration cfg;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="FirebaseManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        public FirebaseManager(
            ILogger<FirebaseManager> logger,
            IConfiguration cfg)
        {
            this.logger = logger;
            this.cfg = cfg;
            this.Create();
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
                string response = await
                    FirebaseMessaging.DefaultInstance.SendAsync(message);
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
        /// Method to create a singleton instance of the firebase app.
        /// </summary>
        private void Create()
        {
            var credentialFile =
                this.cfg.GetValue<string>("Firebase:CREDENTIAL_FILE");
            FirebaseApp.Create(new AppOptions()
            {
                Credential =
                    GoogleCredential.FromFile(credentialFile),
            });
            this.logger.LogDebug("Created firebase app instance");
        }
    }
}
