using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Data.Services.UserProfile;
using FitnessAppAPI.Data.Services.UserProfile.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Profile Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.USER_PROFILE)]
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
        //      POST request to update user exercise default values 
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.UPDATE_USER_DEFAULT_VALUES)]
        public async Task<ActionResult> UpdateUserDefaultValues([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("values", out string? serializedValues))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_CHANGE_USER_DEF_VALUES);
            }

            UserDefaultValuesModel? data = JsonConvert.DeserializeObject<UserDefaultValuesModel>(serializedValues);
            if (data == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "UserDefaultValuesModel"));
            }

            string validationErrors = Utils.ValidateModel(data);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            return CustomResponse(await service.UpdateUserDefaultValues(data, GetUserId()));
        }

        /// <summary>
        //      POST request to update user exercise default values 
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.UPDATE_USER_PROFILE)]
        public async Task<ActionResult> UpdateUserProfile([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("user", out string? serializedUser))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_CHANGE_USER_DEF_VALUES);
            }

            UserModel? data = JsonConvert.DeserializeObject<UserModel>(serializedUser);
            if (data == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "UserModel"));
            }

            var validationErrors = Utils.ValidateModel(data);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            var result = await service.UpdateUserProfile(data);

            if (!result.IsSuccess())
            {
                return CustomResponse(result);
            }

            // Get the updated user
            var updatedUserResult = await userService.GetUserModel(data.Email);

            if (updatedUserResult.Id == "") {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_USER_DOES_NOT_EXISTS);
            }

            return CustomResponse(result.Code, result.Message, [updatedUserResult.ToJson()]);
        }

        /// <summary>
        //      Get request to fetch the user default values for specific exercise
        [HttpGet(Constants.RequestEndPoints.GET_USER_DEFAULT_VALUES)]
        public async Task<ActionResult> Get([FromQuery] long mgExerciseId)
        {
            return CustomResponse(await service.GetExerciseOrUserDefaultValues(mgExerciseId, GetUserId()));   
        }
    }
}
