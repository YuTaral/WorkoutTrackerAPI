using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Services.Notifications.Models;
using FitnessAppAPI.Data.Services.Teams.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using static FitnessAppAPI.Common.Constants;

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

        public async Task<ServiceActionResult<TeamModel>> AddTeam(Dictionary<string, string> requestData, string userId)
        {
            // Validate team
            var validationResult = ValidateTeamData(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            // Validation passed, create the team
            var teamData = validationResult.Data[0];

            var team = new Team
            {
                Image = Utils.DecodeBase64ToByteArray(teamData.Image),
                Name = teamData.Name,
                Description = teamData.Description,
                UserId = userId
            };

            await DBAccess.Teams.AddAsync(team);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<TeamModel>(HttpStatusCode.Created);
        }

        public async Task<ServiceActionResult<TeamModel>> UpdateTeam(Dictionary<string, string> requestData, string userId)
        {
            // Validate team
            var validationResult = ValidateTeamData(requestData);

            if (!validationResult.IsSuccess() || validationResult.Data.Count == 0)
            {
                return validationResult;
            }

            // Validation passed, update the team
            var teamData = validationResult.Data[0];

            // Validate this is the team owner
            var team = await DBAccess.Teams.Where(t => t.Id == teamData.Id).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.NotFound, MSG_TEAM_DOES_NOT_EXIST);
            }
            else if (team.UserId != userId)
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, MSG_EDIT_TEAM_NOT_ALLOWED);
            }

            team.Image = Utils.DecodeBase64ToByteArray(teamData.Image);
            team.Name = teamData.Name;
            team.Description = teamData.Description;

            DBAccess.Entry(team).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<TeamModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<BaseModel>> DeleteTeam(long teamId, string userId)
        {
            // Check if the neccessary data is provided
            if (teamId <= 0)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            // Validate this is the team owner
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_TEAM_DOES_NOT_EXIST);
            } 
            else if (team.UserId != userId)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_DELETE_TEAM_NOT_ALLOWED);
            }

            // Delete the team
            DBAccess.Teams.Remove(team);

            // Delete all notifications related to this team
            DBAccess.Notifications.RemoveRange(DBAccess.Notifications.Where(n => n.TeamId == teamId));

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<BaseModel>> LeaveTeam(Dictionary<string, string> requestData, string userId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("teamId", out string? teamIdString))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            // Validate this is not team owner
            var team = await DBAccess.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
            if (team == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_TEAM_DOES_NOT_EXIST);
            } 
            else if (team.UserId == userId)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_LEAVE_TEAM_NOT_ALLOWED);
            }

            var teamMember = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId && tm.UserId == userId).FirstOrDefaultAsync();
            if (teamMember == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_TEAM_DOES_NOT_EXIST);
            }

            DBAccess.TeamMembers.Remove(teamMember);

            // Delete all notifications related to this team and user
            DBAccess.Notifications.RemoveRange(DBAccess.Notifications
                                                .Where(n => n.TeamId == teamId && 
                                                      (n.SenderUserId == userId || n.ReceiverUserId == userId)));

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<long>> InviteMember(Dictionary<string, string> requestData, string senderUserId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("teamId", out string? teamIdString) || !requestData.TryGetValue("userId", out string? userId))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var teamMember = new TeamMember
            {
                State = MemberTeamState.INVITED.ToString(),
                UserId = userId,
                TeamId = teamId
            };

            DBAccess.TeamMembers.Add(teamMember);
            await DBAccess.SaveChangesAsync();

            // Add the notification for invite
            await notificationService.AddTeamInviteNotification(userId, senderUserId, teamId);

            // Return the team id on success
            return new ServiceActionResult<long>(HttpStatusCode.Created, MSG_SUCCESS, [teamId]);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> RemoveMember(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("member", out string? serializedMember))
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            TeamMemberModel? data = JsonConvert.DeserializeObject<TeamMemberModel>(serializedMember);
            if (data == null)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamMemberModel"));
            }

            var record = await DBAccess.TeamMembers.Where(tm => tm.Id == data.Id).FirstOrDefaultAsync();
            if (record == null)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.NotFound, MSG_MEMBER_IS_NOT_IN_TEAM);
            }

            DBAccess.TeamMembers.Remove(record);
            await DBAccess.SaveChangesAsync();

            // Remove all notifications related to TeamMember record
            await notificationService.DeleteNotifications(data);

            // Return the team member model on succes
            return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.OK, MSG_SUCCESS, [data]);
        }

        public async Task<ServiceActionResult<NotificationModel>> AcceptDeclineInvite(Dictionary<string, string> requestData, string newState)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("userId", out string? userId) || !requestData.TryGetValue("teamId", out string? teamIdString))
            {
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var record = await DBAccess.TeamMembers.Where(tm => tm.UserId == userId && tm.TeamId == teamId).FirstOrDefaultAsync();
            if (record == null)
            {
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_JOIN_DECLINE_TEAM);
            }

            if (newState == MemberTeamState.ACCEPTED.ToString())
            {
                // Update the team members record
                record.State = newState;
                DBAccess.Entry(record).State = EntityState.Modified;
            }
            else
            {
                DBAccess.TeamMembers.Remove(record);
            }

            await DBAccess.SaveChangesAsync();

            // Deal with notifications
            return await HandleNotifications(userId, teamId, newState);
        }

        public async Task<ServiceActionResult<TeamModel>> GetMyTeams(string teamType, string userId)
        {
            // Check if the neccessary data is provided
            if (teamType == "")
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, MSG_INVALID_TEAM_TYPE);
            }

            ViewTeamAs type;

            if (teamType == ViewTeamAs.COACH.ToString())
            {
                type = ViewTeamAs.COACH;
            }
            else if (teamType == ViewTeamAs.MEMBER.ToString())
            {
                type = ViewTeamAs.MEMBER;
            }
            else
            {
                // Invalid value
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, MSG_INVALID_TEAM_TYPE);
            }

            List<TeamModel> teams = [];

            if (type == ViewTeamAs.COACH)
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
                                                                                 tm.State == MemberTeamState.ACCEPTED.ToString()))
                                              .Select(t => ModelMapper.MapToTeamModel(t, type.ToString()))
                                              .ToListAsync();
            }

            return new ServiceActionResult<TeamModel>(HttpStatusCode.OK, MSG_SUCCESS, teams);
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
                                                                   && tm.State == MemberTeamState.ACCEPTED.ToString())
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
                    returnData.Add(ModelMapper.MapToTeamWithMembersModel(t, ViewTeamAs.COACH.ToString(), memberModels));
                }
            }

            return new ServiceActionResult<TeamWithMembersModel>(HttpStatusCode.OK, MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> GetUsersToInvite(string name, long teamId, string userId)
        {
            // Check if the neccessary data is provided
            if (name == "" || teamId <= 0)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, MSG_SEARCH_NAME_NOT_PROVIDED);
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
                        State = MemberTeamState.NOT_INVITED.ToString()
                    };

                    members.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
                }
            }

            return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.OK, MSG_SUCCESS, members);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> GetMyTeamMembers(Dictionary<string, string> requestData)
        {
            if (!requestData.TryGetValue("teamId", out string? teamIdString))
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(teamIdString, out long teamId))
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            return await GetMyTeamMembers(teamId);
        }

        public async Task<ServiceActionResult<TeamMemberModel>> GetMyTeamMembers(long teamId)
        {
            if (teamId <= 0)
            {
                return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var returnData = new List<TeamMemberModel>();

            var members = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId).OrderBy(tm => tm.State).ToListAsync();

            // Add the members
            foreach (var member in members) 
            {
                returnData.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
            }

            return new ServiceActionResult<TeamMemberModel>(HttpStatusCode.OK, MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<BaseModel>> GetJoinedTeamMembers(long teamId, string userId)
        {
            // Check if the neccessary data is provided
            if (teamId <= 0)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_OBJECT_ID_NOT_PROVIDED);
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
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_TEAM_OWNER);
            }

            returnData.Add(coach);

            var members = await DBAccess.TeamMembers.Where(tm => tm.TeamId == teamId && tm.UserId != userId &&
                                                           tm.State == MemberTeamState.ACCEPTED.ToString())
                                                    .OrderBy(tm => tm.State)
                                                    .ToListAsync();

            // Add the members
            foreach (var member in members)
            {
                returnData.Add(await ModelMapper.MapToTeamMemberModel(member, DBAccess));
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, MSG_SUCCESS, returnData);
        }

        public async Task<ServiceActionResult<long>> AssignWorkout(Dictionary<string, string> requestData, string coachId)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workoutId", out string? workoutIdString))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, MSG_WOKOUT_ID_NOT_PROVIDED);
            }

            if (!long.TryParse(workoutIdString, out long workoutId))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, MSG_WOKOUT_ID_NOT_PROVIDED);
            }

            if (!requestData.TryGetValue("memberIds", out string? memberIdsString))
            {
                return new ServiceActionResult<long>(HttpStatusCode.BadRequest, MSG_MEMBER_IDS_NOT_PROVIDED);
            }

            List<long> teamMemberIds = JsonConvert.DeserializeObject<List<long>>(memberIdsString!)!;

            foreach (long id in teamMemberIds)
            {
                // Find the TeamMember record
                var teamMemberRecord = DBAccess.TeamMembers.Where(tm => tm.Id == id).FirstOrDefault();

                if (teamMemberRecord == null)
                {
                    continue;
                }

                var record = new AssignedWorkout
                {
                    WorkoutId = workoutId,
                    TeamMemberId = id,
                    State = AssignedWorkoutState.ASSIGNED.ToString(),
                };

                await DBAccess.AssignedWorkouts.AddAsync(record);
                await DBAccess.SaveChangesAsync();

                // Send notification
                await notificationService.AddWorkoutAssignedNotification(coachId, teamMemberRecord.TeamId, teamMemberRecord.UserId, record.Id);
            }

            return new ServiceActionResult<long>(HttpStatusCode.OK, MSG_WORKOUT_ASSIGNED);
        }


        /// <summary>
        ///    After processing accept / decline invite, update the notification to mark it as inactive
        ///    and send notification to the invite sender for accept / invite. Return updated notifications
        ///    on success
        /// </summary>
        /// <param name="userId">
        ///     The user who accepted / declined the invitation
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="newState">
        ///     The state new MemberTeamState - ACCEPTED / DECLINED notifcation
        /// </param>
        private async Task<ServiceActionResult<NotificationModel>> HandleNotifications(string userId, long teamId, string newState) {
            var notification = await DBAccess.Notifications.Where(n => n.ReceiverUserId == userId &&
                                                                 n.TeamId == teamId &&
                                                                 n.NotificationType == NotificationType.INVITED_TO_TEAM.ToString())
                                                            .FirstOrDefaultAsync();

            if (notification == null)
            {
                // Something went wrong, just return, the invitation has been accepted / declined
                return new ServiceActionResult<NotificationModel>(HttpStatusCode.OK);
            }

            var returnData = new List<NotificationModel>();

            // Update the notification to mark it as inactive
            var updateNotificationResult = await notificationService.UpdateNotification(notification.Id, false);
            if (updateNotificationResult.IsSuccess())
            {
                // Add notification for the team owner
                if (newState == MemberTeamState.ACCEPTED.ToString())
                {
                    await notificationService.AddAcceptedDeclinedNotification(userId, teamId, NotificationType.JOINED_TEAM.ToString());
                }
                else
                {
                    await notificationService.AddAcceptedDeclinedNotification(userId, teamId, NotificationType.DECLINED_TEAM_INVITATION.ToString());

                    // Get the updated list of notifications
                    var updatedNotificationsResult = await notificationService.GetNotifications(userId);
                    returnData = updatedNotificationsResult.Data;
                }
            }

            return new ServiceActionResult<NotificationModel>(HttpStatusCode.OK, MSG_SUCCESS, returnData);
        }

        /// <summary>
        ///    Perform validations whether the provided team data is valid
        ///    Return team model if valid, otherwise Bad Request
        /// </summary>
        /// <param name="requestData">
        ///     The request data - must contain serialized team
        /// </param>
        private static ServiceActionResult<TeamModel> ValidateTeamData(Dictionary<string, string> requestData) {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("team", out string? serializedTeam))
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, MSG_TEAM_FAIL_NO_DATA);
            }

            TeamModel? teamData = JsonConvert.DeserializeObject<TeamModel>(serializedTeam);
            if (teamData == null)
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamModel"));
            }

            string validationErrors = Utils.ValidateModel(teamData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<TeamModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            return new ServiceActionResult<TeamModel>(HttpStatusCode.OK, validationErrors, [teamData]);
        }
    }
}
