using Pronto_MIA.Domain.Entities;

#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types
{
    /// <summary>
    /// Class representing the state of an api user.
    /// </summary>
    public class ApiUserState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUserState"/> class.
        /// </summary>
        /// <param name="user">The user currently using the api.</param>
        public ApiUserState(User user)
        {
            this.User = user;
        }

        /// <summary>
        /// Gets the user currently working with the api.
        /// </summary>
        public User User { get; }
    }
}
