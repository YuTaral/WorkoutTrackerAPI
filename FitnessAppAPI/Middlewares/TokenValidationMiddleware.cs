using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services;

namespace FitnessAppAPI.Middlewares
{
    /// <summary>
    ///      Middleware to execute before each request to validate the token
    /// </summary>
    public class TokenValidationMiddleware(RequestDelegate n)
    {
        private readonly RequestDelegate next = n;

        public async Task Invoke(HttpContext context) {

            var requestPathValue = context.Request.Path.Value;

            // Check if the request is to the api endpoint
            if (IsAPIPath(requestPathValue))
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            if (!SkipValidation(requestPathValue)) 
            {
                // Validate the token
                var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var userService = context.RequestServices.GetRequiredService<IUserService>();
                var tokenValidationResult = await userService.ValidateToken(token, "");

                if (tokenValidationResult.Result.IsTokenExpired())
                {
                    // Return response token expired
                    var response = Utils.CreateResponseObject(Constants.ResponseCode.TOKEN_EXPIRED, Constants.MSG_TOKEN_EXPIRED, []);
                    context.Response.StatusCode = StatusCodes.Status200OK;

                    await context.Response.WriteAsJsonAsync(response);
                    return;
                }
                else if (tokenValidationResult.Result.IsRefreshToken() && tokenValidationResult.Token != "")
                {

                    // Return response token refresh
                    var response = Utils.CreateResponseObject(Constants.ResponseCode.REFRESH_TOKEN, Constants.MSG_TOKEN_EXPIRED, [tokenValidationResult.Token]);
                    context.Response.StatusCode = StatusCodes.Status200OK;

                    await context.Response.WriteAsJsonAsync(response);
                    return;
                }
            }

            // Execute the request
            await next(context);
        }

        /// <summary>
        ///     Return true if the request path is to the api ("/api" or "/favicon.ico")
        /// </summary>
        /// <param name="path">
        ///     The request path
        /// </param>
        private static bool IsAPIPath(string? path)
        {
            return path != null && (path.Equals(Constants.RequestEndPoints.API,
                                                StringComparison.CurrentCultureIgnoreCase)
                                 || path.Equals(Constants.RequestEndPoints.FAVICON,
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
            return path != null && (path.Equals(Constants.RequestEndPoints.REGISTER,
                                                StringComparison.CurrentCultureIgnoreCase)
                                    || path.Equals(Constants.RequestEndPoints.LOGIN,
                                                StringComparison.CurrentCultureIgnoreCase)
                                    || path.Equals(Constants.RequestEndPoints.LOGOUT,
                                                StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
