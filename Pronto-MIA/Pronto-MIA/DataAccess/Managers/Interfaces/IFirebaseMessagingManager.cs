#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Execution;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of a firebase messaging
    /// manager service.
    /// </summary>
    public interface IFirebaseMessagingManager
    {
        /// <summary>
        /// Sends the given message with firebase messaging to the device
        /// specified within the message.
        /// </summary>
        /// <param name="message">Firebase messaging message object which
        /// contains the information required to send a message.</param>
        /// <returns>True if sending was successful.</returns>
        /// <exception cref="QueryException">If an error occured with
        /// the firebase operation.</exception>
        public Task<bool> SendAsync(Message message);

        /// <summary>
        /// Sends the given multicast message to all the FCM registration tokens
        /// specified in it. It also removes all tokens that are no longer valid
        /// from the fcmToken storage.
        /// </summary>
        /// <param name="tokens">The device tokens the message should be sent
        /// to. If there are more than 500 target devices the message will be
        /// sent in batches a 500 devices.
        /// </param>
        /// <param name="notification">The notification to be included
        /// in the message.</param>
        /// <param name="data">The data to be included in the message.</param>
        /// <returns>True if sending was successful.</returns>
        /// <exception cref="QueryException">If a global error occured with
        /// the firebase operation. If some devices could not be reached no
        /// error is thrown.</exception>
        public Task<bool> SendMulticastAsync(
            List<string> tokens,
            Notification notification,
            Dictionary<string, string> data);

        /// <summary>
        /// Method to register a new fcm token for a user. If the token is
        /// already registered the owner will be adjusted.
        /// </summary>
        /// <param name="user"> The owner of the fcm token.
        /// </param>
        /// <param name="fcmToken">The token to be added.</param>
        /// <returns>The created fcm token.</returns>
        public Task<IQueryable<FcmToken>> RegisterFcmToken(
            User user, string fcmToken);

        /// <summary>
        /// Method to remove a fcm token from the database.
        /// </summary>
        /// <param name="fcmToken">The token to be removed.</param>
        /// <returns>True if the token could be removed false if the token did
        /// not exist.</returns>
        public Task<bool> UnregisterFcmToken(string fcmToken);

        /// <summary>
        /// Method to retrieve all currently registered fcm tokens.
        /// </summary>
        /// <returns>The IQueryable of all available fcm tokens.</returns>
        public IQueryable<FcmToken> GetAllFcmToken();
    }
}
