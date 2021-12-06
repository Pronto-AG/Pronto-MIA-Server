namespace Pronto_MIA.Domain.Entities
{
    using System;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;

    /// <summary>
    /// Class which represents an external news object.
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mail"/>
        /// class.
        /// </summary>
        /// <param name="subject">The
        /// <see cref="Subject"/> property.</param>
        /// <param name="content">The
        /// <see cref="Content"/> property.</param>
        public Mail(
            string subject,
            string content)
        {
            this.Subject = subject;
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalNews"/>
        /// class. This constructor is only used for HotChocolate so that
        /// graphql is able to generate an object. This
        /// constructor should not be manually called.
        /// </summary>
        protected Mail()
        {
            this.Subject = null;
            this.Content = null;
        }

        /// <summary>
        /// Gets or sets the subject of the mail to send.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the content of the mail to send.
        /// </summary>
        public string Content { get; set; }
    }
}
