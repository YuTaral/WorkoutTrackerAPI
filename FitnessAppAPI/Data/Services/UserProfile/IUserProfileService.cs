using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.UserProfile.Models;

namespace FitnessAppAPI.Data.Services.UserProfile
{
    /// <summary>
    ///     User profile service interface to define the logic for managing user profile.
    /// </summary>
    public interface IUserProfileService
    {
        public ServiceActionResult AddUserDefaultValues(string userId);
        public ServiceActionResult UpdateUserDefaultValues(UserDefaultValuesModel data, string userId);
    }
}
