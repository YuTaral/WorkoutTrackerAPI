using FitnessAppAPI.Data.Services.User.Models;

namespace FitnessAppAPI.Data.Services
{
    /// <summary>
    ///     User service interface to define the logic for Login / Register.
    /// </summary>
    public interface IUserService
    {
        public Task<ServiceActionResult> Register(string email, string password);
        public Task<TokenResponseModel> Login(string email, string password);
        public Task<ServiceActionResult> Logout();
        public Task<ServiceActionResult> ChangePassword(string oldPassword, string password, string userId);
        public Task<TokenResponseModel> ValidateToken(string token, string userId);
        public Task<UserModel> GetUserModel(string email);
    }
}
