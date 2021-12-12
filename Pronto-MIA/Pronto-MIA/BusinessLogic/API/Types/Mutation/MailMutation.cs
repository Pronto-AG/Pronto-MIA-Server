#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern to the mail to send.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class MailMutation
    {
        /// <summary>
        /// Adds an external news to the application.
        /// </summary>
        /// <param name="mailManager">The manager responsible for
        /// managing external news.</param>
        /// <param name="subject">Short description to identify the
        /// sender.</param>
        /// <param name="content">The content of the mail.</param>
        /// <returns>The newly mail.</returns>
        // [Authorize(Policy = "EditExternalNews")]
        [UseSingleOrDefault]
        public async Task<bool> SendMail(
            [Service] IMailManager mailManager,
            string subject,
            string content)
        {
                var mail = await mailManager.Send(
                    subject,
                    content);
                return mail;
        }
    }
}
