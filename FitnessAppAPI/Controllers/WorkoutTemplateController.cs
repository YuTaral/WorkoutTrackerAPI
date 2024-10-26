using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Workouts.Models;
using FitnessAppAPI.Data.Services.WorkoutTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Templates Controller
    /// </summary>
    [ApiController]
    [Route("api/workout-template/")]
    public class WorkoutTemplateController(IWorkoutTemplateService s) : BaseController
    {
        /// <summary>
        //      WorkoutTemplateService instance
        /// </summary>
        private readonly IWorkoutTemplateService service = s;

        /// <summary>
        //      POST request to create a new workout template
        /// </summary>
        [HttpPost("add")]
        [Authorize]
        public ActionResult Add([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout))
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_ADD_FAIL_NO_DATA, []);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"), []);
            }

            string validationErrors = Utils.ValidateModel(workoutData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, validationErrors, []);
            }

            bool success = service.AddWorkoutTemplate(workoutData, GetUserId());

            // Success check
            if (success == false)
            {
                return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, []);
        }

        /// <summary>
        //      Get request to fetch the workout templates of the logged in user
        /// </summary>
        [HttpGet("get-templates")]
        [Authorize]
        public ActionResult GetTemplates()
        {
            var templates = service.GetWorkoutTemplates(GetUserId());

            if (templates.Count == 0) {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_NO_TEMPLATES, []);
            }

            var returnData = new List<string> { };
            returnData.AddRange(templates.Select(t => t.ToJson()));
            
            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }
    }
}
