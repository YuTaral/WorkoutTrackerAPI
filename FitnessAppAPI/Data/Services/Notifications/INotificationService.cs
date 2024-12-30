using FitnessAppAPI.Data.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FitnessAppAPI.Data.Services.Notifications
{
    /// <summary>
    ///     Notification service interface to define the logic for notifications CRUD operations.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        ///     Add the team invite notification to the notifications table
        /// </summary>
        /// <param name="receiverUserId">
        ///     The user who is being invited
        /// </param>
        /// <param name="senderUserId">
        ///     The user who send the invite
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// 
        public Task<ServiceActionResult> AddTeamInviteNotification(string receiverUserId, string senderUserId, long teamId);

        /// <summary>
        ///     Return the notifications for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id - owner of the notifications
        /// </param>
        public Task<ServiceActionResult> GetNotifications(string userId);

        /// <summary>
        ///     Check whether the current user have at least one active notification
        /// </summary>
        /// <param name="userId">
        ///     The user id - owner of the notification
        /// </param>
        public Task<bool> HasNotification(string userId);
    }
}
