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
                UserId = userId,
                MGExeciseId = 0,
            };

            DBAccess.UserDefaultValues.Add(defaultValues);
            DBAccess.SaveChanges();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS);
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
            var oldWeightUnit = 0L;
            var existing = GetUserDefaultValues(data.MGExerciseId, userId);
            
            if (existing == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_UPDATE_DEFAULT_VALUES);
            }

            if (existing.MGExeciseId == 0 && data.MGExerciseId > 0)
            {
                // If the existing returned with MGExeciseId = 0 and data.MGExerciseId > 0
                // this means we are trying to create default values for specific exercise
                // (GetUserDefaultValues return 0, because the record does not exist yet
                // and the default user values was returned)
                var addResult = AddExerciseDefaultValues(data, userId);

                if (!addResult.IsSuccess())
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_UPDATE_DEFAULT_VALUES);

                }

                // Return updated response
                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED);
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

            // Store the old weight unit
            oldWeightUnit = existing.WeightUnitId;

            // Change the record
            existing.Sets = data.Sets;
            existing.Reps = data.Reps;
            existing.Weight = data.Weight;
            existing.Completed = data.Completed;
            existing.WeightUnitId = unitId;

            DBAccess.Entry(existing).State = EntityState.Modified;

            // If the weight unit has changed, change all records for the user to use the new weight unit
            if (oldWeightUnit != unitId)
            {
                var records = DBAccess.UserDefaultValues.Where(u => u.UserId == userId && u.MGExeciseId > 0).ToList();

                if (records != null && records.Count > 0)
                {
                    foreach (UserDefaultValue r in records)
                    {
                        r.WeightUnitId = unitId;
                        DBAccess.Entry(r).State = EntityState.Modified;
                    }
                }
            }

            DBAccess.SaveChanges();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED,
                                            [ModelMapper.MapToUserDefaultValuesModel(existing, DBAccess)]);

        }

        

        /// <summary>
        ///     Return the user default values for this specific exercise. If there are 
        ///     no default values for the exercise, return the user default values
        /// </summary>
        /// <param name="mgExerciseId">
        ///     The muscle group exercise id
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult GetExerciseOrUserDefaultValues(long mgExerciseId, string userId)
        {
            // Search for the exercise specific values, if they don't exist,
            // user default values will be returned
            var values = GetUserDefaultValues(mgExerciseId, userId);

            if (values == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_FETCH_DEFAULT_VALUES);
            }

            // Return the data, making sure tghe values we are returning are returned with the correct mgExerciseId
            // Even if we return the default values which has mgExerciseId = 0, on the client side, we need to know
            // that we are editing exercise specific values, altough they may be initially fetched with mgExerciseId 0 
            // because there are no exercise specific values yet
            values.MGExeciseId = mgExerciseId;

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS, 
                                            [ModelMapper.MapToUserDefaultValuesModel(values, DBAccess)]);
        }

        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the exercise
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        private ServiceActionResult AddExerciseDefaultValues(UserDefaultValuesModel data, string userId)
        {
            var values = new UserDefaultValue
            {
                Sets = data.Sets,
                Reps = data.Reps,
                Weight = data.Weight,
                WeightUnitId = data.WeightUnit.Id,
                Completed = data.Completed,
                UserId = userId,
                MGExeciseId = data.MGExerciseId
            };

            DBAccess.UserDefaultValues.Add(values);
            DBAccess.SaveChanges();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS);
        }
    }
}
