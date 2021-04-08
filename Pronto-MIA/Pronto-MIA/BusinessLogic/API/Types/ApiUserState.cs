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
        /// <param name="userId">Id of the user.</param>
        /// <param name="userName">Name of the user.</param>
        public ApiUserState(string userId, string userName)
        {
            this.UserId = int.Parse(userId);
            this.UserName = userName;
        }

        /// <summary>
        /// Gets name of the user.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets id of the user.
        /// </summary>
        public int UserId { get; }
    }
}
