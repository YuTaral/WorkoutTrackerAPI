using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
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
        //      Get request to return my teams
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_MY_TEAMS)]
        [Authorize]
        public async Task<ActionResult> GetMyTeams()
        {
            return CustomResponse(await service.GetMyTeams(GetUserId()));
        }
    }
}
