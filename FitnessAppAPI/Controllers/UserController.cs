using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Data.Services.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Controller
    /// </summary>
    [ApiController]
    [Route(Constants.RequestEndPoints.USER)]
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
        [HttpPost(Constants.RequestEndPoints.LOGIN)]
        public ActionResult Login([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if username and password are provided
            if (!requestData.TryGetValue("email", out string? email) || !requestData.TryGetValue("password", out string? password))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_LOGIN_FAIL);
            }

            TokenResponseModel model = service.Login(email, password);

            // Success check
            if (!model.Result.IsSuccess())
            {
                return CustomResponse(model.Result.Code, model.Result.Message);
            }

            // Construct the return data list
            var returnData = new List<string> { model.User.ToJson(), model.Token };

            // Get the last workout
            var result = workoutService.GetLastWorkout(model.User.Id);

            if (result.IsSuccess() && result.Data.Count > 0) 
            {
                // If workout exists for the user, add it to the returnData
                returnData.Add(result.Data[0].ToJson());
            }

            return CustomResponse(result.Code, result.Message, returnData);
        }

        /// <summary>
        //      POST request to register the user
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.REGISTER)]
        public ActionResult Register([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if username and password are provided
            if (!requestData.TryGetValue("email", out string? email) || !requestData.TryGetValue("password", out string? password))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_REG_FAIL);
            }

            // Register the user
            return CustomResponse(service.Register(email, password));
        }

        /// <summary>
        //      POST request to logout the user
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.LOGOUT)]
        [Authorize]
        public ActionResult Logout()
        {
            service.Logout();

            // Double check the user is logged out successfully
            var loggedOut = GetUserId() != "";

            if (loggedOut) { 
                return CustomResponse(Constants.ResponseCode.SUCCESS);
            }

            return CustomResponse(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR);
        }

        /// <summary>
        //      POST request to change password
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.CHANGE_PASSWORD)]
        public ActionResult ChangePassword([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("oldPassword", out string? oldPassword) || !requestData.TryGetValue("password", out string? password))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_CHANGE_PASS_FAIL);
            }

            return CustomResponse(service.ChangePassword(oldPassword, password, GetUserId()));
        }

        /// <summary>
        //      POST request to validate token
        /// </summary>
        [HttpPost(Constants.RequestEndPoints.VALIDATE_TOKEN)]
        public ActionResult ValidateToken([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("token", out string? token))
            {
                return CustomResponse(Constants.ResponseCode.FAIL, Constants.MSG_TOKEN_VALIDATION_FAILED);
            }

            var tokenResponseModel = service.ValidateToken(token, GetUserId());

            if (tokenResponseModel.Result.IsSuccess())
            {
                // Token validation is successfull
                return CustomResponse(Constants.ResponseCode.SUCCESS);

            } 
            else if (tokenResponseModel.Result.IsRefreshToken())
            {
                // Need to refresh the token on the client side, return the token
                return CustomResponse(Constants.ResponseCode.REFRESH_TOKEN, Constants.MSG_SUCCESS, [tokenResponseModel.Token]);
            }

            // Token validation failed, probably token expired
            return CustomResponse(tokenResponseModel.Result.Code, tokenResponseModel.Result.Message);
        }
    }
}
