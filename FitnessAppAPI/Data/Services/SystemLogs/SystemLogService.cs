using FitnessAppAPI.Data.Models;
using System.Net;

namespace FitnessAppAPI.Data.Services.SystemLogs
{
    /// <summary>
    ///     SystemLogService class to implement ISystemLogService interface.
    /// </summary>
    public class SystemLogService(FitnessAppAPIContext DB) : BaseService(DB), ISystemLogService
    {
        public async Task<bool> Add(Exception exception, string userId)
        {
            string stackTrace = "";

            if (exception.StackTrace != null) { 
                stackTrace = exception.StackTrace;
            }

            await AddError(exception.Message, stackTrace, userId);

            return true;
        }

        public async Task<ServiceActionResult<string>> Add(Dictionary<string, string> requestData, string userId)
        {

            if (!requestData.TryGetValue("message", out string? message) || !requestData.TryGetValue("stackTrace", out string? stackTrace))
            {
                // Do not show error, just return with OK status
                return new ServiceActionResult<string>(HttpStatusCode.OK);
            }

            await AddError(message, stackTrace, userId);

            return new ServiceActionResult<string>(HttpStatusCode.OK);
        }


        /// <summary>
        ///     Store the error in the DB
        /// </summary>
        /// <param name="message">
        ///     The error message
        /// </param>
        /// <param name="stackTrace">
        ///     The error stack trace
        /// </param>
        /// <param name="userId">
        ///     The user id, may be empty if not logged in
        /// </param>
        private async Task<bool> AddError(string message, string stackTrace, string userId)
        {
            // Sometimes the stack trace is too long to be stored in the DB, truncate if needed
            if (stackTrace.Length > 4000)
            {
                stackTrace = stackTrace[..4000];
            }

            var systemLog = new SystemLog
            {
                ExceptionDescription = message,
                ExceptionStackTrace = stackTrace,
                Date = DateTime.UtcNow,
                UserId = userId
            };

            await DBAccess.SystemLogs.AddAsync(systemLog);
            await DBAccess.SaveChangesAsync();

            return true;
        }
    }
}
