using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Data.Services.UserProfile.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data.Services.UserProfile
{
    public class UserProfileService(FitnessAppAPIContext DB): BaseService(DB), IUserProfileService
    {
        public async Task<ServiceActionResult> AddUserDefaultValues(string userId)
        {
            var kg = await DBAccess.WeightUnits.Where(w => w.Text == Constants.DBConstants.KG).FirstOrDefaultAsync();
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

        public async Task<ServiceActionResult> UpdateUserDefaultValues(UserDefaultValuesModel data, string userId)
        {
            var oldWeightUnit = 0L;
            var existing = await GetUserDefaultValues(data.MGExerciseId, userId);
            
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
                var addResult = await AddExerciseDefaultValues(data, userId);

                if (!addResult.IsSuccess())
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_UPDATE_DEFAULT_VALUES);

                }

                // Return updated response
                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED);
            }

            // Find the unit record and set the code, the model contains the Text column
            var unitRecord = await DBAccess.WeightUnits.Where(w => w.Id == data.WeightUnit.Id).FirstOrDefaultAsync();
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
                var records = await DBAccess.UserDefaultValues.Where(u => u.UserId == userId && u.MGExeciseId > 0).ToListAsync();

                if (records != null && records.Count > 0)
                {
                    foreach (UserDefaultValue r in records)
                    {
                        r.WeightUnitId = unitId;
                        DBAccess.Entry(r).State = EntityState.Modified;
                    }
                }
            }

            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED,
                                            [await ModelMapper.MapToUserDefaultValuesModel(existing, DBAccess)]);

        }

        public async Task<ServiceActionResult> CreateUserProfile(string userId)
        {
            var profile = new Data.Models.UserProfile
            {
                FullName = "",
                ProfileImage = [],
                UserId = userId
            };

            await DBAccess.UserProfiles.AddAsync(profile);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS);
        }

        public async Task<ServiceActionResult> UpdateUserProfile(UserModel data)
        {
            var profile = await DBAccess.UserProfiles.Where(p => p.UserId == data.Id).FirstOrDefaultAsync();

            if (profile == null)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_FAILED_TO_UPDATE_USER_PROFILE);
            }

            profile.FullName = data.FullName;
            profile.ProfileImage = Utils.DecodeBase64ToByteArray(data.ProfileImage);

            DBAccess.Entry(profile).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_USER_PROFILE_UPDATED);
        }

        public async Task<ServiceActionResult> GetExerciseOrUserDefaultValues(long mgExerciseId, string userId)
        {
            // Search for the exercise specific values, if they don't exist,
            // user default values will be returned
            var values = await GetUserDefaultValues(mgExerciseId, userId);

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
                                            [await ModelMapper.MapToUserDefaultValuesModel(values, DBAccess)]);
        }

        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the exercise
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        private async Task<ServiceActionResult> AddExerciseDefaultValues(UserDefaultValuesModel data, string userId)
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

            await DBAccess.UserDefaultValues.AddAsync(values);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS);
        }
    }
}
