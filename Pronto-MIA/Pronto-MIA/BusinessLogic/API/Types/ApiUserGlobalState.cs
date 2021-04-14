#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types
{
    using HotChocolate;

    /// <summary>
    /// Class to persist the api user state within the hot chocolate global
    /// state attribute.
    /// </summary>
    public class ApiUserGlobalState : GlobalStateAttribute
    {
        /// <summary>
        /// The identifier where the state is stored within the global state
        /// of HotChocolate.
        /// </summary>
        public const string ApiUserStateName = "userState";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUserGlobalState"/>
        /// class.
        /// </summary>
        public ApiUserGlobalState()
            : base(ApiUserStateName)
        {
        }
    }
}
