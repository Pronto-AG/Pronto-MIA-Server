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
    /// Contains all mutations that concern <see cref="EducationalContent"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class EducationalContentQuery
    {
        /// <summary>
        /// Method which retrieves the available educational content.
        /// </summary>
        /// <param name="educationalContentManager">The educational content manager
        /// responsible for managing educational content.</param>
        /// <returns>Queryable of all educational content available to the user.
        /// </returns>
        [UseFiltering]
        [UseSorting]
        public IQueryable<EducationalContent> EducationalContent(
            [Service] IEducationalContentManager educationalContentManager)
        {
            return educationalContentManager.GetAll();
        }
    }
}
