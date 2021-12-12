#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestMailQuery
    {
        private readonly MailQuery mailQuery;

        public TestMailQuery()
        {
            this.mailQuery = new MailQuery();
        }

        [Fact]
        public async void TestSendMail()
        {
            var mailManager = Substitute.For<IMailManager>();

            await this.mailQuery.Mail(
                mailManager, "Subject", "Content");

            await mailManager.Received().Send("Subject", "Content");
        }
    }
}