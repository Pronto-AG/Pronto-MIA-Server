#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of an appointment within the
    /// application.
    /// </summary>
    public class AppointmentManager : IAppointmentManager
    {
        private readonly ILogger logger;
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AppointmentManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// appointments regarding this manager.</param>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        public AppointmentManager(
            ProntoMiaDbContext dbContext,
            ILogger<AppointmentManager> logger)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        public void SetDbContext(ProntoMiaDbContext context)
        {
            this.dbContext = context;
        }

        /// <inheritdoc/>
        public async Task<Appointment>
            Create(
                string title,
                string location,
                DateTime from,
                DateTime to,
                bool isAllDay,
                bool isYearly)
        {
            var appointment = new Appointment(
                title,
                location,
                from,
                to,
                isAllDay,
                isYearly);
            this.dbContext.Appointments.Add(appointment);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "Appointment with id {Id} has been created",
                appointment.Id);
            return appointment;
        }

        /// <inheritdoc/>
        public async Task<Appointment>
            Update(
                int id,
                string? title,
                string? location,
                DateTime? from,
                DateTime? to,
                bool isAllDay,
                bool isYearly)
        {
            var appointment = await this.GetById(id);

            appointment = this.UpdateTitle(
                appointment, title);
            appointment = this.UpdateLocation(
                appointment, location);

            appointment = this.UpdateTimes(
                appointment, from, to);
            appointment = this.UpdateIsAllDay(
                appointment, isAllDay);
            appointment = this.UpdateIsYearly(
                appointment, isYearly);
            await this.dbContext.SaveChangesAsync();
            this.logger.LogInformation(
                "Appointment with id {Id} has been updated",
                appointment.Id);
            return appointment;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var appointment = await this.GetById(id);

            this.dbContext.Remove(appointment);
            await this.dbContext.SaveChangesAsync();

            return id;
        }

        /// <inheritdoc/>
        public IQueryable<Appointment> GetAll()
        {
            return this.dbContext.Appointments;
        }

        /// <inheritdoc/>
        public async Task<Appointment> GetById(int id)
        {
            var appointment = await this.dbContext.Appointments
                .SingleOrDefaultAsync(e => e.Id == id);
            if (appointment != default)
            {
                return appointment;
            }

            this.logger.LogWarning(
                "Invalid appointment id {Id}", id);
            throw DataAccess.Error.AppointmentNotFound
                .AsQueryException();
        }

        /// <summary>
        /// Checks if the from time is smaller that the
        /// to value.
        /// </summary>
        /// <param name="from">Appointment from datetime.</param>
        /// <param name="to">Appointment to datetime.</param>
        /// <exception cref="QueryException">If the from time is
        /// after the to time.</exception>
        private static void CheckTimePlausibility(
            DateTime from,
            DateTime to)
        {
            if (from < to)
            {
                return;
            }

            throw Error.AppointmentImpossibleTime.AsQueryException();
        }

        private Appointment UpdateLocation(
            Appointment appointment, string? location)
        {
            if (location != null)
            {
                appointment.Location =
                    location == string.Empty ? null : location;
            }

            return appointment;
        }

        private Appointment UpdateTitle(
            Appointment appointment, string? title)
        {
            if (title != null)
            {
                appointment.Title =
                    title == string.Empty ? null : title;
            }

            return appointment;
        }

        private Appointment UpdateIsAllDay(
            Appointment appointment, bool isAllDay)
        {
            appointment.IsAllDay = isAllDay;
            return appointment;
        }

        private Appointment UpdateIsYearly(
            Appointment appointment, bool isYearly)
        {
            appointment.IsYearly = isYearly;
            return appointment;
        }

        /// <summary>
        /// Updates the times within the given appointment to the new
        /// values.
        /// </summary>
        /// <param name="appointment">The appointment to be updated.
        /// </param>
        /// <param name="from">The new from datetime.</param>
        /// <param name="to">The new to datetime.
        /// </param>
        private Appointment UpdateTimes(
            Appointment appointment,
            DateTime? from,
            DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                CheckTimePlausibility(
                    from.Value, to.Value);
                appointment.From = from.Value;
                appointment.To = to.Value;
            }
            else if (from.HasValue)
            {
                CheckTimePlausibility(
                    from.Value, appointment.To);
                appointment.From = (DateTime)from;
            }
            else if (to.HasValue)
            {
                CheckTimePlausibility(
                    appointment.From, to.Value);
                appointment.To = to.Value;
            }

            return appointment;
        }
    }
}
