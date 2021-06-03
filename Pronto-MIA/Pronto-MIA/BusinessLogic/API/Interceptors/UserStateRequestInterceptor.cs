namespace Pronto_MIA.BusinessLogic.API.Interceptors
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Pronto_MIA.Services;

    /// <summary>
    /// Class containing the interceptor method used to add the
    /// <see cref="ApiUserState"/> to every request.
    /// </summary>
    public static class UserStateRequestInterceptor
    {
        /// <summary>
        /// Adds <see cref="AddUserState"/> to every request so that
        /// information on the requesting user is available if necessary.
        /// Also checks if the user is existing and allowed to authenticate.
        /// </summary>
        /// <param name="cfg">The configuration of the application used
        /// to create a dbContext.</param>
        /// <param name="context">The httpContext of the current request.
        /// </param>
        /// <param name="builder">The query request builder which will be
        /// extended by the <see cref="ApiUserState"/>.</param>
        /// <param name="dbContext">The database context to be used if no new
        /// context should be created. Primarily used for testing.</param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="QueryException">Throws a query exception if
        /// the user could not be found or the token used for authentication
        /// has been invalidated.</exception>
        public static async ValueTask AddUserState(
            IConfiguration cfg,
            HttpContext context,
            IQueryRequestBuilder builder,
            ProntoMiaDbContext dbContext = null)
        {
            ApiUserState apiUserState = default;
            var identity = context.User.Identity;
            if (identity is { IsAuthenticated: true })
            {
                var user = await GetUser(cfg, context, dbContext);
                CheckIfTokenRevoked(context, user.LastInvalidated);
                apiUserState = new ApiUserState(user.Id, user.UserName);
            }

            builder.SetProperty(
                ApiUserGlobalState.ApiUserStateName,
                apiUserState);
        }

        private static async Task<User> GetUser(
            IConfiguration cfg,
            HttpContext context,
            ProntoMiaDbContext dbContext = null)
        {
            dbContext ??= new ProntoMiaDbContext(
                DatabaseService.GetOptions(cfg));

            await using (dbContext)
            {
                var userId = int.Parse(context
                    .User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = dbContext.Users
                    .SingleOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    throw GetAuthorizationException();
                }

                return user;
            }
        }

        private static void CheckIfTokenRevoked(
            HttpContext context,
            DateTime lastInvalidated)
        {
            var issuedAt = context
                .User.FindFirstValue("issuedAt");
            if (issuedAt == null)
            {
                throw GetAuthorizationException();
            }

            var issuedAtTime = DateTime.Parse(
                issuedAt, CultureInfo.InvariantCulture);
            if (lastInvalidated > issuedAtTime)
            {
                throw GetAuthorizationException(
                    "The token used has been previously invalidated.");
            }
        }

        private static GraphQLException GetAuthorizationException(
            string message = null)
        {
            if (message == null)
            {
                message = "The current user is not authorized " +
                          "to access this resource.";
            }

            return new (
                ErrorBuilder.New()
                    .SetMessage(message)
                    .SetCode("AUTH_NOT_AUTHORIZED").Build());
        }
    }
}
