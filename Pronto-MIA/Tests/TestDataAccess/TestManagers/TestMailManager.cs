namespace Tests.TestDataAccess.TestManagers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using MailKit.Net.Smtp;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using NSubstitute;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    [SuppressMessage(
        "Menees.Analyzers",
        "MEN005",
        Justification = "Test file may be more than 300 lines.")]
    public class TestMailManager
    {
        private readonly MailManager mailManager;

        public TestMailManager()
        {
            this.mailManager = new MailManager(
                TestHelpers.TestConfiguration,
                Substitute.For<ILogger<MailManager>>());
        }

        [Fact]
        public void TestGenerateMessage()
        {
            var expected = new MimeMessage();
            expected.From.Add(MailboxAddress.Parse("noreply@test.ch"));
            expected.To.Add(MailboxAddress.Parse("recipient@test.ch"));
            expected.Subject = "Subject";
            expected.Body = new TextPart("html") { Text = "Content" };
            var result =
                this.mailManager.GenerateMessage("Subject", "Content");
            Assert.NotNull(result);
            Assert.IsType<MimeMessage>(result);
            Assert.Equal(result.GetType(), expected.GetType());
            Assert.Equal(result.From.ToString(), expected.From.ToString());
            Assert.Equal(result.To.ToString(), expected.To.ToString());
            Assert.Equal(result.Subject, expected.Subject);
            Assert.Equal(result.Body.ToString(), expected.Body.ToString());
        }

        [Fact]
        public void TestSmtpAuthentication()
        {
            var client = new SmtpClient();
            var checkAuth = this.mailManager.SmtpAuthentication(client);
            Assert.IsType<bool>(checkAuth);
            Assert.False(checkAuth);
        }
    }
}