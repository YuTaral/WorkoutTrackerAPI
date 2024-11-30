using FitnessAppAPI.Data.Services.User.Models;

namespace FitnessAppAPI.Data.Services
{
    /// <summary>
    ///     User service interface to define the logic for Login / Register.
    /// </summary>
    public interface IUserService
    {
        public ServiceActionResult Register(string email, string password);
        public LoginResponseModel Login(string email, string password);
        public ServiceActionResult Logout(string userId);
        public ServiceActionResult ChangePassword(string oldPassword, string password, string userId);
        public ServiceActionResult ChangeUserDefaultValues(UserDefaultValuesModel data, string userId);
    }
}
