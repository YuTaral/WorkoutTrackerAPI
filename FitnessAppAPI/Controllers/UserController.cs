using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Data.Services.Workouts;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService s, IWorkoutService workoutS) : BaseController
    {
        /// <summary>
        //      UserService instance
        /// </summary>
        private readonly IUserService service = s;

        /// <summary>
        //      WorkoutService instance
        /// </summary>
        private readonly IWorkoutService workoutService = workoutS;

        /// <summary>
        //      POST request to login the user
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if username and password are provided
            if (!requestData.TryGetValue("email", out string? email) || !requestData.TryGetValue("password", out string? password))
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, Constants.MSG_REG_FAIL, []);
            }

            UserModel? user = await service.Login(email, password);

            // Success check
            if (user == null)
            {
                return ReturnResponse(false, Constants.ResponseCode.FAIL, Constants.MSG_LOGIN_FAILED, []);
            }

            // Construct the return data list
            var returnData = new List<string> { user.ToJson() };

            var currentWorkout = workoutService.GetTodayWorkout(user.Id);
            if (currentWorkout != null) {
                returnData.Add(currentWorkout.ToJson());
            }

            return ReturnResponse(false, Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, returnData);
        }


        /// <summary>
        //      POST request to register the user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if username and password are provided
            if (!requestData.TryGetValue("email", out string? email) || !requestData.TryGetValue("password", out string? password))
            {
                return ReturnResponse(true, Constants.ResponseCode.FAIL, Constants.MSG_REG_FAIL, []);
            }

            string response = await service.Register(email, password);

            // Success check
            if (response != Constants.MSG_SUCCESS) {
                return ReturnResponse(false, Constants.ResponseCode.FAIL, response, []);
            }

            return ReturnResponse(false, Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, []);
        }
    }
}
