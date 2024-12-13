using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.WORKOUT)]
    public class WorkoutController(IWorkoutService s, IExerciseService eService) : BaseController
    {
        /// <summary>
        //      WorkoutService instance
        /// </summary>
        private readonly IWorkoutService service = s;

        /// <summary>
        //      ExerciseService instance
        /// </summary>
        private readonly IExerciseService exerciseService = eService;


        /// <summary>
        //      POST request to create a new workout
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.ADD_WORKOUT)]
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

            var userId = GetUserId();

            ServiceActionResult result = await service.AddWorkout(workoutData, userId);

            // Success check
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // If success, return data must contain WorkoutModel
            // Check if this is template and add the exercises if so
            if (workoutData.Template && workoutData.Exercises != null) {
                foreach (ExerciseModel e in workoutData.Exercises) {
                    await exerciseService.AddExerciseToWorkout(e, result.Data[0].Id);
                }

                // Get the updated workout
                var getWorkoutResult = await service.GetWorkout(result.Data[0].Id, userId);
                if (getWorkoutResult.IsSuccess()) {
                    return CustomResponse(result.Code, result.Message, getWorkoutResult.Data);
                }
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to edit a workout
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.UPDATE_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
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
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            return CustomResponse(await service.UpdateWorkout(workoutData, GetUserId()));
        }

        /// <summary>
        //      POST request to delete the workout with the provided id
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DELETE_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] string workoutId)
        {
            // Check if the neccessary data is provided
            if (string.IsNullOrEmpty(workoutId))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            return CustomResponse(await service.DeleteWorkout(long.Parse(workoutId), GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the latest workouts for user
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_WORKOUTS)]
        [Authorize]
        public async Task<ActionResult> GetLatestWorkouts([FromQuery] string filterBy)
        {
            if (string.IsNullOrEmpty(filterBy))
            {
                // Default to ALL
                filterBy = Constants.WorkoutFilters.ALL;
            }

            return CustomResponse(await service.GetLatestWorkouts(filterBy, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the workout with the provided id
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> GetWorkout([FromQuery] long workoutId)
        {
            // Check if the neccessary data is provided
            if (workoutId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            return CustomResponse(await service.GetWorkout(workoutId, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the weight units
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_WEIGHT_UNITS)]
        public async Task<ActionResult> GetWeightUnits()
        {
            return CustomResponse(await service.GetWeightUnits());
        }
    }
}
