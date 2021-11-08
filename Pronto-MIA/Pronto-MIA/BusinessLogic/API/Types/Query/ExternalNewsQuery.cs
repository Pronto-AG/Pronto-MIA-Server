namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="ExternalNews"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class ExternalNewsQuery
    {
        /// <summary>
        /// Method which retrieves the available external news.
        /// </summary>
        /// <param name="externalNewsManager">The external news manager
        /// responsible for managing external news.</param>
        /// <returns>Queryable of all external news available to the user.
        /// </returns>
        [UseFiltering]
        [UseSorting]
        public IQueryable<ExternalNews> ExternalNews(
            [Service] IExternalNewsManager externalNewsManager)
        {
            return externalNewsManager.GetAll();
        }
    }
}
