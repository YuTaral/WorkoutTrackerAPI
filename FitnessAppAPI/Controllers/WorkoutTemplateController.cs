using FitnessAppAPI.Data.Services.WorkoutTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Templates Controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.WORKOUT_TEMPLATES)]
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
            var userId = GetUserId();
            var result = await service.AddWorkoutTemplate(requestData, userId);

            if (!result.IsSuccess())
            {
                return await SendResponse(result);
            }

             return await SendResponse(await service.GetWorkoutTemplates(userId));
        }

        /// <summary>
        //      Patch request to update the workout template
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            var userId = GetUserId();
            var result = await service.UpdateWorkoutTemplate(requestData, userId);

            if (!result.IsSuccess())
            {
                return await SendResponse(result);
            }

            return await SendResponse(await service.GetWorkoutTemplates(userId));
        }

        /// <summary>
        //      Delete request to delete the workout template
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long templateId)
        {
            var result = await service.DeleteWorkoutTemplate(templateId);

            if (!result.IsSuccess())
            {
                return await SendResponse(result);
            }

            return await SendResponse(await service.GetWorkoutTemplates(GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the workout templates of the logged in user
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetTemplates()
        {
            return await SendResponse(await service.GetWorkoutTemplates(GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the workout template by assigned workout id
        /// </summary>
        [HttpGet(RequestEndPoints.GET_WORKOUT_TEMPLATE)]
        [Authorize]
        public async Task<ActionResult> GetWorkoutTemplate([FromQuery] long assignedWorkoutId)
        {
            return await SendResponse(await service.GetWorkoutTemplate(assignedWorkoutId));
        }
    }
}
