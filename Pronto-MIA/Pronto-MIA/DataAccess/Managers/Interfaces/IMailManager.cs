#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using MailKit.Net.Smtp;
    using MimeKit;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of an mail manager
    /// service.
    /// </summary>
    public interface IMailManager
    {
        /// <summary>
        /// Method which sends the mail.
        /// </summary>
        /// <param name="subject">The subject of the mail to send.</param>
        /// <param name="content">The content of the mail to send.</param>
        /// <returns>The [bool] true if mail was send successfully.</returns>
        public Task<bool> Send(
                string subject, string content);
    }
}
