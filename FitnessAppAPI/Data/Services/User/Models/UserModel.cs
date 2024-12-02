using FitnessAppAPI.Data.Services.UserProfile.Models;

namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     UserModel class representing the logged in user.
    ///     Must correspond with client-side UserModel class
    /// </summary>
    /// 
    public class UserModel
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required UserDefaultValuesModel DefaultValues { get; set; }
    }
}
