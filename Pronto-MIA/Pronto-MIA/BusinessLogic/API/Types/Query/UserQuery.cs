namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="User"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class UserQuery
    {
        /// <summary>
        /// Method which allows the user to retrieve a token which may then be
        /// used for authentication in further requests.
        /// </summary>
        /// <param name="userManager">The manager responsible for user
        /// operations.</param>
        /// <param name="userName">The name of the user wanting to authenticate.
        /// </param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A JWT-Bearer-Token which can be used within the
        /// authentication header in order to authenticate the user.</returns>
        /// <exception cref="QueryException">
        /// If the password argument does not match the
        /// users current password a WrongPassword exception will be thrown.
        /// </exception>
        [Sensitive("password")]
        public async Task<string> Authenticate(
            [Service] IUserManager userManager,
            string userName,
            string password)
        {
            return await userManager.Authenticate(userName, password);
        }

        /// <summary>
        /// Method which returns the user making the request.
        /// </summary>
        /// <param name="userManager">The manager responsible for user
        /// operations.</param>
        /// <param name="userState">Provides information about the user
        /// requesting this endpoint.</param>
        /// <returns>The user requesting this endpoint.</returns>
        [Authorize]
        public async Task<User> User(
            [Service] IUserManager userManager,
            [ApiUserGlobalState] ApiUserState userState)
        {
            return await userManager.GetById(userState.User.Id);
        }

        /// <summary>
        /// Method which retrieves the available users. Depending
        /// on the requesting users access rights only a fraction of the
        /// available users might be returned.
        /// </summary>
        /// <param name="userManager">The user manager responsible for
        /// managing application users.</param>
        /// <param name="userState">Provides information about the user
        /// requesting this endpoint.</param>
        /// <returns>Queryable of all users available to the requesting
        /// user.</returns>
        [Authorize(Policy = "ViewUser")]
        [AccessObjectIdArgument("IGNORED")]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> Users(
            [Service] IUserManager userManager,
            [ApiUserGlobalState] ApiUserState userState)
        {
            if (userState.User.AccessControlList.CanViewUsers)
            {
                return userManager.GetAll();
            }

            return userManager.GetAll().Where(
                u => u.DepartmentId == userState.User.DepartmentId);
        }
    }
}
