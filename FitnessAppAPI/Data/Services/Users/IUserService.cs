using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Users.Models;

namespace FitnessAppAPI.Data.Services.Users
{
    /// <summary>
    ///     User service interface to define the logic for Login / Register.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///     Register the user
        /// </summary>
        /// <param name="requestData">
        ///     The request data (user email and password)
        /// </param>
        public Task<ServiceActionResult<BaseModel>> Register(Dictionary<string, string> requestData);

        /// <summary>
        ///     Login the user
        /// </summary>
        /// <param name="requestData">
        ///     The request data (user email and password)
        /// </param>
        public Task<TokenResponseModel> Login(Dictionary<string, string> requestData);

        /// <summary>
        ///     Sign in the user with google id token
        /// </summary>
        /// <param name="requestData">
        ///     The request data (google sign in token)
        /// </param>
        public Task<TokenResponseModel> GoogleSignIn(Dictionary<string, string> requestData);

        /// <summary>
        ///     Log Out the User
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> Logout();

        /// <summary>
        ///     Change user passowrd
        /// </summary>
        /// <param name="requestData">
        ///     The request data (old password and new password)
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<BaseModel>> ChangePassword(Dictionary<string, string> requestData, string userId);

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
