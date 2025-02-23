using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessAppAPI.Data.Services.Notifications;
using FitnessAppAPI.Data.Models;
using System.Net;
using FitnessAppAPI.Data.Services.Notifications.Models;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Team Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.TEAMS)]
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
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.AddTeam(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to edit a team
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.UpdateTeam(requestData, GetUserId()));
        }

        /// <summary>
        //      Delete request to delete the team with the provided id
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long teamId)
        {
            return SendResponse(await service.DeleteTeam(teamId, GetUserId()));
        }

        /// <summary>
        //      PUT request to leave the team with the provided id
        /// </summary>
        [HttpPut(Constants.RequestEndPoints.LEAVE_TEAM)]
        [Authorize]
        public async Task<ActionResult> Leave([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.LeaveTeam(requestData, GetUserId()));
        }

        /// <summary>
        //      POST request to invite the member to the team
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.INVITE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> InviteMember([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.InviteMember(requestData);
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // InviteMember must return team and userId on success
            var teamId = long.Parse(result.Data[0]);
            var userId = result.Data[1];

            var createNotification = await notificationService.AddTeamInviteNotification(userId, GetUserId(), teamId);
            if (!createNotification.IsSuccess()) {
                return SendResponse(createNotification);
            }

            // Get the updated list of team members
            return SendResponse(await service.GetMyTeamMembers(teamId));
        }

        /// <summary>
        //      DELETE request to remove the member from the team
        /// </summary>
        [HttpPatch(Constants.RequestEndPoints.REMOVE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> RemoveMember([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.RemoveMember(requestData);
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // Remove all notifications related to TeamMember record
            await notificationService.DeleteNotifications(result.Data[0]);

            // Get the updated list of team members, 
            return SendResponse(await service.GetMyTeamMembers(result.Data[0].TeamId));
        }

        /// <summary>
        //      PATCH request to accept team invitation
        /// </summary>
        [HttpPatch(Constants.RequestEndPoints.ACCEPT_TEAM_INVITE)]
        [Authorize]
        public async Task<ActionResult> AcceptInvite([FromBody] Dictionary<string, string> requestData)
        {
            return await ProcessAcceptDeclineInvitationRequest(requestData, Constants.MemberTeamState.ACCEPTED.ToString());
        }

        /// <summary>
        //      POST request to decline team invitation
        /// </summary>
        [HttpPatch(Constants.RequestEndPoints.DECLINE_TEAM_INVITE)]
        [Authorize]
        public async Task<ActionResult> DeclineInvite([FromBody] Dictionary<string, string> requestData)
        {
            return await ProcessAcceptDeclineInvitationRequest(requestData, Constants.MemberTeamState.DECLINED.ToString());
        }

        /// <summary>
        //      Get request to return my teams
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.MY_TEAMS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeams([FromQuery] string teamType)
        {
            return SendResponse(await service.GetMyTeams(teamType, GetUserId()));
        }

        /// <summary>
        //      Get request to return my teams with members
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.MY_TEAMS_WITH_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeamsWithMembers()
        {
            return SendResponse(await service.GetMyTeamsWithMembers(GetUserId()));
        }

        /// <summary>
        //      Get request to return users by the specified name which are valid for team invitation
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.USERS_TO_INVITE)]
        [Authorize]
        public async Task<ActionResult> GetUsersToInvite([FromQuery] string name, [FromQuery] long teamId)
        {
            return SendResponse(await service.GetUsersToInvite(name, teamId, GetUserId()));
        }

        /// <summary>
        //      Get team members when logged in user is coach
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.MY_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetTeamMembers([FromQuery] long teamId)
        {
            return SendResponse(await service.GetMyTeamMembers(teamId));
        }

        /// <summary>
        //      Get team members when logged in user is member of the team
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.JOINED_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetJoinedTeamMembers([FromQuery] long teamId)
        {
            return SendResponse(await service.GetJoinedTeamMembers(teamId, GetUserId()));
        }

        /// <summary>
        ///     Call the service method to accept / decline team invitation 
        /// </summary>
        /// <param name="requestData">
        ///     The request data (user id who accepts/declines the invitation and team id)
        /// </param>
        /// <param name="newState">
        ///     "ACCEPTED" to accept the invitation, "DECLINE" to decline the invitation
        /// </param>
        private async Task<ActionResult> ProcessAcceptDeclineInvitationRequest(Dictionary<string, string> requestData, string newState)
        {
            var result = await service.AcceptDeclineInvite(requestData, newState);
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            var returnData = new List<NotificationModel>();

            if (result.Data.Count > 1)
            {
                // The result contains notification, team and user ids, mark it as inactive for the logged in user
                var notificationId = long.Parse(result.Data[0]);
                var teamId = long.Parse(result.Data[1]);
                var userId = result.Data[2];

                var updateNotificationResult = await notificationService.UpdateNotification(notificationId, false);

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
            return SendResponse(HttpStatusCode.OK, result.Message, returnData);
        }
    }
}
