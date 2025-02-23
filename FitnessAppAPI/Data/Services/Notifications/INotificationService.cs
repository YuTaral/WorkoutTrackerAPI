using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications.Models;
using FitnessAppAPI.Data.Services.Teams.Models;

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
        public Task<ServiceActionResult<BaseModel>> AddTeamInviteNotification(string receiverUserId, string senderUserId, long teamId);

        /// <summary>
        ///     Add the team accept invitation notification to the notifications table
        /// </summary>
        /// <param name="senderUserId">
        ///     The user who should be notified
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="notificationType">
        ///     The notification type - "JOINED" or "DECLINED"
        /// </param>
        /// 
        public Task<ServiceActionResult<BaseModel>> AddAcceptedDeclinedNotification(string senderUserId, long teamId, string notificationType);

        /// <summary>
        ///     Change the notification state
        /// </summary>
        /// <param name="requestData">
        ///     The request data (notification id)
        /// </param>
        /// <param name="isActive">
        ///     True if the notification must be set to active, false otherwise
        /// </param>
        public Task<ServiceActionResult<BaseModel>> UpdateNotification(Dictionary<string, string> requestData, bool isActive);

        /// <summary>
        ///     Change the notification state
        /// </summary>
        /// <param name="id">
        ///     The notification id
        /// </param>
        /// <param name="isActive">
        ///     True if the notification must be set to active, false otherwise
        /// </param>
        public Task<ServiceActionResult<BaseModel>> UpdateNotification(long id, bool isActive);

        /// <summary>
        ///     Delete the notification
        /// </summary>
        /// <param name="notificationId">
        ///     The notification id
        /// </param>
        /// <param name="userId">
        ///     The user id - owner of the notifications
        /// </param>
        public Task<ServiceActionResult<BaseModel>> DeleteNotification(long notificationId, string userId);

        /// <summary>
        ///     Delete notifications after TeamMember record has been deleted
        /// </summary>
        /// <param name="data">
        ///     The TeamMember record
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> DeleteNotifications(TeamMemberModel data);

        /// <summary>
        ///     Return the notifications for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id - owner of the notifications
        /// </param>
        public Task<ServiceActionResult<NotificationModel>> GetNotifications(string userId);

        /// <summary>
        ///     Check whether the current user have at least one active notification
        /// </summary>
        /// <param name="userId">
        ///     The user id - owner of the notification
        /// </param>
        public Task<bool> HasNotification(string userId);

        /// <summary>
        ///     Return the join team notification details
        /// </summary>
        /// <param name="notificationId">
        ///     The notification id
        /// </param>
        public Task<ServiceActionResult<JoinTeamNotificationModel>> GetJoinTeamNotificationDetails(long notificationId);
    }
}
