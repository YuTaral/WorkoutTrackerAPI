namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     TokenResponseModel user to return UserModel and JWT Token
    ///     after successfull login / token validation
    /// </summary>
    public class TokenResponseModel(string t, ServiceActionResult<UserModel> r)
    {
        public string Token { get; set; } = t;

        public ServiceActionResult<UserModel> Result { get; set; } = r;
    }
}
