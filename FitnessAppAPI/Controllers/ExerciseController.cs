using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FitnessAppAPI.Data.Services.Workouts;
using Microsoft.AspNetCore.Authorization;
using FitnessAppAPI.Data.Services;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.EXERCISE)]
    public class ExerciseController(IExerciseService s, IWorkoutService ws) : BaseController
    {
        /// <summary>
        //      ExerciseService instance
        /// </summary>
        private readonly IExerciseService service = s;

        /// <summary>
        ///     WorkoutService instance
        /// </summary>
        private readonly IWorkoutService workoutService = ws;

        /// <summary>
        //      POST request to add a new exercise to workout
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.ADD_EXERCISE_TO_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> AddExerciseToWorkout([FromBody] Dictionary<string, string> requestData)
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
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Add the exercise
            long id = long.Parse(workoutId);
            var result = await service.AddExerciseToWorkout(exerciseData, id);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return await GetUpdatedWorkout(id, result);
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to update an exercise
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.UPDATE_EXERCISE_FROM_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> UpdateExerciseFromWorkout([FromBody] Dictionary<string, string> requestData)
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
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Update the exercise
            long id = long.Parse(workoutId);
            var result = await service.UpdateExerciseFromWorkout(exerciseData, id);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return await GetUpdatedWorkout(id, result);
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to delete an exercise from workout
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DELETE_EXERCISE_FROM_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> DeleteExerciseFromWorkout([FromQuery] long exerciseId)
        {
            // Check if the neccessary data is provided
            if (exerciseId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            // Delete the exercise
            var result = await service.DeleteExerciseFromWorkout(exerciseId);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return await GetUpdatedWorkout(result.Data[0].Id, result);
            }

            return CustomResponse(result);
        }

        /// <summary>
        //      POST request to add a new exercise for specific muscle group
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.ADD_EXERCISE)]
        [Authorize]
        public async Task<ActionResult> AddExercise([FromBody] Dictionary<string, string> requestData)
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
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            requestData.TryGetValue("checkExistingEx", out string? checkExistingEx);
            checkExistingEx ??= "Y";

            // Add the exercise
            var result = await service.AddExercise(exerciseData, GetUserId(), checkExistingEx);

            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // If workout id is provided add the exercise to the workout and return the updated workout
            var workoutId = long.Parse(id);
            if (workoutId > 0)
            {
                var addExToWorkoutResult = await service.AddExerciseToWorkout((MGExerciseModel)result.Data[0], workoutId);
                if (addExToWorkoutResult.IsSuccess())
                {
                    // Return the updated workout on success
                    return await GetUpdatedWorkout(workoutId, addExToWorkoutResult);
                }
            }

            // If we got here, workout id is not provided and we don't need to return the updated workout,
            // but the exercises for this specific muscle group, so the client side can update them
            requestData.TryGetValue("onlyForUser", out string? onlyForUser);
            onlyForUser ??= "Y";

            return await GetUpdatedMGExercises(exerciseData.MuscleGroupId, onlyForUser, result);
        }

        /// <summary>
        //      POST request to update the muscle group exercise
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.UPDATE_EXERCISE)]
        [Authorize]
        public async Task<ActionResult> UpdateExercise([FromBody] Dictionary<string, string> requestData)
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
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            // Update the exercise
            var result = await service.UpdateExercise(exerciseData);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Check if we need to return all exercises or only the user defined
            requestData.TryGetValue("onlyForUser", out string? onlyForUser);
            onlyForUser ??= "Y";

            // Return the updated exercises for this muscle group
            return await GetUpdatedMGExercises(exerciseData.MuscleGroupId, onlyForUser, result);
        }

        /// <summary>
        //      POST request to delete an exercise
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.DELETE_EXERCISE)]
        [Authorize]
        public async Task<ActionResult> DeleteExercise([FromQuery] long MGExerciseId)
        {
            // Check if the neccessary data is provided
            if (MGExerciseId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            // Delete the exercise
            var result = await service.DeleteExercise(MGExerciseId, GetUserId());
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Return the updated exercises for this muscle group
            return await GetUpdatedMGExercises(result.Data[0].Id, "Y", result);
        }

        /// <summary>
        //      POST request to mark the set as completed
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.COMPLETE_SET)]
        [Authorize]
        public async Task<ActionResult> CompleteSet([FromQuery] long id, long workoutId)
        {
            // Check if the neccessary data is provided
            if (id < 1 || workoutId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            // Complete the set
            var result = await service.CompleteSet(id);
            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Return the updated workout where the set is completed
            return CustomResponse(await workoutService.GetWorkout(workoutId, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the exercise for muscle groups with the provided id
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_EXERCISES_FOR_MG)]
        [Authorize]
        public async Task<ActionResult> GetExercisesForMuscleGroup([FromQuery] long muscleGroupId, [FromQuery] string onlyForUser)
        {
            // Check if the neccessary data is provided
            if (muscleGroupId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_GET_EXERCISES_FOR_MG_FAILED);
            }

            // Return the exercises for this muscle group
            return CustomResponse(await service.GetExercisesForMG(muscleGroupId, GetUserId(), onlyForUser));
        }

        /// <summary>
        //      GET request to fetch the muscle group exercise with the provided id
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_MG_EXERCISE)]
        [Authorize]
        public async Task<ActionResult> GetMGExercise([FromQuery] long mGExerciseId)
        {
            // Check if the neccessary data is provided
            if (mGExerciseId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_OBJECT_ID_NOT_PROVIDED);
            }

            // Return the muscle group exercise
            return CustomResponse(await service.GetMGExercise(mGExerciseId));
        }

        /// <summary>
        ///     Try to fetch workout with the provided id, and if success returns response, combining
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

        private async Task<OkObjectResult> GetUpdatedWorkout(long id, ServiceActionResult previousActionResult) {
            var getWorkoutResult = await workoutService.GetWorkout(id, GetUserId());
            if (getWorkoutResult.IsSuccess())
            {
                // Combine the response and message from the previous action result with the updated workout
                return CustomResponse(previousActionResult.Code, previousActionResult.Message, getWorkoutResult.Data);
            }

            // If get workout failed, return the previous result
            return CustomResponse(previousActionResult);
        }

        /// <summary>
        ///     Try to fetch the muscle group exercises for the muscle group with the provided id, 
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
        private async Task<OkObjectResult> GetUpdatedMGExercises(long muscleGroupId, string onlyForUser, ServiceActionResult previousActionResult) {
            var getExercisesForMGResult = await service.GetExercisesForMG(muscleGroupId, GetUserId(), onlyForUser);

            if (getExercisesForMGResult.IsSuccess())
            {
                return CustomResponse(previousActionResult.Code, previousActionResult.Message, 
                                        getExercisesForMGResult.Data);
            }

            return CustomResponse(previousActionResult);
        }
    }
}
