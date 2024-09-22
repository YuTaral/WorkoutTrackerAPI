using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController(IWorkoutService s) : BaseController
    {
        /// <summary>
        //      WorkoutService instance
        /// </summary>
        private readonly IWorkoutService service = s;


        /// <summary>
        //      POST request to create a new workout
        /// </summary>
        [HttpPost("add")]
        public ActionResult Add([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout) || !requestData.TryGetValue("userId", out string? userId))
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_ADD_FAIL_NO_DATA, []);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null) { 
                return ReturnResponse(true, Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"), []);
            }

            WorkoutModel? workout = service.AddWorkout(workoutData, userId);

            // Success check
            if (workout == null)
            {
                return ReturnResponse(false, Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_ERROR, []);
            }

            return ReturnResponse(false, Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [workout.ToJson()]);
        }

        /// <summary>
        //      POST request to create a new exercise
        /// </summary>
        [HttpPost("add-exercise")]
        public ActionResult AddExercise([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA, []);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"), []);
            }

            // Add the exercise
            long id = long.Parse(workoutId);
            if (service.AddExercise(exerciseData, id)) {
                return ReturnResponse(false, Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [service.GetWorkout(id).ToJson()]);
            } 

            return ReturnResponse(false, Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to update an exercise
        /// </summary>
        [HttpPost("update-exercise")]
        public ActionResult UpdateExercise([FromBody] Dictionary<string, string> requestData)
        {

            /// Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_UPDATE_FAIL_NO_DATA, []);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"), []);
            }

            // Update the exercise
            long id = long.Parse(workoutId);
            if (service.UpdateExercise(exerciseData, id)) {
                return ReturnResponse(false, Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [service.GetWorkout(id).ToJson()]);
            }

            return ReturnResponse(false, Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to delete an exercise
        /// </summary>
        [HttpPost("delete-exercise")]
        public ActionResult DeleteExercise([FromBody] long exerciseId)
        {
            /// Check if the neccessary data is provided
            if (exerciseId < 1)
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID, []);
            }

            // Delete the exercise
            var workoutId = service.DeleteExercise(exerciseId);
            if (workoutId > 0)
            {
                return ReturnResponse(false, Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [service.GetWorkout(workoutId).ToJson()]);
            }

            return ReturnResponse(false, Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }
    }
}
