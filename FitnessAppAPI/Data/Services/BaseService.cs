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
        ///     Return the user default values for exercises
        /// </summary>
        /// <param name="mgExerciseId">
        ///     The muscle group exercise id, or 0 if we need the user default values
        /// </param>
        /// <param name="userId">
        ///     The user Id
        /// </param>
        protected UserDefaultValue? GetUserDefaultValues(long mgExerciseId, string userId)
        {
            if (mgExerciseId == 0) {
                // Return the default values
                return DBAccess.UserDefaultValues.Where(u => u.UserId == userId && u.MGExeciseId == mgExerciseId).FirstOrDefault();
            } 
            else
            {
                // Try to fetch the exercise specific values
                var values = DBAccess.UserDefaultValues.Where(u => u.UserId == userId && u.MGExeciseId == mgExerciseId).FirstOrDefault();

                if (values == null) {
                    // If there are no exercise specific values, return the user default values, which has MGExeciseId = 0
                    return DBAccess.UserDefaultValues.Where(u => u.UserId == userId && u.MGExeciseId == 0).FirstOrDefault();
                }

                return values;
            }
        }
    }
}
