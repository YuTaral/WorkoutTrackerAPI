using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FitnessAppAPI.Data.Services.Workouts;
using NuGet.Protocol;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseController(IExerciseService s, IWorkoutService ws) : BaseController
    {
        /// <summary>
        //      ExerciseService instance
        /// </summary>
        private readonly IExerciseService service = s;

        /// <summary>
        /// WorkoutService instance
        /// </summary>
        private readonly IWorkoutService workoutService = ws;

        /// <summary>
        //      POST request to create a new exercise
        /// </summary>
        [HttpPost("add")]
        [Authorize]
        public ActionResult AddExercise([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA, []);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"), []);
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, validationErrors, []);
            }

            // Add the exercise
            long id = long.Parse(workoutId);
            if (service.AddExercise(exerciseData, id))
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [workoutService.GetWorkout(id).ToJson()]);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to update an exercise
        /// </summary>
        [HttpPost("update")]
        [Authorize]
        public ActionResult UpdateExercise([FromBody] Dictionary<string, string> requestData)
        {

            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_UPDATE_FAIL_NO_DATA, []);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"), []);
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, validationErrors, []);
            }

            // Update the exercise
            long id = long.Parse(workoutId);
            if (service.UpdateExercise(exerciseData, id))
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [workoutService.GetWorkout(id).ToJson()]);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to delete an exercise
        /// </summary>
        [HttpPost("delete")]
        [Authorize]
        public ActionResult DeleteExercise([FromQuery] long exerciseId)
        {
            // Check if the neccessary data is provided
            if (exerciseId < 1)
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID, []);
            }

            // Delete the exercise
            var workoutId = service.DeleteExercise(exerciseId);
            if (workoutId > 0)
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [workoutService.GetWorkout(workoutId).ToJson()]);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }
    }
}
