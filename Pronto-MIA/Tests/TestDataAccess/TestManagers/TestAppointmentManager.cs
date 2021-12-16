namespace Tests.TestDataAccess.TestManagers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
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
    public class TestAppointmentManager
    {
        private readonly AppointmentManager appointmentManager;
        private readonly ProntoMiaDbContext dbContext;

        public TestAppointmentManager()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);

            this.appointmentManager = new AppointmentManager(
                this.dbContext,
                Substitute.For<ILogger<AppointmentManager>>());
        }

        [Fact]
        public async Task TestCreate()
        {
            var appointment = await this.appointmentManager.Create(
                "Title",
                "Location",
                DateTime.MinValue,
                DateTime.MaxValue,
                false,
                false);

            appointment = await this.dbContext.Appointments
                .FirstOrDefaultAsync(
                    dP => dP.Id == appointment.Id);
            Assert.NotNull(appointment);
            Assert.Equal(DateTime.MinValue, appointment.From);
            Assert.Equal(DateTime.MaxValue, appointment.To);
            Assert.Equal("Title", appointment.Title);
            Assert.Equal("Location", appointment.Location);
            Assert.False(appointment.IsAllDay);
            Assert.False(appointment.IsYearly);

            this.dbContext.Appointments.Remove(appointment);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestCreateEmptyLocation()
        {
            var appointment = await this.appointmentManager.Create(
                "Title",
                string.Empty,
                DateTime.MinValue,
                DateTime.MaxValue,
                false,
                false);

            appointment = await this.dbContext.Appointments
                .FirstOrDefaultAsync(
                    dP => dP.Id == appointment.Id);
            Assert.NotNull(appointment);
            Assert.Equal(DateTime.MinValue, appointment.From);
            Assert.Equal(DateTime.MaxValue, appointment.To);
            Assert.Equal("Title", appointment.Title);
            Assert.False(appointment.IsAllDay);
            Assert.False(appointment.IsYearly);
            Assert.Equal(string.Empty, appointment.Location);

            this.dbContext.Appointments.Remove(appointment);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        [SuppressMessage(
        "Menees.Analyzers",
        "MEN003",
        Justification = "Test may be more than 30 lines.")]
        public async Task TestUpdateFrom()
        {
            var appointment = this.GetSampleAppointment();
            this.dbContext.Appointments.Add(appointment);
            await this.dbContext.SaveChangesAsync();
            var newFrom = DateTime.UtcNow;

            await this.appointmentManager.Update(
                appointment.Id, null, null, newFrom, null, false, false);

            Assert.Equal(newFrom, appointment.From);
            Assert.Equal(
                this.GetSampleAppointment().Title,
                appointment.Title);
            Assert.Equal(
                this.GetSampleAppointment().Location,
                appointment.Location);
            Assert.Equal(
                this.GetSampleAppointment().To,
                appointment.To);
            Assert.Equal(
                this.GetSampleAppointment().IsAllDay,
                appointment.IsAllDay);
            Assert.Equal(
                this.GetSampleAppointment().IsYearly,
                appointment.IsYearly);

            this.dbContext.Remove(appointment);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        [SuppressMessage(
        "Menees.Analyzers",
        "MEN003",
        Justification = "Test may be more than 30 lines.")]
        public async Task TestUpdateTo()
        {
            var appointment = this.GetSampleAppointment();
            this.dbContext.Appointments.Add(appointment);
            await this.dbContext.SaveChangesAsync();
            var newTo = DateTime.UtcNow;

            await this.appointmentManager.Update(
                appointment.Id, null, null, null, newTo, false, false);

            Assert.Equal(newTo, appointment.To);
            Assert.Equal(
                this.GetSampleAppointment().Title,
                appointment.Title);
            Assert.Equal(
                this.GetSampleAppointment().Location,
                appointment.Location);
            Assert.Equal(
                this.GetSampleAppointment().From,
                appointment.From);
            Assert.Equal(
                this.GetSampleAppointment().IsAllDay,
                appointment.IsAllDay);
            Assert.Equal(
                this.GetSampleAppointment().IsYearly,
                appointment.IsYearly);

            this.dbContext.Remove(appointment);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        [SuppressMessage(
        "Menees.Analyzers",
        "MEN003",
        Justification = "Test may be more than 30 lines.")]
        public async Task TestUpdateLocation()
        {
            var appointment = this.GetSampleAppointment();
            this.dbContext.Appointments.Add(appointment);
            await this.dbContext.SaveChangesAsync();
            var newLocation = "New";

            await this.appointmentManager.Update(
                appointment.Id, null, newLocation, null, null, false, false);

            Assert.Equal(
                this.GetSampleAppointment().From,
                appointment.From);
            Assert.Equal(
                this.GetSampleAppointment().To,
                appointment.To);
            Assert.Equal(
                this.GetSampleAppointment().Title,
                appointment.Title);
            Assert.Equal(
                this.GetSampleAppointment().IsAllDay,
                appointment.IsAllDay);
            Assert.Equal(
                this.GetSampleAppointment().IsYearly,
                appointment.IsYearly);
            Assert.Equal(newLocation, appointment.Location);

            this.dbContext.Remove(appointment);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUpdateInvalidAppointment()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.appointmentManager.Update(
                        -5, null, null, null, null, false, false));
            Assert.Equal(
                Error.AppointmentNotFound.ToString(),
                error.Errors[0].Code);
        }

        [Fact]
        public async Task TestRemove()
        {
            var appointment = this.GetSampleAppointment();
            this.dbContext.Appointments.Add(appointment);
            await this.dbContext.SaveChangesAsync();

            await this.appointmentManager.Remove(appointment.Id);

            Assert.Null(
                await this.dbContext.Appointments
                    .FirstOrDefaultAsync(dP => dP.Id == appointment.Id));
        }

        [Fact]
        public async Task TestRemoveInvalid()
        {
            var error = await Assert.ThrowsAsync<QueryException>(
                async () =>
                    await this.appointmentManager.Remove(int.MaxValue));
            Assert.Equal(
                Error.AppointmentNotFound.ToString(), error.Errors[0].Code);
        }

        [Fact]
        public async Task TestGetAllSingle()
        {
            var appointment = this.GetSampleAppointment();
            this.dbContext.Appointments.Add(appointment);
            await this.dbContext.SaveChangesAsync();

            var appointments = this.appointmentManager.GetAll();
            Assert.Equal(1, await appointments.CountAsync());

            this.dbContext.Remove(appointment);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllMultiple()
        {
            var appointment = this.GetSampleAppointment();
            var appointment2 = this.GetSampleAppointment();
            this.dbContext.Appointments.Add(appointment);
            this.dbContext.Appointments.Add(appointment2);
            await this.dbContext.SaveChangesAsync();

            var appointments = this.appointmentManager.GetAll();
            Assert.Equal(2, await appointments.CountAsync());

            this.dbContext.Remove(appointment);
            this.dbContext.Remove(appointment2);
            await this.dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task TestGetAllEmpty()
        {
            this.dbContext.Appointments.RemoveRange(
                this.dbContext.Appointments);
            await this.dbContext.SaveChangesAsync();

            var appointments = this.appointmentManager.GetAll();
            Assert.Empty(appointments);

            TestDataProvider.InsertTestData(this.dbContext);
        }

        private Appointment GetSampleAppointment()
        {
            return new Appointment(
                "Title",
                "Location",
                DateTime.MinValue,
                DateTime.MaxValue,
                false,
                false);
        }
    }
}