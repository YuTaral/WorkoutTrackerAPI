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
        public Task<ServiceActionResult> AddUserDefaultValues(string userId);
        public Task<ServiceActionResult> UpdateUserDefaultValues(UserDefaultValuesModel data, string userId);
        public Task<ServiceActionResult> UpdateUserProfile(UserModel user);
        public Task<ServiceActionResult> GetExerciseOrUserDefaultValues(long mgExerciseId, string userId);
        public Task<ServiceActionResult> CreateUserProfile(string userId);
    }
}
