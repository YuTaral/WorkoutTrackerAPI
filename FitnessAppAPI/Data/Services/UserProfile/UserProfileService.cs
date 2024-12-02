using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.UserProfile.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data.Services.UserProfile
{
    public class UserProfileService(FitnessAppAPIContext DB): BaseService(DB), IUserProfileService
    {
        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult AddUserDefaultValues(string userId)
        {
            var kg = DBAccess.WeightUnits.Where(w => w.Code == "KG").FirstOrDefault();
            if (kg == null)
            {
                // Must NOT happen
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_UNEXPECTED_DB_ERROR);
            }

            // Create ExerciseDefaultValue record for the user
            var defaultValues = new UserDefaultValue
            {
                Sets = 0,
                Reps = 0,
                Weight = 0,
                WeightUnitCode = kg.Code,
                Completed = false,
                UserId = userId
            };

            DBAccess.UserDefaultValues.Add(defaultValues);
            DBAccess.SaveChanges();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS);
        }

        /// <summary>
        ///     Change user default values
        /// </summary>
        /// <param name="data">
        ///     The new default values
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult UpdateUserDefaultValues(UserDefaultValuesModel data, string userId)
        {
            return ExecuteServiceAction(userId =>
            {
                var existing = GetUserDefaultValues(userId);
                if (existing == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_DOES_NOT_EXISTS);
                }

                // Find the unit record and set the code, the model contains the Text column
                var unitRecord = DBAccess.WeightUnits.Where(w => w.Text == data.WeightUnitText).FirstOrDefault();
                var unitCode = "";

                if (unitRecord == null)
                {
                    unitCode = existing.WeightUnitCode;
                }
                else
                {
                    unitCode = unitRecord.Code;
                }

                // Change the record
                existing.Sets = data.Sets;
                existing.Reps = data.Reps;
                existing.Weight = data.Weight;
                existing.Completed = data.Completed;
                existing.WeightUnitCode = unitCode;

                DBAccess.Entry(existing).State = EntityState.Modified;
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED,
                    [ModelMapper.MapToUserDefaultValuesModel(existing, DBAccess)]);

            }, userId);
        }
    }
}
