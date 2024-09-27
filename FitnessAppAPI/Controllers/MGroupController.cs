using FitnessAppAPI.Data.Services.MuscleGroups;
using FitnessAppAPI.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;

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
        public ActionResult GetMuscleGroups([FromQuery] string userId)
        {
            // Check if the neccessary data is provided
            if (userId.IsNullOrEmpty())
            {
                return ReturnResponse(Constants.ResponseCode.BAD_REQUEST, Constants.MSG_OBJECT_ID_NOT_PROVIDED, []);
            }

            // Fetch the default and user defined muscle groups
            var returnData = new List<string> { };
            var muscleGroups = service.GetMuscleGroups(userId);

            if (muscleGroups != null)
            {
                foreach (var mg in muscleGroups)
                {
                    returnData.Add(mg.ToJson());
                }
            }

            return ReturnResponse(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }
    }
}
