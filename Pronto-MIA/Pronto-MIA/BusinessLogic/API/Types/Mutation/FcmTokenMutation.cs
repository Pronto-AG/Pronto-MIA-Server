using Pronto_MIA.DataAccess;

namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="FcmToken"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class FcmTokenMutation
    {
        /// <summary>
        /// Registers a fcm token for the authenticated user. If the token
        /// already exists it will be overwritten with the currently
        /// authenticated user.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure that the user can be changed
        /// in all managers.</param>
        /// <param name="userManager">The manager responsible
        /// for managing application users.</param>
        /// <param name="firebaseTokenManager">The manager responsible for
        /// firebase token operations.</param>
        /// <param name="userState">Information about the current user.</param>
        /// <param name="fcmToken">The token to be registered.</param>
        /// <returns>True if the token was saved successfully.</returns>
        [Authorize]
        [UseSingleOrDefault]
        [UseProjection]
        [Sensitive("fcmToken")]
        public async Task<IQueryable<FcmToken>> RegisterFcmToken(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IUserManager userManager,
            [Service] IFirebaseTokenManager firebaseTokenManager,
            [ApiUserGlobalState] ApiUserState userState,
            string fcmToken)
        {
            userManager.SetDbContext(dbContext);
            firebaseTokenManager.SetDbContext(dbContext);

            var user = await userManager.GetById(userState.UserId);
            return await firebaseTokenManager.RegisterFcmToken(
                    user, fcmToken);
        }

        /// <summary>
        /// Unregisters a given fcm token from the user it was assigned to.
        /// If the token cannot be found nothing will be done.
        /// </summary>
        /// <param name="firebaseTokenManager">The manager managing
        /// operations with firebase tokens.
        /// </param>
        /// <param name="fcmToken">The fcm token to be removed.</param>
        /// <returns>True if the token could be removed.</returns>
        public async Task<bool> UnregisterFcmToken(
            [Service] IFirebaseTokenManager firebaseTokenManager,
            string fcmToken)
        {
            return await firebaseTokenManager.UnregisterFcmToken(fcmToken);
        }
    }
}
