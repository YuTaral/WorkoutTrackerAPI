using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using System;

namespace FitnessAppAPI.Data.Services.SystemLogs
{
    /// <summary>
    ///     SystemLogService class to implement ISystemLogService interface.
    /// </summary>
    public class SystemLogService(FitnessAppAPIContext DB) : BaseService(DB), ISystemLogService
    {
        public void Add(Exception exception, string userId)
        {
            string stackTrace = "";

            if (exception.StackTrace != null) { 
                stackTrace = exception.StackTrace;
            }

            var systemLog = new SystemLog
            {
                ExceptionDescription = exception.Message,
                ExceptionStackTrace = stackTrace,
                Date = DateTime.UtcNow,
                UserId = userId
            };

            DBAccess.SystemLogs.Add(systemLog);
            DBAccess.SaveChanges();
        }
    }
}
