using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Services.SystemLogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

/// <summary>
/// Class to implement IExceptionFilter to store the unexpected exceptions in the System Logs table
/// </summary>
public class GlobalExceptionFilter(ISystemLogService systemLogService) : IExceptionFilter
{
    private readonly ISystemLogService service = systemLogService;

    public void OnException(ExceptionContext context)
    {
        // Add the exception to the system logs table
        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        service.Add(context.Exception, userId);

        // Show unexpected error message to the user
        context.Result = new JsonResult(Utils.CreateResponseObject(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []))
        {
            StatusCode = StatusCodes.Status200OK,
        };
    }
}
