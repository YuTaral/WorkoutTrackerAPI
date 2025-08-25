using FitnessAppAPI.Data.Services.MuscleGroups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.MUSCLE_GROUPS)]
    public class MGroupController(IMuscleGroupService s) : BaseController
    {
        /// <summary>
        //      MuscleGroupService instance
        /// </summary>
        private readonly IMuscleGroupService service = s;

        /// <summary>
        //      GET request to fetch the muscle groups for the user with the provided id
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetMuscleGroups()
        {
            // Fetch the default and user defined muscle groups
            return await SendResponse(await service.GetMuscleGroups(GetUserId()));
        }
    }
}
