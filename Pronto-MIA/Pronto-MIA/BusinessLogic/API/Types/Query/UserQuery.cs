namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.Logging;
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
        [Sensitive("password")]
        public async Task<string> Authenticate(
            [Service] IUserManager userManager,
            string userName,
            string password)
        {
            return await userManager.Authenticate(userName, password);
        }

        /// <summary>
        /// Method which retrieves the available deployment plans.
        /// </summary>
        /// <param name="userManager">The user manager responsible for
        /// managing application users.</param>
        /// <returns>Queryable of all available users.</returns>
        [Authorize(Policy = "CanViewUsers")]
        [UseFiltering]
        [UseSorting]
        public IQueryable<User> Users(
            [Service] IUserManager userManager)
        {
            return userManager.GetAll();
        }
    }
}
