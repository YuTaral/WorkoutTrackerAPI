﻿using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.Workouts.Models;
using FitnessAppAPI.Data.Services.WorkoutTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     Workout Templates Controller
    /// </summary>
    [ApiController]
    [Route("api/workout-template/")]
    public class WorkoutTemplateController(IWorkoutTemplateService s) : BaseController
    {
        /// <summary>
        //      WorkoutTemplateService instance
        /// </summary>
        private readonly IWorkoutTemplateService service = s;

        /// <summary>
        //      POST request to create a new workout template
        /// </summary>
        [HttpPost("add")]
        [Authorize]
        public ActionResult Add([FromBody] Dictionary<string, string> requestData)
        {
            // Check if the neccessary data is provided
            if (!requestData.TryGetValue("workout", out string? serializedWorkout))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_WORKOUT_ADD_FAIL_NO_DATA);
            }

            WorkoutModel? workoutData = JsonConvert.DeserializeObject<WorkoutModel>(serializedWorkout);
            if (workoutData == null)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, string.Format(Constants.MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "WorkoutModel"));
            }

            string validationErrors = Utils.ValidateModel(workoutData);
            if (!validationErrors.IsNullOrEmpty())
            {
                return CustomResponse(Constants.ResponseCode.UNEXPECTED_ERROR, validationErrors);
            }

            return CustomResponse(service.AddWorkoutTemplate(workoutData, GetUserId()));
        }

        /// <summary>
        //      POST request to delete the workout template
        /// </summary>
        [HttpPost("delete")]
        [Authorize]
        public ActionResult Delete([FromQuery] long templateId)
        {
            // Check if the neccessary data is provided
            if (templateId < 1)
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_EXERCISE_DELETE_FAIL_NO_ID);
            }

            // Delete the template
            return CustomResponse(service.DeleteWorkoutTemplate(templateId, GetUserId()));
        }

        /// <summary>
        //      Get request to fetch the workout templates of the logged in user
        /// </summary>
        [HttpGet("get-templates")]
        [Authorize]
        public ActionResult GetTemplates()
        {
            return CustomResponse(service.GetWorkoutTemplates(GetUserId()));
        }
    }
}
