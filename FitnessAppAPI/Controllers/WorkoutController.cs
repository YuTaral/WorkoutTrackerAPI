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
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout) || !requestData.TryGetValue("userId", out string? userId))
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_WORKOUT_ADD_FAIL_NO_DATA, []);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null) 
            { 
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"), []);
            }

            WorkoutModel? workout = service.AddWorkout(workoutData, userId);

            // Success check
            if (workout == null)
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_UNEXPECTED_ERROR, []);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [workout.ToJson()]);
        }

        /// <summary>
        //      POST request to edit a workout
        /// </summary>
        [HttpPost("edit")]
        public ActionResult Edit([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout) || !requestData.TryGetValue("userId", out string? userId))
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_WORKOUT_ADD_FAIL_NO_DATA, []);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"), []);
            }

            WorkoutModel? workout = service.EditWorkout(workoutData, userId);

            // Success check
            if (workout == null)
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_UNEXPECTED_ERROR, []);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [workout.ToJson()]);
        }

        /// <summary>
        //      POST request to delete the workout with the provided id
        /// </summary>
        [HttpPost("delete")]
        public ActionResult Delete([FromQuery] string workoutId)
        {
            // Check if the neccessary data is provided
            if (workoutId.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            var success = service.DeleteWorkout(long.Parse(workoutId));

            if (success)
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, []);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to create a new exercise
        /// </summary>
        [HttpPost("add-exercise")]
        public ActionResult AddExercise([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA, []);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"), []);
            }

            // Add the exercise
            long id = long.Parse(workoutId);
            if (service.AddExercise(exerciseData, id)) 
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [service.GetWorkout(id).ToJson()]);
            } 

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to update an exercise
        /// </summary>
        [HttpPost("update-exercise")]
        public ActionResult UpdateExercise([FromBody] Dictionary<string, string> requestData)
        {

            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_EXERCISE_UPDATE_FAIL_NO_DATA, []);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"), []);
            }

            // Update the exercise
            long id = long.Parse(workoutId);
            if (service.UpdateExercise(exerciseData, id)) 
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [service.GetWorkout(id).ToJson()]);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      POST request to delete an exercise
        /// </summary>
        [HttpPost("delete-exercise")]
        public ActionResult DeleteExercise([FromBody] long exerciseId)
        {
            // Check if the neccessary data is provided
            if (exerciseId < 1)
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID, []);
            }

            // Delete the exercise
            var workoutId = service.DeleteExercise(exerciseId);
            if (workoutId > 0)
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, [service.GetWorkout(workoutId).ToJson()]);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      GET request to fetch the latest workouts for user
        /// </summary>
        [HttpGet("get-workouts")]
        public ActionResult GetLatestWorkouts([FromQuery] string userId)
        {
            // Check if the neccessary data is provided
            if (userId.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            var returnData = new List<string> {};
            var latestWorkouts = service.GetLatestWorkouts(userId);

            if (latestWorkouts != null) 
            {
                foreach (var w in latestWorkouts)
                {
                    returnData.Add(w.ToJson());
                }
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }

        /// <summary>
        //      GET request to fetch the workout with the provided id
        /// </summary>
        [HttpGet("get-workout")]
        public ActionResult GetWorkout([FromQuery] string workoutId)
        {
            // Check if the neccessary data is provided
            if (workoutId.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, 
                [service.GetWorkout(long.Parse(workoutId)).ToJson()]);
        }

        /// <summary>
        //      GET request to fetch the muscle groups for the user with the provided id
        /// </summary>
        [HttpGet("get-muscle-groups")]
        public ActionResult GetMuscleGroups([FromQuery] string userId)
        {
            // Check if the neccessary data is provided
            if (userId.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            // Fetch the default and user defined muscle groups
            var returnData = new List<string> { };
            var muscleGroups = service.GetMuscleGroups(userId);

            if (muscleGroups != null)
            {
                foreach (var mg in muscleGroups)
                {
                    returnData.Add(mg.ToJson());
                }
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }
    }
}
