using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.Exercises.Models;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Workouts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.WORKOUTS)]
    public class WorkoutController(IWorkoutService s) : BaseController
    {
        /// <summary>
        //      WorkoutService instance
        /// </summary>
        private readonly IWorkoutService service = s;

        /// <summary>
        //      POST request to create a new workout
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.AddWorkout(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to edit a workout
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.UpdateWorkout(requestData, GetUserId()));
        }

        /// <summary>
        //      POST request to delete the workout with the provided id
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long workoutId)
        {
            return SendResponse(await service.DeleteWorkout(workoutId, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the latest workouts for user
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetLatestWorkouts([FromQuery] string startDate)
        {
            return SendResponse(await service.GetLatestWorkouts(startDate, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the weight units
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.WEIGHT_UNITS)]
        public async Task<ActionResult> GetWeightUnits()
        {
            return SendResponse(await service.GetWeightUnits());
        }
    }
}
