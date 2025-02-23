using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.SystemLogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

/// <summary>
/// Class to implement IExceptionFilter to store the unexpected exceptions in the System Logs table
/// </summary>
public class GlobalExceptionFilter(IServiceProvider s) : IExceptionFilter
{
    private readonly IServiceProvider serviceProvider = s;

    public void OnException(ExceptionContext context)
    {
        var userId = "";

        try
        {
            userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        } 
        catch (Exception)
        {
            // log the error without user id if error occurs while retrieving user id
            userId = "";
        }

        // User fire and forget technique, getting the service form the service provider. If we inject
        // the systemLogService through the constructor, in some cases it is already disposed when 
        // OnException is reached and adding the record to system logs is impossible
        _ = Task.Run(async () =>
        {
            var systemLogService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISystemLogService>();

            try
            {
                await systemLogService.Add(context.Exception, userId);
            }
            catch (Exception){}
        });

        // Show internal server error message to the user
        context.Result = new JsonResult(Utils.CreateResponseObject((int) HttpStatusCode.InternalServerError, Constants.MSG_UNEXPECTED_ERROR, []))
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}
