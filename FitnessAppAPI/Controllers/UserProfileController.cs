﻿using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.UserProfile;
using FitnessAppAPI.Data.Services.UserProfile.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Profile Controller
    /// </summary>
    [ApiController]
    [Route("api/user-profile")]
    public class UserProfileController(IUserProfileService s) : BaseController
    {
        /// <summary>
        //      IUserProfileService instance
        /// </summary>
        private readonly IUserProfileService service = s;

        /// <summary>
        //      POST request to update user exercise default values 
        /// </summary>
        [HttpPost("update-default-values")]
        public ActionResult UpdateUserDefaultValues([FromBody] Dictionary<string, string> requestData)
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
            if (!validationErrors.IsNullOrEmpty())
            {
                return CustomResponse(Constants.ResponseCode.FAIL, validationErrors);
            }

            return CustomResponse(service.UpdateUserDefaultValues(data, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the weight units
        /// </summary>
        [HttpGet("get-weight-units")]
        public ActionResult GetWeightUnits()
        {
            return CustomResponse(service.GetWeightUnits(GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the user default values for specific exercise
        [HttpGet("get-user-default-values")]
        public ActionResult Get([FromQuery] long mgExerciseId)
        {
            return CustomResponse(service.GetExerciseOrUserDefaultValues(mgExerciseId, GetUserId()));   
        }
    }
}
