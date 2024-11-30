using Microsoft.Identity.Client;

namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     LoginResponseModel user to return UserModel and JWT Token
    ///     after successfull login
    /// </summary>
    public class LoginResponseModel
    {
        public required UserModel User { get; set; }
        public required string Token { get; set; }
        public required ServiceActionResult Result { get; set; }

    }
}
