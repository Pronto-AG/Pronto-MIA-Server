namespace Pronto_MIA.DataAccess.Adapters
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using Pronto_MIA.DataAccess.Adapters.Interfaces;

    /// <summary>
    /// Class representing the wrapping adapter for firebase messaging.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FirebaseMessagingAdapter : IFirebaseMessagingAdapter
    {
        private readonly FirebaseMessaging instance;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="FirebaseMessagingAdapter"/> class. This class is used
        /// as a wrapper around firebase messaging in order to get testable
        /// code.
        /// </summary>
        /// <param name="instance">The instance of the firebase messaging
        /// service which should be wrapped.</param>
        public FirebaseMessagingAdapter(FirebaseMessaging instance)
        {
            this.instance = instance;
        }

        /// <inheritdoc/>
        public Task<string> SendAsync(Message message)
        {
            return this.instance.SendAsync(message);
        }

        /// <inheritdoc/>
        public Task<BatchResponse> SendMulticastAsync(MulticastMessage message)
        {
            return this.instance.SendMulticastAsync(message);
        }
    }
}
