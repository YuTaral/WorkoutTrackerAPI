namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     TokenResponseModel user to return UserModel and JWT Token
    ///     after successfull login / token validation
    /// </summary>
    public class TokenResponseModel(UserModel u, string t, ServiceActionResult r)
    {
        public UserModel User { get; set; } = u;
        public string Token { get; set; } = t;
        public ServiceActionResult Result { get; set; } = r;
    }
}
