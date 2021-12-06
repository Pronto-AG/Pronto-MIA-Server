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

        /// <inheritdoc/>
        public async Task<bool> Send(string subject, string content)
        {
            var smtpServer =
                this.cfg.GetValue<string>("Smtp:smtp_server");
            var smtpUsername =
                 this.cfg.GetValue<string>("Smtp:smtp_username");
            var smtpPassword =
                 this.cfg.GetValue<string>("Smtp:smtp_password");
            var smtpPort =
                this.cfg.GetValue<int>("Smtp:smtp_port");
            var smtpRecipient =
                this.cfg.GetValue<string>("Smtp:smtp_recipient");
            var smtpSender =
                this.cfg.GetValue<string>("Smtp:smtp_sender");

            MimeMessage message = new ();
            message.From.Add(
                MailboxAddress.Parse(smtpSender));
            message.To.Add(
                MailboxAddress.Parse(smtpRecipient));
            message.Subject = subject;
            message.Body =
                new TextPart("html") { Text = content };

            using (var client = new SmtpClient())
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                          (sender, certificate, chain, sslPolicyErrors) => true;
                    client.Connect(
                        smtpServer,
                        smtpPort,
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

                if (client.Capabilities.HasFlag(
                    SmtpCapabilities.Authentication))
                {
                    try
                    {
                        client.Authenticate(
                            smtpUsername, smtpPassword);
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
                }

                try
                {
                    await client.SendAsync(message);
                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine("Error sending message: {0}", ex.Message);
                    Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);

                    switch (ex.ErrorCode)
                    {
                        case SmtpErrorCode.RecipientNotAccepted:
                            Console.WriteLine(
                                "\tRecipient not accepted: {0}", ex.Mailbox);
                            break;
                        case SmtpErrorCode.SenderNotAccepted:
                            Console.WriteLine(
                                "\tSender not accepted: {0}", ex.Mailbox);
                            break;
                        case SmtpErrorCode.MessageNotAccepted:
                            Console.WriteLine("\tMessage not accepted.");
                            break;
                    }
                }
                catch (SmtpProtocolException ex)
                {
                    Console.WriteLine(
                        "Protocol error while sending message: {0}",
                        ex.Message);
                }

                client.Disconnect(true);
                return true;
            }
        }
    }
}
