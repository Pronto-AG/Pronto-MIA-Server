#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of a firebase token
    /// manager service.
    /// </summary>
    public interface IFirebaseTokenManager
    {
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
        /// Method that removes multiple tokens from the database.
        /// </summary>
        /// <param name="tokenIds">A hashset containing the ids of the tokens
        /// to be deleted.</param>
        /// <returns>A task that can be awaited.</returns>
        public Task UnregisterMultipleFcmToken(HashSet<string> tokenIds);

        /// <summary>
        /// Method to retrieve all currently registered fcm tokens.
        /// </summary>
        /// <returns>The IQueryable of all available fcm tokens.</returns>
        public IQueryable<FcmToken> GetAllFcmToken();
    }
}