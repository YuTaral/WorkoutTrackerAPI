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
        ///     The user who send the notification
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
        ///     Delete notifications for assigned workout
        /// </summary>
        /// <param name="ids">
        ///     The records' AssignedWorkoutId value
        /// </param>
        /// <param name="notificationType">
        ///     The notification type - may be empty for all notification types
        /// </param>
        public Task<ServiceActionResult<BaseModel>> DeleteNotificationsForAssignedWorkout(List<long> ids, string notificationType);

        /// <summary>
        ///     Return the notifications for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id - owner of the notifications
        /// </param>
        /// <param name="showReviewed">
        ///     Whether to show reviewed notifications or not
        /// </param>
        public Task<ServiceActionResult<NotificationModel>> GetNotifications(string userId, bool showReviewed);

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

        /// <summary>
        ///     Add notification for workout assign
        /// </summary>
        /// <param name="senderUserId">
        ///     The user send the workout
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="receiverUserId">
        ///     The user who must be notified
        /// </param>
        /// <param name="assignedWorkoutRecId">
        ///     Id of the assigned workout record
        /// </param>
        /// 
        public Task<ServiceActionResult<BaseModel>> AddWorkoutAssignedNotification(string senderUserId, long teamId, string receiverUserId, long assignedWorkoutRecId);

        /// <summary>
        ///     Add notification for workout assign completed
        /// </summary>
        /// <param name="senderUserId">
        ///     The user who completed the workout (member)
        /// </param>
        /// <param name="receiverUserId">
        ///     The user who must be notified (coach)
        /// </param>
        ///  <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="assignedWorkoutRecId">
        ///     Id of the assigned workout record
        /// </param>
        /// 
        public Task<ServiceActionResult<BaseModel>> AssignedWorkoutFinishedNotification(string senderUserId, string receiverUserId, long teamId, long assignedWorkoutRecId);

        /// <summary>
        ///     Update the notification related to this assigned workout and mark it as inactive
        /// </summary>
        /// <param name="assignedWorkoutId">
        ///     The assigned workout id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> UpdateAssignedWorkoutNotificationToInactive(long assignedWorkoutId);
    }
}
