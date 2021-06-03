namespace Pronto_MIA.BusinessLogic.API.Interceptors
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
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
        /// <param name="executor">The executor of the current request.</param>
        /// <param name="builder">The query request builder which will be
        /// extended by the <see cref="ApiUserState"/>.</param>
        /// <param name="token">The cancellation token of the request.</param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="QueryException">Throws a query exception if
        /// the user could not be found or the token used for authentication
        /// has been invalidated.</exception>
        public static async ValueTask AddUserState(
            IConfiguration cfg,
            HttpContext context,
            IRequestExecutor executor,
            IQueryRequestBuilder builder,
            CancellationToken token)
        {
            ApiUserState apiUserState = default;
            var identity = context.User.Identity;
            if (identity is { IsAuthenticated: true })
            {
                var user = await GetUser(cfg, context);
                CheckIfTokenRevoked(context, user.LastInvalidated);
                apiUserState = new ApiUserState(user);
            }

            builder.SetProperty(
                ApiUserGlobalState.ApiUserStateName,
                apiUserState);
        }

        private static async Task<User> GetUser(
            IConfiguration cfg,
            HttpContext context)
        {
            await using (var dbContext = new ProntoMiaDbContext(
                DatabaseService.GetOptions(cfg)))
            {
                var userId = int.Parse(context
                    .User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userName = context
                    .User.FindFirstValue(ClaimTypes.Name);
                var user = dbContext.Users
                    .Include(u => u.Department)
                    .Include(u => u.AccessControlList)
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
                throw new AuthenticationException();
            }
        }

        private static GraphQLException GetAuthorizationException()
        {
            return new (
                ErrorBuilder.New()
                    .SetMessage("The current user is not authorized " +
                                "to access this resource.")
                    .SetCode("AUTH_NOT_AUTHORIZED").Build());
        }
    }
}
