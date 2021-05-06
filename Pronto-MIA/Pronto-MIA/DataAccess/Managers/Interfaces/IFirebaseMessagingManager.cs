#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Execution;

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
        /// specified in it.
        /// </summary>
        /// <param name="tokens">The device tokens the message should be sent
        /// to. If there are more than 500 target devices the message will be
        /// sent in batches a 500 devices.
        /// </param>
        /// <param name="notification">The notification to be included
        /// in the message.</param>
        /// <param name="data">The data to be included in the message.</param>
        /// <returns>A hashset containing the ids of all fcm registration tokens
        /// which could not be reached because they are not valid.</returns>
        /// <exception cref="QueryException">If a global error occured with
        /// the firebase operation. If some devices could not be reached no
        /// error is thrown.</exception>
        public Task<HashSet<string>> SendMulticastAsync(
            List<string> tokens,
            Notification notification,
            Dictionary<string, string> data);
    }
}
