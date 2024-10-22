using FitnessAppAPI.Data.Services.User.Models;

namespace FitnessAppAPI.Data.Services
{
    /// <summary>
    ///     User service interface define the logic for Login / Register.
    /// </summary>
    public interface IUserService
    {
        public Task<String> Register(string email, string password);
        public Task<LoginResponseModel?> Login(string email, string password);
        public Task<Boolean> Logout();
    }
}
