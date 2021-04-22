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
    }
}
