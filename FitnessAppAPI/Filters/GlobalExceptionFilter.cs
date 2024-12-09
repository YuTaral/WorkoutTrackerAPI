using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
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

        _ = LogExceptionAsync(context.Exception, userId);

        // Show unexpected error message to the user
        context.Result = new JsonResult(Utils.CreateResponseObject(Constants.ResponseCode.UNEXPECTED_ERROR, Constants.MSG_UNEXPECTED_ERROR, []))
        {
            StatusCode = StatusCodes.Status200OK,
        };
    }

    /// <summary>
    ///     Fire and forget technique to store the exception, as OnException cannot be async
    /// </summary>
    /// <param name="exception">
    ///     The exception
    /// </param>
    /// <param name="userId">
    ///     The logged in user id, may be empty
    /// </param>
    /// <returns></returns>
    private async Task LogExceptionAsync(Exception exception, string userId)
    {
        await service.Add(exception, userId);  // Log exception asynchronously
    }
}
