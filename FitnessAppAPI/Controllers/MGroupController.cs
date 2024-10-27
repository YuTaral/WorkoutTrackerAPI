using FitnessAppAPI.Data.Services.MuscleGroups;
using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Controller
    /// </summary>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MGroupController(IMuscleGroupService s) : BaseController
    {
        /// <summary>
        //      MuscleGroupService instance
        /// </summary>
        private readonly IMuscleGroupService service = s;

        /// <summary>
        //      GET request to fetch the muscle groups for the user with the provided id
        /// </summary>
        [HttpGet("get-muscle-groups")]
        [Authorize]
        public ActionResult GetMuscleGroups()
        {
            // Fetch the default and user defined muscle groups
            var returnData = new List<string> { };
            var muscleGroups = service.GetMuscleGroups(GetUserId());

            if (muscleGroups != null)
            {
                returnData.AddRange(muscleGroups.Select(mg => mg.ToJson()));
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }
    }
}
