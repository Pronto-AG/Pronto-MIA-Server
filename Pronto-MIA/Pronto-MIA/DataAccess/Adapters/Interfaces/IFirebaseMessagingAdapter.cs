namespace Pronto_MIA.DataAccess.Adapters.Interfaces
{
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;

    /// <summary>
    /// Interface with all methods needed from firebase messaging.
    /// </summary>
    public interface IFirebaseMessagingAdapter
    {
        /// <summary>
        /// Sends the message to the devices specified within the message.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <returns>A task that completes with a message ID string, which
        /// represents successful handoff to FCM.</returns>
        public Task<string> SendAsync(Message message);

        /// <summary>
        /// Sends the given multicast message to all the FCM registration tokens
        /// specified in it.
        /// </summary>
        /// <exception
        /// cref="T:FirebaseAdmin.Messaging.FirebaseMessagingException">If an
        /// error occurs while sending the messages.</exception>
        /// <param name="message">The message to be sent. Must not be null.
        /// </param>
        /// <returns>A <see cref="T:FirebaseAdmin.Messaging.BatchResponse" />
        /// containing details of the batch operation's outcome.</returns>
        public Task<BatchResponse> SendMulticastAsync(MulticastMessage message);
    }
}
