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
            var kg = DBAccess.WeightUnits.Where(w => w.Text == Constants.DBConstants.KG).FirstOrDefault();
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
                WeightUnitId = kg.Id,
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
                var unitRecord = DBAccess.WeightUnits.Where(w => w.Id == data.WeightUnit.Id).FirstOrDefault();
                var unitId = 0L;

                if (unitRecord == null)
                {
                    unitId = existing.WeightUnitId;
                }
                else
                {
                    unitId = unitRecord.Id;
                }

                // Change the record
                existing.Sets = data.Sets;
                existing.Reps = data.Reps;
                existing.Weight = data.Weight;
                existing.Completed = data.Completed;
                existing.WeightUnitId = unitId;

                DBAccess.Entry(existing).State = EntityState.Modified;
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED,
                    [ModelMapper.MapToUserDefaultValuesModel(existing, DBAccess)]);

            }, userId);
        }

        /// <summary>
        ///     Return the weight units
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult GetWeightUnits(string userId)
        {
            return ExecuteServiceAction(userId =>
            {
                var units = DBAccess.WeightUnits.Select(w => (BaseModel) ModelMapper.MapToWeightUnitModel(w)).ToList();

                if (units.Count == 0) {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_FETCH_WEIGHT_UNITS);
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, units);

            }, userId);
        }
    }
}
