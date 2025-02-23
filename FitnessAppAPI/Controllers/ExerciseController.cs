using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using FitnessAppAPI.Data.Services.Workouts;
using Microsoft.AspNetCore.Authorization;
using FitnessAppAPI.Data.Services;
using System.Net;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.EXERCISES)]
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
        [HttpPost(Constants.RequestEndPoints.TO_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> AddExerciseToWorkout([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.AddExerciseToWorkout(requestData);

            if (result.IsSuccess())
            {
                // Return the updated workout on success,
                // AddExerciseToWorkout must return workout id
                return await GetUpdatedWorkout(result.Data[0], result);
            }

            return SendResponse(result);
        }

        /// <summary>
        //      Patch request to update an exercise
        /// </summary>
        [HttpPatch(Constants.RequestEndPoints.EXERCISE_FROM_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> UpdateExerciseFromWorkout([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.UpdateExerciseFromWorkout(requestData);

            if (result.IsSuccess())
            {
                // Return the updated workout on success,
                // UpdateExerciseFromWorkout must return the workout id
                return await GetUpdatedWorkout(result.Data[0], result);
            }

            return SendResponse(result);
        }

        /// <summary>
        //      Delete request to delete an exercise from workout
        /// </summary>
        [HttpDelete(Constants.RequestEndPoints.EXERCISE_FROM_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> DeleteExerciseFromWorkout([FromQuery] long exerciseId)
        {
            // Delete the exercise
            var result = await service.DeleteExerciseFromWorkout(exerciseId);

            if (result.IsSuccess())
            {
                // Return the updated workout on success
                return await GetUpdatedWorkout(result.Data[0], result);
            }

            return SendResponse(result);
        }

        /// <summary>
        //      POST request to add a new exercise for specific muscle group
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddExercise([FromBody] Dictionary<string, string> requestData)
        {
            requestData.TryGetValue("checkExistingEx", out string? checkExistingEx);
            checkExistingEx ??= "Y";

            // Add the exercise
            var result = await service.AddExercise(requestData, GetUserId());

            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // If workout id is provided add the exercise to the workout and return the updated workout
            if (requestData.TryGetValue("workoutId", out string? id))
            {
                var workoutId = long.Parse(id);
                if (workoutId > 0)
                {
                    var addExToWorkoutResult = await service.AddExerciseToWorkout(result.Data[0], workoutId);
                    if (addExToWorkoutResult.IsSuccess())
                    {
                        // Return the updated workout on success
                        return await GetUpdatedWorkout(workoutId, addExToWorkoutResult);
                    }
                }
            }
           
            // If we got here, workout id is not provided and we don't need to return the updated workout,
            // but the exercises for this specific muscle group, so the client side can update them
            requestData.TryGetValue("onlyForUser", out string? onlyForUser);
            onlyForUser ??= "Y";

            return await GetUpdatedMGExercises(result.Data[0].MuscleGroupId, onlyForUser, result);
        }

        /// <summary>
        //      Patch request to update the muscle group exercise
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateExercise([FromBody] Dictionary<string, string> requestData)
        {
            // Update the exercise
            var result = await service.UpdateExercise(requestData);
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // Check if we need to return all exercises or only the user defined
            requestData.TryGetValue("onlyForUser", out string? onlyForUser);
            onlyForUser ??= "Y";

            // Return the updated exercises for this muscle group,
            // UpdateExercise must return MuscleGroupId
            return await GetUpdatedMGExercises(result.Data[0].MuscleGroupId, onlyForUser, result);
        }

        /// <summary>
        //      Delete request to delete an exercise
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteExercise([FromQuery] long MGExerciseId)
        {
            // Delete the exercise
            var result = await service.DeleteExercise(MGExerciseId, GetUserId());
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // Return the updated exercises for this muscle group,
            // DeleteExercise must return MuscleGroupId
            ServiceActionResult<MGExerciseModel> getExercisesForMgResult = await service.GetExercisesForMG(result.Data[0], "Y", GetUserId());

            if (getExercisesForMgResult.IsSuccess())
            {
                // Return the exercises for this muscle group
                return SendResponse(getExercisesForMgResult);
            }

            // Return the response from Delete Exercise
            return SendResponse(result);
        }

        /// <summary>
        //      Patch request to mark the set as completed
        /// </summary>
        [HttpPatch(Constants.RequestEndPoints.COMPLETE_SET)]
        [Authorize]
        public async Task<ActionResult> CompleteSet([FromBody] Dictionary<string, string> requestData)
        {
            // Complete the set
            var result = await service.CompleteSet(requestData);
            if (!result.IsSuccess())
            {
                return SendResponse(result);
            }

            // Return the updated workout where the set is completed
            if (requestData.TryGetValue("workoutId", out string? workoutIdString))
            {
                if (long.TryParse(workoutIdString, out long workoutId))
                {
                    return SendResponse(await workoutService.GetWorkout(workoutId, GetUserId()));
                }
            }

            return SendResponse(HttpStatusCode.OK);
        }

        /// <summary>
        //      GET request to fetch the exercise for muscle groups with the provided id
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.EXERCISES_FOR_MG)]
        [Authorize]
        public async Task<ActionResult> GetExercisesForMuscleGroup([FromQuery] long muscleGroupId, [FromQuery] string onlyForUser)
        {
            return SendResponse(await service.GetExercisesForMG(muscleGroupId, onlyForUser, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the muscle group exercise with the provided id
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.MG_EXERCISE)]
        [Authorize]
        public async Task<ActionResult> GetMGExercise([FromQuery] long mGExerciseId)
        {
            return SendResponse(await service.GetMGExercise(mGExerciseId));
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

        private async Task<ObjectResult> GetUpdatedWorkout(long id, ServiceActionResult<long> previousActionResult) {
            var getWorkoutResult = await workoutService.GetWorkout(id, GetUserId());
            if (getWorkoutResult.IsSuccess())
            {
                // Combine the response and message from the previous action result with the updated workout
                return SendResponse((HttpStatusCode) previousActionResult.Code, previousActionResult.Message, getWorkoutResult.Data);
            }

            // If get workout failed, return the previous result
            return SendResponse(previousActionResult);
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
        private async Task<ObjectResult> GetUpdatedMGExercises(long muscleGroupId, string onlyForUser, ServiceActionResult<MGExerciseModel> previousActionResult) {
            var getExercisesForMGResult = await service.GetExercisesForMG(muscleGroupId, onlyForUser, GetUserId());

            if (getExercisesForMGResult.IsSuccess())
            {
                return SendResponse((HttpStatusCode) previousActionResult.Code, previousActionResult.Message, getExercisesForMGResult.Data);
            }

            return SendResponse(previousActionResult);
        }
    }
}
