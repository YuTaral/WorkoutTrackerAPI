using FitnessAppAPI.Data.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FitnessAppAPI.Common.Constants;


namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Team Controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.TEAMS)]
    public class TeamController(ITeamService s) : BaseController
    {   
        /// <summary>
        //      TeamService instance
        /// </summary>
        private readonly ITeamService service = s;

        /// <summary>
        //      POST request to create a new team
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.AddTeam(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to edit a team
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateTeam(requestData, GetUserId()));
        }

        /// <summary>
        //      Delete request to delete the team with the provided id
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long teamId)
        {
            return await SendResponse(await service.DeleteTeam(teamId, GetUserId()));
        }

        /// <summary>
        //      PUT request to leave the team with the provided id
        /// </summary>
        [HttpPut(RequestEndPoints.LEAVE_TEAM)]
        [Authorize]
        public async Task<ActionResult> Leave([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.LeaveTeam(requestData, GetUserId()));
        }

        /// <summary>
        //      POST request to invite the member to the team
        /// </summary>
        [HttpPost(RequestEndPoints.INVITE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> InviteMember([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.InviteMember(requestData, GetUserId());
            if (!result.IsSuccess())
            {
                return await SendResponse(result);
            }

            // InviteMember team id on success, Get the updated list of team members
            return await SendResponse(await service.GetMyTeamMembers(result.Data[0]));
        }

        /// <summary>
        //      DELETE request to remove the member from the team
        /// </summary>
        [HttpPatch(RequestEndPoints.REMOVE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> RemoveMember([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.RemoveMember(requestData);
            if (!result.IsSuccess())
            {
                return await SendResponse(result);
            }

            // Get the updated list of team members
            return await SendResponse(await service.GetMyTeamMembers(result.Data[0].TeamId));
        }

        /// <summary>
        //      PATCH request to accept team invitation
        /// </summary>
        [HttpPatch(RequestEndPoints.ACCEPT_TEAM_INVITE)]
        [Authorize]
        public async Task<ActionResult> AcceptInvite([FromBody] Dictionary<string, string> requestData)
        {
            return await ProcessAcceptDeclineInvitationRequest(requestData, MemberTeamState.ACCEPTED.ToString());
        }

        /// <summary>
        //      POST request to decline team invitation
        /// </summary>
        [HttpPatch(RequestEndPoints.DECLINE_TEAM_INVITE)]
        [Authorize]
        public async Task<ActionResult> DeclineInvite([FromBody] Dictionary<string, string> requestData)
        {
            return await ProcessAcceptDeclineInvitationRequest(requestData, MemberTeamState.DECLINED.ToString());
        }

        /// <summary>
        //      Get request to return my teams
        /// </summary>
        [HttpGet(RequestEndPoints.MY_TEAMS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeams([FromQuery] string teamType)
        {
            return await SendResponse(await service.GetMyTeams(teamType, GetUserId()));
        }

        /// <summary>
        //      Get request to return my teams with members
        /// </summary>
        [HttpGet(RequestEndPoints.MY_TEAMS_WITH_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeamsWithMembers()
        {
            return await SendResponse(await service.GetMyTeamsWithMembers(GetUserId()));
        }

        /// <summary>
        //      Get request to return users by the specified name which are valid for team invitation
        /// </summary>
        [HttpGet(RequestEndPoints.USERS_TO_INVITE)]
        [Authorize]
        public async Task<ActionResult> GetUsersToInvite([FromQuery] string name, [FromQuery] long teamId)
        {
            return await SendResponse(await service.GetUsersToInvite(name, teamId, GetUserId()));
        }

        /// <summary>
        //      Get team members when logged in user is coach
        /// </summary>
        [HttpGet(RequestEndPoints.MY_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetTeamMembers([FromQuery] long teamId)
        {
            return await SendResponse(await service.GetMyTeamMembers(teamId));
        }

        /// <summary>
        //      Get team members when logged in user is member of the team
        /// </summary>
        [HttpGet(RequestEndPoints.JOINED_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetJoinedTeamMembers([FromQuery] long teamId)
        {
            return await SendResponse(await service.GetJoinedTeamMembers(teamId, GetUserId()));
        }

        /// <summary>
        //      POST request to assign the workout
        /// </summary>
        [HttpPost(RequestEndPoints.ASSIGN_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> AssignWorkout([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.AssignWorkout(requestData, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the assigned workouts the specified start date and team id
        /// </summary>
        [HttpGet(RequestEndPoints.ASSIGNED_WORKOUTS)]
        [Authorize]
        public async Task<ActionResult> GetAssignedWorkouts([FromQuery] string startDate, [FromQuery] long teamId, [FromQuery] string teamType)
        {
            return await SendResponse(await service.GetAssignedWorkouts(startDate, teamId, teamType, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the assigned workout by the specified assigned workout id
        /// </summary>
        [HttpGet(RequestEndPoints.ASSIGNED_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> GetAssignedWorkout([FromQuery] long assignedWorkoutId)
        {
            return await SendResponse(await service.GetAssignedWorkout(assignedWorkoutId));
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
            return await SendResponse(await service.AcceptDeclineInvite(requestData, newState));
        }
        
    }
}
