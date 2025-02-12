using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Data.Services.UserProfile.Models;

namespace FitnessAppAPI.Data.Services.UserProfile
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
        public Task<ServiceActionResult> AddUserDefaultValues(string userId);

        /// <summary>
        ///     Change user default values
        /// </summary>
        /// <param name="data">
        ///     The new default values
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult> UpdateUserDefaultValues(UserDefaultValuesModel data, string userId);

        /// <summary>
        ///     Create record in ExerciseDefaultValue with the default values for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        /// <param name="email">
        ///     The user email
        /// </param
        public Task<ServiceActionResult> CreateUserProfile(string userId, string email);

        /// <summary>
        ///     Update user profile
        /// </summary>
        /// <param name="data">
        ///     The user model
        /// </param>
        public Task<ServiceActionResult> UpdateUserProfile(UserModel user);

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
        public Task<ServiceActionResult> GetExerciseOrUserDefaultValues(long mgExerciseId, string userId);
    }
}
