using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Workouts.Models;
using FitnessAppAPI.Data.Services.WorkoutTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Templates Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.WORKOUT_TEMPLATES)]
    public class WorkoutTemplateController(IWorkoutTemplateService s) : BaseController
    {
        /// <summary>
        //      WorkoutTemplateService instance
        /// </summary>
        private readonly IWorkoutTemplateService service = s;

        /// <summary>
        //      POST request to create a new workout template
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.AddWorkoutTemplate(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to update the workout template
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.UpdateWorkoutTemplate(requestData, GetUserId()));
        }

        /// <summary>
        //      Delete request to delete the workout template
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long templateId)
        {
            return SendResponse(await service.DeleteWorkoutTemplate(templateId));
        }

        /// <summary>
        //      Get request to fetch the workout templates of the logged in user
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetTemplates()
        {
            return SendResponse(await service.GetWorkoutTemplates(GetUserId()));
        }
    }
}
