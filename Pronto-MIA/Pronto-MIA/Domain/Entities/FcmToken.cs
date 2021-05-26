namespace Pronto_MIA.Domain.Entities
{
    /// <summary>
    /// Token representing a device registered with firebase cloud messaging.
    /// </summary>
    public class FcmToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FcmToken"/> class.
        /// </summary>
        /// <param name="id">The token itself.</param>
        /// <param name="owner">The <see cref="User"/>
        /// the token belongs to.</param>
        public FcmToken(string id, User owner)
        {
            this.Id = id;
            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FcmToken"/> class.
        /// Used for entity framework construction.
        /// </summary>
        protected FcmToken()
        {
            this.Id = default;
            this.Owner = default;
        }

        /// <summary>
        /// Gets or sets the token identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this token.
        /// </summary>
        public virtual User Owner { get; set; }
    }
}
