using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.AspNetCore.Authorization;
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

            WorkoutModel? workout = service.AddWorkout(workoutData, GetUserId());

            // Success check
            if (workout == null)
            {
                return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
            }

            // Check if this is template and add the exercises if so
            if (workoutData.Template && workoutData.Exercises != null) {
                foreach (ExerciseModel e in workoutData.Exercises) {
                    exerciseService.AddExercise(e, workout.Id);
                }

                workout = service.GetWorkout(workout.Id);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_ADDED, [workout.ToJson()]);
        }

        /// <summary>
        //      POST request to edit a workout
        /// </summary>
        [HttpPost("edit")]
        [Authorize]
        public ActionResult Edit([FromBody] Dictionary<string, string> requestData)
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
                return ReturnResponse(Constants.ResponseCode.FAIL, validationErrors, []);
            }

            WorkoutModel? workout = service.EditWorkout(workoutData, GetUserId());

            // Success check
            if (workout == null)
            {
                return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_UPDATED, [workout.ToJson()]);
        }

        /// <summary>
        //      POST request to delete the workout with the provided id
        /// </summary>
        [HttpPost("delete")]
        [Authorize]
        public ActionResult Delete([FromQuery] string workoutId)
        {
            // Check if the neccessary data is provided
            if (workoutId.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            if (service.DeleteWorkout(long.Parse(workoutId)))
            {
                return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_WORKOUT_DELETED, []);
            }

            return ReturnResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []);
        }

        /// <summary>
        //      GET request to fetch the latest workouts for user
        /// </summary>
        [HttpGet("get-workouts")]
        [Authorize]
        public ActionResult GetLatestWorkouts()
        {
            var returnData = new List<string> {};
            var latestWorkouts = service.GetLatestWorkouts(GetUserId());

            if (latestWorkouts != null) 
            {
                returnData.AddRange(latestWorkouts.Select(w => w.ToJson()));
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }

        /// <summary>
        //      GET request to fetch the workout with the provided id
        /// </summary>
        [HttpGet("get-workout")]
        [Authorize]
        public ActionResult GetWorkout([FromQuery] long workoutId)
        {
            // Check if the neccessary data is provided
            if (workoutId < 1)
            {
                return ReturnResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, 
                [service.GetWorkout(workoutId).ToJson()]);
        }
    }
}
