﻿using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Controllers
{
    /// <summary>
    ///     User Controller
    /// </summary>
    [ApiController]
    [Route(RequestEndPoints.USERS)]
    public class UserController(IUserService s) : BaseController
    {
        /// <summary>
        //      UserService instance
        /// </summary>
        private readonly IUserService service = s;

        /// <summary>
        //      POST request to login the user
        /// </summary>
        [HttpPost(RequestEndPoints.LOGIN)]
        public async Task<ActionResult> Login([FromBody] Dictionary<string, string> requestData) { 
            TokenResponseModel model = await service.Login(requestData);

            // Success check
            if (!model.Result.IsSuccess())
            {
                return SendResponse((HttpStatusCode) model.Result.Code, model.Result.Message);
            }

            return SendResponse((HttpStatusCode) model.Result.Code, model.Result.Message, model);
        }

        /// <summary>
        //      POST request to register the user
        /// </summary>
        [HttpPost(RequestEndPoints.REGISTER)]
        public async Task<ActionResult> Register([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.Register(requestData));
        }


        /// <summary>
        //      POST request to sign in the user with google id token
        /// </summary>
        [HttpPost(RequestEndPoints.GOOGLE_SIGN_IN)]
        public async Task<ActionResult> GoogleSignIn([FromBody] Dictionary<string, string> requestData)
        {
            TokenResponseModel model = await service.GoogleSignIn(requestData);

            // Success check
            if (!model.Result.IsSuccess())
            {
                return SendResponse((HttpStatusCode)model.Result.Code, model.Result.Message);
            }

            return SendResponse((HttpStatusCode)model.Result.Code, model.Result.Message, model);
        }

        /// <summary>
        //      POST request to logout the user
        /// </summary>
        [HttpPost(RequestEndPoints.LOGOUT)]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await service.Logout();

            // Double check the user is logged out successfully
            var loggedOut = GetUserId() != "";

            if (loggedOut) { 
                return SendResponse(HttpStatusCode.OK);
            }

            return SendResponse(HttpStatusCode.InternalServerError, Constants.MSG_UNEXPECTED_ERROR);
        }

        /// <summary>
        //      POST request to change password
        /// </summary>
        [HttpPut(RequestEndPoints.CHANGE_PASSWORD)]
        public async Task<ActionResult> ChangePassword([FromBody] Dictionary<string, string> requestData)
        {
            return SendResponse(await service.ChangePassword(requestData, GetUserId()));
        }

        /// <summary>
        //      POST request to validate token
        /// </summary>
        [HttpPost(RequestEndPoints.VALIDATE_TOKEN)]
        public async Task<ActionResult> ValidateToken([FromBody] Dictionary<string, string> requestData)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("token", out string? token))
            {
                return SendResponse(HttpStatusCode.BadRequest, MSG_TOKEN_VALIDATION_FAILED);
            }

            var tokenResponseModel = await service.ValidateToken(token, GetUserId());

            if (tokenResponseModel.Result.IsSuccess())
            {
                // Token validation is successfull
                return SendResponse(HttpStatusCode.OK);

            } 
            else if (tokenResponseModel.Result.Code == (int) HttpStatusCode.Unauthorized && tokenResponseModel.Token != "")
            {
                // Need to refresh the token on the client side, return the token
                return SendResponse(HttpStatusCode.Unauthorized, MSG_SUCCESS, [tokenResponseModel.Token]);
            }

            // Token validation failed, probably token expired
            return SendResponse((HttpStatusCode) tokenResponseModel.Result.Code, tokenResponseModel.Result.Message);
        }
    }
}
