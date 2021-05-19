#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of a user manager service.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Method to authenticate a user.
        /// </summary>
        /// <param name="userName">The username of the user to be authenticated.
        /// </param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The generated JWT token the user can use in order to
        /// authenticate in further requests.</returns>
        /// <exception cref="QueryException">Throws if the user could not be
        /// found or has provided an invalid password.</exception>
        public Task<string> Authenticate(
            string userName, string password);

        /// <summary>
        /// Method that returns a User object for the given username.
        /// </summary>
        /// <param name="userName">The name of the user to be found.</param>
        /// <returns>User or default if user could not be found.</returns>
        public Task<User?>
            GetByUserName(string userName);

        /// <summary>
        /// Method that creates a new User object with the given Username
        /// and password.
        /// </summary>
        /// <param name="userName">The username for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>The newly created user.</returns>
        /// <exception cref="QueryException">Returns UserAlreadyExists exception
        /// if the username already exists. Alternatively returns
        /// PasswordTooWeak exception if the provided password does not meet the
        /// policy requirements.
        /// </exception>
        public Task<User>
            Create(string userName, string password);

        /// <summary>
        /// Method that updates the user with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="id">Id of the user to be updated.</param>
        /// <param name="userName">The new username of the user.</param>
        /// <param name="password">The new password for the user.</param>
        /// <returns>The updated user.</returns>
        /// <exception cref="QueryException">Returns UserNotFound
        /// exception if the user with the given id could not be found.
        /// Alternatively returns PasswordTooWeak exception if the
        /// provided password does not meet the policy requirements.
        /// </exception>
        public Task<User> Update(int id, string? userName, string? password);

        /// <summary>
        /// Removes the user with the given id.
        /// </summary>
        /// <param name="id">Id of the user to be removed.</param>
        /// <returns>The id of the user that was removed.</returns>
        /// <exception cref="QueryException">Returns UserNotFound
        /// exception if the user with the given id could not be found.
        /// </exception>
        public Task<int> Remove(int id);
    }
}
