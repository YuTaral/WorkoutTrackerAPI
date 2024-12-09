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
    [Route(Constants.RequestEndPoints.WORKOUT_TEMPLATE)]
    public class WorkoutTemplateController(IWorkoutTemplateService s) : BaseController
    {
        /// <summary>
        //      WorkoutTemplateService instance
        /// </summary>
        private readonly IWorkoutTemplateService service = s;

        /// <summary>
        //      POST request to create a new workout template
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.ADD_WORKOUT_TEMPLATE)]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_ADD_FAIL_NO_DATA);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"));
            }

            string validationErrors = Utils.ValidateModel(workoutData);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.UNEXPECTED_ERROR, validationErrors);
            }

            return CustomResponse(await service.AddWorkoutTemplate(workoutData, GetUserId()));
        }

        /// <summary>
        //      POST request to delete the workout template
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DELETE_WORKOUT_TEMPLATE)]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long templateId)
        {
            // Check if the neccessary data is provided
            if (templateId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            // Delete the template
            return CustomResponse(await service.DeleteWorkoutTemplate(templateId));
        }

        /// <summary>
        //      Get request to fetch the workout templates of the logged in user
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_WORKOUT_TEMPLATES)]
        [Authorize]
        public async Task<ActionResult> GetTemplates()
        {
            return CustomResponse(await service.GetWorkoutTemplates(GetUserId()));
        }
    }
}
