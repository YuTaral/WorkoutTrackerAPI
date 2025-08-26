using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.UserProfiles.Models;
using FitnessAppAPI.Data.Services.Users.Models;

namespace FitnessAppAPI.Data.Services.UserProfiles
{
    /// <summary>
    ///     User profile service interface to define the logic for managing user profile.
    /// </summary>
    public interface IUserProfileService
    {
        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> AddUserDefaultValues(string userId);

        /// <summary>
        ///     Change user default values
        /// </summary>
        /// <param name="requestData">
        ///     The request data (new default values)
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<UserDefaultValuesModel>> UpdateUserDefaultValues(Dictionary<string, string> requestData, string userId);

        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        /// <param name="email">
        ///     The user email
        /// </param
        public Task<ServiceActionResult<BaseModel>> CreateUserProfile(string userId, string email);

        /// <summary>
        ///     Update user profile
        /// </summary>
        /// <param name="requestData">
        ///     The reqeust data (user model)
        /// </param>
        public Task<ServiceActionResult<UserModel>> UpdateUserProfile(Dictionary<string, string> requestData);

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
        public Task<ServiceActionResult<UserDefaultValuesModel>> GetExerciseOrUserDefaultValues(long mgExerciseId, string userId);
    }
}
