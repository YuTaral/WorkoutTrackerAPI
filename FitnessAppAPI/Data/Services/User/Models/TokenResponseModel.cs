namespace FitnessAppAPI.Data.Services.User.Models
{
    /// <summary>
    ///     TokenResponseModel user to return UserModel and JWT Token
    ///     after successfull login / token validatoin
    /// </summary>
    public class TokenResponseModel
    {
        public required UserModel User { get; set; }
        public required string Token { get; set; }
        public required ServiceActionResult Result { get; set; }
    }
}
