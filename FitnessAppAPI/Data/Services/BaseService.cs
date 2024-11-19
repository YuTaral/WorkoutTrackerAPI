using Microsoft.EntityFrameworkCore;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;

namespace FitnessAppAPI.Data.Services
{
    /// <summary>
    ///     The BaseService class which contains the common logic for all services
    /// </summary>
    public class BaseService
    {
        protected ServiceActionResult ExecuteServiceAction(Func<ServiceActionResult> serviceAction)
        {
            try
            {
                return serviceAction();
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific errors
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_DB_ERROR);
            }
            catch (Exception ex)
            {
                // Handle general errors
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_ERROR);
            }
        }

        /// <summary>
        ///     Checks whether a user with the provided id exists and returns true / false
        /// </summary>
        protected bool UserExists(FitnessAppAPIContext DBAccess, string userId)
        {
            return DBAccess.Users.Find(userId) != null;
        }

        /// <summary>
        ///    Returns List<BaseModel> adding the BaseModel
        /// </summary>
        /// <param name="model">
        ///     The model to add
        /// </param>
        protected List<BaseModel> CreateReturnData(BaseModel model)
        {
            var returnData = new List<BaseModel>();
            returnData.Add(model);
            return returnData;
        }
    }
}
