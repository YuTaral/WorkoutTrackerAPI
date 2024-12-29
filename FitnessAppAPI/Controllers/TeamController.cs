using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FitnessAppAPI.Data.Services.Teams.Models;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Team Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.TEAM)]
    public class TeamController(ITeamService s) : BaseController
    {   
        /// <summary>
        //      TeamService instance
        /// </summary>
        private readonly ITeamService service = s;

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

            return CustomResponse(await service.UpdateTeam(teamData));
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

            // Get the updated list of team members
            return CustomResponse(await service.GetTeamMembers(teamId));
        }

        /// <summary>
        //      POST request to remove the member from the team
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.REMOVE_MEMBER)]
        [Authorize]
        public async Task<ActionResult> RemoveMember([FromQuery] long recordId)
        {
            // Check if the neccessary data is provided
            if (recordId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            var result = await service.RemoveMember(recordId);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Get the updated list of team members, the remove member action must
            // return team id on success
            return CustomResponse(await service.GetTeamMembers(result.Data[0].Id));
        }

        /// <summary>
        //      Get request to return my teams
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_MY_TEAMS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeams()
        {
            return CustomResponse(await service.GetMyTeams(GetUserId()));
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
        //      Get team members 
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_TEAM_MEMBERS)]
        [Authorize]
        public async Task<ActionResult> GetTeamMembers([FromQuery] long teamId)
        {
            // Check if the neccessary data is provided
            if (teamId == 0)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_SEARCH_NAME_NOT_PROVIDED);
            }

            return CustomResponse(await service.GetTeamMembers(teamId));
        }
    }
}
