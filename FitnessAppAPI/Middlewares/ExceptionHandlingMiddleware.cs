using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.SystemLogs;
using System.Net;
using System.Security.Claims;

namespace FitnessAppAPI.Middlewares
{
    /// <summary>
    ///      Middleware to catch all errors and log them
    /// </summary>
    public class ExceptionHandlingMiddleware(RequestDelegate n, IServiceProvider s)
    {
        private readonly RequestDelegate next = n;
        private readonly IServiceProvider serviceProvider = s;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Try to execute the next middleware
                await next(context);

            } 
            catch (Exception ex)
            {
                string userId;

                try
                {
                    userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                }
                catch (Exception)
                {
                    // log the error without user id if error occurs while retrieving user id
                    userId = "";
                }

                // User fire and forget technique, getting the service form the service provider. If we inject
                // the systemLogService through the constructor, in some cases it is already disposed when 
                // OnException is reached and adding the record to system logs is impossible
                _ = Task.Run(() =>
                {
                    var systemLogService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISystemLogService>();

                    try
                    {
                        systemLogService.Add(ex, userId);
                    }
                    catch (Exception) { }
                });


                // Show internal server error message to the user
                var response = Utils.CreateResponseObject((int)HttpStatusCode.InternalServerError, Constants.MSG_UNEXPECTED_ERROR, []);

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(response);
                return;
            }
        }
    }
}
