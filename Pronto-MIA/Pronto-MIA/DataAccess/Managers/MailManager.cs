#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a external news within the
    /// application.
    /// </summary>
    public class MailManager : IMailManager
    {
        private readonly IConfiguration cfg;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ExternalNewsManager"/> class.
        /// </summary>
        /// <param name="cfg">The configuration of this application.</param>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this manager.</param>
        public MailManager(IConfiguration cfg, ILogger<MailManager> logger)
        {
            this.cfg = cfg;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the smtp server.
        /// </summary>
        public string GetSmtpServer
        {
            get => this.cfg.GetValue<string>("Smtp:smtp_server");
        }

        /// <summary>
        /// Gets the username for the smtp server.
        /// </summary>
        public string GetSmtpUsername
        {
            get => this.cfg.GetValue<string>("Smtp:smtp_username");
        }

        /// <summary>
        /// Gets the password for the smtp server.
        /// </summary>
        public string GetSmtpPassword
        {
            get => this.cfg.GetValue<string>("Smtp:smtp_password");
        }

        /// <summary>
        /// Gets the port of the smtp server to send.
        /// </summary>
        public int GetSmtpPort
        {
            get => this.cfg.GetValue<int>("Smtp:smtp_port");
        }

        /// <summary>
        /// Gets the recipient of the mail to send.
        /// </summary>
        public string GetSmtpRecipient
        {
            get => this.cfg.GetValue<string>("Smtp:smtp_recipient");
        }

        /// <summary>
        /// Gets the sender of the mail to send.
        /// </summary>
        public string GetSmtpSender
        {
            get => this.cfg.GetValue<string>("Smtp:smtp_sender");
        }

        /// <summary>
        /// Generates the Message to send.
        /// </summary>
        /// <param name="subject">The subject of the message.</param>
        /// <param name="content">The content of the message.</param>
        /// <returns> [MimeMessage] message.</returns>
        public Task<MimeMessage> GenerateMessage(string subject, string content)
        {
            MimeMessage message = new ();
            message.From.Add(
                MailboxAddress.Parse(this.GetSmtpSender));
            message.To.Add(
                MailboxAddress.Parse(this.GetSmtpRecipient));
            message.Subject = subject;
            message.Body =
                new TextPart("html") { Text = content };
            return message;
        }

        /// <inheritdoc/>
        public async Task<bool> Send(string subject, string content)
        {
            MimeMessage message = await this.GenerateMessage(subject, content);

            using (var client = new SmtpClient())
            {
                if (this.SmtpConnection(client))
                {
                    await this.SendMail(message, client);
                }

                client.Disconnect(true);
                return true;
            }
        }

        /// <summary>
        /// Sends the generated message.
        /// </summary>
        /// <param name="message">The Message to send.</param>
        /// <param name="client">The smtp client from which the mail
        /// should be send.</param>
        /// <returns> [bool] indicates if the mail could be send.</returns>
        public async Task<bool> SendMail(
            MimeMessage message,
            SmtpClient client)
        {
            try
            {
                await client.SendAsync(message);
            }
            catch (SmtpCommandException ex)
            {
                Console.WriteLine("Error sending message: {0}", ex.Message);
                Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
                return false;
            }
            catch (SmtpProtocolException ex)
            {
                Console.WriteLine(
                    "Protocol error while sending message: {0}",
                    ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Established the connection to the smtp server.
        /// </summary>
        /// <param name="client">The smtp client from which the mail
        /// should be send.</param>
        /// <returns> [bool] indicates if the connection could be
        /// established.</returns>
        public bool SmtpConnection(SmtpClient client)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                      (sender, certificate, chain, sslPolicyErrors) => true;
                client.Connect(
                    this.GetSmtpServer,
                    this.GetSmtpPort,
                    SecureSocketOptions.SslOnConnect);
            }
            catch (SmtpCommandException ex)
            {
                Console.WriteLine(
                    "Error trying to connect: {0}", ex.Message);
                Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
                return false;
            }
            catch (SmtpProtocolException ex)
            {
                Console.WriteLine(
                    "Protocol error while trying to connect: {0}",
                    ex.Message);
                return false;
            }

            return this.SmtpAuthentication(client);
        }

        /// <summary>
        /// Authentication on the smtp server.
        /// </summary>
        /// <param name="client">The smtp client from which the mail
        /// should be send.</param>
        /// <returns> [bool] indicates if the authentication was
        /// successful.</returns>
        public bool SmtpAuthentication(SmtpClient client)
        {
            if (client.Capabilities.HasFlag(
                            SmtpCapabilities.Authentication))
            {
                try
                {
                    client.Authenticate(
                        this.GetSmtpUsername, this.GetSmtpPassword);
                }
                catch (AuthenticationException ex)
                {
                    Console.WriteLine("Invalid user name or password.", ex);
                    return false;
                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine(
                        "Error trying to authenticate: {0}", ex.Message);
                    Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
                    return false;
                }
                catch (SmtpProtocolException ex)
                {
                    Console.WriteLine(
                        "Protocol error while trying to authenticate: {0}",
                        ex.Message);
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
