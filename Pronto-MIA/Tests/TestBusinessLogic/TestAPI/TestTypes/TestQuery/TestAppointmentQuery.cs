#nullable enable
namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.BusinessLogic.API.Types.Query;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestAppointmentQuery
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly AppointmentQuery appointmentQuery;

        public TestAppointmentQuery()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.appointmentQuery = new AppointmentQuery();
        }

        [Fact]
        public async Task TestAppointment()
        {
            var appointmentManager =
                Substitute.For<IAppointmentManager>();
            appointmentManager.GetAll().Returns(
                this.dbContext.Appointments);
            var appointmentCount =
                await this.dbContext.Appointments.CountAsync();

            var result = this.appointmentQuery.Appointments(
               appointmentManager);

            appointmentManager.Received().GetAll();
            Assert.Equal(appointmentCount, await result.CountAsync());

            await this.dbContext.SaveChangesAsync();
        }
    }
}