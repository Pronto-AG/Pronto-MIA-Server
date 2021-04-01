using System;

#nullable enable
namespace Pronto_MIA.BusinessLogic.API
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the query operation of graphql.
    /// </summary>
    public class Query
    {
        #pragma warning disable SA1642
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="cfg">The configuration used by the query class.</param>
        #pragma warning restore SA1642
        public Query(IConfiguration cfg)
        {
            this.Cfg = cfg;
        }

        private IConfiguration Cfg { get; }

        /// <summary>
        /// Method to get speakers from the database.
        /// </summary>
        /// <param name="context">The database context to get the speakers from.
        /// </param>
        /// <returns>An IQueryable containing the speakers.</returns>
        [Authorize]
        public IQueryable<Speaker> GetSpeakers(
            [Service] ProntoMIADbContext context) => context.Speakers;

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
        /// <exception cref="QueryException">If a problem occured during
        /// authentication.</exception>
        public string? Authenticate(
            [Service] UserManager userManager,
            string userName,
            string password)
        {
            var checkUser = userManager.Authenticate(userName, password);
            if (checkUser.Item1 != null)
            {
                throw ((DataAccess.Error)checkUser.Item1).AsQueryException();
            }

            return checkUser.Item2;
        }

        /// <summary>
        /// Method to get a pdf from the server.
        /// </summary>
        /// <returns>Link to the pdf.</returns>
        [Authorize]
        public DeploymentPlan GetPdf()
        {
            var dpl = new DeploymentPlan
            {
                Link = new Uri("https://localhost:5001/StaticFiles/upload.pdf"),
            };
            return dpl;
        }
    }
}

