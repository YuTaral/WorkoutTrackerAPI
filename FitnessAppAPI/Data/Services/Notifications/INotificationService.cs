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
        public Task<ServiceActionResult> AddAcceptedDeclinedNotification(string senderUserId, long teamId, string notificationType);

        /// <summary>
        ///     Change the notification state
        /// </summary>
        /// <param name="id">
        ///     The notification id
        /// </param>
        /// <param name="isActive">
        ///     True if the notification must be set to active, false otherwise
        /// </param>
        public Task<ServiceActionResult> UpdateNotification(long id, bool isActive);

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

        /// <summary>
        ///     Return the join team notification details
        /// </summary>
        /// <param name="id">
        ///     The notification id
        /// </param>
        public Task<ServiceActionResult> GetJoinTeamNotificationDetails(long id);
    }
}
