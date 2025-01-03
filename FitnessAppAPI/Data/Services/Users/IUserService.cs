using FitnessAppAPI.Data.Services.User.Models;

namespace FitnessAppAPI.Data.Services
{
    /// <summary>
    ///     User service interface to define the logic for Login / Register.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///     Register the user
        /// </summary>
        /// <param name="email">
        ///     The user email
        /// </param>
        /// <param name="password">
        ///     The user password
        /// </param>
        public Task<ServiceActionResult> Register(string email, string password);

        /// <summary>
        ///     Login the user
        /// </summary>
        /// <param name="email">
        ///     The user email
        /// </param>
        /// <param name="password">
        ///     The user password
        /// </param>
        public Task<TokenResponseModel> Login(string email, string password);

        /// <summary>
        ///     Log Out the User
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult> Logout();

        /// <summary>
        ///     Change user passowrd
        /// </summary>
        /// <param name="oldPassword">
        ///     The old password
        /// </param>
        /// <param name="s">
        ///     The new password
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult> ChangePassword(string oldPassword, string password, string userId);

        /// <summary>
        ///     Validate the token
        /// </summary>
        /// <param name="token">
        ///     The token to validate
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<TokenResponseModel> ValidateToken(string token, string userId);

        /// <summary>
        ///     Create UserModel object
        /// </summary>
        /// <param name="email">
        ///     The user email
        /// </param> 
        public Task<UserModel> GetUserModel(string email);
    }
}
