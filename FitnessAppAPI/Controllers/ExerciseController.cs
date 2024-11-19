using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FitnessAppAPI.Data.Services.Workouts;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using FitnessAppAPI.Data.Services;

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
        //      POST request to add a new exercise to workout
        /// </summary>
        [HttpPost("add-to-workout")]
        [Authorize]
        public ActionResult AddExerciseToWorkout([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Add the exercise
            long id = long.Parse(workoutId);
            var result = service.AddExerciseToWorkout(exerciseData, id);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return GetUpdatedWorkout(id, result);
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to update an exercise
        /// </summary>
        [HttpPost("update-exercise-from-workout")]
        [Authorize]
        public ActionResult UpdateExerciseFromWorkout([FromBody] Dictionary<string, string> requestData)
        {

            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? workoutId))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_UPDATE_FAIL_NO_DATA);
            }

            ExerciseModel? exerciseData = JsonConvert.DeserializeObject<ExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "ExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Update the exercise
            long id = long.Parse(workoutId);
            var result = service.UpdateExerciseFromWorkout(exerciseData, id);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return GetUpdatedWorkout(id, result);
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to delete an exercise from workout
        /// </summary>
        [HttpPost("delete-exercise-from-workout")]
        [Authorize]
        public ActionResult DeleteExerciseFromWorkout([FromQuery] long exerciseId)
        {
            // Check if the neccessary data is provided
            if (exerciseId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            // Delete the exercise
            var result = service.DeleteExerciseFromWorkout(exerciseId);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return GetUpdatedWorkout(result.ResponseData[0].Id, result);
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to add a new exercise for specific muscle group
        /// </summary>
        [HttpPost("add")]
        [Authorize]
        public ActionResult AddExercise([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise) || !requestData.TryGetValue("workoutId", out string? id))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA);
            }

            MGExerciseModel? exerciseData = JsonConvert.DeserializeObject<MGExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "MGExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Add the exercise
            var result = service.AddExercise(exerciseData, GetUserId());

            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // If workout id is provided add the exercise to the workout and return the updated workout
            var workoutId = long.Parse(id);
            if (workoutId > 0)
            {
                var addExToWorkoutResult = service.AddExerciseToWorkout((ExerciseModel) result.ResponseData[0], workoutId);
                if (addExToWorkoutResult.IsSuccess())
                {
                    // Return the updated workout on success
                    return GetUpdatedWorkout(workoutId, addExToWorkoutResult);
                }
            }

            // If we got here, workout id is not provided and we don't need to return the updated workout,
            // but the exercises for this specific muscle group, so the client side can update them
            requestData.TryGetValue("onlyForUser", out string? onlyForUser);
            if (onlyForUser == null)
            {
                onlyForUser = "Y";
            }

            return GetUpdatedMGExercises(exerciseData.MuscleGroupId, onlyForUser, result);
        }

        /// <summary>
        //      POST request to update the muscle group exercise
        /// </summary>
        [HttpPost("update")]
        [Authorize]
        public ActionResult UpdateExercise([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("exercise", out string? serializedExercise))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_ADD_FAIL_NO_DATA);
            }

            MGExerciseModel? exerciseData = JsonConvert.DeserializeObject<MGExerciseModel>(serializedExercise);
            if (exerciseData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "MGExerciseModel"));
            }

            string validationErrors = Utils.ValidateModel(exerciseData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Update the exercise
            var result = service.UpdateExercise(exerciseData);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Return the updated exercises for this muscle group
            return GetUpdatedMGExercises(exerciseData.MuscleGroupId, "Y", result);
        }

        /// <summary>
        //      POST request to delete an exercise
        /// </summary>
        [HttpPost("delete")]
        [Authorize]
        public ActionResult DeleteExercise([FromQuery] long MGExerciseId)
        {
            // Check if the neccessary data is provided
            if (MGExerciseId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            // Delete the exercise
            var result = service.DeleteExercise(MGExerciseId);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Return the updated exercises for this muscle group
            return GetUpdatedMGExercises(result.ResponseData[0].Id, "Y", result);
        }

        /// <summary>
        //      GET request to fetch the exercise for muscle groups with the provided id
        /// </summary>
        [HttpGet("get-by-mg-id")]
        [Authorize]
        public ActionResult GetExercisesForMuscleGroup([FromQuery] long muscleGroupId, [FromQuery] string onlyForUser)
        {
            // Check if the neccessary data is provided
            if (muscleGroupId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_GET_EXERCISES_FOR_MG_FAILED);
            }

            // Return the exercises for this muscle group
            return CustomResponse(service.GetExercisesForMG(muscleGroupId, GetUserId(), onlyForUser));
        }

        /// <summary>
        ///     Tries to fetch workout with the provided id, and if success returns response, combining
        ///     result.ResponseCode, result.ResponseMessage and the workout. The combination is used in
        ///     order to display the message set in previousActionResult variable, which is the result of the 
        ///     previous action (the actual action) executed in the controller method. E.g adding exercise to workout, returing whether
        ///     the add was successfull and returning the updated workout
        /// </summary>
        /// <param name="id">
        ///     The workout id to fetch
        /// </param>
        /// <param name="mainActionResult">
        ///     The result of the main action executed in the controller method
        /// </param>
        
        private OkObjectResult GetUpdatedWorkout(long id, ServiceActionResult previousActionResult) {
            var getWorkoutResult = workoutService.GetWorkout(id);
            if (getWorkoutResult.IsSuccess())
            {
                // Combine the response and message from the previous action result with the updated workout
                return CustomResponse(previousActionResult.ResponseCode, previousActionResult.ResponseMessage, getWorkoutResult.ResponseData);
            }

            // If get workout failed, return the previous result
            return CustomResponse(previousActionResult);
        }

        /// <summary>
        ///     Tries to fetch the muscle group exercises for the muscle group with the provided id, 
        ///     and if success returns response, combining result.ResponseCode, result.ResponseMessage and the exercises. 
        ///     The combination is used in order to display the message set in previousActionResult variable, 
        ///     which is the result of the previous action (the actual action) executed in the controller method. 
        ///     E.g adding exercise for muscle group, returing whether the add was successfull and returning the exercises
        ///     for this specific muscle group
        /// </summary>
        /// <param name="muscleGroupId">
        ///     The muscle group id
        /// </param>
        /// <param name="onlyForUser">
        ///     "Y" if the exercise must be only the ones which are user defined, "N" if all 
        /// </param>
        /// <param name="previousActionResult">
        ///     The result of the main action executed in the controller method
        /// </param>
        private OkObjectResult GetUpdatedMGExercises(long muscleGroupId, string onlyForUser, ServiceActionResult previousActionResult) {
            var getExercisesForMGResult = service.GetExercisesForMG(muscleGroupId, GetUserId(), onlyForUser);

            if (getExercisesForMGResult.IsSuccess())
            {
                return CustomResponse(previousActionResult.ResponseCode, previousActionResult.ResponseMessage, 
                                    getExercisesForMGResult.ResponseData);
            }

            return CustomResponse(previousActionResult);
        }

    }
}
