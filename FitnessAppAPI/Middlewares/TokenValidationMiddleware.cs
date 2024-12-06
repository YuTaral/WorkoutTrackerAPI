using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;
using Microsoft.AspNetCore.Http;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Middlewares
{
    /// <summary>
    ///      Middleware to execute before each request to validate the token
    /// </summary>
    public class TokenValidationMiddleware(RequestDelegate n)
    {
        private readonly RequestDelegate next = n;

        public Task Invoke(HttpContext context) {

            var requestPathValue = context.Request.Path.Value;

            // Check if the request is to the api endpoint
            if (IsAPIPath(requestPathValue))
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            }

            // Check if request is to the login / logout endpoint
            if (SkipValidation(requestPathValue)) 
            {
                return next(context);
            }

            // Validate the token
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var userService = context.RequestServices.GetRequiredService<IUserService>();
            var tokenValidationResult = userService.ValidateToken(token, "");

            if (tokenValidationResult.Result.IsTokenExpired())
            {
                // Return response token expired
                var response = Utils.CreateResponseObject(Constants.ResponseCode.TOKEN_EXPIRED, Constants.MSG_TOKEN_EXPIRED, []);
                context.Response.StatusCode = StatusCodes.Status200OK;

                return context.Response.WriteAsJsonAsync(response);
            }
            else if (tokenValidationResult.Result.IsRefreshToken() && tokenValidationResult.Token != "")
            {

                // Return response token refresh
                var response = Utils.CreateResponseObject(Constants.ResponseCode.REFRESH_TOKEN, Constants.MSG_TOKEN_EXPIRED, [tokenValidationResult.Token]);
                context.Response.StatusCode = StatusCodes.Status200OK;

                return context.Response.WriteAsJsonAsync(response);
            }

            // Execute the request
            return next(context);
        }

        /// <summary>
        ///     Return true if the request path is to the api ("/api" or "/favicon.ico")
        /// </summary>
        /// <param name="path">
        ///     The request path
        /// </param>
        private static bool IsAPIPath(string? path)
        {
            return path != null && (path.Equals(Constants.RequestEndpoints.API,
                                                StringComparison.CurrentCultureIgnoreCase)
                                 || path.Equals(Constants.RequestEndpoints.FAVICON,
                                                StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        ///     Return true if the token validation should be skipped (when request is for login / logout)
        /// </summary>
        /// <param name="path">
        ///     The request path
        /// </param>
        private static bool SkipValidation(string? path)
        {
            return path != null && (path.Equals(Constants.RequestEndpoints.REGISTER,
                                                StringComparison.CurrentCultureIgnoreCase)
                                    || path.Equals(Constants.RequestEndpoints.LOGIN,
                                                StringComparison.CurrentCultureIgnoreCase)
                                    || path.Equals(Constants.RequestEndpoints.LOGOUT,
                                                StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
