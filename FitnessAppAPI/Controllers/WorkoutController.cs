using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.WORKOUTS)]
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
            return await SendResponse(await service.AddWorkout(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to edit a workout
        /// </summary>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateWorkout(requestData, GetUserId()));
        }

        /// <summary>
        //      Patch request to finish the workout
        /// </summary>
        [HttpPatch(RequestEndPoints.FINISH_WORKOUT)]
        [Authorize]
        public async Task<ActionResult> FinishWorkout([FromQuery] long workoutId)
        {
            return await SendResponse(await service.FinishWorkout(workoutId, GetUserId()));
        }

        /// <summary>
        //      POST request to delete the workout with the provided id
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromQuery] long workoutId)
        {
            return await SendResponse(await service.DeleteWorkout(workoutId, GetUserId()));
        }

        /// <summary>
        //      GET request to fetch the latest workouts for user
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetLatestWorkouts([FromQuery] string startDate)
        {
            return await SendResponse(await service.GetLatestWorkouts(startDate, GetUserId()));
        }
    }
}
