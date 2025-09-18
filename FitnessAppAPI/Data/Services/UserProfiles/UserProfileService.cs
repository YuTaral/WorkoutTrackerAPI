using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.UserProfiles.Models;
using FitnessAppAPI.Data.Services.Users.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Data.Services.UserProfiles
{
    public class UserProfileService(FitnessAppAPIContext DB): BaseService(DB), IUserProfileService
    {
        public async Task<ServiceActionResult<BaseModel>> AddUserDefaultValues(string userId)
        {
            // Create ExerciseDefaultValue record for the user
            var defaultValues = new UserDefaultValue
            {
                Sets = 0,
                Reps = 0,
                Weight = 0,
                Rest = 0,
                WeightUnit = DBConstants.KG,
                Completed = false,
                UserId = userId,
                MGExeciseId = 0,
            };

            DBAccess.UserDefaultValues.Add(defaultValues);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.Created);
        }

        public async Task<ServiceActionResult<UserDefaultValuesModel>> UpdateUserDefaultValues(Dictionary<string, string> requestData, string userId)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("values", out string? serializedValues))
            {
                return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.BadRequest, MSG_CHANGE_USER_DEF_VALUES);
            }

            UserDefaultValuesModel? data = JsonConvert.DeserializeObject<UserDefaultValuesModel>(serializedValues);
            if (data == null)
            {
                return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "UserDefaultValuesModel"));
            }

            string validationErrors = Utils.ValidateModel(data);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            var existing = await GetUserDefaultValues(data.MGExerciseId, userId);
            if (existing == null)
            {
                return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_UPDATE_DEFAULT_VALUES);
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
                    return new ServiceActionResult<UserDefaultValuesModel>((HttpStatusCode) addResult.Code, MSG_FAILED_TO_UPDATE_DEFAULT_VALUES);

                }

                return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.Created);
            }

            // Change the record
            existing.Sets = data.Sets;
            existing.Reps = data.Reps;
            existing.Weight = data.Weight;
            existing.Rest = data.Rest;
            existing.Completed = data.Completed;
            existing.WeightUnit = data.WeightUnit;

            DBAccess.Entry(existing).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.OK, MSG_SUCCESS, [ModelMapper.MapToUserDefaultValuesModel(existing, DBAccess)]);

        }

        public async Task<ServiceActionResult<BaseModel>> CreateUserProfile(string userId, string email, string? name, string? imageURL)
        {
            // Set the full name to the part before @ in email if name is not provided
            string fullname;
            byte[] profileImage = [];

            if (string.IsNullOrEmpty(name)) {
                fullname = email[..email.IndexOf("@")];
            } else {
                fullname = name; 
            }

            if (fullname.Length > 100) {
                fullname = fullname[..100];
            }

            if (!string.IsNullOrEmpty(imageURL))
            {
                profileImage = await Utils.DownloadImageAsBytesAsync(imageURL);
            }

            var profile = new UserProfile
            {
                FullName = fullname,
                ProfileImage = profileImage,
                UserId = userId
            };

            await DBAccess.UserProfiles.AddAsync(profile);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.Created);
        }

        public async Task<ServiceActionResult<UserModel>> UpdateUserProfile(Dictionary<string, string> requestData)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("user", out string? serializedUser))
            {
                return new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, MSG_CHANGE_USER_DEF_VALUES);
            }

            UserModel? data = JsonConvert.DeserializeObject<UserModel>(serializedUser);
            if (data == null)
            {
                return new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, string.Format(MSG_WORKOUT_FAILED_TO_DESERIALIZE_OBJ, "UserModel"));
            }

            var validationErrors = Utils.ValidateModel(data);
            if (!string.IsNullOrEmpty(validationErrors))
            {
                return new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, validationErrors);
            }

            var profile = await DBAccess.UserProfiles.Where(p => p.UserId == data.Id).FirstOrDefaultAsync();
            if (profile == null)
            {
                return new ServiceActionResult<UserModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_UPDATE_USER_PROFILE);
            }

            profile.FullName = data.FullName;
            profile.ProfileImage = Utils.DecodeBase64ToByteArray(data.ProfileImage);

            DBAccess.Entry(profile).State = EntityState.Modified;
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<UserModel>(HttpStatusCode.OK, MSG_SUCCESS, [data]);
        }

        public async Task<ServiceActionResult<UserDefaultValuesModel>> GetExerciseOrUserDefaultValues(long mgExerciseId, string userId)
        {
            // Search for the exercise specific values, if they don't exist,
            // user default values will be returned
            var values = await GetUserDefaultValues(mgExerciseId, userId);

            if (values == null)
            {
                return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.NotFound, MSG_FAILED_TO_FETCH_DEFAULT_VALUES);
            }

            // Return the data, making sure tghe values we are returning are returned with the correct mgExerciseId
            // Even if we return the default values which has mgExerciseId = 0, on the client side, we need to know
            // that we are editing exercise specific values, altough they may be initially fetched with mgExerciseId 0 
            // because there are no exercise specific values yet
            values.MGExeciseId = mgExerciseId;

            return new ServiceActionResult<UserDefaultValuesModel>(HttpStatusCode.OK, MSG_SUCCESS, [ModelMapper.MapToUserDefaultValuesModel(values, DBAccess)]);
        }

        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the exercise
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        private async Task<ServiceActionResult<BaseModel>> AddExerciseDefaultValues(UserDefaultValuesModel data, string userId)
        {
            var values = new UserDefaultValue
            {
                Sets = data.Sets,
                Reps = data.Reps,
                Weight = data.Weight,
                Rest = data.Rest,
                WeightUnit = data.WeightUnit,
                Completed = data.Completed,
                UserId = userId,
                MGExeciseId = data.MGExerciseId
            };

            await DBAccess.UserDefaultValues.AddAsync(values);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }
    }
}
