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
        /// <param name="userId">The id of the user currently using the api.
        /// </param>
        /// <param name="userName">The username of the user currently using the
        /// api.</param>
        public ApiUserState(int userId, string userName)
        {
            this.UserId = userId;
            this.UserName = userName;
        }

        /// <summary>
        /// Gets the id of the user currently working with the api.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Gets the name of the user currently working with the api.
        /// </summary>
        public string UserName { get;  }
    }
}
