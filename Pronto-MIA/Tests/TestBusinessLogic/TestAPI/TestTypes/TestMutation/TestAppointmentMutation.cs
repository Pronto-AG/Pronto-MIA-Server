namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestMutation
{
    using System;
    using System.Collections.Generic;
    using FirebaseAdmin.Messaging;
    using HotChocolate.Types;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.Types.Mutation;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestAppointmentMutation
    {
        private readonly ProntoMiaDbContext dbContext;
        private readonly AppointmentMutation appointmentMutation;

        public TestAppointmentMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.appointmentMutation = new AppointmentMutation();
        }

        [Fact]
        public async void TestCreateAppointment()
        {
            var appointmentManager =
                Substitute.For<IAppointmentManager>();

            await this.appointmentMutation.CreateAppointment(
                this.dbContext,
                appointmentManager,
                "Test",
                string.Empty,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false);

            appointmentManager.Received().SetDbContext(this.dbContext);
            await appointmentManager.ReceivedWithAnyArgs()
                .Create(default!, default, default, default, default, default);
        }

        [Fact]
        public async void TestUpdateAppointment()
        {
            var appointmentManager =
                Substitute.For<IAppointmentManager>();

            await this.appointmentMutation.UpdateAppointment(
                this.dbContext,
                appointmentManager,
                1,
                "Test",
                string.Empty,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false,
                false);

            appointmentManager.Received().SetDbContext(this.dbContext);
            await appointmentManager.ReceivedWithAnyArgs()
                .Update(
                    default,
                    default,
                    default,
                    default,
                    default,
                    default,
                    default);
        }

        [Fact]
        public async void TestRemoveAppointment()
        {
            var appointmentManager =
                Substitute.For<IAppointmentManager>();

            await this.appointmentMutation.RemoveAppointment(
                appointmentManager,
                1);

            await appointmentManager.Received()
                .Remove(1);
        }
    }
}