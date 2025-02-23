using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Services.Notifications.Models;
using FitnessAppAPI.Data.Services.Teams.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace FitnessAppAPI.Data.Services.Teams
{
    /// <summary>
    ///     Team service class to implement ITeamService interface.
    /// </summary>
    public class TeamService(FitnessAppAPIContext DB, INotificationService service) : BaseService(DB), ITeamService
    {
        /// <summary>
        //      INotificationService instance
        /// </summary>
        private readonly INotificationService notificationService = service;

        public async Task<ServiceActionResult<BaseModel>> AddTeam(Dictionary<string, string> requestData, string userId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("team", out string? serializedTeam))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_TEAM_FAIL_NO_DATA);
            }

            TeamModel? teamData = JsonConvert.DeserializeObject<TeamModel>(serializedTeam);
            if (teamData == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamModel"));
            }

            string validationErrors = Utils.ValidateModel(teamData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            var team = new Team
            {
                Image = Utils.DecodeBase64ToByteArray(teamData.Image),
                Name = teamData.Name,
                Description = teamData.Description,
                UserId = userId
            };

            await DBAccess.Teams.AddAsync(team);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.Created, Constants.MSG_TEAM_ADDED);
        }

        public async Task<ServiceActionResult<BaseModel>> UpdateTeam(Dictionary<string, string> requestData, string userId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("team", out string? serializedTeam))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_UPDATE_TEAM_FAIL_NO_DATA);
            }

            TeamModel? teamData = JsonConvert.DeserializeObject<TeamModel>(serializedTeam);
            if (teamData == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamModel"));
            }

            string validationErrors = Utils.ValidateModel(teamData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            // Validate this is the team owner
            var team = await DBAccess.Teams.Where(t => t.Id == teamData.Id).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_TEAM_DOES_NOT_EXIST);
            }
            else if (team.UserId != userId)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_EDIT_TEAM_NOT_ALLOWED);
            }

            team.Image = Utils.DecodeBase64ToByteArray(teamData.Image);
            team.Name = teamData.Name;
            team.Description = teamData.Description;

            DBAccess.Entry(team).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, Constants.MSG_TEAM_UPDATED);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteTeam(long teamId, string userId)
        {
            // Check if the neccessary data is provided
            if (teamId <= 0)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            // Validate this is the team owner
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_TEAM_DOES_NOT_EXIST);
            } 
            else if (team.UserId != userId)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_DELETE_TEAM_NOT_ALLOWED);
            }

            // Delete the team
            DBAccess.Teams.Remove(team);

            // Delete all notifications related to this team
            DBAccess.Notifications.RemoveRange(DBAccess.Notifications.Where(n => n.TeamId == teamId));

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, Constants.MSG_TEAM_DELETED);
        }

        public async Task<ServiceActionResult<BaseModel>> LeaveTeam(Dictionary<string, string> requestData, string userId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("teamId", out string? teamIdString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            // Validate this is not team owner
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_TEAM_DOES_NOT_EXIST);
            } 
            else if (team.UserId == userId)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_LEAVE_TEAM_NOT_ALLOWED);
            }

            var teamMember = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId && tm.UserId == userId).FirstOrDefaultAsync();
            if (teamMember == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_TEAM_DOES_NOT_EXIST);
            }

            DBAccess.TeamMembers.Remove(teamMember);

            // Delete all notifications related to this team and user
            DBAccess.Notifications.RemoveRange(DBAccess.Notifications
                                                .Where(n => n.TeamId == teamId && 
                                                      (n.SenderUserId == userId || n.ReceiverUserId == userId)));

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, Constants.MSG_LEFT_TEAM);
        }

        public async Task<ServiceActionResult<long>> InviteMember(Dictionary<string, string> requestData, string senderUserId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("teamId", out string? teamIdString) || !requestData.TryGetValue("userId", out string? userId))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var teamMember = new TeamMember
            {
                State = Constants.MemberTeamState.INVITED.ToString(),
                UserId = userId,
                TeamId = teamId
            };

            DBAccess.TeamMembers.Add(teamMember);
            await DBAccess.SaveChangesAsync();

            // Add the notification for invite
            await notificationService.AddTeamInviteNotification(userId, senderUserId, teamId);

            // Return the team id on success
            return new ServiceActionResult<long>(HttpStatusCode.Created, Constants.MSG_MEMBER_INVITE, [teamId]);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> RemoveMember(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("member", out string? serializedMember))
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            TeamMemberModel? data = JsonConvert.DeserializeObject<TeamMemberModel>(serializedMember);
            if (data == null)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamMemberModel"));
            }

            var record = await DBAccess.TeamMembers.Where(tm => tm.Id == data.Id).FirstOrDefaultAsync();
            if (record == null)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.NotFound, Constants.MSG_MEMBER_IS_NOT_IN_TEAM);
            }

            DBAccess.TeamMembers.Remove(record);
            await DBAccess.SaveChangesAsync();

            // Remove all notifications related to TeamMember record
            await notificationService.DeleteNotifications(data);

            // Return the team member model on succes
            return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.OK, Constants.MSG_MEMBER_REMOVED, [data]);
        }

        public async Task<ServiceActionResult<NotificationModel>> AcceptDeclineInvite(Dictionary<string, string> requestData, string newState)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("userId", out string? userId) || !requestData.TryGetValue("teamId", out string? teamIdString))
            {
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var returnMessage = "";

            var record = await DBAccess.TeamMembers.Where(tm => tm.UserId == userId && tm.TeamId == teamId).FirstOrDefaultAsync();
            if (record == null)
            {
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.NotFound, Constants.MSG_FAILED_TO_JOIN_DECLINE_TEAM);
            }

            if (newState == Constants.MemberTeamState.ACCEPTED.ToString())
            {
                // User accepted invitation, set the return message and mark the recrod state as ACCEPTED
                returnMessage = Constants.MSG_JOINED_TEAM;

                // Update the team members record
                record.State = newState;
                DBAccess.Entry(record).State = EntityState.Modified;
            }
            else
            {
                // User declined the invitation, set the return message and delete the record
                returnMessage = Constants.MSG_TEAM_INVITATION_DECLINED;

                DBAccess.TeamMembers.Remove(record);
            }

            await DBAccess.SaveChangesAsync();

            var notification = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId &&
                                                                 n.TeamId == teamId &&
                                                                 n.NotificationType == Constants.NotificationType.INVITED_TO_TEAM.ToString())
                                                            .FirstOrDefaultAsync();

            if (notification == null)
            {
                // Something went wrong, just return, the invitation has been accepted / declined
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.OK, returnMessage);
            }

             var returnData = new List<NotificationModel>();

            // Update the notification to mark it as inactive
            var updateNotificationResult = await notificationService.UpdateNotification(notification.Id, false);
            if (updateNotificationResult.IsSuccess())
            {
                // Add notification for the team owner
                if (newState == Constants.MemberTeamState.ACCEPTED.ToString())
                {
                    await notificationService.AddAcceptedDeclinedNotification(userId, teamId, Constants.NotificationType.JOINED_TEAM.ToString());
                }
                else
                {
                    await notificationService.AddAcceptedDeclinedNotification(userId, teamId, Constants.NotificationType.DECLINED_TEAM_INVITATION.ToString());

                    // Get the updated list of notifications
                    var updatedNotificationsResult = await notificationService.GetNotifications(userId);
                    returnData = updatedNotificationsResult.Data;
                }
            }

            return new ServiceActionResult<NotificationModel>(HttpStatusCode.OK, returnMessage, returnData);
        }

        public async Task<ServiceActionResult<TeamModel>> GetMyTeams(string teamType, string userId)
        {
            // Check if the neccessary data is provided
            if (teamType == "")
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, Constants.MSG_INVALID_TEAM_TYPE);
            }

            Constants.ViewTeamAs type;

            if (teamType == Constants.ViewTeamAs.COACH.ToString())
            {
                type = Constants.ViewTeamAs.COACH;
            }
            else if (teamType == Constants.ViewTeamAs.MEMBER.ToString())
            {
                type = Constants.ViewTeamAs.MEMBER;
            }
            else
            {
                // Invalid value
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, Constants.MSG_INVALID_TEAM_TYPE);
            }

            List<TeamModel> teams = [];

            if (type == Constants.ViewTeamAs.COACH)
            {
                // Fetch teams where user is coach
                teams = await DBAccess.Teams.Where(t => t.UserId == userId)
                                            .Select(t => ModelMapper.MapToTeamModel(t, type.ToString()))
                                            .ToListAsync();
            } 
            else
            {
                // Fetch teams where user is member
                teams = await DBAccess.Teams.Where(t => DBAccess.TeamMembers.Any(tm => tm.TeamId == t.Id && tm.UserId == userId && 
                                                                                 tm.State == Constants.MemberTeamState.ACCEPTED.ToString()))
                                              .Select(t => ModelMapper.MapToTeamModel(t, type.ToString()))
                                              .ToListAsync();
            }

            return new ServiceActionResult<TeamModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, teams);
        }

        public async Task<ServiceActionResult<TeamWithMembersModel>> GetMyTeamsWithMembers(string userId)
        {
            var teams = await DBAccess.Teams.Where(t => t.UserId == userId).ToListAsync();
            var returnData = new List<TeamWithMembersModel>();

            foreach (Team t  in teams)
            {
                var memberModels = new List<TeamMemberModel>();

                // Get the members of the team
                var members = await DBAccess.TeamMembers.Where(tm => tm.TeamId == t.Id 
                                                                   && tm.State == Constants.MemberTeamState.ACCEPTED.ToString())
                                                        .ToListAsync();
                // If there are members, add the member models
                if (members.Count > 0)
                {
                    foreach (TeamMember tm in members)
                    {
                        memberModels.Add(await ModelMapper.MapToTeamMemberModel(tm, DBAccess));
                    }
                }

                if (memberModels.Count > 0) {
                    // Add only the teams which have at least 1 member
                    returnData.Add(ModelMapper.MapToTeamWithMembersModel(t, Constants.ViewTeamAs.COACH.ToString(), memberModels));
                }
            }

            return new ServiceActionResult<TeamWithMembersModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> GetUsersToInvite(string name, long teamId, string userId)
        {
            // Check if the neccessary data is provided
            if (name == "" || teamId <= 0)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, Constants.MSG_SEARCH_NAME_NOT_PROVIDED);
            }

            // Trim and convert to lower
            name = name.Trim().ToLower();

            var members = new List<TeamMemberModel>();

            // Fetch users with this name
            var users = await DBAccess.UserProfiles.Where(u => u.UserId != userId && u.FullName.ToLower().Contains(name) &&
                                                        !DBAccess.TeamMembers.Any(tm => tm.TeamId == teamId && tm.UserId == u.UserId))
                                                    .ToListAsync();

            if (users.Count > 0) {

                foreach (var user in users) {

                    // Create member record and map it to model
                    var member = new TeamMember
                    {
                        Id = 0,
                        UserId = user.UserId,
                        TeamId = teamId,
                        State = Constants.MemberTeamState.NOT_INVITED.ToString()
                    };

                    members.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
                }
            }

            return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, members);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> GetMyTeamMembers(Dictionary<string, string> requestData)
        {
            if (!requestData.TryGetValue("teamId", out string? teamIdString))
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            return await GetMyTeamMembers(teamId);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> GetMyTeamMembers(long teamId)
        {
            if (teamId <= 0)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var returnData = new List<TeamMemberModel>();

            var members = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId).OrderBy(tm => tm.State).ToListAsync();

            // Add the members
            foreach (var member in members) 
            {
                returnData.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
            }

            return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<BaseModel>> GetJoinedTeamMembers(long teamId, string userId)
        {
            // Check if the neccessary data is provided
            if (teamId <= 0)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var returnData = new List<BaseModel>();

            // Add the coach
            var coach = await DBAccess.Teams.Where(t => t.Id == teamId)
                                            .Select(t => DBAccess.UserProfiles.Where(p => p.UserId == t.UserId)
                                                                                .Select(p => ModelMapper.MapToTeamCoachModel(p))
                                                                                .FirstOrDefault())
                                            .FirstOrDefaultAsync();
            if (coach == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, Constants.MSG_FAILED_TO_TEAM_OWNER);
            }

            returnData.Add(coach);

            var members = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId && tm.UserId != userId &&
                                                           tm.State == Constants.MemberTeamState.ACCEPTED.ToString())
                                                    .OrderBy(tm => tm.State)
                                                    .ToListAsync();

            // Add the members
            foreach (var member in members)
            {
                returnData.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, Constants.MSG_SUCCESS, returnData);
        }
    }
}
