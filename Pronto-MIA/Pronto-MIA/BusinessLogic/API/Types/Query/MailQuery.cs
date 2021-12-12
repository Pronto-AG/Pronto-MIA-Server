namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using System.Threading.Tasks;
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
    public class MailQuery
    {
        /// <summary>
        /// Method which retrieves the mail to send.
        /// </summary>
        /// <param name="mailManager">The mail manager
        /// responsible for managing mails.</param>
        /// <param name="subject">The subject
        /// for the mail to send.</param>
        /// <param name="content">The content
        /// for the mail to send.</param>
        /// <returns>Queryable of mail to send.
        /// </returns>
        [UseFiltering]
        [UseSorting]
        public async Task<bool> Mail(
            [Service] IMailManager mailManager,
            string subject,
            string content)
        {
            return await mailManager.Send(subject, content);
        }
    }
}
