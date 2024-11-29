using Microsoft.AspNetCore.Identity;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessAppAPI.Data
{
    /// <summary>
    ///     User service class to implement IUserService interface.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public UserService(UserManager<User> userManager, IUserStore<User> userStore, SignInManager<User> signInManager,
                            IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        ///     Register the user.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <param name="password">The user password.</param>
        public ServiceActionResult Register(string email, string password)
        {
            // Validate
            if (!Utils.IsValidEmail(email))
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_REG_FAIL_EMAIL);
            }

            // Create the user
            var user = CreateUser();
            _userStore.SetUserNameAsync(user, email, CancellationToken.None).Wait();
            _emailStore.SetEmailAsync(user, email, CancellationToken.None).Wait();
            var result = _userManager.CreateAsync(user, password).Result;

            if (!result.Succeeded)
            {
                return new ServiceActionResult(Constants.ResponseCode.FAIL, Utils.UserErrorsToString(result.Errors));
            }

            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_USER_REGISTER_SUCCESS);
        }

        /// <summary>
        ///     Login the user.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <param name="password">The user password.</param>
        public LoginResponseModel? Login(string email, string password)
        {
            // Login attempt
            var result = _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false).Result;

            // Process result
            if (!result.Succeeded)
            {
                return null;
            }

            // Retrieve the logged in user
            var user = _userManager.FindByEmailAsync(email).Result;

            if (user == null || user.Email == null)
            {
                return null;
            }

            // Generate JwtToken
            var token = GenerateJwtToken(user);
            if (token == "")
            {
                return null;
            }

            // Return LoginResponseModel as the token is not BaseModel
            return new LoginResponseModel
            {
                User = new UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                },
                Token = token
            };
        }

        /// <summary>
        /// Log Out the User
        /// </summary>
        public ServiceActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS);
        }

        /// <summary>
        /// Generate JwtToken for the logged in user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                return "";
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(14),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        ///     Create a User instance using the Activator.CreateInstance.
        /// </summary>
        /// <returns>User object, if method fails returns InvalidOperationException.</returns>
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        /// <summary>
        ///     Check whether the backing user store supports user emails.
        /// </summary>
        /// <returns>UserStore object if user store supports user email, otherwise throws NotSupportedException.</returns>
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
