namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="Appointment"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class AppointmentQuery
    {
        /// <summary>
        /// Method which retrieves the appointments.
        /// </summary>
        /// <param name="appointmentManager">The appointment manager
        /// responsible for managing appointment.</param>
        /// <returns>Queryable of all appointments to the user.
        /// </returns>
        [Authorize(Policy = "ViewAppointment")]
        [AccessObjectIdArgument("IGNORED")]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Appointment> Appointments(
            [Service] IAppointmentManager appointmentManager)
        {
            return appointmentManager.GetAll();
        }
    }
}
