using FitnessAppAPI.Data.Services.MuscleGroups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route("api/muscle-group")]
    public class MGroupController(IMuscleGroupService s) : BaseController
    {
        /// <summary>
        //      MuscleGroupService instance
        /// </summary>
        private readonly IMuscleGroupService service = s;

        /// <summary>
        //      GET request to fetch the muscle groups for the user with the provided id
        /// </summary>
        [HttpGet("get-by-user")]
        [Authorize]
        public ActionResult GetMuscleGroups()
        {
            // Fetch the default and user defined muscle groups
            return CustomResponse(service.GetMuscleGroups(GetUserId()));
        }
    }
}
