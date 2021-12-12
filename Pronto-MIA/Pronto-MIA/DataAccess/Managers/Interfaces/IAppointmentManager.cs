#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of an appointment manager
    /// service.
    /// </summary>
    public interface IAppointmentManager
    {
        /// <summary>
        /// Method to overwrite the <see cref="ProntoMiaDbContext"/>
        /// used by the manager. This can be used if transactions
        /// over multiple managers have to be implemented.
        /// </summary>
        /// <param name="dbContext">The db context to be used by the
        /// manager.</param>
        public void SetDbContext(ProntoMiaDbContext dbContext);

        /// <summary>
        /// Method which creates a new appointment object.
        /// </summary>
        /// <param name="title">A short description to identify the
        /// appointment.</param>
        /// <param name="location">A location of the appointment.</param>
        /// <param name="from">A start datetime of the appointment.
        /// </param>
        /// <param name="to">A end datetime of the appointment.
        /// </param>
        /// <param name="isAllDay">An identifier if appointment is all day.
        /// </param>
        /// <param name="isYearly">An identifier if appointment is isYearly.
        /// </param>
        /// <returns>The new appointment.</returns>
        public Task<Appointment>
            Create(
                string title,
                string location,
                DateTime from,
                DateTime to,
                bool isAllDay,
                bool isYearly);

        /// <summary>
        /// Updates the appointment identified by the given id.
        /// </summary>
        /// <param name="id">The id of the appointment to be updated.
        /// </param>
        /// <param name="title">A short description to identify the
        /// appointment.</param>
        /// <param name="location">A location of the appointment.</param>
        /// <param name="from">A start datetime of the appointment.
        /// </param>
        /// <param name="to">A end datetime of the appointment.
        /// </param>
        /// <param name="isAllDay">An identifier if appointment is all day.
        /// </param>
        /// <param name="isYearly">An identifier if appointment is isYearly.
        /// </param>
        /// <returns>The updated appointment.</returns>
        /// <exception cref="QueryException">If the appointment to be
        /// updated could not be found.</exception>
        public Task<Appointment>
            Update(
                int id,
                string? title,
                string? location,
                DateTime? from,
                DateTime? to,
                bool isAllDay,
                bool isYearly);

        /// <summary>
        /// Removes the appointment identified by the given id.
        /// </summary>
        /// <param name="id">Id of the appointment to be removed.</param>
        /// <returns>The id of the appointment that was removed.</returns>
        /// <exception cref="QueryException">If the appointment to remove
        /// could not be found.</exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get all appointments.
        /// </summary>
        /// <returns>All appointments.</returns>
        public IQueryable<Appointment> GetAll();

        /// <summary>
        /// Method to get a appointment with the help of its id.
        /// </summary>
        /// <param name="id">The id of the appointment.</param>
        /// <returns>The appointment with the given id.</returns>
        /// <exception cref="QueryException">If the appointment
        /// with the given id could not be found.</exception>
        public Task<Appointment> GetById(int id);
    }
}
