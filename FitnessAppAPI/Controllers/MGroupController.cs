using FitnessAppAPI.Data.Services.MuscleGroups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.MUSCLE_GROUP)]
    public class MGroupController(IMuscleGroupService s) : BaseController
    {
        /// <summary>
        //      MuscleGroupService instance
        /// </summary>
        private readonly IMuscleGroupService service = s;

        /// <summary>
        //      GET request to fetch the muscle groups for the user with the provided id
        /// </summary>
        [HttpGet(Constants.RequestEndPoints.GET_MUSCLE_GROUPS_FOR_USER)]
        [Authorize]
        public ActionResult GetMuscleGroups()
        {
            // Fetch the default and user defined muscle groups
            return CustomResponse(service.GetMuscleGroups(GetUserId()));
        }
    }
}
