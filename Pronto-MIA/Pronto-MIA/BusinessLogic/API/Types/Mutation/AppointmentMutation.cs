#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FirebaseAdmin.Messaging;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="Appointment"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class AppointmentMutation
    {
        /// <summary>
        /// Adds an appointment to the application.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="appointmentManager">The manager responsible for
        /// managing appointment.</param>
        /// <param name="firebaseMessagingManager">The manager used to inform
        /// affected users that a new appointment has been created.
        /// </param>
        /// <param name="firebaseTokenManager">The manager responsible for the
        /// fcm tokens used by this operation.
        /// </param>
        /// <param name="title">Short description to identify the
        /// appointment.</param>
        /// <param name="location">A Location of the appointment.
        /// </param>
        /// <param name="from">A start datetime of the appointment.
        /// </param>
        /// <param name="to">A end datetime of the appointment.
        /// </param>
        /// <param name="isAllDay">An identifier if appointment is all day.
        /// </param>
        /// <param name="isYearly">An identifier if appointment is yearly.
        /// </param>
        /// <returns>The newly generated appointment.</returns>
        [Authorize(Policy = "EditAppointment")]
        [UseSingleOrDefault]
        public async Task<Appointment> CreateAppointment(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IAppointmentManager appointmentManager,
            [Service] IFirebaseMessagingManager firebaseMessagingManager,
            [Service] IFirebaseTokenManager firebaseTokenManager,
            string title,
            string location,
            DateTime from,
            DateTime to,
            bool isAllDay,
            bool isYearly)
        {
            appointmentManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var appointment = await appointmentManager.Create(
                    title,
                    location,
                    from,
                    to,
                    isAllDay,
                    isYearly);

                await dbContextTransaction.CommitAsync();

                await this.SendNotification(
                    appointment,
                    firebaseMessagingManager,
                    firebaseTokenManager);

                return appointment;
            }
        }

        /// <summary>
        /// Method that updates the appointment with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="dbContext">The database context that will be
        /// used. Using the same <see cref="ProntoMiaDbContext"/> over
        /// multiple managers will ensure transactional safety.</param>
        /// <param name="appointmentManager">The manager responsible for
        /// managing appointment.</param>
        /// <param name="id">The id of the appointment to be adjusted.
        /// </param>
        /// <param name="title">Short description to identify the
        /// appointment.</param>
        /// <param name="location">A Location of the appointment.
        /// </param>
        /// <param name="from">A start datetime of the appointment.
        /// </param>
        /// <param name="to">A end datetime of the appointment.
        /// </param>
        /// <param name="isAllDay">An identifier if appointment is all day.
        /// </param>
        /// <param name="isYearly">An identifier if appointment is yearly.
        /// </param>
        /// <returns>The updated appointment.</returns>
        /// <exception cref="QueryException">Returns AppointmentNotFound
        /// exception if the appointment with given id could not be found.
        /// </exception>
        [Authorize(Policy = "EditAppointment")]
        [AccessObjectIdArgument("id")]
        public async Task<Appointment> UpdateAppointment(
            [Service] ProntoMiaDbContext dbContext,
            [Service] IAppointmentManager appointmentManager,
            int id,
            string? title,
            string? location,
            DateTime? from,
            DateTime? to,
            bool isAllDay,
            bool isYearly)
        {
            appointmentManager.SetDbContext(dbContext);
            await using (var dbContextTransaction = await
                dbContext.Database.BeginTransactionAsync())
            {
                var appointment = await appointmentManager.Update(
                    id,
                    title,
                    location,
                    from,
                    to,
                    isAllDay,
                    isYearly);
                await dbContextTransaction.CommitAsync();
                return appointment;
            }
        }

        /// <summary>
        /// Method that removes the appointment with the given id.
        /// </summary>
        /// <param name="appointmentManager">The manager responsible for
        /// managing appointment.</param>
        /// <param name="id">Id of the appointment to be removed.</param>
        /// <returns>The id of the appointment which was removed.</returns>
        /// <exception cref="QueryException">Returns AppointmentNotFound
        /// exception if the appointment with the given id could not be
        /// found.
        /// </exception>
        [Authorize(Policy = "EditAppointment")]
        [AccessObjectIdArgument("id")]
        public async Task<int> RemoveAppointment(
            [Service] IAppointmentManager appointmentManager,
            int id)
        {
            return await appointmentManager.Remove(id);
        }

        private Dictionary<string, string> CreateNotificationData(
            int appointmentId)
        {
            return new ()
            {
                { "Action", "publish" },
                { "TargetType", "appointment" },
                { "TargetId", appointmentId.ToString() },
            };
        }

        private async Task SendNotification(
            Appointment appointment,
            IFirebaseMessagingManager firebaseMessagingManager,
            IFirebaseTokenManager firebaseTokenManager)
        {
            var tokens = await firebaseTokenManager
                    .GetAllFcmToken()
                    .Select(token => token.Id).ToListAsync();
            var notification = new Notification
            {
                Title = "Kalendereintrag erstellt",
                Body = @$"Der Kalendereintrag 
                    {appointment.Title} wurde soeben erstellt.",
            };
            var data = this.CreateNotificationData(appointment.Id);

            var badTokens = await firebaseMessagingManager
                .SendMulticastAsync(tokens, notification, data);
            await firebaseTokenManager
                .UnregisterMultipleFcmToken(badTokens);
        }
    }
}
