#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using System.Threading.Tasks;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestMailMutation
    {
        private readonly MailMutation mailMutation;

        public TestMailMutation()
        {
            this.mailMutation = new MailMutation();
        }

        [Fact]
        public async Task TestSendMail()
        {
            var mailManager =
                Substitute.For<IMailManager>();

            await this.mailMutation.SendMail(mailManager, "Subject", "Content");

            await mailManager.Received()
                .Send("Subject", "Content");
        }
    }
}