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
    /// Contains all mutations that concern <see cref="InternalNews"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class InternalNewsQuery
    {
        /// <summary>
        /// Method which retrieves the available internal news.
        /// </summary>
        /// <param name="internalNewsManager">The internal news manager
        /// responsible for managing internal news.</param>
        /// <returns>Queryable of all internal news available to the user.
        /// </returns>
        [UseFiltering]
        [UseSorting]
        public IQueryable<InternalNews> InternalNews(
            [Service] IInternalNewsManager internalNewsManager)
        {
            return internalNewsManager.GetAll();
        }
    }
}
