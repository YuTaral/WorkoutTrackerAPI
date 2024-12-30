using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data.Services.Notifications
{
    /// <summary>
    ///     Notification service class to implement INotificationService interface.
    /// </summary>
    public class NotificationService(FitnessAppAPIContext DB) : BaseService(DB), INotificationService
    {

        public async Task<ServiceActionResult> AddTeamInviteNotification(string receiverUserId, string senderUserId, long teamId)
        {
            var teamName = DBAccess.Teams.Where(t => t.Id == teamId).Select(t => t.Name).FirstOrDefault();

            // Must not happen
            teamName ??= "Unknown";

            // Create the invite to team notification
            var notification = new Notification
            {
                NotificationType = Constants.NotificationType.INVITED_TO_TEAM.ToString(),
                ReceiverUserId = receiverUserId,
                SenderUserId = senderUserId,
                NotificationText = string.Format(Constants.DBConstants.InviteNotification, teamName),
                DateTime = DateTime.Now,
                IsActive = true,
            };

            return await AddNotification(notification);
        }

        public async Task<ServiceActionResult> GetNotifications(string userId)
        {
            var notifcations = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId)
                                                             .OrderByDescending(n => n.DateTime)
                                                             .Select(n => (BaseModel) ModelMapper.MapToNotificationModel(n))
                                                             .ToListAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, notifcations);
        }

        public async Task<bool> HasNotification(string userId)
        {
            var notification = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId && n.IsActive).FirstOrDefaultAsync();

            return notification != null;
        }

        private async Task<ServiceActionResult> AddNotification(Notification data)
        {
            DBAccess.Notifications.Add(data);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS);
        }
    }
}
