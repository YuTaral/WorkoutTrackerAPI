using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications.Models;
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
            var teamName = await DBAccess.Teams.Where(t => t.Id == teamId).Select(t => t.Name).FirstOrDefaultAsync();

            // Must not happen
            teamName ??= "Unknown";

            // Create the invite to team notification
            var notification = new Notification
            {
                NotificationType = Constants.NotificationType.INVITED_TO_TEAM.ToString(),
                ReceiverUserId = receiverUserId,
                SenderUserId = senderUserId,
                NotificationText = string.Format(Constants.DBConstants.InviteToTeamNotification, teamName),
                DateTime = DateTime.Now,
                IsActive = true,
                TeamId = teamId
            };

            return await AddNotification(notification);
        }

        public async Task<ServiceActionResult> AddAcceptedDeclinedNotification(string senderUserId, long teamId, string notificationType)
        {
            var notificationText = "";
            var senderName = await DBAccess.UserProfiles.Where(u => u.UserId == senderUserId).Select(t => t.FullName).FirstOrDefaultAsync();

            // Must not happen
            senderName ??= "Unknown user";

            // Find the notification receiver id
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_TEAM_OWNER);
            }

            if (notificationType == Constants.NotificationType.JOINED_TEAM.ToString()) 
            {
                notificationText = string.Format(Constants.DBConstants.AcceptTeamInvitationNotification, senderName, team.Name);
            } 
            else
            {
                notificationText = string.Format(Constants.DBConstants.DeclineTeamInvitationNotification, senderName, team.Name);
            }

            var notification = new Notification
            {
                NotificationType = notificationType,
                ReceiverUserId = team.UserId,
                SenderUserId = senderUserId,
                NotificationText = notificationText,
                DateTime = DateTime.Now,
                IsActive = true,
                TeamId = teamId
            };

            return await AddNotification(notification);
        }

        public async Task<ServiceActionResult> UpdateNotification(long id, bool isActive)
        {
            var notification = await DBAccess.Notifications.Where(n => n.Id == id).FirstOrDefaultAsync();
            if ( notification == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            notification.IsActive = isActive;
            DBAccess.Entry(notification).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS);
        }

        public async Task<ServiceActionResult> DeleteNotification(NotificationModel data, string userId)
        {
            var notification = await DBAccess.Notifications.Where(n => n.Id == data.Id).FirstOrDefaultAsync();
            if (notification == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_DELETE_NOTIFICATION_FAILED);
            }

            if (notification.IsActive && notification.NotificationType == Constants.NotificationType.INVITED_TO_TEAM.ToString())
            { 
                // If the user is deleting INVITED_TO_TEAM notification, remove the recrod from TeamMembers
                var record = await DBAccess.TeamMembers.Where(tm => tm.UserId == userId && 
                                                              tm.TeamId == data.TeamId && 
                                                              tm.State == Constants.MemberTeamState.INVITED.ToString())
                                                        .FirstOrDefaultAsync();

                if (record != null)
                {
                    // Remove the record and send notification to the team owner
                    DBAccess.TeamMembers.Remove(record);
                    await AddAcceptedDeclinedNotification(userId, record.TeamId, Constants.NotificationType.DECLINED_TEAM_INVITATION.ToString());
                }
            }

            // Delete the notification
            DBAccess.Notifications.Remove(notification);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_NOTIFICATION_DELETED);
        }

        public async Task<ServiceActionResult> GetNotifications(string userId)
        {
            var notifcationModels = new List<BaseModel>();
            var notifications = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId)
                                                             .OrderByDescending(n => n.DateTime)
                                                             .ToListAsync();

            foreach (var n in notifications) {
                notifcationModels.Add(await ModelMapper.MapToNotificationModel(n, DBAccess));    
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, notifcationModels);
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

        public async Task<ServiceActionResult> GetJoinTeamNotificationDetails(long id)
        {
            var formattedDate = "";
            var description = "";
            var teamModel = ModelMapper.GetEmptyTeamModel();

            var notification = await DBAccess.Notifications.Where(n => n.Id == id).FirstOrDefaultAsync();

            if (notification == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            formattedDate = Utils.FormatDefaultDateTime(notification.DateTime);

            var team = await DBAccess.Teams.Where(t => t.Id == notification.TeamId).FirstOrDefaultAsync();
            if (team != null)
            {
                teamModel.Name = team.Name;
                teamModel.Image = Utils.EncodeByteArrayToBase64Image(team.Image);
                teamModel.Id = team.Id;
            }

            var sender = await DBAccess.UserProfiles.Where(p => p.UserId == notification.SenderUserId).FirstOrDefaultAsync();
            if (sender != null)
            {
                if (!string.IsNullOrWhiteSpace(sender.FullName))
                {
                    // Use sender name
                    description = string.Format(Constants.NotificationText.AskTeamInviteAccept, sender.FullName, formattedDate);
                } 
                else
                {
                    // Do not use sender name
                    description = string.Format(Constants.NotificationText.AskTeamInviteAcceptNoSender, formattedDate);
                }
            }

            var model = new JoinTeamNotificationModel
            {
                Id = id,
                TeamName = teamModel.Name,
                Description = description,
                TeamImage = teamModel.Image,
                NotificationType = Constants.NotificationType.INVITED_TO_TEAM.ToString(),
                TeamId = teamModel.Id
            };

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [model]);
        }
    }
}
