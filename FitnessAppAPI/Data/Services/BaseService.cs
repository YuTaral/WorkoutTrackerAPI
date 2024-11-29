using Microsoft.EntityFrameworkCore;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services
{
    /// <summary>
    ///     The BaseService class which contains the common logic for all services
    /// </summary>
    public class BaseService(FitnessAppAPIContext DB)
    {
        protected readonly FitnessAppAPIContext DBAccess = DB;

        /// <summary>
        ///     Method used to exercute all services actions, adding try catch to cath the database or unexpected errors
        ///     and store them in the database
        /// </summary>
        protected ServiceActionResult ExecuteServiceAction(Func<string, ServiceActionResult> serviceAction, string userId)
        {
            try
            {
                return serviceAction(userId);
            }
            catch (DbUpdateException dbEx)
            {
               
                // Add system log for the db error
                var systemLog = new SystemLog
                {
                    ActionName = serviceAction.Method.Name,
                    ExceptionType = Constants.DBConstants.ExceptionTypeDB,
                    ExceptionDescription = dbEx.Message,
                    Date = DateTime.UtcNow,
                    UserId = userId
                };

                DBAccess.SystemLogs.Add(systemLog);
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_DB_ERROR);
            }
            catch (Exception ex)
            {
                // Add system log for the db error
                var systemLog = new SystemLog
                {
                    ActionName = serviceAction.Method.Name,
                    ExceptionType = Constants.DBConstants.ExceptionTypeUnexpected,
                    ExceptionDescription = ex.Message,
                    Date = DateTime.UtcNow,
                    UserId = userId
                };

                DBAccess.SystemLogs.Add(systemLog);
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_ERROR);
            }
        }
    }
}
