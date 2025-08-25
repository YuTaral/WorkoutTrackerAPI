using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.UserProfile;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Profile Controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.USER_PROFILES)]
    public class UserProfileController(IUserProfileService s, IUserService uService) : BaseController
    {
        /// <summary>
        //      IUserProfileService instance
        /// </summary>
        private readonly IUserProfileService service = s;

        /// <summary>
        //      IUserService instance
        /// </summary>
        private readonly IUserService userService = uService;

        /// <summary>
        //      Patch request to update user exercise default values 
        /// </summary>
        [HttpPatch(RequestEndPoints.DEFAULT_VALUES)]
        public async Task<ActionResult> UpdateUserDefaultValues([FromBody] Dictionary<string, string> requestData)
        {
            return await SendResponse(await service.UpdateUserDefaultValues(requestData, GetUserId()));
        }

        /// <summary>
        //      PATCH request to update user exercise default values 
        /// </summary>
        [HttpPatch]
        public async Task<ActionResult> UpdateUserProfile([FromBody] Dictionary<string, string> requestData)
        {
            var result = await service.UpdateUserProfile(requestData);

            if (!result.IsSuccess())
            {
                return await SendResponse(result);
            }

            // Get the updated user
            var updatedUserResult = await userService.GetUserModel(result.Data[0].Email);

            if (updatedUserResult.Id == "") {
                return await SendResponse(HttpStatusCode.NotFound, MSG_USER_DOES_NOT_EXISTS);
            }

            return await SendResponse((HttpStatusCode)result.Code, result.Message, [updatedUserResult]);
        }

        /// <summary>
        //      Get request to fetch the user default values for specific exercise
        [HttpGet(RequestEndPoints.DEFAULT_VALUES)]
        public async Task<ActionResult> Get([FromQuery] long mgExerciseId)
        {
            return await SendResponse(await service.GetExerciseOrUserDefaultValues(mgExerciseId, GetUserId()));   
        }
    }
}
