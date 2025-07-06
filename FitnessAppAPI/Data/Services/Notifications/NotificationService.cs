using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications.Models;
using FitnessAppAPI.Data.Services.Teams.Models;
using FitnessAppAPI.Data.Services.User.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using static FitnessAppAPI.Common.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessAppAPI.Data.Services.Notifications
{
    /// <summary>
    ///     Notification service class to implement INotificationService interface.
    /// </summary>
    public class NotificationService(FitnessAppAPIContext DB) : BaseService(DB), INotificationService
    {

        public async Task<ServiceActionResult<BaseModel>> AddTeamInviteNotification(string receiverUserId, string senderUserId, long teamId)
        {
            var teamName = await DBAccess.Teams.Where(t => t.Id == teamId).Select(t => t.Name).FirstOrDefaultAsync();

            // Must not happen
            teamName ??= "Unknown";

            // Create the invite to team notification
            var notification = new Notification
            {
                NotificationType = NotificationType.INVITED_TO_TEAM.ToString(),
                ReceiverUserId = receiverUserId,
                SenderUserId = senderUserId,
                NotificationText = string.Format(DBConstants.InviteToTeamNotification, teamName),
                DateTime = DateTime.UtcNow,
                IsActive = true,
                TeamId = teamId
            };

            return await AddNotification(notification);
        }

        public async Task<ServiceActionResult<BaseModel>> AddAcceptedDeclinedNotification(string senderUserId, long teamId, string notificationType)
        {
            var notificationText = "";
            var senderName = await DBAccess.UserProfiles.Where(u => u.UserId == senderUserId).Select(t => t.FullName).FirstOrDefaultAsync();

            // Must not happen
            senderName ??= "Unknown user";

            // Find the notification receiver id
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_TEAM_OWNER);
            }

            if (notificationType == NotificationType.JOINED_TEAM.ToString()) 
            {
                notificationText = string.Format(DBConstants.AcceptTeamInvitationNotification, senderName, team.Name);
            } 
            else
            {
                notificationText = string.Format(DBConstants.DeclineTeamInvitationNotification, senderName, team.Name);
            }

            var notification = new Notification
            {
                NotificationType = notificationType,
                ReceiverUserId = team.UserId,
                SenderUserId = senderUserId,
                NotificationText = notificationText,
                DateTime = DateTime.UtcNow,
                IsActive = true,
                TeamId = teamId
            };

            return await AddNotification(notification);
        }

        public async Task<ServiceActionResult<BaseModel>> UpdateNotification(Dictionary<string, string> requestData, bool isActive)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("id", out string? idString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            if (!long.TryParse(idString, out long id))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            return await UpdateNotification(id, isActive);
        }

        public async Task<ServiceActionResult<BaseModel>> UpdateNotification(long id, bool isActive)
        {
            var notification = await DBAccess.Notifications.Where(n => n.Id == id).FirstOrDefaultAsync();
            if (notification == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            notification.IsActive = isActive;
            DBAccess.Entry(notification).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteNotification(long notificationId, string userId)
        {
            // Check if the neccessary data is provided
            if (notificationId <= 0)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_DELETE_NOTIFICATION_FAILED);
            }

            var notification = await DBAccess.Notifications.Where(n => n.Id == notificationId).FirstOrDefaultAsync();
            if (notification == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_DELETE_NOTIFICATION_FAILED);
            }

            if (notification.IsActive && notification.NotificationType == NotificationType.INVITED_TO_TEAM.ToString())
            { 
                // If the user is deleting INVITED_TO_TEAM notification, remove the record from TeamMembers
                var record = await DBAccess.TeamMembers.Where(tm => tm.UserId == userId && 
                                                              tm.TeamId == notification.TeamId && 
                                                              tm.State == MemberTeamState.INVITED.ToString())
                                                        .FirstOrDefaultAsync();

                if (record != null)
                {
                    // Remove the record and send notification to the team owner
                    DBAccess.TeamMembers.Remove(record);
                    await AddAcceptedDeclinedNotification(userId, record.TeamId, NotificationType.DECLINED_TEAM_INVITATION.ToString());
                }
            }

            // Delete the notification
            DBAccess.Notifications.Remove(notification);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteNotifications(TeamMemberModel data)
        {
            // Find all notifications related to the deleted TeamMember record
            var notifications = await DBAccess.Notifications.Where(n => n.TeamId == data.TeamId && 
                                                                  (n.SenderUserId == data.UserId || n.ReceiverUserId == data.UserId))
                                                            .ToListAsync();

            return await DeleteNotifications(notifications);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteNotificationsForAssignedWorkout(List<long> ids, string notificationType)
        {
            // Find all notifications AssignedWorkoutId value is in the list with ids and notificaion type
            var notifications = await DBAccess.Notifications
                                .Where(n => n.AssignedWorkoutId.HasValue
                                       && ids.Contains(n.AssignedWorkoutId.Value) 
                                       && ((n.NotificationType == notificationType) || notificationType == ""))
                                .ToListAsync();

            return await DeleteNotifications(notifications);
        }

        public async Task<ServiceActionResult<NotificationModel>> GetNotifications(string userId)
        {
            var notifcationModels = new List<NotificationModel>();
            var notifications = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId)
                                                             .OrderByDescending(n => n.DateTime)
                                                             .ToListAsync();

            foreach (var n in notifications) {
                notifcationModels.Add(await ModelMapper.MapToNotificationModel(n, DBAccess));    
            }

            return new ServiceActionResult<NotificationModel>(HttpStatusCode.OK, MSG_SUCCESS, notifcationModels);
        }

        public async Task<bool> HasNotification(string userId)
        {
            var notification = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId && n.IsActive).FirstOrDefaultAsync();

            return notification != null;
        }

        private async Task<ServiceActionResult<BaseModel>> AddNotification(Notification data)
        {
            DBAccess.Notifications.Add(data);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.Created);
        }

        public async Task<ServiceActionResult<JoinTeamNotificationModel>> GetJoinTeamNotificationDetails(long notificationId)
        {
            // Check if the neccessary data is provided
            if (notificationId <= 0)
            {
                return new ServiceActionResult<JoinTeamNotificationModel>(HttpStatusCode.BadRequest, MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
            }

            var formattedDate = "";
            var description = "";
            var teamModel = ModelMapper.GetEmptyTeamModel();

            var notification = await DBAccess.Notifications.Where(n => n.Id == notificationId).FirstOrDefaultAsync();

            if (notification == null)
            {
                return new ServiceActionResult<JoinTeamNotificationModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_GET_NOTIFICATION_DETAILS);
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
                    description = string.Format(NotificationText.AskTeamInviteAccept, sender.FullName, formattedDate);
                } 
                else
                {
                    // Do not use sender name
                    description = string.Format(NotificationText.AskTeamInviteAcceptNoSender, formattedDate);
                }
            }

            var model = new JoinTeamNotificationModel
            {
                Id = notificationId,
                TeamName = teamModel.Name,
                Description = description,
                TeamImage = teamModel.Image,
                NotificationType = NotificationType.INVITED_TO_TEAM.ToString(),
                TeamId = teamModel.Id
            };

            return new ServiceActionResult<JoinTeamNotificationModel>(HttpStatusCode.OK, MSG_SUCCESS, [model]);
        }

        public async Task<ServiceActionResult<BaseModel>> AddWorkoutAssignedNotification(string senderUserId, long teamId, string receiverUserId, long assignedWorkoutRecId)
        {
            var notification = new Notification
            {
                NotificationType = NotificationType.WORKOUT_ASSIGNED.ToString(),
                ReceiverUserId = receiverUserId,
                SenderUserId = senderUserId,
                NotificationText = DBConstants.WorkoutAssigned,
                DateTime = DateTime.UtcNow,
                IsActive = true,
                TeamId = teamId,
                AssignedWorkoutId = assignedWorkoutRecId
            };

            return await AddNotification(notification);
        }

        public async Task<ServiceActionResult<BaseModel>> AssignedWorkoutFinishedNotification(string senderUserId, string receiverUserId, long teamId, long assignedWorkoutRecId)
        {
            var notificationExists = await DBAccess.Notifications
                                    .Where(n => n.AssignedWorkoutId == assignedWorkoutRecId && 
                                           n.NotificationType == NotificationType.WORKOUT_ASSIGNMENT_COMPLETED.ToString())
                                    .FirstOrDefaultAsync();

            if (notificationExists != null)
            {
                // Notificaiton already exists, update it as active
                // (member may have completed the same assignment twice,no need for new notification record)
                notificationExists.IsActive = true;
                notificationExists.DateTime = DateTime.UtcNow;
                DBAccess.Entry(notificationExists).State = EntityState.Modified;
                await DBAccess.SaveChangesAsync();
            } 
            else
            {
                var profile = await DBAccess.UserProfiles.Where(p => p.UserId == senderUserId).FirstOrDefaultAsync();
                if (profile != null)
                {
                    var notification = new Notification
                    {
                        NotificationType = NotificationType.WORKOUT_ASSIGNMENT_COMPLETED.ToString(),
                        ReceiverUserId = receiverUserId,
                        SenderUserId = senderUserId,
                        NotificationText = string.Format(NotificationText.WorkoutAssignmentFinished, profile.FullName),
                        DateTime = DateTime.UtcNow,
                        IsActive = true,
                        TeamId = teamId,
                        AssignedWorkoutId = assignedWorkoutRecId
                    };

                    return await AddNotification(notification);
                }
            }
            

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        private async Task<ServiceActionResult<BaseModel>> DeleteNotifications(List<Notification> notifications)
        {
            DBAccess.RemoveRange(notifications);
            await DBAccess.SaveChangesAsync();
            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }
    }
}
