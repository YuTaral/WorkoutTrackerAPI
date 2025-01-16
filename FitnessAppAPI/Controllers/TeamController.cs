using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FitnessAppAPI.Data.Services.Teams.Models;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Team Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.TEAM)]
    public class TeamController(ITeamService s, INotificationService notificationS) : BaseController
    {   
        /// <summary>
        //      TeamService instance
        /// </summary>
        private readonly ITeamService service = s;

        /// <summary>
        //      INotificationService instance
        /// </summary>
        private readonly INotificationService notificationService = notificationS;

        /// <summary>
        //      POST request to create a new team
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.ADD_TEAM)]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {

            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("team", out string? serializedTeam))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_TEAM_FAIL_NO_DATA);
            }

            TeamModel? teamData = JsonConvert.DeserializeObject<TeamModel>(serializedTeam);
            if (teamData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamModel"));
            }

            string validationErrors = Utils.ValidateModel(teamData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.UNEXPECTED_ERROR, validationErrors);
            }

            var userId = GetUserId();

            return CustomResponse(await service.AddTeam(teamData, userId));
        }

        /// <summary>
        //      POST request to edit a team
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.UPDATE_TEAM)]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("team", out string? serializedTeam))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_UPDATE_TEAM_FAIL_NO_DATA);
            }

            TeamModel? teamData = JsonConvert.DeserializeObject<TeamModel>(serializedTeam);
            if (teamData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamModel"));
            }

            string validationErrors = Utils.ValidateModel(teamData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            return CustomResponse(await service.UpdateTeam(teamData, GetUserId()));
        }

        /// <summary>
        //      POST request to delete the team with the provided id
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DELETE_TEAM)]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            return CustomResponse(await service.DeleteTeam(teamId, GetUserId()));
        }

        /// <summary>
        //      POST request to leave the team with the provided id
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.LEAVE_TEAM)]
        [Authorize]
        public async Task<ActionResult> Leave([FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            return CustomResponse(await service.LeaveTeam(teamId, GetUserId()));
        }

        /// <summary>
        //      POST request to invite the member to the team
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.INVITE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> InviteMember([FromQuery] string userId, [FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (teamId == 0 || string.IsNullOrEmpty(userId))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var result = await service.InviteMember(teamId, userId);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            var createNotification = await notificationService.AddTeamInviteNotification(userId, GetUserId(), teamId);
            if (!createNotification.IsSuccess()) {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_SEND_NOTIFICATION);
            }


            // Get the updated list of team members
            return CustomResponse(await service.GetMyTeamMembers(teamId));
        }

        /// <summary>
        //      POST request to remove the member from the team
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.REMOVE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> RemoveMember(Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("member", out string? serializedMember))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_NOTIFICATION_DELETED);
            }

            TeamMemberModel? data = JsonConvert.DeserializeObject<TeamMemberModel>(serializedMember);
            if (data == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "TeamMemberModel"));
            }

            var result = await service.RemoveMember(data);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Remove member action must return team id on success
            var teamId = result.Data[0].Id;

            // Remove all notifications related to TeamMember record
            await notificationService.DeleteNotifications(data, teamId);

            // Get the updated list of team members, 
            return CustomResponse(await service.GetMyTeamMembers(teamId));
        }

        /// <summary>
        //      POST request to accept team invitation
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.ACCEPT_TEAM_INVITE)]
        [Authorize]
        public async Task<ActionResult> AcceptInvite([FromQuery] string userId, long teamId)
        {
            return await ProcessAcceptDeclineInvitationRequest(userId, teamId, Constants.MemberTeamState.ACCEPTED.ToString());
        }

        /// <summary>
        //      POST request to decline team invitation
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DECLINE_TEAM_INVITE)]
        [Authorize]
        public async Task<ActionResult> DeclineInvite([FromQuery] string userId, long teamId)
        {
            return await ProcessAcceptDeclineInvitationRequest(userId, teamId, Constants.MemberTeamState.DECLINED.ToString());
        }

        /// <summary>
        //      Get request to return my teams
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_MY_TEAMS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeams([FromQuery] string teamType)
        {
            // Check if the neccessary data is provided
            if (string.IsNullOrEmpty(teamType))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_INVALID_TEAM_TYPE);
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
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_INVALID_TEAM_TYPE);
            }


            return CustomResponse(await service.GetMyTeams(type, GetUserId()));
        }

        /// <summary>
        //      Get request to return users by the specified name which are valid for team invitation
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_USERS_TO_INVITE)]
        [Authorize]
        public async Task<ActionResult> GetUsersToInvite([FromQuery] string name, [FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (string.IsNullOrEmpty(name) || teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_SEARCH_NAME_NOT_PROVIDED);
            }

            return CustomResponse(await service.GetUsersToInvite(name, teamId, GetUserId()));
        }

        /// <summary>
        //      Get team members when logged in user is coach
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_MY_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetTeamMembers([FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_TEAM_MEMBERS);
            }

            return CustomResponse(await service.GetMyTeamMembers(teamId));
        }

        /// <summary>
        //      Get team members when logged in user is member of the team
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_JOINED_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetJoinedTeamMembers([FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_TEAM_MEMBERS);
            }

            return CustomResponse(await service.GetJoinedTeamMembers(teamId, GetUserId()));
        }

        /// <summary>
        ///     Call the service method to accept / decline team invitation 
        /// </summary>
        /// <param name="userId">
        ///     The user id who accepts/declines the invitation
        /// </param>
        /// <param name="teamId">
        ///     The team id
        /// </param>
        /// <param name="newState">
        ///     "ACCEPTED" to accept the invitation, "DECLINE" to decline the invitation
        /// </param>
        private async Task<ActionResult> ProcessAcceptDeclineInvitationRequest(string userId, long teamId, string newState)
        {
            // Check if the neccessary data is provided
            if (string.IsNullOrEmpty(userId) || teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var result = await service.AcceptDeclineInvite(userId, teamId, newState);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            var returnData = new List<BaseModel>();

            if (result.Data.Count > 0 && result.Data[0].Id > 0)
            {
                // The result contains notification id, mark it as inactive for the logged in user
                var updateNotificationResult = await notificationService.UpdateNotification(result.Data[0].Id, false);

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
            }

            // Return response, showing the message from AcceptDeclineInvite action and the data returned from GetNotifications
            return CustomResponse(result.Code, result.Message, returnData);
        }
    }
}
